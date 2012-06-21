using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public interface ISearchContext
    {
        Guid SiteId
        {
            get;
            set;
        }

        SPSite Site
        {
            get;
        }

        Guid WebId
        {
            get;
            set;
        }

        int Match(ITemplate template);
    }

    public class SearchMatchLevel
    {
        /// <summary>
        /// Exactly by Id
        /// </summary>
        public const int ITEM_BY_ID = 1000;
        /// <summary>
        /// Folder/Discusion is Matched by ID
        /// </summary>
        public const int FOLDER_DISC_BY_ID = 900;

        /// <summary>
        /// List/Library Matched By ID
        /// </summary>
        public const int LIST_LIB_BY_ID = 800;

        /// <summary>
        /// Web matched by ID by parent web
        /// </summary>
        public const int Web_BY_ID = 700;

        /// <summary>
        /// Item is matched By ContentTypeID exactly
        /// </summary>
        public const int ITEM_BY_CT_ID_EXACTLY = 600;

        /// <summary>
        /// Item parent (Folder,Discusion) is matched By ContentTypeID exactly
        /// </summary>
        public const int PARENT_BY_CT_ID_EXACTLY = 550;

        /// <summary>
        /// Item is matched By ContentTypeID by inheritance
        /// </summary>
        public const int ITEM_BY_CT_ID_INHERITED = 530;

        /// <summary>
        /// Parent (item, folder) is matched By ContentTypeID by inheritance
        /// </summary>
        public const int PARENT_BY_CT_ID_INHERITED = 520;

        /// <summary>
        /// Item exactly by group matched
        /// </summary>
        public const int ITEM_BY_GROUP = 500;

        /// <summary>
        /// Parent (folder,discusion) by group matched
        /// </summary>
        public const int PARENT_BY_GROUP = 450;

        /// <summary>
        /// List by group matched
        /// </summary>
        public const int LIST_BY_GROUP = 450;

        /// <summary>
        /// Web matched by group
        /// </summary>
        public const int WEB_BY_GROUP = 400;

        public const int NONE = -1;
    }

}
