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

        int Match(ITemplate template);

    }

    public class SearchMatchLevel
    {
        public const int MAX = 1000;
        public const int NONE = -1;
        public const int MIDLE = 300;
    }

}
