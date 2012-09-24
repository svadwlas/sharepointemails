using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    public class OneLineSubstitution : BaseSubstitution
    {

        public string Pattern
        {
            get { return "remove new lines"; }
        }

        public string Description
        {
            get { return Pattern; }
        }

        public override string Process(string text, ISubstitutionContext context)
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
