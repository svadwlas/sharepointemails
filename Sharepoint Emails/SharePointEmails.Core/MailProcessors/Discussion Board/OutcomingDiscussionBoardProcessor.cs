using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using SharePointEmails.Logging;
using Microsoft.SharePoint;

namespace SharePointEmails.Core.MailProcessors
{
    internal class OutcomingDiscussionBoardProcessor : IOutcomingDiscussionBoardProcessor
    {
        ILogger m_Logger = null;
        IThreadStrategy m_strategy = null;
        public OutcomingDiscussionBoardProcessor(ILogger logger, IThreadStrategy strategy)
        {
            m_Logger = logger;
            m_strategy = strategy;
        }

        public void Precess(SEMessage mail, SPAlertEventData ed)
        {
            m_strategy.OnMailSending(mail,ed.itemId);
        }
    }
}
