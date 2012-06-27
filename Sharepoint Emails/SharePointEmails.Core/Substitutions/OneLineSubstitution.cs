using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core.Substitutions
{
    public class OneLineSubstitution : ISubstitution
    {

        public string Pattern
        {
            get { return "remove new lines"; }
        }

        public string Description
        {
            get { return Pattern; }
        }

        public string Process(string text, ISubstitutionContext context, ProcessMode mode)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text.Replace(Environment.NewLine, "");
            }
            else
            {
                return text;
            }
        }
    }
}
