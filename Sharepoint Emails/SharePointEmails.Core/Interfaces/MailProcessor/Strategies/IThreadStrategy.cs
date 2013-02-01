using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Utilities;
using System.Collections.Specialized;

namespace SharePointEmails.Core.MailProcessors
{
    internal interface IThreadStrategy
    {
        int GetRelatedItemId(SEMessage mail);
        void OnMailSending(SEMessage mail, int itemId);
    }
}
