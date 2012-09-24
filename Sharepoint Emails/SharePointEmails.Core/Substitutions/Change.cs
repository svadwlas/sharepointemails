using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharePointEmails.Core.Extensions;
namespace SharePointEmails.Core.Substitutions
{
    public class FieldChange
    {
        public static FieldChange Create(XElement field)
        {
            var type = field.GetAtributeValue("Type");
            switch (type)
            {
                case "user": return new UserFieldChange(field);
                default: return new FieldChange(field);
            }
        }

        protected XElement m_field;

        protected FieldChange(XElement field)
        {
            m_field = field;
            FieldDisplayName = m_field.GetAtributeValue("DisplayName");
            FieldName = m_field.GetAtributeValue("Name");
            IsChanged = m_field.GetAtributeValue("New") != null && m_field.GetAtributeValue("New") != m_field.GetAtributeValue("Old");
            FieldType = m_field.GetAtributeValue("Type");
        }
        public bool IsChanged { set; get; }
        public string FieldName { set; get; }
        public string FieldDisplayName { set; get; }

        public virtual string GetText(ModifiersCollection modifiers)
        {
            if (modifiers.Contains(Modifier.Old))
            {
                return m_field.GetAtributeValue("Old");
            }
            else if (modifiers.Contains(Modifier.New))
            {
                return m_field.GetAtributeValue("New");
            }
            else
            {
                return m_field.GetAtributeValue("New") ?? m_field.GetAtributeValue("Old");
            }
        }

        public string FieldType { get; set; }
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
                return m_field.GetAtributeValue("LookupOldF");
            }
            else if (modifiers.Contains(Modifier.New))
            {
                return m_field.GetAtributeValue("LookupNewF");
            }
            else
            {
                return m_field.GetAtributeValue("LookupNewF") ?? m_field.GetAtributeValue("LookupOldF");
            }
        }
    }
}
