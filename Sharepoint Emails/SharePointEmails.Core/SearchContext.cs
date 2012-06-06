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

        public int Match(ITemplate template)
        {
            return -1;
        }
    }
}
