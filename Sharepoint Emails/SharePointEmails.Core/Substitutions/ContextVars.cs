using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.Substitutions
{
    /// <summary>
    /// Context objects
    /// </summary>
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
                    SPUser suser=null;
                    foreach (SPUser user in SWeb.SiteUsers)
                    {
                        if (string.Equals(user.LoginName, modifierName,StringComparison.InvariantCultureIgnoreCase))
                        {
                            suser = user;
                            break;
                        }
                        if (string.Equals(user.Name, modifierName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (suser != null)
                            {
                                throw new Exception(string.Format("Two user with same name. User1={0} and User2={1}", suser.LoginName, user.LoginName)); ;
                            }
                            suser = user;
                        }
                    }
                    if(suser!=null)
                    {
                        SUser=suser;
                    }
                    else
                    {
                        throw new Exception(string.Format("cannot find user. Name={0} ",modifierName));
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteTrace(string.Format("Cannot get SUser/ Name={0}",modifierName),ex, SeverityEnum.Error);
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
                    Logger.WriteTrace(string.Format("Cannot get DUser. Email={0}", toemail), ex, SeverityEnum.Error);
                }
            }
        }

        public SPWeb SWeb{get;set;}

        public SPSite SSite{get;set;}

        public SPUser SUser { set; get; }//force alert

        public SPUser DUser { set; get; }//alert destination

        public SPUser CUser { set; get; }//create alert

        public SPList SList{get;set;}

        public SPListItem SItem{get;set;}
    }
}
