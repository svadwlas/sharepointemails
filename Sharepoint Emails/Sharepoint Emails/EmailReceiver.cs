using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SharepointEmails
{
    class EmailReceiver : SPEmailEventReceiver
    {
        public override void EmailReceived(SPList list, SPEmailMessage emailMessage, string receiverData)
        {
            
        }
    }
}
