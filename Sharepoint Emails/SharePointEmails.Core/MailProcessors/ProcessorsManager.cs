using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.MailProcessors;
using SharePointEmails.Logging;
using Microsoft.SharePoint.Utilities;
using SharePointEmails.Core.MailProcessors.Strategies;

namespace SharePointEmails.Core.MailProcessors
{
    public class ProcessorsManager
    {

        private static ILogger Logger
        {
            get
            {
                return ClassContainer.Instance.Resolve<ILogger>();
            }
        }


        static ProcessorsManager m_instance = null;
        static object lockObj = new object();
        public static ProcessorsManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (lockObj)
                    {
                        if (m_instance == null)
                        {
                            m_instance = new ProcessorsManager();
                        }
                    }
                }
                return m_instance;
            }
        }

        Type ReceiverType
        {
            get
            {
                return typeof(SharePointEmails.EmailReceiver);
            }
        }

        public bool IsIntegrationEnabled(SPList list)
        {
            //if (list == null || list.BaseTemplate != SPListTemplateType.DiscussionBoard) return false;
            var emailReceivers = GetReceivers(list);
            return emailReceivers.Any(p => p == ReceiverType.FullName);
        }

        public IEnumerable<string> GetReceivers(SPList list)
        {
            return list.EventReceivers.OfType<SPEventReceiverDefinition>().Where(p => p.Type == SPEventReceiverType.EmailReceived).Select(p => p.Class);
        }

        public void DisableDiscussionBoardProcessing(SPList list)
        {
            SPEventReceiverDefinition receiver = null;
            do
            {
                receiver = list.EventReceivers.OfType<SPEventReceiverDefinition>().Where(p=>string.Equals(p.Class, ReceiverType.FullName,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (receiver != null)
                {
                    receiver.Delete();
                }

            } while (receiver != null);
        }

        public void EnableDiscussionBoardProcessing(SPList list)
        {
            DisableDiscussionBoardProcessing(list);
            var def = list.EventReceivers.Add();
            def.Assembly = ReceiverType.Assembly.FullName;
            def.Class = ReceiverType.FullName;
            def.Name = "SharePointEmails Email Receiver";
            def.Type = SPEventReceiverType.EmailReceived;
            def.SequenceNumber = 999;
            def.Synchronization = SPEventReceiverSynchronization.Synchronous;
            def.Update();
        }

        internal IIncomingMessageProcessor CreateIncomingProcessor(SPList list, SEMessage message)
        {
            try
            {
                if (list.BaseTemplate == SPListTemplateType.DiscussionBoard)
                {
                    return new IncomingDiscussionBoardProcessor(list, message, Logger, new SubjectThreadStrategy(), new TextParserStrategy());
                }
                else if(list.BaseTemplate==SPListTemplateType.DocumentLibrary)
                {
                    return new IncomingDiscussionBoardProcessor(list, message, Logger, new SubjectThreadStrategy(), new TextParserStrategy());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTrace("Error during in processor creating", SeverityEnum.CriticalError);
                Logger.WriteTrace(ex, SeverityEnum.CriticalError);
            }
            return null;
        }

        internal IOutcomingDiscussionBoardProcessor CreateOutcomingProcessor(SPList list)
        {
            try
            {
                if (list.BaseTemplate == SPListTemplateType.DiscussionBoard)
                {
                    if (IsIntegrationEnabled(list))
                    {
                        return new OutcomingDiscussionBoardProcessor(Logger,new SubjectThreadStrategy());
                    }
                    else
                    {
                        Logger.WriteTrace("Integratoin for List=" + list.Title + " is disabled", SeverityEnum.Trace);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTrace("Error during out processor creating", SeverityEnum.CriticalError);
                Logger.WriteTrace(ex, SeverityEnum.CriticalError);
            }
            return null;
        }
    }
}
