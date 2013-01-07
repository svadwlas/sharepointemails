using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharePointEmails.Core.Substitutions;
using SharePointEmails.Logging;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    public class FieldSubstitution : BaseSubstitution
    {
        public string Pattern
        {
            get
            {
                return @"\[\$Field:([^\]\[\: ]+)(\:{0,1}.*?)\]";
            }
        }

        public string Description
        {
            get
            {
                return "Field substitution";
            }
        }

        public override string Process(string text, ISubstitutionContext context)
        {
            string res = text;
            foreach (Match m in Regex.Matches(res, Pattern))
            {
                try
                {
                    ModifiersCollection modifiers = ModifiersCollection.Parse(m.Groups[2].Value);
                    string fieldTextValue = context.GetCurrentFieldValue(m.Groups[1].Value, modifiers);
                    if (fieldTextValue != null)
                    {
                        res = res.Replace(m.Value, fieldTextValue);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteTrace(ex, SeverityEnum.Error);
                }
            }
            return res;
        }
    }
}
