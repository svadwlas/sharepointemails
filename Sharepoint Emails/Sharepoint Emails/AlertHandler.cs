using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core;
using Microsoft.SharePoint.Utilities;

namespace SharepointEmails
{
    public class AlertHandler : IAlertNotifyHandler
    {
        public bool OnNotification(SPAlertHandlerParams ahp)
        {
            System.Diagnostics.Debugger.Launch();

            if (ahp.a != null)
            {
                var item = ahp.a.Item;
                Message message = null;
                if (item != null)
                {
                    message = Application.Current.GetMessage(item,ahp.a.EventType);
                }
                if (ahp.eventData.Length > 0)
                {
                    using (SPSite site = new SPSite(ahp.siteId))
                    {
                        using (SPWeb web = site.OpenWeb(ahp.webId))
                        {
                            var list = web.Lists[ahp.a.ListID];
                            var listItem=list.GetItemById(ahp.eventData[0].itemId);
                            if (listItem != null)
                            {
                                message = Application.Current.GetMessage(listItem, ahp.a.EventType);
                            }
                        }
                    }
                }
                if (message != null)
                {
                    Application.Current.Logger.Write("Message was sent", SharePointEmails.Logging.SeverityEnum.Verbose);
                    return true;
                }
                else
                {
                    Application.Current.Logger.Write("Message not found", SharePointEmails.Logging.SeverityEnum.Verbose);
                    return false;
                }
            }

            return false;
        }
    }
}
