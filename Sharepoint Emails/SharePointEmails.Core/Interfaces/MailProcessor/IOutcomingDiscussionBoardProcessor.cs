using System;
using Microsoft.SharePoint;
namespace SharePointEmails.Core.MailProcessors
{
    internal interface IOutcomingDiscussionBoardProcessor
    {
        void Precess(SEMessage mail, SPAlertEventData ed);
    }
}
