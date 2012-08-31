using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.MailProcessors;
using SharePointEmails.Logging;
using Microsoft.SharePoint.Utilities;

namespace SharePointEmails.Core.MailProcessors
{
    public class ProcessorsManager
    {
        static ILogger m_Logger = null;

        private static ILogger Logger
        {
            get
            {
                if (m_Logger == null)
                {
                    m_Logger = ClassContainer.Instance.Resolve<ILogger>();
                }
                return m_Logger;
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
                return typeof(SharepointEmails.EmailReceiver);
            }
        }

        public bool IsDiscussionBoardIntegrationEnabled(SPList list)
        {
            if (list == null || list.BaseTemplate != SPListTemplateType.DiscussionBoard) return false;
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
                receiver = list.EventReceivers.OfType<SPEventReceiverDefinition>().Where(p => p.Class == ReceiverType.FullName).FirstOrDefault();
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

        public IIncomingMessageProcessor CreateIncomingProcessor(SPList list, SPEmailMessage message)
        {
            try
            {
                if (list.BaseTemplate == SPListTemplateType.DiscussionBoard)
                {
                    return new IncomingDiscussionBoardProcessor(list, message, Logger);
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error during in processor creating", SeverityEnum.CriticalError);
                Logger.Write(ex, SeverityEnum.CriticalError);
            }
            return null;
        }

        public OutcomingDiscussionBoardProcessor CreateOutcomingProcessor(SPList list)
        {
            try
            {
                if (list.BaseTemplate == SPListTemplateType.DiscussionBoard)
                {
                    if (IsDiscussionBoardIntegrationEnabled(list))
                    {
                        return new OutcomingDiscussionBoardProcessor(Logger);
                    }
                    else
                    {
                        Logger.Write("Integratoin for List=" + list.Title + " is disabled", SeverityEnum.Trace);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error during out processor creating", SeverityEnum.CriticalError);
                Logger.Write(ex, SeverityEnum.CriticalError);
            }
            return null;
        }
    }
}
