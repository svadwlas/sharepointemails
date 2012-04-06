using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace SharepointEmails
{
    [System.Runtime.InteropServices.Guid("B0656A8B-C2E0-43CF-9A00-6EEF0681FCE5")]
    public class SharePointEmailsServiceInstance : SPIisWebServiceInstance
    {
        private const string SharePointServiceInstanceName = "SharePointEmailsServiceInstance";

        public SharePointEmailsServiceInstance()
        {
        }

        internal SharePointEmailsServiceInstance(SPServer server, SharePointEmailsService service)
            : base(server, service)
        {

        }

        internal SharePointEmailsServiceInstance(string name, SPServer server, SharePointEmailsService service)
            : base(server, service)
        {
        }

        public override string DisplayName
        {
            get
            {
                return SharePointServiceInstanceName;
            }
        }

        internal bool IsLocal
        {
            get
            {
                SPServer local = SPServer.Local;
                SPServer server = base.Server;
                return ((local != null) && (local.Id == server.Id));
            }
        }

        internal static bool IsLocalInstanceOnline
        {
            get
            {
                bool flag;
                try
                {
                    bool isOnline = false;
                    SPServer local = SPServer.Local;
                    if (local != null)
                    {
                        SharePointEmailsServiceInstance instanse = local.ServiceInstances.GetValue<SharePointEmailsServiceInstance>();
                    }
                    flag = isOnline;
                }
                catch
                {
                    throw;
                }
                return flag;
            }
        }

        internal bool IsOnline
        {
            get
            {
                return (base.Status == SPObjectStatus.Online);
            }
        }

        internal static SharePointEmailsServiceInstance Local
        {
            get
            {
                SPServer local = SPServer.Local;
                if (local == null)
                {
                    throw new InvalidOperationException();
                }
                return local.ServiceInstances.GetValue<SharePointEmailsServiceInstance>();
            }
        }

        public override string TypeName
        {
            get
            {
                return SharePointServiceInstanceName;
            }
        }

        public override string Description
        {
            get
            {
                return SharePointServiceInstanceName;
            }
        }

        public override bool Hidden
        {
            get
            {
                return false;
            }
        }
    }
}
