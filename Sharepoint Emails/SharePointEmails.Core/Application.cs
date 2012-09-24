using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Logging;
using SharePointEmails.Core.Configuration;
using Microsoft.SharePoint.Administration;
using SharePointEmails.Core.MailProcessors;
using SharePointEmails.Core.Exceptions;
using System.IO;
using System.Collections.Specialized;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public class Application
    {
        static object _CurentLock = new object();
        static object _LoggerLock = new object();

        public static Application Current
        {
            get
            {
                if (_Curent == null)
                {
                    lock (_CurentLock)
                    {
                        if (_Curent == null)
                        {
                            _Curent = new Application();
                        }
                    }
                }
                return _Curent;
            }
        }
        static Application _Curent;

        public ILogger Logger
        {
            get
            {
                if (_Logger == null)
                {
                    lock (_LoggerLock)
                    {
                        if (_Logger == null)
                        {
                            _Logger = ClassContainer.Instance.Resolve<ILogger>();
                        }
                    }
                }
                return _Logger;
            }
        }
        ILogger _Logger;

        ITemplatesManager Manager
        {
            get
            {
                return ClassContainer.Instance.Resolve<ITemplatesManager>();
            }
        }

        #region public

        public bool IsDisabledForFarm()
        {
            return false;
            return FarmConfig.Disabled;
        }

        public bool IsDisabledForSite(SPSite site)
        {
            return !site.Features.Any(p => p.DefinitionId == Constants.FeatureId);
        }

        public bool IsDisabledForWeb(SPWeb web)
        {
            return false;
            var temp = web;
            while (temp != null)
            {
                var config = WebConfig(temp);
                if (config != null)
                {
                    if (temp.Equals(web) && ((config.Disabled)))
                        return true;
                    else if (config.Disabled && config.DisableIncludeChilds)
                        return true;
                }
            }
            return false;
        }

        public WebConfiguration WebConfig(SPWeb web)
        {
            var configManager = ClassContainer.Instance.Resolve<ConfigurationManager>();
            var res = configManager.GetConfigOrdefault(web);
            return res;
        }

        public SEMessage OnNotification(SPWeb web, SPAlertHandlerParams ahp)
        {
            SPList list = null;
            if (ahp.a != null)
            {
                if (ahp.eventData.Length == 1)
                {
                    GeneratedMessage message = null;
                    var receiverEmail = ahp.headers["to"];
                    var ed = ahp.eventData[0];
                    list = web.Lists[ahp.a.ListID];
                    try
                    {
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
                    if (message != null)
                    {
                        var mail = SEMessage.Create(message, ahp.headers, ahp.body);

                        Application.Current.Logger.Write("Message will be sent sent", SharePointEmails.Logging.SeverityEnum.Verbose);

                        var processor = ProcessorsManager.Instance.CreateOutcomingProcessor(list);
                        if (processor != null)
                        {
                            processor.Precess(mail, ed);
                        }

                        SaveMessage(message, mail.headers);
                        return mail;
                    }
                    else
                    {
                        Application.Current.Logger.Write("Message not generated", SharePointEmails.Logging.SeverityEnum.Verbose);
                    }
                }
                else
                {
                    Application.Current.Logger.Write("OnNotification - More then 1 eventdata. currently not supported", SharePointEmails.Logging.SeverityEnum.Warning);
                }
            }
            return null;
        }

        public void OnIncomingMail(SPList list, Microsoft.SharePoint.Utilities.SPEmailMessage emailMessage)
        {
            Logger.Write("List " + list.Title + " recieved mail from " + emailMessage.EnvelopeSender, SeverityEnum.Trace);
            try
            {
                var processor = ProcessorsManager.Instance.CreateIncomingProcessor(list, SEMessage.Create(emailMessage));
                if (processor != null)
                {
                    Logger.Write(processor.GetType().FullName + " was found as incoming processor for list " + list.Title, SeverityEnum.Trace);
                    try
                    {
                        processor.Process();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Error during processing of message", SeverityEnum.CriticalError);
                        Logger.Write(ex, SeverityEnum.CriticalError);
                    }
                }
                else
                {
                    Logger.Write("No incoming processor found for list " + list.Title, SeverityEnum.Trace, AreasEnum.IncomingMessages);
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in the handler", SeverityEnum.CriticalError);
                Logger.Write(ex, SeverityEnum.CriticalError);
            }
        }

        public ConfigurationManager GetConfigurationManager()
        {
            return ClassContainer.Instance.Resolve<ConfigurationManager>();
        }

        #endregion

        #region privates

        FarmConfiguration FarmConfig
        {
            get
            {
                var configManager = ClassContainer.Instance.Resolve<ConfigurationManager>();
                var res = configManager.GetConfigOrdefault(SPFarm.Local);
                return res;
            }
        }

        void SaveMessage(GeneratedMessage message, StringDictionary newheaders)
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"sentEmails\" + DateTime.Now.ToString("hh_mm_ss"));
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "body.html"), message.Body);

            string text = string.Empty;

            foreach (string key in newheaders.Keys)
            {
                text += key + ":" + newheaders[key] + Environment.NewLine;
            }

            text += "Subj:" + Environment.NewLine + message.Subject + Environment.NewLine +
                "Body:" + Environment.NewLine + message.Body;



            File.WriteAllText(Path.Combine(folder, "data.txt"), text);
        }

        GeneratedMessage GetMessageForItem(SPList list, int ItemID, SPEventType type, string eventXML, string modifierName, string receiverEmail, int alertCreatorID)
        {
            ISearchContext search = SearchContext.Create(list, ItemID, eventXML, type,receiverEmail);
            var res = Manager.GetTemplate(search);
            if (res != null)
            {
                Logger.Write("Found template:", SeverityEnum.Verbose);
                Logger.Write(res.ToString(), SeverityEnum.Verbose);
                var substitutionContext = new SubstitutionContext(eventXML, list, ItemID, modifierName, receiverEmail, alertCreatorID,type);
                return new GeneratedMessage
                    {
                        Body = res.GetProcessedBody(substitutionContext),
                        Subject = res.GetProcessedSubj(substitutionContext),
                        From = res.GetProcessedFrom(substitutionContext),
                        Replay = res.GetProcessedReplay(substitutionContext)
                    };
            }
            else
            {
                Logger.Write("Found template is null", SeverityEnum.Error);
            }
            return null;
        }

        #endregion

        
    }
}
