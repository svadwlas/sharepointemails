using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.IO;
using Microsoft.SharePoint.WebControls;

namespace SharePointEmails.Core.Extensions
{
    public static class SPObjectsHelper
    {
        public static string GetAttachmentContent(this SPListItem item, string filename)
        {
            string fileName = null;
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
        public static string GetLookupFileContent(this SPListItem item, string fieldName)
        {
            var lookupValue = new SPFieldLookupValue(item[fieldName].ToString());
            var listId = new Guid(((SPFieldLookup)item.Fields.GetFieldByInternalName(fieldName)).LookupList);
            var list = item.Web.Lists[listId];
            var litem = list.GetItemById(lookupValue.LookupId);
            using (var reader = new StreamReader(litem.File.OpenBinaryStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static bool ShoulBeValidated(this SPField item, string dependentField, bool shouldOnDisabled = true)
        {
            if (SPContext.Current.FormContext != null)
            {
                foreach (BaseFieldControl field in SPContext.Current.FormContext.FieldControlCollection)
                {
                    if (field.FieldName == dependentField && ((bool)field.Value && shouldOnDisabled || (!((bool)field.Value) && !shouldOnDisabled)))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        public static string GetValueFromTextFieldOrFile(this SPListItem item, bool getFromFile, string textfield, string fileField, out bool attached)
        {
            attached = false;
            if (getFromFile)
            {
                return item.GetLookupFileContent(fileField) ?? "";
            }
            else
            {
                var res = item[textfield] as string;
                var content = item.GetAttachmentContent(res);
                if (content != null)
                {
                    attached = true;
                    res = content;
                }
                return res;
            }
        }

        public static string GetValueFromTextFieldOrFile(this SPListItem item, bool getFromFile, string textfield, string fileField)
        {
            bool a;
            return item.GetValueFromTextFieldOrFile(getFromFile, textfield, fileField,out a);
        }

        public static T GetFieldValue<T>(this SPListItem item, string fieldName, T defaultValue = default(T))
        {
            var obj = (item.Fields.ContainsFieldWithStaticName(fieldName)) ? item[fieldName] : null;
            return obj == null ? defaultValue : (T)obj;
        }
        public static T GetFieldValue<T>(this SPListItem item, Guid fieldGuid , T defaultValue = default(T))
        {
            var obj = (item.Fields.Contains(fieldGuid)) ? item[fieldGuid] : null;
            return obj == null ? defaultValue : (T)obj;
        }

        public static SPUser GetUserByEmail(this SPWeb web, string email)
        {
            return web.SiteUsers.GetByEmail(email);
        }
    }
}
