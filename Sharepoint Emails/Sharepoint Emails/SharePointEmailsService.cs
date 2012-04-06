using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace SharepointEmails
{
    [System.Runtime.InteropServices.Guid("645A1F69-F565-4EF9-BE33-7912667F7A7C")]
    public class SharePointEmailsService : SPIisWebService, IServiceAdministration
    {
        private const string ServiceName = "SharePointEmailsService";
        private static SharePointEmailsService m_Local = null;

        public SharePointEmailsService() { }

        internal SharePointEmailsService(SPFarm farm)
            : base(farm)
        {
            this.Name = ServiceName;
        }


        internal SharePointEmailsServiceApplication CreateApplication(string name, SPIisWebServiceApplicationPool applciationPool)
        {
            SharePointEmailsServiceApplication application = new SharePointEmailsServiceApplication(name, this, applciationPool);
            application.Update();
            application.AddServiceEndpoint("", SPIisWebServiceBindingType.Http);
            application.AddServiceEndpoint("secure", SPIisWebServiceBindingType.Https, "secure");
            return application;
        }

        #region IServiceAdministration

        public SPServiceApplication CreateApplication(string name, Type serviceApplicationType, SPServiceProvisioningContext provisioningContext)
        {
            System.Diagnostics.Debugger.Launch();

            if (!ValidateType(serviceApplicationType))
            {
                return null;
            }
            SharePointEmailsServiceApplication webApplicationByName = this.GetWebApplicationByName(name);
            if ((webApplicationByName == null) && (provisioningContext != null) && (provisioningContext.IisWebServiceApplicationPool != null))
            {
                webApplicationByName = this.CreateApplication(name, provisioningContext.IisWebServiceApplicationPool);
            }
            return webApplicationByName;
        }

        private bool ValidateType(Type serviceApplicationType)
        {
            if (serviceApplicationType != typeof(SharePointEmailsServiceApplication))
            {
                return false;
            }
            return true;
        }

        public SPServiceApplicationProxy CreateProxy(string name, SPServiceApplication serviceApplication, SPServiceProvisioningContext provisioningContext)
        {
            System.Diagnostics.Debugger.Launch();
            if (null == serviceApplication)
            {
                throw new ArgumentNullException("serviceApplication");
            }
            if (serviceApplication.GetType() != typeof(SharePointEmailsServiceApplication))
            {
                throw new NotSupportedException();
            }

            SharePointEmailsServiceProxy serviceProxy = (SharePointEmailsServiceProxy)base.Farm.GetObject(name, base.Farm.Id, typeof(SharePointEmailsServiceProxy));
            if (serviceProxy == null)
            {
                serviceProxy = new SharePointEmailsServiceProxy(base.Farm);
                serviceProxy.Update();
            }
            SharePointEmailsServiceApplicationProxy applicationProxy = SharePointEmailsServiceProxy.Local.GetApplicationProxy(name);
            if (applicationProxy != null)
            {
                return applicationProxy;
            }
            return new SharePointEmailsServiceApplicationProxy(name, serviceProxy, (SharePointEmailsServiceApplication)serviceApplication);
        }

        public SPPersistedTypeDescription GetApplicationTypeDescription(Type serviceApplicationType)
        {
            if (!ValidateType(serviceApplicationType))
            {
                return null;
            }
            return new SPPersistedTypeDescription("SharePointEmails Service Application", "SharePointEmails Service Application");
        }

        public Type[] GetApplicationTypes()
        {
            //System.Diagnostics.Debugger.Launch();

            return new Type[] { typeof(SharePointEmailsServiceApplication) };
        }


        public override SPAdministrationLink GetCreateApplicationLink(Type serviceApplicationType)
        {
            return new SPAdministrationLink("/_admin/SharePointEmailsServiceApplication/CreateApplication.aspx");
        }

        #endregion IServiceAdministration

        internal SharePointEmailsServiceApplicationProxy CreateProxy(string name, SPServiceApplication serviceApplication)
        {
            return (SharePointEmailsServiceApplicationProxy)this.CreateProxy(name, serviceApplication, null);
        }

        internal SharePointEmailsServiceApplication GetWebApplicationByName(string applicationName)
        {
            return EnsureSettingsAttributesInitialization(base.Applications.GetValue<SharePointEmailsServiceApplication>(applicationName));
        }

        private static SharePointEmailsServiceApplication EnsureSettingsAttributesInitialization(SharePointEmailsServiceApplication application)
        {
            if (application != null)
            {
                //application.InitAttributes();
            }
            return application;
        }

        internal static SharePointEmailsService GetServiceByFarm(SPFarm farm)
        {
            if (null == farm)
            {
                throw new InvalidOperationException("The SharePoint farm has not been provisioned properly.");
            }
            return farm.Services.GetValue<SharePointEmailsService>();
        }

        internal SharePointEmailsServiceApplication GetWebApplicationById(Guid applicationId)
        {
            return EnsureSettingsAttributesInitialization(base.Applications.GetValue<SharePointEmailsServiceApplication>(applicationId));
        }

        public override string DisplayName
        {
            get
            {
                System.Diagnostics.Debugger.Launch();
                return ServiceName;
            }
        }

        public static SharePointEmailsService Local
        {
            get
            {
                if (SharePointEmailsService.m_Local == null)
                {
                    SharePointEmailsService.m_Local = SPFarm.Local.Services.GetValue<SharePointEmailsService>(ServiceName);
                }
                return SharePointEmailsService.m_Local;
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

        public override string TypeName
        {
            get
            {
                System.Diagnostics.Debugger.Launch();
                return ServiceName;
            }
        }

    }
}
