using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public interface ISubstitutionContext
    {
        SPList List
        {
            get;
            set;
        }

        SPSite Site
        {
            get;
            set;
        }

        SPListItem ListItem
        {
            get;
            set;
        }
    }
}
