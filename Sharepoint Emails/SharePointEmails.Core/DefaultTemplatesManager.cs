using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Logging;
using SharePointEmails.Core.Exceptions;
using SharePointEmails.Core.Configuration;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core
{
    public class DefaultTemplatesManager : ITemplatesManager
    {
        ILogger Logger { set; get; }

        IConfigurationManager ConfigManager { set; get; }

        public DefaultTemplatesManager(ILogger logger, IConfigurationManager configManager)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (configManager == null) throw new ArgumentNullException("configManager");
            Logger = logger;
            ConfigManager = configManager;
        }

        private ITemplate FindTemplateRec(ISearchContext context, SPWeb web, Dictionary<ITemplate, int> matched, int deep = 0)
        {
            if (web == null)
            {
                if (matched.Count == 0)
                {
                    throw new SeTemplateNotFound("Template is not found");
                }
                else
                {
                    return matched.OrderBy(p => p.Value).Last().Key;
                }
            }
            if (EnabledAndfeatureActivated(web))
            {
                var list = web.Lists.TryGetList(Constants.TemplateListName);
                if (list != null)
                {
                    foreach (var item in TemplatesList.Create(list))
                    {
                        var res = context.Match(item);
                        if (res != SearchMatchLevel.NONE)
                        {
                            matched[item] = res + deep;
                        }
                    }
                }
            }

            return FindTemplateRec(context, web.ParentWeb, matched, ++deep);
        }

        bool EnabledAndfeatureActivated(SPWeb web)
        {
            var config = ConfigManager.GetConfig(web);
            if (config == null)
                return false;

            if (config.Disabled) return false;
            return true;
        }

        public ITemplate GetTemplate(ISearchContext context)
        {
                return FindTemplateRec(context,context.Site.RootWeb,new Dictionary<ITemplate,int>());
                //currently only on the root web
                //using (var web = site.AllWebs[context.WebId])
                //{
                //    return FindTemplateRec(context, web, new Dictionary<ITemplate, int>());
                //}
        }
    }

    public class TemplatesList:IEnumerable<ITemplate>
    {
        public static IEnumerable<ITemplate> mock = null;
        public static IEnumerable<ITemplate> Create(SPList list)
        {
            return mock??new TemplatesList(list);
        }
        SPList _list;
        internal TemplatesList() { }
        internal TemplatesList(SPList list)
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
