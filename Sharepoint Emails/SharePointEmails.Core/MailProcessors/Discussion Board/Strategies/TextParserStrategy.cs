using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;

namespace SharePointEmails.Core.MailProcessors.Strategies
{
    internal class TextParserStrategy : SharePointEmails.Core.MailProcessors.Strategies.ITextParserStrategy
    {
        public Microsoft.SharePoint.SPListItem CreateDiscussion(Microsoft.SharePoint.SPList m_list, SEMessage m_message)
        {
            var item = SPUtility.CreateNewDiscussion(m_list, m_message[SEMessage.SUBJECT]);
            item[SPBuiltInFieldId.Body] = m_message.PlainBody;
            return item;
        }

        public SPListItem CreateReply(SPListItem relatedItem, SEMessage m_message)
        {
            var reply = SPUtility.CreateNewDiscussionReply(relatedItem);
            reply[SPBuiltInFieldId.Body] = m_message.PlainBody;
            reply[SPBuiltInFieldId.Title] = m_message.PlainBody;
            return reply;
        }
    }
}
