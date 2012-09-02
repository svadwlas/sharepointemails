using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SharePointEmails.Core;
using SharePointEmails.Logging;
using System.Configuration;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.MailProcessors;
using System.ComponentModel;
using SharePointEmails.Core.MailProcessors;

namespace SharepointEmails
{
    [Description("Processor of incoming emails")]
    [DisplayName("SharePointEmails email receiver")]
    public class EmailReceiver : SPEmailEventReceiver
    {
        public override void EmailReceived(SPList list, SPEmailMessage emailMessage, string receiverData)
        {
            Application.Current.OnIncomingMail(list, emailMessage);
        }
    }
}
