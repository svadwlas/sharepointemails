using System;
using Microsoft.SharePoint;
namespace SharePointEmails.Core.MailProcessors.Strategies
{
    internal interface ITextParserStrategy
    {
        SPListItem CreateDiscussion(SPList m_list, SEMessage m_message);
        SPListItem CreateReply(SPListItem relatedItem, SEMessage m_message);
    }
}
