using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        string m_eventData = string.Empty;
        SPListItem m_item = null;

        public SubstitutionContext(string eventData)
        {
            m_eventData = eventData;
        }

        public string GetField(string fieldName)
        {
            return "value of '" + fieldName + "'";
        }

        public List<string> GetAvailableFields()
        {
            var res = new List<string>() { "testField1", "testField2" };

            return res;
        }
    }
}
