using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace SharepointEmails.XsltHandler
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class XsltHandler : SPItemEventReceiver
    {
        void UpdateItem(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            properties.ListItem["Title"] = properties.ListItem.File.Name;
            properties.ListItem.Update();
            EventFiringEnabled = true;
        }
       /// <summary>
       /// An item was updated.
       /// </summary>
       public override void ItemUpdated(SPItemEventProperties properties)
       {
           UpdateItem(properties);
           base.ItemUpdated(properties);
       }

       public override void ItemAdded(SPItemEventProperties properties)
       {
           UpdateItem(properties);
           base.ItemAdded(properties);
       }


    }
}
