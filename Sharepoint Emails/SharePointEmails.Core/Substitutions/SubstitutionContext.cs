using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Linq;
using SharePointEmails.Core.Substitutions;
using System.Reflection;
using SharePointEmails.Logging;
using System.Globalization;

namespace SharePointEmails.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        ContextVars Vars;

        string m_eventData = string.Empty;
        SPList m_sourceList=null;
        ILogger Logger;

        SPEventType m_eventType = SPEventType.All;

        public List<FieldChange> Changes {private set; get; }

        public SubstitutionContext(string eventData):this(eventData,null){}

        public SubstitutionContext(string eventData, SPList sourceList):this(eventData,sourceList,-1,null,null,-1,SPEventType.All){}

        public SubstitutionContext(string eventData, SPList sourceList, int ItemID, string modifierName, string toemail, int CreateUserId,SPEventType eventType)
        {
            Logger = Application.Current.Logger;
            m_eventType = eventType;
            m_sourceList = sourceList;
            Vars = new ContextVars(sourceList, ItemID, modifierName, toemail, CreateUserId);
            Changes = (!string.IsNullOrEmpty(eventData)) ? XDocument.Parse(eventData).Descendants("Field").Select(p => FieldChange.Create(p)).ToList() : new List<FieldChange>();
        }

        string GetValueFromObjByPath(object obj, string path)
        {
            object temp = obj;
            foreach (var m in path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if ((temp = temp.GetType().InvokeMember(m, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, temp, new object[0])) == null)
                    return "";
            }
            return (temp == null) ? "" : temp.ToString();
        }

        public string GetField(string fieldName, ModifiersCollection modifiers = null)
        {
            var change = Changes.Where(p => p.FieldDisplayName == fieldName || p.FieldName == fieldName).FirstOrDefault();
            return (change != null)?change.GetText(modifiers ?? ModifiersCollection.Empty):null;
        }

        public string GetContextValue(string value, ModifiersCollection modifiers)
        {
            return GetValueFromObjByPath(Vars, value);
        }

        public static string GetTestXML()
        {
            XDocument res = new XDocument();
            res.Add(new XElement("Data"));
            var eventData = new XElement("EventData");
            res.Root.Add(eventData);
            eventData.SetAttributeValue("EventType", (int)SPEventType.Modify);
            eventData.SetAttributeValue("EventTypeName", SPEventType.Modify.ToString());
            var b = true;
            foreach (var change in new string[] { "Title", "FileName", "YesNoField" })
            {
                var el = new XElement("Field");
                el.SetAttributeValue("Type", "TypeOf"+change);
                el.SetAttributeValue("DisplayName", change);
                el.SetAttributeValue("Name", "_" + change);
                el.SetAttributeValue("Changed", b = !b);
                el.SetAttributeValue("New", "new value of " + change);
                el.SetAttributeValue("Old", "old value of " + change);
                el.SetAttributeValue("Value", "some value of " + change);
                eventData.Add(el);
            }
            return res.ToString();
        }

        public string GetXML()
        {
            XDocument res = new XDocument();
            res.Add(new XElement("Data"));
            var eventData = new XElement("EventData");
            res.Root.Add(eventData);
            eventData.SetAttributeValue("EventType", (int)m_eventType);
            eventData.SetAttributeValue("EventTypeName", m_eventType.ToString());
            foreach (var change in Changes)
            {
                var el = new XElement("Field");
                el.SetAttributeValue("Type", change.FieldType??string.Empty);
                el.SetAttributeValue("DisplayName", change.FieldDisplayName ?? string.Empty);
                el.SetAttributeValue("Name", change.FieldName ?? string.Empty);
                el.SetAttributeValue("Changed", change.IsChanged);
                el.SetAttributeValue("New", (change.GetText(new ModifiersCollection { Modifier.New }) ?? string.Empty));
                el.SetAttributeValue("Old", (change.GetText(new ModifiersCollection { Modifier.Old }) ?? string.Empty));
                el.SetAttributeValue("Value", change.GetText(ModifiersCollection.Empty) ?? string.Empty);
                eventData.Add(el);
            }
            return res.ToString();
        }

        public CultureInfo getDestinationCulture()
        {
            return CultureInfo.CurrentCulture;
        }

        class ContextVars
        {
            ILogger Logger;
            public ContextVars(SPList sourceList, int ItemID, string modifierName, string toemail, int CUserID)
            {
                Logger = Application.Current.Logger;
                SList = sourceList;
                if (SList != null)
                {
                    SWeb = SList.ParentWeb;
                    if (SWeb != null)
                    {
                        SSite = SWeb.Site;
                    }
                }
                if (CUserID != -1 && SWeb != null)
                {
                    try
                    {
                        CUser = SWeb.SiteUsers[CUserID];
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex, SeverityEnum.Error);
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

            public SPUser SUser { set; get; }//force alert

            public SPUser DUser { set; get; }//alert destination

            public SPUser CUser { set; get; }//create alert

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
