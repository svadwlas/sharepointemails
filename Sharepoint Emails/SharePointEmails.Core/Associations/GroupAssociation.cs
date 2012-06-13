using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core.Associations
{
    [Serializable]
    public class GroupAssociation : Association
    {
        public GroupType ItemType { set; get; }

        public override AssType Type
        {
            get { return AssType.Group; }
        }

        public override int IsMatch(SPList list, SPContentTypeId ctId, int ItemId)
        {
            if (ctId == null) return SearchMatchLevel.NONE;
            switch (ItemType)
            {
                case GroupType.AllList:
                    {
                        if (!(list is SPDocumentLibrary)) return SearchMatchLevel.LIST_BY_GROUP;
                        break;
                    }
                case GroupType.AllDocumentLibrary:
                    {
                        if (list is SPDocumentLibrary) return SearchMatchLevel.LIST_BY_GROUP;
                        break;
                    }
                case GroupType.AllDiscusionBoard:
                    {
                        if (ctId.IsChildOf(SPBuiltInContentTypeId.Discussion) || ctId.IsChildOf(SPBuiltInContentTypeId.Message))
                            return SearchMatchLevel.PARENT_BY_GROUP;
                        break;
                    }
                case GroupType.AllBlogs:
                    {
                        if (ctId.IsChildOf(SPBuiltInContentTypeId.BlogPost) || (ctId.IsChildOf(SPBuiltInContentTypeId.BlogComment)))
                            return SearchMatchLevel.ITEM_BY_GROUP;
                        break;
                    }
            }
            //case Core.ItemType.MyTask: return ((obj is SPListItem) && ((SPListItem)obj).ContentType.Id.IsChildOf((SPBuiltInContentTypeId.Task))&&(((SPListItem)obj)[]));
            return SearchMatchLevel.NONE;
        }

        public override void Validate()
        {
            base.Validate();
            if (ItemType == GroupType.None) throw new Exception("Choose group type");
        }
    }

    [Serializable]
    public enum GroupType
    {
        None = -1,
        AllList = 1,
        AllDiscusionBoard = 2,
        AllDocumentLibrary = 3,
        AllBlogs = 4,
        AllTasks = 5,
        AllMyTasks = 6
    }
}
