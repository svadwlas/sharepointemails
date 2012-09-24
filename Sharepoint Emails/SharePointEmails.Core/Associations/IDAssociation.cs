using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Associations
{
    [Serializable]
    public class IDAssociation : Association
    {
        public static string GetId(Guid listGuid, int itemId)
        {
            return listGuid.ToString().ToLower()+":" + itemId;
        }

        public string ItemID { set; get; }
        public string ItemName { set; get; }
        public string ItemType { set; get; }

        public override AssType Type
        {
            get { return AssType.ID; }
        }

        public string RelativeItemUrl { set; get; }

        private SPListItem GetParent(SPList list, int itemId)
        {
            try
            {
                var item = list.GetItemById(itemId);
                return item.Folder.ParentFolder.Item;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        bool Compare(string id)
        {
            return this.ItemID.ToLower() == id.ToLower();
        }

        public override int IsMatch(SPList list, SPContentTypeId ctId, int itemId)
        {
            var itemID = GetId(list.ID, itemId);
            if (Compare(itemID))
                return SearchMatchLevel.ITEM_BY_ID;
            else
            {
                var parent = GetParent(list, itemId);
                while (parent != null)
                {
                    var id = GetId(list.ID, parent.ID);
                    if (Compare(id))
                    {
                        return SearchMatchLevel.FOLDER_DISC_BY_ID;
                    }
                }
                if (Compare(list.ID.ToString()))
                    return SearchMatchLevel.LIST_LIB_BY_ID;
                else
                {
                    var web = list.ParentWeb;
                    while (web != null)
                    {
                        if (Compare(list.ParentWeb.ID.ToString()))
                            return SearchMatchLevel.Web_BY_ID;
                    }
                }
            }

            return SearchMatchLevel.NONE;
        }

        public override string ToString()
        {
            var s = "Type:" + Type + Environment.NewLine +
              "Name: " + Name + Environment.NewLine +
              "ItemName: " + ItemName + Environment.NewLine +
              "ItemType: " + ItemType + Environment.NewLine +
              "ItemID: " + ItemID + Environment.NewLine;
            return s;
        }

        public override string ValueToShortDisplay
        {
            get { return ItemName ?? ItemID; }
        }
    }
}
