using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.IO;
using Microsoft.SharePoint.WebControls;

namespace SharePointEmails.Core
{
    public static class SPListItemHelper
    {
        public static string GetAttachmentContent(this SPListItem item, string filename)
        {
            string fileName=null;
            try
            {
                fileName = Path.GetFileName(filename);
            }
            catch (ArgumentException) { }
            if (!string.IsNullOrEmpty(fileName) && item.Attachments != null)
            {
                foreach (string a in item.Attachments)
                {
                    if (!string.IsNullOrEmpty(a) && a.ToLower() == fileName.ToLower())
                    {
                        return item.ParentList.ParentWeb.GetFileAsString(item.Attachments.UrlPrefix + a);
                    }
                }
            }
            return null;
        }
        public static string GetLookupFileContent(this SPListItem item,string fieldName)
        {
            try
            {
                var lookupValue = item[fieldName] as SPFieldLookupValue;
                var listId = new Guid(((SPFieldLookup)item.Fields.GetFieldByInternalName(fieldName)).LookupList);
                var list = item.Web.Lists[listId];
                var litem = list.GetItemById(lookupValue.LookupId);
                using (var reader = new StreamReader(litem.File.OpenBinaryStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool ShoulBeValidated(this SPField item, string dependentField, bool shouldOnDisabled=true)
        {
            if (SPContext.Current.FormContext != null)
            {
                foreach (BaseFieldControl field in SPContext.Current.FormContext.FieldControlCollection)
                {
                    if (field.FieldName == dependentField && ((bool)field.Value&&shouldOnDisabled||(!((bool)field.Value)&&!shouldOnDisabled)))
                    {
                        return  false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
