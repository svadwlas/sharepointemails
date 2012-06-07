using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public  class SearchContext:ISearchContext
    {

        public static ISearchContext Create(SPListItem item,TemplateTypeEnum type)
        {
            return new SearchContext(item,type);
        }

        SPListItem m_item;

        SearchContext(TemplateTypeEnum type)
        {
            Type = type;
        }
        SearchContext(SPListItem item,TemplateTypeEnum type):this(type)
        {
            m_item = item;
            SiteId=item.Web.Site.ID;
            WebId = item.Web.ID;
        }

        public Guid SiteId {get;set;}

        public Guid WebId{get;set;}

        public TemplateTypeEnum Type { set; get; }

        public int Match(ITemplate template)
        {
            if((template.EventTypes&(int)TemplateTypeEnum.AllItemEvents)!=0)
                return SearchMatchLevel.LOWERMAX;
            if ((Type == TemplateTypeEnum.AllItemEvents)
                && (Contains(TemplateTypeEnum.ItemAdded) || Contains(TemplateTypeEnum.ItemRemoved) || Contains(TemplateTypeEnum.ItemUpdated)))
                return SearchMatchLevel.LOWERMAX;
            if (Contains(template.EventTypes))
                return SearchMatchLevel.MAX;
            return SearchMatchLevel.NONE;
        }

        public bool Contains(int type)
        {
            return (((int)Type & type) != 0);
        }
        public bool Contains(TemplateTypeEnum type)
        {
            return Contains((int)type);
        }
    }
}
