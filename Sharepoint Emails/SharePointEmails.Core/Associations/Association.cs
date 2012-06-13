using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.Associations
{
    [Serializable]
    [XmlInclude(typeof(GroupAssociation))]
    [XmlInclude(typeof(IDAssociation))]
    [XmlInclude(typeof(ContentTypeAssociation))]
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
        public abstract int IsMatch(SPList list, SPContentTypeId ctId, int ItemId);
        public abstract AssType Type { get; }

        protected ILogger Logger
        {
            get
            {
                return ClassContainer.Instance.Resolve<ILogger>();
            }
        }

        public virtual void Validate()
        {
            if (string.IsNullOrEmpty(Name)) throw new Exception("Name cannot be empty");
            if (string.IsNullOrEmpty(ID)) throw new Exception("ID cannot be empty");
        }
    }

    

    public enum AssType
    {
        Group = 1, ID = 2, ContentType=3//don't change ids
    }
}
