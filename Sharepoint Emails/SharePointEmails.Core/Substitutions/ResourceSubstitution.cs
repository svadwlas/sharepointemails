using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharePointEmails.Logging;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using System.Collections;
using System.Threading;

namespace SharePointEmails.Core.Substitutions
{
    public class ResourceSubstitution : ISubstitution
    {
        ILogger Logger;
        public ResourceSubstitution()
        {
            Logger = Application.Current.Logger;
        }

        public string Pattern
        {
            get { return "{$Resources:<File name>, <Resource Key>}"; }
        }

        public string Description
        {
            get { return "resources"; }
        }

        public string Process(string text, ISubstitutionContext context,ProcessMode mode)
        {
            string res = text;
            foreach (Match m in Regex.Matches(res, @"\{\$Resources:(.+?)\,(.+?)\}"))
            {
                try
                {
                    var lcid = (mode == ProcessMode.Test) ? (uint)context.getDestinationCulture().LCID : (uint)Thread.CurrentThread.CurrentCulture.LCID;
                    var fieldTextValue = SPUtility.GetLocalizedString("$Resources:" + m.Groups[2].Value, m.Groups[1].Value, lcid);
                    if (fieldTextValue != null && fieldTextValue.StartsWith("$Resources:"))
                    {
                        Logger.Write(string.Format("{0} - is not localized for LCID={1}", m.Value, lcid), SeverityEnum.Warning);
                    }
                    res = res.Replace(m.Value, fieldTextValue ?? "no value");
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
