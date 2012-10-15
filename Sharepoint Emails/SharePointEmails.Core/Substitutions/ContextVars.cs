using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.Substitutions
{
    class ContextVars
    {
        ILogger Logger;
        public ContextVars(SPList sourceList, int ItemID, string modifierName, string toemail, int CUserID)
        {
            Logger = Application.Current.Logger;
            SList = sourceList;
            if (SList != null)
            {
                SWeb = SList.ParentWeb;
                if (SWeb != null)
                {
                    SSite = SWeb.Site;
                }
            }
            if (CUserID != -1 && SWeb != null)
            {
                try
                {
                    CUser = SWeb.SiteUsers.GetByID(CUserID);
                }
                catch (Exception ex)
                {
                    Logger.WriteTrace(ex, SeverityEnum.Error);
                }
            }
            if (sourceList != null && ItemID != -1)
            {
                try
                {
                    SItem = sourceList.GetItemById(ItemID);
                }
                catch (Exception ex)
                {
                    Logger.WriteTrace(ex, SeverityEnum.Error);
                }
            }
            if (!string.IsNullOrEmpty(modifierName) && SWeb != null)
            {
                try
                {
                    SUser = SWeb.SiteUsers[modifierName];
                }
                catch (Exception ex)
                {
                    Logger.WriteTrace("Cannot get SUser", SeverityEnum.Error);
                    Logger.WriteTrace(ex, SeverityEnum.Error);
                }
            }

            if (!string.IsNullOrEmpty(toemail) && SWeb != null)
            {
                try
                {
                    DUser = SWeb.SiteUsers.GetByEmail(toemail);
                }
                catch (Exception ex)
                {
                    Logger.WriteTrace("Cannot get DUser", SeverityEnum.Error);
                    Logger.WriteTrace(ex, SeverityEnum.Error);
                }
            }
        }

        public SPWeb SWeb
        {
            get;
            set;
        }

        public SPSite SSite
        {
            get;
            set;
        }

        public SPUser SUser { set; get; }//force alert

        public SPUser DUser { set; get; }//alert destination

        public SPUser CUser { set; get; }//create alert

        public SPList SList
        {
            get;
            set;
        }

        public SPListItem SItem
        {
            get;
            set;
        }

        public bool DUserCanApprove { set; get; }

        public ApproveClass Approve { set; get; }
    }

    struct ApproveClass
    {
        public bool CanApprove { set; get; }

        public string ApproveUrl { set; get; }

        public string RejectUrl { set; get; }

        public string PageUrl { set; get; }

        public string ToTrace()
        {
            return string.Format("CanApprove={0}, ApproveUrl={1}, RejectUrl={2}, PageUrl={3}", CanApprove, ApproveUrl, RejectUrl, PageUrl);
        }
    }
}
