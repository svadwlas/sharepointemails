using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Associations
{
    [Serializable]
    public class ContentTypeAssociation : Association
    {
        public string ContentTypeID { set; get; }
        public bool IncludingChilds { set; get; }
        public string ContentTypeName { set; get; }

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
                    if ((IncludingChilds) && (ct.IsChildOf(ctId) || (ct.CompareTo(ctId) == 0)))
                    {
                        return SearchMatchLevel.ITEM_BY_CT_ID_INHERITED;
                    }
            }
            catch (Exception ex)
            {
                Logger.WriteTrace(ex, Logging.SeverityEnum.Error);
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
                Logger.WriteTrace(ex, Logging.SeverityEnum.Warning);
                throw new Exception("wrong content type id");
            }
            var notexists = true;
            if (notexists)
            {
                foreach (SPWeb web in SPContext.Current.Site.AllWebs)
                {
                    foreach(SPContentType c in web.ContentTypes)
                    {
                        if(c.Id==ct)
                        {
                            notexists=false;
                            ContentTypeName = c.Name;
                            break;
                        }
                    }
                    foreach (SPList list in web.Lists)
                    {
                        foreach (SPContentType c in list.ContentTypes)
                        {
                            if (c.Id == ct)
                            {
                                notexists = false;
                                ContentTypeName = c.Name;
                                break;
                            }
                        }
                    }
                    web.Dispose();
                }
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

        public override string ValueToShortDisplay
        {
            get { return ContentTypeName ?? ContentTypeID; }
        }
    }
}
