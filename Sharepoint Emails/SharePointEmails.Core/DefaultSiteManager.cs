using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public class SiteManager : ISiteManager
    {
        public SPList CreateHiddenTemplatesList(SPWeb web)
        {
            var list = web.Lists.TryGetList(Constants.TemplateListName);
            if (list == null)
            {
                var guid = web.Lists.Add(Constants.TemplateListName, Constants.TemplateListName, "Lists/" + Constants.TemplateListName, SEMailTemplateCT.FeatureId, 10000,"100");
                web.Update();
                list = web.Lists.TryGetList(Constants.TemplateListName);
            }
            else
            {
                AddNeededContentTypes(list);
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

        private void AddNeededContentTypes(SPList list)
        {
            var ct = list.ParentWeb.ContentTypes[new SPContentTypeId(SEMailTemplateCT.CTId)];
            if (ct != null)
            {
                list.ContentTypes.Add(ct);
            }
            list.Update();
        }
    }
}
