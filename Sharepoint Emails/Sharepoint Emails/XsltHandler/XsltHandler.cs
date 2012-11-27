using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Web;
using System.IO;
using System.Xml.Xsl;
using System.Xml;
using SharePointEmails.Core;

namespace SharePointEmails.XsltHandler
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class XsltHandler : SPItemEventReceiver
    {
        void UpdateItem(SPItemEventProperties properties)
        {
            if (string.Equals(properties.ListTitle, Constants.XsltLibrary))
            {
                EventFiringEnabled = false;
                properties.ListItem["Title"] = properties.ListItem.File.Name;
                properties.ListItem.Update();
                EventFiringEnabled = true;
            }
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

        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);
        }
    }
}
