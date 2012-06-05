using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core
{
    public  class SearchContext:ISearchContext
    {
        public Guid SiteId
        {
            get;
            set;
        }

        public Guid WebId
        {
            get;
            set;
        }

        public Guid ListId
        {
            get;
            set;
        }

        public Guid ItemId
        {
            get;
            set;
        }

        public bool Is()
        {
            throw new NotImplementedException();
        }


        public string ContentTypeId
        {
            get;
            set;
        }


        public GroupType ItemType
        {
            get;
            set;
        }
    }
}
