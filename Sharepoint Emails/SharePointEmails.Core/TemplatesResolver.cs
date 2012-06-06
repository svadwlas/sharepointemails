using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Logging;
using SharePointEmails.Core.Exceptions;

namespace SharePointEmails.Core
{
    public class DefaultTemplatesManager : ITemplatesManager
    {
        ILogger Logger { set; get; }

        public DefaultTemplatesManager(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            Logger = logger;
        }

        private ITemplate FindTemplateRec(ISearchContext context, SPWeb web,Dictionary<ITemplate,int> matched,int deep=0)
        {
            if (web == null)
            {
                if (matched.Count == 0)
                {
                    throw new SeBaseException("Template is not found");
                }
                else
                {
                    return matched.OrderBy(p => p.Value).Last().Key;
                }
            }
            var list = web.Lists.TryGetList(Constants.TemplateListName);
            if (list != null)
            {
                foreach (var item in new TemplatesList(list))
                {
                    var res=context.Match(item);
                    if (res != SearchMatchLevel.NONE)
                    {
                        matched[item] = res+deep;
                    }
                }
            }
           return FindTemplateRec(context, web.ParentWeb,matched,++deep);
        }

        public ITemplate GetTemplate(ISearchContext context)
        {
            using (var site = new SPSite(context.SiteId))
            {
                using (var web = site.AllWebs[context.WebId])
                {
                    return FindTemplateRec(context, web, new Dictionary<ITemplate, int>());
                }
            }
        }
    }

    class TemplatesList:IEnumerable<ITemplate>
    {
        SPList _list;
        public TemplatesList(SPList list)
        {
            _list=list;
        }

        public IEnumerator<ITemplate> GetEnumerator()
        {
            foreach (SPListItem item in _list.Items)
            {
                var template= new Template(item);
                yield return template;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
