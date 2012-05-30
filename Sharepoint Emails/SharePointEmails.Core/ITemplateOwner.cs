using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core
{
    public interface ITemplateOwner
    {
        Guid SiteId
        {
            get;
            set;
        }

        Guid WebId
        {
            get;
            set;
        }

        Guid ListId
        {
            get;
            set;
        }

        Guid ItemId
        {
            get;
            set;
        }
    
        bool Is();
    }
}
