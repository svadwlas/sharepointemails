using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Diagnostics;
using Microsoft.SharePoint.Administration;

namespace SharepointEmails.Features.SharePointEmailsServiceApplicationFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("2ff340ec-4047-4539-a363-c548997fefc2")]
    public class SharePointEmailsServiceApplicationFeatureEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
           // System.Diagnostics.Debugger.Launch();

            SPFarm farm = SPFarm.Local;
           

            // check if the service has already been installed in the farm
            var service = SharePointEmailsService.Local;
            if (service == null)
            {
                // create the service
                service = new SharePointEmailsService(farm);
                service.Status = SPObjectStatus.Online;
                service.Update();
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
           // System.Diagnostics.Debugger.Launch();
            SPFarm farm = SPFarm.Local;
            SPServer server = SPServer.Local;

            //Remove all instances
            var instance = SharePointEmailsServiceInstance.Local;
            while (instance != null)
            {
                server.ServiceInstances.Remove(instance.Id);
                instance = SharePointEmailsServiceInstance.Local;
            }    //Uninstall the service proxy
            var serviceProxy = SharePointEmailsServiceProxy.Local;
            if (serviceProxy != null)
            {
                farm.ServiceProxies.Remove(serviceProxy.Id);
            }

            //Remove service and jobs
            var service = SharePointEmailsService.Local;
            if (service != null)
                farm.Services.Remove(service.Id);
            //System.Diagnostics.Debugger.Launch();
            //// uninstall the instance
            //SharePointEmailsServiceInstance serviceInstance = SharePointEmailsServiceInstance.Local;
            //if (serviceInstance != null)
            //    SPServer.Local.ServiceInstances.Remove(serviceInstance.Id);

            //// uninstall the service
            //SharePointEmailsService service = SharePointEmailsService.Local;
            //if (service != null)
            //    SPFarm.Local.Services.Remove(service.Id);
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
