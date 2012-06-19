using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Linq;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        const string OLD_VALUE = ":O";
        const string NEW_VALUE = ":N";

        string m_eventData = string.Empty;
        SPListItem m_item = null;

        List<FieldChange> Changes;

        public SubstitutionContext(string eventData)
        {
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

        public string GetField(string fieldName, string modifiers)
        {
            var change=Changes.Where(p => p.FieldDisplayName == fieldName || p.FieldName == fieldName).FirstOrDefault();
            if (change != null)
            {

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
    }
}
