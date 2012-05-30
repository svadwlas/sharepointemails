using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public class OwnersManager
    {
        public static OwnersManager Instanse
        {
            get
            {
                return new OwnersManager();
            }
        }
        public ITemplateOwner Create(SPList List)
        {
            throw new System.NotImplementedException();
        }

        public ITemplateOwner Create(SPWeb Web)
        {
            throw new System.NotImplementedException();
        }

        public Guid GetSiteId(ITemplateOwner owner)
        {
            return owner.SiteId;
        }
    }
}
