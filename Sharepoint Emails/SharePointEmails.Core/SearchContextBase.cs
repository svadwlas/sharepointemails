using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    class SearchContextBase:ISearchContext
    {
        protected Guid SiteId { set; get; }

        protected Guid WebId { set; get; }

        protected SPContentTypeId ItemContentTypeId { get; set; }

        public TemplateTypeEnum Type { set; get; }

        protected void Init(Guid siteId, Guid webId, SPContentTypeId contentTypeId, TemplateTypeEnum type)
        {
            SiteId = siteId;
            WebId = webId;
            ItemContentTypeId = contentTypeId;
            Type = type;
        }

        public virtual int Match(ITemplate template)
        {

            if (template.State == TemplateStateEnum.Draft) return SearchMatchLevel.NONE;
            if ((template.EventTypes & (int)TemplateTypeEnum.AllItemEvents) != 0)
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
