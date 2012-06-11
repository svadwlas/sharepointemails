using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public  class SearchContext:ISearchContext
    {

        public static ISearchContext Create(Guid siteId, Guid webId, Guid listId, int itemId, string eventData, TemplateTypeEnum type)
        {
            return new SearchContext(siteId,webId,listId,itemId,eventData,type);
        }

        SearchContext(Guid siteId, Guid webId, Guid listId, int itemId, string eventData, TemplateTypeEnum type)
        {
            SiteId = siteId;
            WebId = webId;
            Type = type;
            ItemId = itemId;
            using (var site = new SPSite(siteId))
            {
                using (var web = site.OpenWeb(webId))
                {
                    SPList list = null;
                    list = web.Lists[listId];
                    ListType = list.BaseType;
                }
            }
        }

        public Guid SiteId {get;set;}

        public Guid WebId{get;set;}

        public SPContentTypeId ItemContentTypeId { set; get; }

        public int ItemId { set; get; }

        public TemplateTypeEnum Type { set; get; }

        public SPBaseType ListType { set; get; }

        int CheckAsses(ITemplate template)
        {
            return SearchMatchLevel.MAX;
        }

        public int Match(ITemplate template)
        {
            if (template.State == TemplateStateEnum.Draft) return SearchMatchLevel.NONE;
            if ((Type == TemplateTypeEnum.AllItemEvents) && (Contains(template.EventTypes, TemplateTypeEnum.ItemAdded)
                                                            || Contains(template.EventTypes, TemplateTypeEnum.ItemRemoved)
                                                            || Contains(template.EventTypes, TemplateTypeEnum.ItemUpdated)))
            {
                return CheckAsses(template);
            }
            else
            {
                return SearchMatchLevel.NONE;
            }
        }

        public bool Contains(int parent,int type)
        {
            return (((int)parent & type) != 0);
        }
        public bool Contains(int parent, TemplateTypeEnum type)
        {
            return (((int)parent & (int)type) != 0);
        }
        public bool Contains(TemplateTypeEnum parent, TemplateTypeEnum type)
        {
            return Contains((int)parent,(int)type);
        }
    }
}
