using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Linq;

namespace SharePointEmails.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        const string OLD_VALUE = ":O";
        const string NEW_VALUE = ":N";

        string m_eventData = string.Empty;
        SPListItem m_item = null;
        XDocument m_data = null;

        public SubstitutionContext(string eventData)
        {
            m_eventData = eventData;
            m_data = XDocument.Parse(eventData);
        }

        bool HasModifier(string all, string test)
        {
            if (string.IsNullOrEmpty(all)) return false;
            return all.Split(':').Contains(test.Trim(':'));
        }

        public string GetField(string fieldName, string modifiers)
        {
            string val = null;
            if (m_data != null)
            {
             
                var element = m_data.Root.Elements().Where(p => p.Name == "Field"
                    && p.Attribute("DisplayName") != null && p.Attribute("Name").Value == fieldName).FirstOrDefault();
                if (element == null)
                {
                    element = m_data.Root.Elements().Where(p => p.Name == "Field"
                    && p.Attribute("DisplayName") != null && p.Attribute("DisplayName").Value == fieldName).FirstOrDefault();
                }
                if (element != null)
                {
                    var type = element.Attribute("Type").Value;
                    switch (type)
                    {
                        case "User":
                            {
                                if (HasModifier(modifiers, OLD_VALUE))
                                {
                                    val = (element.Attribute("Old") != null) ? element.Attribute("Old").Value : (string)null;
                                }
                                else
                                {
                                    val = (element.Attribute("New") != null) ? element.Attribute("New").Value : (string)null;
                                }
                                break;
                            }

                        default:
                            {
                                if (HasModifier(modifiers, OLD_VALUE))
                                {
                                    val = (element.Attribute("Old") != null) ? element.Attribute("Old").Value : (string)null;
                                }
                                else
                                {
                                    val = (element.Attribute("New") != null) ? element.Attribute("New").Value : (string)null;
                                }
                                break;
                            }
                    }
                }
            }
            return val;
        }

        public List<string> GetAvailableFields()
        {
            var res = new List<string>() { "testField1", "testField2" };

            return res;
        }
    }
}
