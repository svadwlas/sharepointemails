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

        string GetErrorMessage(string error)
        {
            return "XSLT file must be valid and not empty" + Environment.NewLine + error;
        }

        public override void ItemAdding(SPItemEventProperties properties)
        {
            //System.Diagnostics.Debugger.Launch();
            //var context = HttpContext.Current;
            //if (context != null && context.Request.Files != null)
            //{
            //    foreach (string filename in context.Request.Files.AllKeys)
            //    {
            //        var file = context.Request.Files[filename];
            //        if (file.ContentLength <= 0)
            //        {
            //            throw new Exception(GetErrorMessage(""));
            //        }
            //        try
            //        {
            //            using (XmlReader reader = XmlReader.Create(file.InputStream))
            //            {
            //                var parser = new XslCompiledTransform();
            //                parser.Load(reader);
            //            }
            //        }
            //        catch (XsltException ex)
            //        {
            //            throw new Exception(GetErrorMessage((ex.InnerException != null) ? ex.InnerException.Message : ex.Message));
            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception(GetErrorMessage(ex.Message), ex);
            //        }
            //    }
            //}
            base.ItemAdding(properties);
        }
    }
}
