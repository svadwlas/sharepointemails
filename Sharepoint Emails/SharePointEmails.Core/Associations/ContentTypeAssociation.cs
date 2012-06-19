using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core.Associations
{
    [Serializable]
    public class ContentTypeAssociation : Association
    {
        public string ContentTypeID { set; get; }
        public bool IncludingChilds { set; get; }

        public override int IsMatch(Microsoft.SharePoint.SPList list, SPContentTypeId ctId, int ItemId)
        {
            if (string.IsNullOrEmpty(ContentTypeID)) return SearchMatchLevel.NONE;
            SPContentTypeId ct;
            try
            {
                ct = new SPContentTypeId(ContentTypeID);
                    if (ct.CompareTo(ctId) == 0)
                    {
                        return SearchMatchLevel.ITEM_BY_CT_ID_EXACTLY;
                    }
                    if ((IncludingChilds) && (ctId.IsChildOf(ct) || (ct.CompareTo(ctId) == 0)))
                    {
                        return SearchMatchLevel.ITEM_BY_CT_ID_INHERITED;
                    }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, Logging.SeverityEnum.Error);
            }
            return SearchMatchLevel.NONE;
        }

        public override AssType Type
        {
            get { return AssType.ContentType; }
        }

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(ContentTypeID)) throw new Exception("Specify content type id");
            SPContentTypeId ct;
            try
            {
                ct = new SPContentTypeId(ContentTypeID);
            }
            catch (Exception ex)
            {
                Logger.Write(ex, Logging.SeverityEnum.Warning);
                throw new Exception("wrong content type id");
            }
            var notexists = true;
            foreach (SPContentType c in SPContext.Current.Site.RootWeb.ContentTypes)
            {
                if (c.Id == ct)
                    notexists = false;
            }
            if (notexists)
                throw new Exception("Content type doesn't exist");
        }

        public override string ToString()
        {
            var s = "Type:" + Type + Environment.NewLine +
                "Name: " + Name + Environment.NewLine +
                "ContentTypeID: " + ContentTypeID + Environment.NewLine +
                "IncludingChilds: " + IncludingChilds + Environment.NewLine;
            return s;
        }
    }
}
