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
using SharePointEmails.Core.Enums;
using SharePointEmails.Core.Transformations;

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

            internal set
            {
                _Curent = null;
            }
        }
        static Application _Curent;

        

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
                var config = GetWebConfiguration(temp);
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

        public WebConfiguration GetWebConfiguration(SPWeb web)
        {
            var configManager = ClassContainer.Instance.Resolve<ConfigurationManager>();
            var res = configManager.GetConfigOrdefault(web);
            return res;
        }

        /// <summary>
        /// On alert notification
        /// </summary>
        /// <param name="web">current web</param>
        /// <param name="eventArgs">params of alert and event</param>
        /// <returns>Generated cutom message</returns>
        public SEMessage OnNotification(SPWeb web, SPAlertHandlerParams eventArgs)
        {
            SPList list = null;
            var eventID = Guid.NewGuid();
            if (eventArgs.a != null)
            {
                if (eventArgs.eventData.Length == 1)
                {
                    GeneratedMessage message = null;
                    var receiverEmail = eventArgs.headers["to"];
                    var ed = eventArgs.eventData[0];
                    list = web.Lists[eventArgs.a.ListID];
                    try
                    {
                        message = GetMessageForItem(eventID, list, ed.itemId, (SPEventType)ed.eventType, ed.eventXml, ed.modifiedBy, receiverEmail, eventArgs.a.UserId);
                    }
                    catch (SeTemplateNotFound ex)
                    {
                        Application.Current.Logger.WriteTrace("TEMPLATE NOT FOUND", ex, SharePointEmails.Logging.SeverityEnum.Verbose);
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Logger.WriteTrace("ERROR DURING GETTING MESSAGE", SharePointEmails.Logging.SeverityEnum.Verbose);
                        throw;
                    }
                    if (message != null)
                    {
                        var mail = SEMessage.Create(eventID, message, eventArgs.headers, eventArgs.body);

                        Application.Current.Logger.WriteTrace("Message will be sent sent", SharePointEmails.Logging.SeverityEnum.Verbose);

                        var processor = ProcessorsManager.Instance.CreateOutcomingProcessor(list);
                        if (processor != null)
                        {
                            processor.Precess(mail, ed);
                        }

                        LogMessage(message, mail.headers);
                        return mail;
                    }
                    else
                    {
                        Application.Current.Logger.WriteTrace("Message not generated", SharePointEmails.Logging.SeverityEnum.Verbose);
                    }
                }
                else
                {
                    Application.Current.Logger.WriteTrace("OnNotification - More then 1 eventdata. currently not supported", SharePointEmails.Logging.SeverityEnum.Warning);
                }
            }
            return null;
        }

        /// <summary>
        /// On Incoming message
        /// </summary>
        /// <param name="list">list wich received the message</param>
        /// <param name="emailMessage">received message</param>
        public void OnIncomingMail(SPList list, Microsoft.SharePoint.Utilities.SPEmailMessage emailMessage)
        {
            Logger.WriteTrace("List " + list.Title + " received mail from " + emailMessage.EnvelopeSender, SeverityEnum.Trace);
            try
            {
                var processor = ProcessorsManager.Instance.CreateIncomingProcessor(list, SEMessage.Create(emailMessage));
                if (processor != null)
                {
                    Logger.WriteTrace(processor.GetType().FullName + " was found as incoming processor for list " + list.Title, SeverityEnum.Trace);
                    try
                    {
                        processor.Process();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteTrace("Error during processing of message",ex, SeverityEnum.CriticalError);
                    }
                }
                else
                {
                    Logger.WriteTrace("No incoming processor found for list " + list.Title, SeverityEnum.Trace, Category.IncomingMessages);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTrace("Error in the handler",ex, SeverityEnum.CriticalError);
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

        void LogMessage(GeneratedMessage message, StringDictionary newheaders)
        {
            var text = new StringBuilder();
            text.AppendLine("GENERATED message\r\n");
            foreach (string key in newheaders.Keys)
            {
                text.AppendLine(key + ":" + newheaders[key] + "\r\n");
            }
            text.AppendLine("Subj:" + Environment.NewLine + message.Subject + Environment.NewLine + "Body:" + Environment.NewLine + message.Body);
            Logger.WriteTrace(text.ToString(), SeverityEnum.Trace);
        }

        private string GetProcessedItem(ISubstitutionContext context, string input, MessageFieldType itemType)
        {
            Logger.WriteTrace(string.Format("{0} processing ",itemType),SeverityEnum.Trace);
            var worker = SubstitutionManager.GetWorker(context, itemType);
            var res= worker.PreProcess(input, context);
            var transformation = TransformationManager.GetTransformation(itemType, context);
            res = transformation.Transform(input, context,worker.OnPartLoaded);
            res = worker.PostProcess(input, context);
            Logger.WriteTrace(string.Format("{0} result: {1}", itemType,res), SeverityEnum.Trace);
            return res;
        }

        #region managers

        TransformationManager TransformationManager
        {
            get
            {
                return ClassContainer.Instance.Resolve<TransformationManager>();
            }
        }

        SubstitutionManager SubstitutionManager
        {
            get
            {
                return ClassContainer.Instance.Resolve<SubstitutionManager>();
            }
        }

        public ILogger Logger
        {
            get
            {
                return ClassContainer.Instance.Resolve<ILogger>();
            }
        }

        ITemplatesManager TemplateManager
        {
            get
            {
                return ClassContainer.Instance.Resolve<ITemplatesManager>();
            }
        }

        #endregion

        internal GeneratedMessage GetMessageForItem(Guid eventID, SPList list, int ItemID, SPEventType type, string eventXML, string modifierName, string receiverEmail, int alertCreatorID)
        {
            ISearchContext search = SearchContext.Create(list, ItemID, eventXML, type,receiverEmail);
            var res = TemplateManager.GetTemplate(search);
            if (res != null)
            {
                Logger.WriteTrace("Found template:"+Environment.NewLine+res.ToString(), SeverityEnum.Trace);
                var substitutionContext = new SubstitutionContext(eventID,eventXML, list, ItemID, modifierName, receiverEmail, alertCreatorID,type);
                Logger.WriteTrace("XML data:" + Environment.NewLine + substitutionContext.GetXML(), SeverityEnum.Trace);
                return new GeneratedMessage
                    {
                        Body = GetProcessedItem(substitutionContext,res.Body,MessageFieldType.ForBody),
                        Subject = GetProcessedItem(substitutionContext, res.Subject, MessageFieldType.ForSubject),
                        From = GetProcessedItem(substitutionContext, res.From, MessageFieldType.ForFrom),
                        Replay = GetProcessedItem(substitutionContext, res.Replay, MessageFieldType.ForReplay)
                    };
            }
            else
            {
                Logger.WriteTrace("Found template is null", SeverityEnum.Error);
            }
            return null;
        }

        #endregion
    }
}
