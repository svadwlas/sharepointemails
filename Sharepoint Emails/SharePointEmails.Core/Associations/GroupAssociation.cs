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

        public override bool IsMatch(object obj)
        {
            if (obj == null) return false;
            switch (ItemType)
            {
                case GroupType.None:
                    {
                        return false;
                    }
                case GroupType.AllList:
                    {
                        return ((obj is SPWeb) || (obj is SPListItem));
                    }
                case GroupType.AllDocumentLibrary:
                    {
                        return ((obj is SPDocumentLibrary) || ((obj is SPListItem) && (((SPListItem)obj).ParentList.BaseType == SPBaseType.DocumentLibrary)));
                    }
                case GroupType.AllDiscusionBoard:
                    {
                        return (((obj is SPList) && (((SPList)obj).BaseType == SPBaseType.DiscussionBoard)) || ((obj is SPListItem) && (((SPListItem)obj).ParentList.BaseType == SPBaseType.DiscussionBoard)));
                    }
                case GroupType.AllBlogs:
                    {
                        return ((obj is SPListItem) && ((SPListItem)obj).ContentType.Id.IsChildOf((SPBuiltInContentTypeId.BlogPost)));
                    }
                //case Core.ItemType.MyTask: return ((obj is SPListItem) && ((SPListItem)obj).ContentType.Id.IsChildOf((SPBuiltInContentTypeId.Task))&&(((SPListItem)obj)[]));
                default: return false;
            }
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
