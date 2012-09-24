using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core.Interfaces
{
    internal interface ISiteManager
    {
        SPList CreateHiddenTemplatesList(SPWeb web);
        SPList CreateXsltTemplatesList(SPWeb web);

        void DeleteHiddenTemplateList(SPWeb web);
    }
}
