using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public  class SearchContext:ISearchContext
    {

        public static ISearchContext Create(SPList list, int itemId, string eventData, SPEventType type)
        {
            return new SearchContext(list,itemId,eventData,type);
        }

        TemplateTypeEnum get(SPEventType type)
        {
            switch (type)
            {
                case SPEventType.All: return TemplateTypeEnum.AllItemEvents;
            }
            return TemplateTypeEnum.AllItemEvents;
        }

        SearchContext(SPList list, int itemId, string eventData, SPEventType type)
        {
            List = list;
            Type = get(type);
            ItemId = itemId;
            try
            {
                var item = list.GetItemById(itemId);
                ItemContentTypeId = item.ContentTypeId;
            }
            catch
            {
            }
        }

        SPList List { set; get; }

        public SPContentTypeId ItemContentTypeId { set; get; }

        public int ItemId { set; get; }

        public TemplateTypeEnum Type { set; get; }

        int CheckAsses(ITemplate template)
        {
            int res = SearchMatchLevel.NONE;
            foreach (var ass in template.Asses)
            {
                var m = ass.IsMatch(List, ItemContentTypeId, ItemId);
                if (m > res) res = m;
            }
            return res;
        }

        public int Match(ITemplate template)
        {
            if (template.State == TemplateStateEnum.Draft) return SearchMatchLevel.NONE;
            if ((Type == TemplateTypeEnum.AllItemEvents) && (Contains(template.EventTypes, TemplateTypeEnum.AllItemEvents)
                                                            ||Contains(template.EventTypes, TemplateTypeEnum.ItemAdded)
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

        public Guid SiteId
        {
            get
            {
                return List.ParentWeb.Site.ID;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Guid WebId
        {
            get
            {
                return List.ParentWeb.ID;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public SPSite Site
        {
            get { return List.ParentWeb.Site; }
        }
    }
}
