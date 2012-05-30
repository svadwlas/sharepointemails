using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public class DefaultSiteManager : ISiteManager
    {
        public SPList CreateHiddenTemplatesList(SPWeb web)
        {
            var list=web.Lists.TryGetList(Constants.TemplateListName);
            if (list == null)
            {
                var guid=web.Lists.Add(Constants.TemplateListName, Constants.TemplateListName, SPListTemplateType.GenericList);
                web.Update();
                list=web.Lists.TryGetList(Constants.TemplateListName);
            }
            return list;
        }

        public void DeleteHiddenTemplateList(SPWeb web)
        {
            var list = web.Lists.TryGetList(Constants.TemplateListName);
            if (list != null)
            {
                web.Lists.Delete(list.ID);
                web.Update();
            }
        }
    }
}
