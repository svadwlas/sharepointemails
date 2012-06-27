using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.Substitutions
{
    class ContextVarsSubstitution : ISubstitution
    {
        ILogger Logger { set; get; }
        public ContextVarsSubstitution()
        {
            Logger = Application.Current.Logger;
        }
        public string Pattern
        {
            get { return "{VarPath}"; }
        }

        public string Description
        {
            get { return "Context variables"; }
        }

        public string Process(string text, ISubstitutionContext context, ProcessMode mode)
        {
            var res = text;
            foreach (Match m in Regex.Matches(text, @"\{([^\$]+?)\}"))
            {
                try
                {
                    res = res.Replace(m.Value, (mode == ProcessMode.Test) ? "value of " + m.Value : context.GetContextValue(m.Groups[1].Value) ?? "no value");
                }
                catch (Exception ex)
                {
                    Logger.Write(ex, SeverityEnum.Error);
                }
            }
            return res;
        }
    }
}
