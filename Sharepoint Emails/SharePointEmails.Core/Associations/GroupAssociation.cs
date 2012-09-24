using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Interfaces;

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
                        if (!ctId.IsChildOf(SPBuiltInContentTypeId.Document)) return SearchMatchLevel.LIST_BY_GROUP;
                        break;
                    }
                case GroupType.AllDocumentLibrary:
                    {
                        if (ctId.IsChildOf(SPBuiltInContentTypeId.Document)) return SearchMatchLevel.LIST_BY_GROUP;
                        break;
                    }
                case GroupType.AllDiscusions:
                    {
                        if (ctId.IsChildOf(SPBuiltInContentTypeId.Discussion))
                            return SearchMatchLevel.ITEM_BY_GROUP;
                        break;
                    }
                case GroupType.AllMessages:
                    {
                        if (ctId.IsChildOf(SPBuiltInContentTypeId.Message))
                            return SearchMatchLevel.ITEM_BY_GROUP;
                        break;
                    }
                case GroupType.AllBlogComments:
                    {
                        if ((ctId.IsChildOf(SPBuiltInContentTypeId.BlogComment)))
                            return SearchMatchLevel.ITEM_BY_GROUP;
                        break;
                    }
                case GroupType.AllBlogPosts:
                    {
                          if (ctId.IsChildOf(SPBuiltInContentTypeId.BlogPost))
                            return SearchMatchLevel.ITEM_BY_GROUP;
                        break;
                    }
                case GroupType.AllTasks:
                    {
                        if (ctId.IsChildOf(SPBuiltInContentTypeId.Task))
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

        public override string ToString()
        {
            var s = "Type:" + Type + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "ItemType: " + ItemType + Environment.NewLine;
            return s;
        }

        public override string ValueToShortDisplay
        {
            get { return ItemType.ToString(); }
        }
    }

    [Serializable]
    public enum GroupType
    {
        None = -1,
        AllList = 1,
        AllDiscusions = 2,
        AllDocumentLibrary = 3,
        AllBlogPosts = 4,
        AllTasks = 5,
        AllMyTasks = 6,
        AllMessages = 7,
        AllBlogComments=8
    }
}
