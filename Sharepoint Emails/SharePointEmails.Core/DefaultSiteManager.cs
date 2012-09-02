using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    internal class SiteManager : ISiteManager
    {
        public SPList CreateHiddenTemplatesList(SPWeb web)
        {
            var list = web.Lists.TryGetList(Constants.TemplateListName);
            if (list == null)
            {
                var guid = web.Lists.Add(Constants.TemplateListName, Constants.TemplateListName, "Lists/" + Constants.TemplateListName, TemplateCT.FeatureId, 10000, "100");
                
                web.Update();
                list = web.Lists[guid];
                foreach (var s in new string[] { TemplateCT.TemplateBodyFile, TemplateCT.TemplateSubjectFile })
                {
                    list = web.Lists.TryGetList(Constants.TemplateListName);
                    var lookup = web.Lists.TryGetList(Constants.XsltLibrary);
                    list.Fields.Delete(s);
                    list.Update();
                    list.Fields.AddLookup(s, lookup.ID, false);
                    list.Fields[s].Update();
                    list.Update();
                }
            }
            else
            {
                AddNeededContentTypes(list);
            }
            return list;
        }

        public SPList CreateXsltTemplatesList(SPWeb web)
        {
            var list = web.Lists.TryGetList(Constants.XsltLibrary);
            if (list == null)
            {
                var guid = web.Lists.Add(Constants.XsltLibrary, Constants.XsltLibrary, SPListTemplateType.DocumentLibrary);
                web.Update();
                list = web.Lists.TryGetList(Constants.XsltLibrary);
            }
            else
            {
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
            var ct = list.ParentWeb.ContentTypes[new SPContentTypeId(TemplateCT.CTId)];
            if (ct != null)
            {
                list.ContentTypes.Add(ct);
            }
            list.Update();
        }
    }
}
