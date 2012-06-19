using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharePointEmails.Core.Substitutions
{
    class FieldChange
    {
        public static FieldChange Create(XElement field)
        {
            var type = GetAttValue(field, "Type");
            switch (type)
            {
                case "user": return new UserFieldChange(field);
                default: return new FieldChange(field);
            }
        }
        public static string GetAttValue(XElement el, string name)
        {
            return (el.Attribute(name) == null) ? null : el.Attribute(name).Value;
        }
        protected FieldChange(XElement field)
        {
            FieldDisplayName = GetAttValue(field, "DisplayName");
            FieldName = GetAttValue(field, "Name");
            IsChanged = GetAttValue(field, "New") != GetAttValue(field, "Old");
            NewValue = GetAttValue(field, "New");
            OldValue = GetAttValue(field, "Old");
        }
        public bool IsChanged { set; get; }
        public string NewValue { set; get; }
        public string OldValue { set; get; }
        public string FieldName { set; get; }
        public string FieldDisplayName { set; get; }
    }

    class UserFieldChange:FieldChange
    {
        public UserFieldChange(XElement field)
            : base(field)
        {
            NewValue = FieldChange.GetAttValue(field, "LookupNewF");
            OldValue = FieldChange.GetAttValue(field, "LookupOldF");
        }
    }
}
