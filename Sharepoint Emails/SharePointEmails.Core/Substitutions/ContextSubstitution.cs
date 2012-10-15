using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using SharePointEmails.Logging;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    class ContextVarsSubstitution : BaseSubstitution
    {
        public string Pattern
        {
            get { return "{VarPath}"; }
        }

        public string Description
        {
            get { return "Context variables"; }
        }

        public override string Process(string text, ISubstitutionContext context)
        {
            var res = text;
            foreach (Match m in Regex.Matches(text, @"\{([^\$ ]+?)\}"))
            {
                try
                {
                    var value = context.GetContextValue(m.Groups[1].Value);
                    if (value != null)
                    {
                        res = res.Replace(m.Value, value);
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
