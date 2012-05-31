using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Serialization;

namespace SharePointEmails.Core
{
    [Serializable]
    public class WebConfiguration
    {
        public List<Association> Associations { set; get; }
    }

    [Serializable]
    [XmlInclude(typeof(TypeAssociation))]
    [XmlInclude(typeof(IDAssociation))]
    public abstract class Association
    {
        public Guid ID { set; get; }
        public Association()
        {
            ID = Guid.NewGuid();
        }
        public abstract bool IsMatch(object obj);
    }

    [Serializable]
    public class TypeAssociation : Association
    {
        public ItemType ItemType { set; get; }
        public override bool IsMatch(object obj)
        {
            if (obj == null) return false;
            switch (ItemType)
            {
                case Core.ItemType.None: return false;
                case Core.ItemType.Web: return obj is SPWeb;
                case Core.ItemType.DocumentLibrary: return obj is SPDocumentLibrary;
                case Core.ItemType.DiscusionBoard: return ((obj is SPList) && ((SPList)obj).BaseType == SPBaseType.DiscussionBoard);
                case Core.ItemType.List: return (obj is SPList);
                case Core.ItemType.Message: return ((obj is SPListItem) && ((SPListItem)obj).ContentType.Id.IsChildOf((SPBuiltInContentTypeId.Message)));
                case Core.ItemType.Discussion: return ((obj is SPListItem) && ((SPListItem)obj).ContentType.Id.IsChildOf((SPBuiltInContentTypeId.Discussion)));
                case Core.ItemType.Item: return (obj is SPListItem);
                case Core.ItemType.Post: return ((obj is SPListItem) && ((SPListItem)obj).ContentType.Id.IsChildOf((SPBuiltInContentTypeId.BlogPost)));
                case Core.ItemType.Task: return ((obj is SPListItem) && ((SPListItem)obj).ContentType.Id.IsChildOf((SPBuiltInContentTypeId.Task)));
                //case Core.ItemType.MyTask: return ((obj is SPListItem) && ((SPListItem)obj).ContentType.Id.IsChildOf((SPBuiltInContentTypeId.Task))&&(((SPListItem)obj)[]));
                default: return false;
            }
        }
    }

    [Serializable]
    public enum ItemType
    {
        None,List, Web,Item,DiscusionBoard,DocumentLibrary,Message,Discussion,Post,Task,MyTask
    }

    [Serializable]
    public class IDAssociation : Association
    {
        public Guid ID { set; get; }
        public string ItemName { set; get; }
        public string RelativeItemUrl { set; get; }
        public override bool IsMatch(object obj)
        {
            if ((ID == Guid.Empty)||(obj==null)) return false;
            if (obj is SPList) return ((SPList)obj).ID.Equals(ID);
            if (obj is SPListItem) return ((SPListItem)obj).ID.Equals(ID);
            if (obj is SPWeb) return ((SPWeb)obj).ID.Equals(ID);
            return false;
        }
    }
    
}
