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

            if (Application.Current.IsDisabledForFarm()) return false;

            if (ahp.a != null)
            {
                var item = ahp.a.Item;
                Message message = null;
                if (ahp.eventData.Length > 0)
                {
                    using (SPSite site = new SPSite(ahp.siteId))
                    {
                        using (SPWeb web = site.OpenWeb(ahp.webId))
                        {
                            if (Application.Current.IsDisabledForWeb(web))
                            {
                                return false;
                            }

                            foreach (var ed in ahp.eventData)
                            {
                                message = Application.Current.GetMessage(null, ahp.a.EventType,ed.eventXml);
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
