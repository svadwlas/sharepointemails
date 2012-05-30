using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public class RequestManager
    {
        public static RequestManager Instanse
        {
            get
            {
                return new RequestManager();
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
