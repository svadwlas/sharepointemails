using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core.MailProcessors
{
    public class ProcessorsManager
    {
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
    }
}
