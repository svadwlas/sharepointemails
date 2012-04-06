using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace SharepointEmails
{
    [System.Runtime.InteropServices.Guid("6AE34461-652A-443A-9122-05ACEF9BE883")]
    [SupportedServiceApplication("49041028-59F7-4605-B2D1-919F209386A2", "1.0.0.0", typeof(SharePointEmailsServiceApplicationProxy))]
    internal sealed class SharePointEmailsServiceProxy : SPIisWebServiceProxy, IServiceProxyAdministration
    {
        public SharePointEmailsServiceProxy() { }

        internal SharePointEmailsServiceProxy(SPFarm farm)
            : base(farm)
        {
        }

        internal SharePointEmailsServiceProxy(string name, SPFarm farm)
            : base(farm)
        {
        }

        #region IServiceProxyAdministration
        public SPServiceApplicationProxy CreateProxy(Type serviceApplicationProxyType, string name, Uri serviceApplicationUri, SPServiceProvisioningContext provisioningContext)
        {
            if (serviceApplicationProxyType != typeof(SharePointEmailsServiceProxy))
            {
                throw new NotSupportedException();
            }
            return new SharePointEmailsServiceApplicationProxy(name, this, serviceApplicationUri);
        }

        public SPPersistedTypeDescription GetProxyTypeDescription(Type serviceApplicationProxyType)
        {
            return new SPPersistedTypeDescription("SharePointEmails Service Proxy", "SharePointEmails Service Proxy");
        }

        public Type[] GetProxyTypes()
        {
            return new Type[] { typeof(SharePointEmailsServiceApplicationProxy) };
        }
        #endregion IServiceProxyAdministration

        internal SharePointEmailsServiceApplicationProxy GetApplicationProxy(string name)
        {
            return this.ApplicationProxies.GetValue<SharePointEmailsServiceApplicationProxy>(name);
        }

        internal static SharePointEmailsServiceProxy GetServiceProxyByFarm(SPFarm farm)
        {
            SPServiceProxyCollection serviceProxies = farm.ServiceProxies;
            if (serviceProxies == null)
            {
                throw new InvalidOperationException("The SharePoint farm has not been provisioned properly.");
            }

            SharePointEmailsServiceProxy proxy = serviceProxies.GetValue<SharePointEmailsServiceProxy>();

            return proxy;
        }

        internal static SharePointEmailsServiceProxy Local
        {
            get
            {
                return GetServiceProxyByFarm(LocalFarm);
            }
        }

        private static SPFarm LocalFarm
        {
            get
            {
                SPFarm local = SPFarm.Local;
                if (local == null)
                {
                    throw new InvalidOperationException("The SharePoint farm has not been provisioned properly.");
                }
                return local;
            }
        }
    }
}
