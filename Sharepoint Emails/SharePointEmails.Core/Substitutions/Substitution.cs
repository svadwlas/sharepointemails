using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharePointEmails.Core
{
    public class FieldSubstitution : ISubstitution
    {
        public string Pattern
        {
            get
            {
                return "[FieldName]";
            }
        }

        public string Description
        {
            get
            {
                return "Field substitution";
            }
        }

        public string Process(string text, ISubstitutionContext context)
        {
            string res = text;
            var mc = Regex.Matches(res, @"\[(.+?)\]");
            foreach (Match m in mc)
            {

                var fieldName = m.Value.Trim(']', '[');
                string fieldTextValue = context.GetField(fieldName);
                if (fieldTextValue == null)
                {
                    fieldTextValue = "no \"" + fieldName + "\"";
                }
                res = res.Replace(m.Value, fieldTextValue);
            }
            return res;
        }

        public List<string> GetAvailableKeys(ISubstitutionContext context)
        {
            var res = new List<string>() { "field1", "field2" };
            return res;
        }
    }
}
