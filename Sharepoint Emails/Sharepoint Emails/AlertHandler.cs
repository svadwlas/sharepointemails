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

namespace SharepointEmails
{
    public class AlertHandler : IAlertNotifyHandler
    {
        public bool OnNotification(SPAlertHandlerParams ahp)
        {
            try
            {
                Application.Current.Logger.Write("Start OnNotification", SharePointEmails.Logging.SeverityEnum.Verbose);

                if (Application.Current.IsDisabledForFarm())
                {
                    Application.Current.Logger.Write("OnNotification - Application disabled on farm", SharePointEmails.Logging.SeverityEnum.Verbose);
                    return false;
                }

                if (ahp.a != null)
                {
                    var item = ahp.a.Item;
                    Message message = null;
                    Application.Current.Logger.Write("ItemType: " + ahp.a.EventType, SharePointEmails.Logging.SeverityEnum.Verbose);
                    foreach (SPAlertEventData ed in ahp.eventData)
                    {
                        Application.Current.Logger.Write("EventData:", SharePointEmails.Logging.SeverityEnum.Verbose);
                        Application.Current.Logger.Write("EventType: " + ed.eventType, SharePointEmails.Logging.SeverityEnum.Verbose);
                        Application.Current.Logger.Write("EventXml: " + ed.eventXml, SharePointEmails.Logging.SeverityEnum.Verbose);
                        Application.Current.Logger.Write("formattedEvent: " + ed.formattedEvent, SharePointEmails.Logging.SeverityEnum.Verbose);
                        Application.Current.Logger.Write("itemFullUrl: " + ed.itemFullUrl, SharePointEmails.Logging.SeverityEnum.Verbose);
                        Application.Current.Logger.Write("itemId: " + ed.itemId, SharePointEmails.Logging.SeverityEnum.Verbose);
                        Application.Current.Logger.Write("modifiedBy: " + ed.modifiedBy, SharePointEmails.Logging.SeverityEnum.Verbose);
                    }
                    if (ahp.eventData.Length == 1)
                    {
                        using (SPSite site = new SPSite(ahp.siteId))
                        {
                            using (SPWeb web = site.OpenWeb(ahp.webId))
                            {
                                if (Application.Current.IsDisabledForWeb(web))
                                {
                                    Application.Current.Logger.Write("Disabled on site", SharePointEmails.Logging.SeverityEnum.Warning);
                                    return false;
                                }

                                var list = web.Lists[ahp.a.ListID];
                                foreach (var ed in ahp.eventData)
                                {
                                    try
                                    {
                                        System.Diagnostics.Debugger.Launch();
                                        message = Application.Current.GetMessageForItem(list, ed.itemId, ahp.a.EventType, ed.eventXml);
                                    }
                                    catch (Exception ex)
                                    {
                                        Application.Current.Logger.Write("ERROR DURING GETTING MESSAGE", SharePointEmails.Logging.SeverityEnum.Verbose);
                                        Application.Current.Logger.Write(ex, SharePointEmails.Logging.SeverityEnum.Error);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Application.Current.Logger.Write("OnNotification - More then 1 eventdata", SharePointEmails.Logging.SeverityEnum.Verbose);
                    }
                    if (message != null)
                    {
                        Application.Current.Logger.Write("Message was sent", SharePointEmails.Logging.SeverityEnum.Verbose);
                        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"sentEmails\" + DateTime.Now.ToString("hh_mm_ss"));
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                        File.WriteAllText(Path.Combine(folder, "body.html"), message.Body);
                        File.WriteAllText(Path.Combine(folder, "data.txt"),
                            "Subj:" + Environment.NewLine + message.Subject + Environment.NewLine +
                            "Body:" + Environment.NewLine + message.Body);
                        return true;
                    }
                    else
                    {
                        Application.Current.Logger.Write("Message not found", SharePointEmails.Logging.SeverityEnum.Verbose);
                        return false;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Application.Current.Logger.Write("ERROR OnNotification", SharePointEmails.Logging.SeverityEnum.Verbose);
                Application.Current.Logger.Write(ex,SharePointEmails.Logging.SeverityEnum.CriticalError);
                return false;
            }
            finally
            {
                Application.Current.Logger.Write("End OnNotification", SharePointEmails.Logging.SeverityEnum.Verbose);
            }
        }

        public static void RegisterForAll(string file)
        {
            File.Copy(file, file + "_backup" + DateTime.Now.ToString("mm_dd_hh_mm_ss"));
            var doc = XDocument.Load(file);
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
            doc.Save(file);
        }

        public static void UnRegisterForAll(string file)
        {
            File.Copy(file, file + "_backup_before_restore" + DateTime.Now.ToString("mm_dd_hh_mm_ss"));
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
