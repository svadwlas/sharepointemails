using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core
{
    public interface ISearchContext
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

        GroupType ItemType { get; set; }

        string ContentTypeId { get; }
    
        bool Is();
    }
}
