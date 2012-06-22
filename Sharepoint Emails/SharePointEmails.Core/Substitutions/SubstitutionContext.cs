using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Linq;
using SharePointEmails.Core.Substitutions;
using System.Reflection;
using SharePointEmails.Logging;

namespace SharePointEmails.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        ContextVars Vars;
        const string OLD_VALUE = ":O";
        const string NEW_VALUE = ":N";

        string m_eventData = string.Empty;
        SPListItem m_sourceItem = null;
        SPList m_sourceList=null;
        ILogger Logger;

        List<FieldChange> Changes;

        public SubstitutionContext(string eventData):this(eventData,null){}

        public SubstitutionContext(string eventData, SPList sourceList):this(eventData,sourceList,-1,null,null){}

        public SubstitutionContext(string eventData, SPList sourceList, int ItemID, string modifierName, string toemail)
        {
            Logger = ClassContainer.Instance.Resolve<ILogger>();
            m_sourceList = sourceList;
            Vars = new ContextVars(sourceList, ItemID, modifierName, toemail);
            Changes = XDocument.Parse(eventData).Descendants("Field").Select(p => FieldChange.Create(p)).ToList();
        }

        string GetAttValue(XElement el, string name)
        {
            return (el.Attribute(name) == null) ? null : el.Attribute(name).Value;
        }

        bool HasModifier(string all, string test)
        {
            if (string.IsNullOrEmpty(all)) return false;
            return all.Split(':').Contains(test.Trim(':'));
        }

        public string GetField(string fieldName, ModifiersCollection modifiers)
        {
            var change=Changes.Where(p => p.FieldDisplayName == fieldName || p.FieldName == fieldName).FirstOrDefault();
            if (change != null)
            {
                return change.GetText(modifiers);
            }
            else
            {
                return null;
            }
        }

        public List<string> GetAvailableFields()
        {
            var res = new List<string>() { "testField1", "testField2" };

            return res;
        }

        string GetFromObj(object obj, string path)
        {
            object temp = obj;
            foreach (var m in path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if ((temp = temp.GetType().InvokeMember(m, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, temp, new object[0])) == null)
                    return null;
            }
            return (temp == null) ? "" : temp.ToString();
        }

        public string GetContextValue(string value, ModifiersCollection modifiers)
        {
            return GetFromObj(Vars, value);
        }

        class ContextVars
        {
            ILogger Logger;
            public ContextVars(SPList sourceList, int ItemID, string modifierName, string toemail)
            {
                Logger = ClassContainer.Instance.Resolve<ILogger>();
                SList = sourceList;
                if (SList != null)
                {
                    SWeb = SList.ParentWeb;
                    if (SWeb != null)
                    {
                        SSite = SWeb.Site;
                    }
                }
                if (sourceList != null && ItemID != -1)
                {
                    try
                    {
                        SItem = sourceList.GetItemById(ItemID);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex, SeverityEnum.Error);
                    }
                }
                if (!string.IsNullOrEmpty(modifierName) && SWeb != null)
                {
                    try
                    {
                        SUser = SWeb.SiteUsers[modifierName];
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Cannot get SUser", SeverityEnum.Error);
                        Logger.Write(ex, SeverityEnum.Error);
                    }
                }

                if (!string.IsNullOrEmpty(modifierName) && SWeb != null)
                {
                    try
                    {
                        DUser = SWeb.SiteUsers.GetByEmail(toemail);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Cannot get DUser", SeverityEnum.Error);
                        Logger.Write(ex, SeverityEnum.Error);
                    }
                }
            }

            public SPWeb SWeb
            {
                get;
                set;
            }

            public SPSite SSite
            {
                get;
                set;
            }

            public SPUser SUser { set; get; }

            public SPUser DUser { set; get; }

            public SPList SList
            {
                get;
                set;
            }

            public SPListItem SItem
            {
                get;
                set;
            }
        }
    }
}
