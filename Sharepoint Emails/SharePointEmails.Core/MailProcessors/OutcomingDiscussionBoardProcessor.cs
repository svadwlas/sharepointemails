using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace SharePointEmails.Core.MailProcessors
{
    class OutcomingDiscussionBoardProcessor
    {
        void Precess(ref StringDictionary headers, ref string body, int ItemId)
        {
            headers["subject"] = headers["subject"] + string.Format("({0})",ItemId);
        }
    }
}
