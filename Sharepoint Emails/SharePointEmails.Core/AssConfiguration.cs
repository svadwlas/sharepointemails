using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Serialization;
using System.IO;

namespace SharePointEmails.Core
{
    [Serializable]
    public class TemplateConfiguration
    {
        static XmlSerializer Searilizer
        {

            get
            {
                if (_Serializer == null)
                {
                    _Serializer = new XmlSerializer(typeof(TemplateConfiguration));
                }
                return _Serializer;
            }
        }static XmlSerializer _Serializer;

        public static TemplateConfiguration Parse(string str)
        {
            using (var reader = new StringReader(str))
            {
                return (TemplateConfiguration)Searilizer.Deserialize(reader);
            }
        }

        public static TemplateConfiguration ParseOrDefault(string str)
        {
            try
            {
                return Parse(str);
            }
            catch
            {
                return new TemplateConfiguration();
            }
        }

        public static TemplateConfiguration Empty { get { return new TemplateConfiguration(); } }

        public override string ToString()
        {
            var value = new StringBuilder();
            using (var writer = new StringWriter(value))
            {
                Searilizer.Serialize(writer, this);
            }
            return value.ToString();
        }

        public List<Association> Associations { set; get; }

        public TemplateConfiguration()
        {
            Associations = new List<Association>();
        }
    }

    public enum AssType
    {
        Group=1, ID=2//don't change ids
    }

    [Serializable]
    [XmlInclude(typeof(GroupAssociation))]
    [XmlInclude(typeof(IDAssociation))]
    public abstract class Association
    {
        public string ID { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        
        public Association()
        {
            ID = Guid.NewGuid().ToString();
            Name = "Ass" + ID;
        }
        public abstract bool IsMatch(object obj);
        public abstract AssType Type { get; }
    }

    [Serializable]
    public class GroupAssociation : Association
    {
        public GroupType ItemType { set; get; }

        public override AssType Type
        {
            get { return  AssType.Group; }
        }

        public override bool IsMatch(object obj)
        {
            if (obj == null) return false;
            switch (ItemType)
            {
                case Core.GroupType.None:
                    {
                        return false;
                    }
                case Core.GroupType.AllList:
                    {
                        return ((obj is SPWeb)||(obj is SPListItem));
                    }
                case Core.GroupType.AllDocumentLibrary:
                    {
                        return ((obj is SPDocumentLibrary)||((obj is SPListItem)&&(((SPListItem)obj).ParentList.BaseType==SPBaseType.DocumentLibrary)));
                    }
                case Core.GroupType.AllDiscusionBoard:
                    {
                        return (((obj is SPList)&&(((SPList)obj).BaseType==SPBaseType.DiscussionBoard)) || ((obj is SPListItem) && (((SPListItem)obj).ParentList.BaseType==SPBaseType.DiscussionBoard)));
                    }
                case Core.GroupType.AllBlogs:
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
        None=-1,
        AllList=1,
        AllDiscusionBoard=2,
        AllDocumentLibrary=3,
        AllBlogs=4,
        AllTasks=5, 
        AllMyTasks=6
    }

    [Serializable]
    public class IDAssociation : Association
    {
        public Guid ItemID { set; get; }
        public string ItemName { set; get; }
        public string ItemType { set; get; }

        public override AssType Type
        {
            get { return AssType.ID; }
        }

        public string RelativeItemUrl { set; get; }
        public override bool IsMatch(object obj)
        {
            if ((ItemID == Guid.Empty) || (obj == null)) return false;
            if (obj is SPList) return ((SPList)obj).ID.Equals(ID);
            if (obj is SPListItem) return ((SPListItem)obj).ID.Equals(ID);
            if (obj is SPWeb) return ((SPWeb)obj).ID.Equals(ID);
            return false;
        }
    }
    
}
