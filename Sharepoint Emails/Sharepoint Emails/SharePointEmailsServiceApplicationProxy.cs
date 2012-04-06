using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;

namespace SharepointEmails
{
    [System.Runtime.InteropServices.Guid("E841EAE6-28A7-4FAA-883B-7FE30C065CDB")]
    public sealed class SharePointEmailsServiceApplicationProxy:SPIisWebServiceApplicationProxy
    {
        [Persisted]
        private SharePointEmailsServiceApplication m_Application;

        public SharePointEmailsServiceApplicationProxy()
        {
        }

        internal SharePointEmailsServiceApplicationProxy(string name, SPIisWebServiceProxy serviceProxy, SharePointEmailsServiceApplication serviceApplication)
            : base(name, serviceProxy, serviceApplication.Uri)
        {
            this.m_Application = serviceApplication;
        }

        internal SharePointEmailsServiceApplicationProxy(string name, SPIisWebServiceProxy serviceProxy, Uri serviceApplicationAddress)
            : base(name, serviceProxy, serviceApplicationAddress)
        {
        }

        internal void AddToDefaultGroup(bool setDefault)
        {
            if (setDefault)
            {
                SPServiceApplicationProxyGroup group = SPServiceApplicationProxyGroup.Default;
                group.Add(this);
                group.Update();
            }
        }

        public SharePointEmailsServiceApplication Application
        {
            get
            {

                System.Diagnostics.Debugger.Launch();
                if (this.m_Application == null)
                {
                    return null;
                }
                return SharePointEmailsService.Local.GetWebApplicationByName(this.m_Application.Name);
            }
        }

        internal static SharePointEmailsServiceApplicationProxy Local
        {
            get
            {
                SPServiceContext current = SPServiceContext.Current;
                if (current == null)
                {
                    throw new InvalidOperationException("Could not retrieve  the application");
                }
                return (SharePointEmailsServiceApplicationProxy)current.GetDefaultProxy(typeof(SharePointEmailsServiceApplicationProxy));
            }
        }

        public override string TypeName
        {
            get
            {
                return "SharePointEmails Service Application Proxy";
            }
        }

        public static SharePointEmailsServiceApplicationProxy GetProxy(SPServiceContext serviceContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }
            return (serviceContext.GetDefaultProxy(typeof(SharePointEmailsServiceApplicationProxy)) as SharePointEmailsServiceApplicationProxy);
        }
    }
}
