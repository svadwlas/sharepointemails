using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using SharePointEmails.Core.Exceptions;
using System.Web;
using System.Collections.Specialized;
using SharePointEmails.Core.MailProcessors;

namespace SharePointEmails
{
    public class AlertHandler : IAlertNotifyHandler
    {
        string GetTrace(SPAlertHandlerParams ahp)
        {
            var text = new StringBuilder();
            text.AppendLine("INCOMING DATA:");
            foreach (string s in ahp.headers.Keys)
            {
                text.AppendLine(s + ": " + ahp.headers[s]);
            }
            foreach (SPAlertEventData ed in ahp.eventData)
            {
                text.AppendLine("EventData:");
                text.AppendLine("EventType: " + (SPEventType)ed.eventType);
                text.AppendLine("EventXml: " + ed.eventXml);
                text.AppendLine("formattedEvent: " + ed.formattedEvent);
                text.AppendLine("itemFullUrl: " + ed.itemFullUrl);
                text.AppendLine("itemId: " + ed.itemId);
                text.AppendLine("modifiedBy: " + ed.modifiedBy);
            }
            if (ahp.a != null)
            {
                text.AppendLine("ListID:" + ahp.a.ListID);
                text.AppendLine("ListUrl:" + ahp.a.ListUrl);

            }
            else
            {
                text.AppendLine("ahp.a is null");
            }
            return text.ToString();
        }

        public bool OnNotification(SPAlertHandlerParams ahp)
        {
            try
            {
                Application.Current.Logger.WriteTrace("Start OnNotification" + Environment.NewLine + GetTrace(ahp), SharePointEmails.Logging.SeverityEnum.Verbose);
                bool handled = false;
                
                using (SPSite site = new SPSite(ahp.siteId))
                {
                    using (SPWeb web = site.OpenWeb(ahp.webId))
                    {
                        if (!Application.Current.IsDisabledForFarm())
                        {
                            if (!Application.Current.IsDisabledForSite(site))
                            {
                                if (!Application.Current.IsDisabledForWeb(web))
                                {
                                    var mail = Application.Current.OnNotification(web, ahp);
                                    if (mail != null)
                                    {
                                        try
                                        {
                                            SPUtility.SendEmail(web, mail.headers, mail.HtmlBody);
                                            handled = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            Application.Current.Logger.WriteTrace("Cannot send generated message", SharePointEmails.Logging.SeverityEnum.CriticalError);
                                            Application.Current.Logger.WriteTrace(ex, SharePointEmails.Logging.SeverityEnum.CriticalError);
                                        }
                                    }
                                    else
                                    {
                                        Application.Current.Logger.WriteTrace("Mail is null", SharePointEmails.Logging.SeverityEnum.Warning);
                                    }
                                }
                                else
                                {
                                    Application.Current.Logger.WriteTrace("Disabled on web", SharePointEmails.Logging.SeverityEnum.Warning);
                                }
                                if (!handled)
                                {
                                    Application.Current.Logger.WriteTrace("Not handled. Send default message", SharePointEmails.Logging.SeverityEnum.Trace);
                                    return SPUtility.SendEmail(web, ahp.headers, ahp.body);
                                }
                            }
                            else
                            {
                                Application.Current.Logger.WriteTrace("OnNotification - Application disabled on site collection", SharePointEmails.Logging.SeverityEnum.Verbose);
                            }
                        }
                        else
                        {
                            Application.Current.Logger.WriteTrace("OnNotification - Application disabled on farm", SharePointEmails.Logging.SeverityEnum.Verbose);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Logger.WriteTrace("ERROR OnNotification", SharePointEmails.Logging.SeverityEnum.CriticalError);
                Application.Current.Logger.WriteTrace(ex, SharePointEmails.Logging.SeverityEnum.CriticalError);
            }
            finally
            {
                Application.Current.Logger.WriteTrace("End OnNotification", SharePointEmails.Logging.SeverityEnum.Verbose);
            }
            return false;
        }

        public static void RegisterForAll(string sourceFile, out string tmpFile)
        {
            tmpFile = Path.Combine(Path.GetDirectoryName(sourceFile), Path.ChangeExtension(Path.GetFileNameWithoutExtension(sourceFile) + DateTime.Now.ToString("_mm_dd_hh_mm_ss"), Path.GetExtension(sourceFile)));
            File.Copy(sourceFile, tmpFile);
            var doc = XDocument.Load(tmpFile);
            foreach (var template in doc.Descendants("AlertTemplate"))
            {
                var props = template.Elements("Properties").FirstOrDefault();
                if (props != null)
                {
                    var NotificationHandlerAssembly = props.Elements("NotificationHandlerAssembly").FirstOrDefault();
                    if (NotificationHandlerAssembly == null)
                    {
                        NotificationHandlerAssembly = new XElement("NotificationHandlerAssembly");
                        props.Add(NotificationHandlerAssembly);
                    }
                    var NotificationHandlerClassName = props.Elements("NotificationHandlerClassName").FirstOrDefault();
                    if (NotificationHandlerClassName == null)
                    {
                        NotificationHandlerClassName = new XElement("NotificationHandlerClassName");
                        props.Add(NotificationHandlerClassName);
                    }

                    NotificationHandlerAssembly.Value = Assembly.GetExecutingAssembly().FullName;
                    NotificationHandlerClassName.Value = typeof(AlertHandler).FullName;
                }
            }
            doc.Save(tmpFile);
        }

        public static void UnRegisterForAll(string file)
        {
            File.Copy(file, file + "_backup_before_Unregistering" + DateTime.Now.ToString("mm_dd_hh_mm_ss"));
            var doc = XDocument.Load(file);
            foreach (var template in doc.Descendants("AlertTemplate"))
            {
                var props = template.Elements("Properties").FirstOrDefault();
                if (props != null)
                {
                    var NotificationHandlerAssembly = props.Elements("NotificationHandlerAssembly").FirstOrDefault();
                    var NotificationHandlerClassName = props.Elements("NotificationHandlerClassName").FirstOrDefault();

                    if (NotificationHandlerClassName != null) NotificationHandlerClassName.Remove();
                    if (NotificationHandlerAssembly != null) NotificationHandlerAssembly.Remove();
                }
            }
            doc.Save(file);
        }
    }
}
