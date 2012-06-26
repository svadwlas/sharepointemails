using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharePointEmails.Core.Substitutions;
using SharePointEmails.Logging;

namespace SharePointEmails.Core
{
    public class FieldSubstitution : ISubstitution
    {
        ILogger Logger { set; get; }
        public FieldSubstitution()
        {
            Logger = ClassContainer.Instance.Resolve<ILogger>();
        }

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

        public string Process(string text, ISubstitutionContext context, ProcessMode mode)
        {
            string res = text;
            foreach (Match m in Regex.Matches(res, @"\[([^\]\:]+)(\:{0,1}.*?)\]"))
            {
                try
                {
                    ModifiersCollection modifiers = ModifiersCollection.Parse(m.Groups[2].Value);
                    string fieldTextValue = (mode == ProcessMode.Test) ? "value of " + m.Value : context.GetField(m.Groups[1].Value, modifiers);
                    res = res.Replace(m.Value, fieldTextValue ?? "no value");
                }
                catch (Exception ex)
                {
                    Logger.Write(ex, SeverityEnum.Error);
                }
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
