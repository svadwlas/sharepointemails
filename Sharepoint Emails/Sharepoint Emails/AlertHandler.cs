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

namespace SharepointEmails
{
    public class AlertHandler : IAlertNotifyHandler
    {
        public bool OnNotification(SPAlertHandlerParams ahp)
        {
            try
            {

                //     System.Diagnostics.Debugger.Launch();
                Application.Current.Logger.Write("Start OnNotification", SharePointEmails.Logging.SeverityEnum.Verbose);

                if (Application.Current.IsDisabledForFarm())
                {
                    Application.Current.Logger.Write("OnNotification - Application disabled on farm", SharePointEmails.Logging.SeverityEnum.Verbose);
                    return false;
                }
                using (SPSite site = new SPSite(ahp.siteId))
                {
                    if (Application.Current.IsDisabledForSite(site)) return false;
                    using (SPWeb web = site.OpenWeb(ahp.webId))
                    {
                        SPUtility.SendEmail(web, ahp.headers, ahp.body);
                        if (ahp.a != null)
                        {
                            var receiverEmail = ahp.headers["to"];
                            GeneratedMessage message = null;

                            Application.Current.Logger.Write("RECEIVER:" + receiverEmail, SharePointEmails.Logging.SeverityEnum.Verbose);
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
                            if (ahp.eventData.Length == 1)
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

                                        //  System.Diagnostics.Debugger.Launch();
                                        message = Application.Current.GetMessageForItem(list, ed.itemId, (SPEventType)ed.eventType, ed.eventXml, ed.modifiedBy, receiverEmail, ahp.a.UserId);
                                    }
                                    catch (SeTemplateNotFound ex)
                                    {
                                        Application.Current.Logger.Write("TEMPLATE NOT FOUND", SharePointEmails.Logging.SeverityEnum.Verbose);
                                        Application.Current.Logger.Write(ex, SharePointEmails.Logging.SeverityEnum.Warning);
                                    }
                                    catch (Exception ex)
                                    {
                                        Application.Current.Logger.Write("ERROR DURING GETTING MESSAGE", SharePointEmails.Logging.SeverityEnum.Verbose);
                                        Application.Current.Logger.Write(ex, SharePointEmails.Logging.SeverityEnum.CriticalError);
                                    }
                                }
                            }
                            else
                            {
                                Application.Current.Logger.Write("OnNotification - More then 1 eventdata. currently not supported", SharePointEmails.Logging.SeverityEnum.Warning);
                            }
                            if (message != null)
                            {
                                var newheaders = new StringDictionary();
                                var newBody = ahp.body;
                                foreach (string key in ahp.headers.Keys)
                                {
                                    newheaders[key] = ahp.headers[key];
                                }
                                if (message.Subject != null)
                                {
                                    newheaders["subject"] = message.Subject;
                                }
                                if (message.Body != null)
                                {
                                    newBody = message.Body;
                                }
                                if (message.Replay != null)
                                {
                                    newheaders["reply-to"] = message.Replay;
                                }
                                if (message.From != null)
                                {
                                    newheaders["from"] = message.From;
                                }

                                Application.Current.Logger.Write("Message will be sent sent", SharePointEmails.Logging.SeverityEnum.Verbose);
                                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"sentEmails\" + DateTime.Now.ToString("hh_mm_ss"));
                                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                                File.WriteAllText(Path.Combine(folder, "body.html"), message.Body);

                                string text = string.Empty;

                                foreach (string key in newheaders.Keys)
                                {
                                    text += key + ":" + newheaders[key] + Environment.NewLine;
                                }

                                text+="Subj:" + Environment.NewLine + message.Subject + Environment.NewLine +
                                    "Body:" + Environment.NewLine + message.Body;



                                File.WriteAllText(Path.Combine(folder, "data.txt"),text);
                                var headers = ahp.headers;


                                var res = SPUtility.SendEmail(web, newheaders, newBody);
                                if (res)
                                {
                                    Application.Current.Logger.Write("Message has been sent sent", SharePointEmails.Logging.SeverityEnum.Verbose);
                                }
                                else
                                {
                                    Application.Current.Logger.Write("Message has NOT been sent sent", SharePointEmails.Logging.SeverityEnum.Verbose);
                                }
                                return res;
                            }
                            else
                            {
                                Application.Current.Logger.Write("Message not generated", SharePointEmails.Logging.SeverityEnum.Verbose);
                                return SPUtility.SendEmail(web, ahp.headers, message.Body);
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Application.Current.Logger.Write("ERROR OnNotification", SharePointEmails.Logging.SeverityEnum.CriticalError);
                Application.Current.Logger.Write(ex, SharePointEmails.Logging.SeverityEnum.CriticalError);
                return false;
            }
            finally
            {
                Application.Current.Logger.Write("End OnNotification", SharePointEmails.Logging.SeverityEnum.Verbose);
            }
        }

        public static void RegisterForAll(string file)
        {
            File.Copy(file, file + "_backup_before_Registering" + DateTime.Now.ToString("mm_dd_hh_mm_ss"));
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
