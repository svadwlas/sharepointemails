using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core.Associations
{
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
