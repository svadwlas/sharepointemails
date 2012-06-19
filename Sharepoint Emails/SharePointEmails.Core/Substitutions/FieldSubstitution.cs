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

        //[Title:O:]
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
                var modifiers="";
                var fieldNameWithModifiers = m.Value.Trim(']', '[');
                var mod = Regex.Match(fieldNameWithModifiers, @"\:.+");
                if (mod != null && mod.Value != null)
                {
                    modifiers = mod.Value;
                }
                
                string fieldTextValue = context.GetField(fieldNameWithModifiers.Replace(modifiers,""),modifiers);
                if (fieldTextValue == null)
                {
                    fieldTextValue = "no \"" + fieldNameWithModifiers + "\"";
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
