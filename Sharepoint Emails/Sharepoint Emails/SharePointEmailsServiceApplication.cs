using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Workflow;
using SharepointEmails.WebServices.SharePointEmailsWebApplication;
using Microsoft.SharePoint.Utilities;
using System.IO;

namespace SharepointEmails
{
    [System.Runtime.InteropServices.Guid("49041028-59F7-4605-B2D1-919F209386A2")]
    public sealed class SharePointEmailsServiceApplication:SPIisWebServiceApplication,ISharePointEmailsService
    {
        private SharePointEmailsServiceInstance m_ServiceImstance;

        public SharePointEmailsServiceApplication()
        {
        }

        internal SharePointEmailsServiceApplication(string name, SharePointEmailsService service, SPIisWebServiceApplicationPool pool)
            : base(name, service, pool)
        {
        }

        internal static SharePointEmailsServiceApplication GetApplicationById(Guid applicationId)
        {
            return SharePointEmailsService.Local.GetWebApplicationById(applicationId);
        }

        internal static SharePointEmailsServiceApplication GetApplicationByName(string applicationName)
        {
            return SharePointEmailsService.Local.GetWebApplicationByName(applicationName);
        }

        protected override string VirtualPath
        {
            get { return "Service.svc"; }
        }

        public override SPAdministrationLink ManageLink
        {
            get
            {
                return new SPAdministrationLink(string.Format("/_admin/SharePointEmailsApplication/SharePointEmailsServiceAdmin.aspx?id={0}", Id.ToString()));
            }
        }

        protected override string InstallPath
        {
            get { return Path.GetFullPath(SPUtility.GetGenericSetupPath(@"WebServices\SharePointEmailsServiceApplication"));}
        }

        public Uri DefaultServicePath
        {
            get { return this.DefaultEndpoint.Uri ; }
        }

        public SharePointEmailsServiceInstance OnlineInstance
        {
            get
            {
                if (null == m_ServiceImstance)
                {
                    foreach (SPServiceInstance i in this.ServiceInstances)
                    {
                        SharePointEmailsServiceInstance instance = i as SharePointEmailsServiceInstance;
                        if ((instance != null) && instance.IsOnline)
                        {
                            m_ServiceImstance = instance;
                            break;
                        }
                    }
                }

                return m_ServiceImstance;
            }
        }

        #region ISharePointEmailsService

        public string Hello(string name)
        {
            return string.Format("Hello {0}", name);
        }

        #endregion ISharePointEmailsService
    }
}
