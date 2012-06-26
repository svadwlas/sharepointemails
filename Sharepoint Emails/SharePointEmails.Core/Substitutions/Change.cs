using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharePointEmails.Core.Substitutions
{
   public class FieldChange
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

        protected XElement m_field;
        public static string GetAttValue(XElement el, string name)
        {
            return (el.Attribute(name) == null) ? null : el.Attribute(name).Value;
        }
        protected FieldChange(XElement field)
        {
            m_field = field;
            FieldDisplayName = GetAttValue(field, "DisplayName");
            FieldName = GetAttValue(field, "Name");
            IsChanged = GetAttValue(field, "New") != null && GetAttValue(field, "New") != GetAttValue(field, "Old");
        }
        public bool IsChanged { set; get; }
        public string FieldName { set; get; }
        public string FieldDisplayName { set; get; }

        public virtual string GetText(ModifiersCollection modifiers)
        {
            if (modifiers.Contains(Modifier.Old))
            {
                return GetAttValue(m_field, "Old");
            }
            else if (modifiers.Contains(Modifier.New))
            {
                return GetAttValue(m_field, "New");
            }
            else
            {
                return GetAttValue(m_field, "New") ?? GetAttValue(m_field, "Old");
            }
        }
    }

    class UserFieldChange : FieldChange
    {
        public UserFieldChange(XElement field)
            : base(field)
        {
        }

        public override string GetText(ModifiersCollection modifiers)
        {
            if (modifiers.Contains(Modifier.Old))
            {
                return GetAttValue(m_field, "LookupOldF");
            }
            else if (modifiers.Contains(Modifier.New))
            {
                return GetAttValue(m_field, "LookupNewF");
            }
            else
            {
                return GetAttValue(m_field, "LookupNewF") ?? GetAttValue(m_field, "LookupOldF");
            }
        }
    }
}
