using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.MailProcessors
{
    public class OutcomingDiscussionBoardProcessor
    {
        ILogger m_Logger = null;
        public OutcomingDiscussionBoardProcessor(ILogger logger)
        {
            m_Logger = logger;
        }

        public void Precess(ref StringDictionary headers, ref string body, int ItemId)
        {
            headers["subject"] = headers["subject"] + string.Format("({0})",ItemId);
        }
    }
}
