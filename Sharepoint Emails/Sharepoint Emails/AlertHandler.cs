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

namespace SharepointEmails
{
    public class AlertHandler : IAlertNotifyHandler
    {
        void Trace(SPAlertHandlerParams ahp)
        {
            foreach (string s in ahp.headers.Keys)
            {
                Application.Current.Logger.Write(s+": "+ahp.headers[s], SharePointEmails.Logging.SeverityEnum.Verbose);
            }
            foreach (SPAlertEventData ed in ahp.eventData)
            {
                Application.Current.Logger.Write("EventData:", SharePointEmails.Logging.SeverityEnum.Verbose);
                Application.Current.Logger.Write("EventType: " + (SPEventType)ed.eventType, SharePointEmails.Logging.SeverityEnum.Verbose);
                Application.Current.Logger.Write("EventXml: " + ed.eventXml, SharePointEmails.Logging.SeverityEnum.Verbose);
                Application.Current.Logger.Write("formattedEvent: " + ed.formattedEvent, SharePointEmails.Logging.SeverityEnum.Verbose);
                Application.Current.Logger.Write("itemFullUrl: " + ed.itemFullUrl, SharePointEmails.Logging.SeverityEnum.Verbose);
                Application.Current.Logger.Write("itemId: " + ed.itemId, SharePointEmails.Logging.SeverityEnum.Verbose);
                Application.Current.Logger.Write("modifiedBy: " + ed.modifiedBy, SharePointEmails.Logging.SeverityEnum.Verbose);
            }
        }

        public bool OnNotification(SPAlertHandlerParams ahp)
        {
            try
            {
                bool handled = false;
                
                Application.Current.Logger.Write("Start OnNotification", SharePointEmails.Logging.SeverityEnum.Verbose);
                Trace(ahp);

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
                                            Application.Current.Logger.Write("Cannot send generated message", SharePointEmails.Logging.SeverityEnum.CriticalError);
                                            Application.Current.Logger.Write(ex, SharePointEmails.Logging.SeverityEnum.CriticalError);
                                        }
                                    }
                                    else
                                    {
                                        Application.Current.Logger.Write("Mail is null", SharePointEmails.Logging.SeverityEnum.Warning);
                                    }
                                }
                                else
                                {
                                    Application.Current.Logger.Write("Disabled on web", SharePointEmails.Logging.SeverityEnum.Warning);
                                }
                                if (!handled)
                                {
                                    Application.Current.Logger.Write("Not handled. Send default message", SharePointEmails.Logging.SeverityEnum.Trace);
                                    return SPUtility.SendEmail(web, ahp.headers, ahp.body);
                                }
                            }
                            else
                            {
                                Application.Current.Logger.Write("OnNotification - Application disabled on site collection", SharePointEmails.Logging.SeverityEnum.Verbose);
                            }
                        }
                        else
                        {
                            Application.Current.Logger.Write("OnNotification - Application disabled on farm", SharePointEmails.Logging.SeverityEnum.Verbose);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Logger.Write("ERROR OnNotification", SharePointEmails.Logging.SeverityEnum.CriticalError);
                Application.Current.Logger.Write(ex, SharePointEmails.Logging.SeverityEnum.CriticalError);
            }
            finally
            {
                Application.Current.Logger.Write("End OnNotification", SharePointEmails.Logging.SeverityEnum.Verbose);
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
