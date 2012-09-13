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
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;
using System.Configuration;

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

        public SubstitutionContext(string eventData, SPList sourceList, int itemId, string modifierName, string receiverEmail, int alertCreatorId, SPEventType eventType)
        {
            Logger = Application.Current.Logger;
            m_eventType = eventType;
            m_sourceList = sourceList;

            Vars = new ContextVars(sourceList, itemId, modifierName, receiverEmail, alertCreatorId);
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

        string GetListEmail(SPList list)
        {
            try
            {
                if (list != null)
                {
                    var alias = list.EmailAlias;
                    if (!string.IsNullOrEmpty(alias))
                    {
                        var service = SPFarm.Local.GetChild<SPIncomingEmailService>();
                        if (service != null)
                        {
                            var serverAdress = service.ServerDisplayAddress;
                            if (!string.IsNullOrEmpty(serverAdress))
                            {
                                return string.Format("{0}@{1}", alias, serverAdress);
                            }
                            else
                            {
                                Logger.Write("No email server address for list ", SeverityEnum.Warning);
                            }
                        }
                        else
                        {
                            Logger.Write("Cannot get incoming email service", SeverityEnum.Warning);
                        }
                    }
                    else
                    {
                        Logger.Write("No email alias for list ", SeverityEnum.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, SeverityEnum.CriticalError);
            }
            return string.Empty;
        }

        string GetAdminEmail(SPList list)
        {
            try
            {
                if (list != null)
                {
                    var web = list.ParentWeb;
                    var adminEmail = web.SiteAdministrators.OfType<SPUser>().Where(p => !string.IsNullOrEmpty(p.Email)).Select(p => p.Email).FirstOrDefault();
                    if (!string.IsNullOrEmpty(adminEmail))
                    {
                        return adminEmail;
                    }
                    else
                    {
                        Logger.Write("No admins with emails", SeverityEnum.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, SeverityEnum.CriticalError);
            }
            return string.Empty;
        }

        public string GetXML()
        {
            XDocument res = new XDocument();
            res.Add(new XElement("Data"));
            var eventData = new XElement("EventData");
            res.Root.Add(eventData);
            eventData.SetAttributeValue("EventType", (int)m_eventType);
            eventData.SetAttributeValue("EventTypeName", m_eventType.ToString());
            eventData.SetAttributeValue("ListEmail",GetListEmail(Vars.SList));
            eventData.SetAttributeValue("AdminEmail", GetAdminEmail(Vars.SList) );
            foreach (var change in Changes)
            {
                var el = new XElement("Field");
                el.SetAttributeValue("Type", change.FieldType ?? string.Empty);
                el.SetAttributeValue("DisplayName", change.FieldDisplayName??change.FieldName ?? string.Empty);
                el.SetAttributeValue("Name", change.FieldName ?? string.Empty);
                el.SetAttributeValue("Changed", change.IsChanged);
                el.SetAttributeValue("New", (change.GetText(new ModifiersCollection { Modifier.New }) ?? string.Empty));
                el.SetAttributeValue("Old", (change.GetText(new ModifiersCollection { Modifier.Old }) ?? string.Empty));
                el.SetAttributeValue("Value", change.GetText(ModifiersCollection.Empty) ?? string.Empty);
                if (Vars.SList != null)
                {
                    if (Vars.SList.Fields.ContainsFieldWithStaticName(change.FieldName))
                    {
                        var hidden = Vars.SList.Fields.GetFieldByInternalName(change.FieldName).Hidden;
                        el.SetAttributeValue("Hidden", hidden);
                    }
                }
                eventData.Add(el);
            }


            if (Vars.SItem != null && Vars.DUser != null)
            {
                var approve = new XElement("Approve");
                approve.SetAttributeValue("RejectUrl", "http://google.com/");
                approve.SetAttributeValue("ApproveUrl", "http://google.com?search=reject");
                approve.SetAttributeValue("PageUrl", "http://google.com?search=pageurl");
                approve.SetAttributeValue("CanApprove", Vars.SItem.DoesUserHavePermissions(Vars.DUser, SPBasePermissions.ApproveItems));
                approve.SetAttributeValue("Status", (Vars.SItem.ModerationInformation != null) ? Vars.SItem.ModerationInformation.Status.ToString() : "");
                eventData.Add(approve);               
            }
            else
            {
                Logger.Write("Approve info not generated", SeverityEnum.Trace);
            }

            var disc = DiscussionBoardXml.Create().GetElement(Vars.SItem);
            if (disc != null)
            {
                eventData.Add(disc);
            }
            return res.ToString();
        }

        public CultureInfo GetDestinationCulture()
        {
            return CultureInfo.CurrentCulture;
        }

        public SPList GetTemplateLibrary()
        {
            if (Vars.SSite!= null)
            {
                return Vars.SSite.RootWeb.Lists.TryGetList(Constants.XsltLibrary);
            }
            return null;
        }
    }

}
