﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharePointEmails.Logging;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using System.Collections;
using System.Threading;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    public class ResourceSubstitution : BaseSubstitution
    {
        public override string Process(string text, ISubstitutionContext context)
        {
            string res = text;
            if (!string.IsNullOrEmpty(res))
            {
                foreach (Match m in Regex.Matches(res, @"\{\$Resources:(.+?)\,(.+?)\}"))
                {
                    try
                    {
                        var lcid = (uint)Thread.CurrentThread.CurrentCulture.LCID;
                        var resourceValue = SPUtility.GetLocalizedString("$Resources:" + m.Groups[2].Value, m.Groups[1].Value, lcid);
                        if (resourceValue != null && resourceValue.StartsWith("$Resources:"))
                        {
                            Logger.WriteTrace(string.Format("{0} - is not localized for LCID={1}", m.Value, lcid), SeverityEnum.Warning);
                        }
                        if (resourceValue != null)
                        {
                            res = res.Replace(m.Value, resourceValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteTrace(ex, SeverityEnum.Error);
                    }
                }
            }
            return res;
        }
    }
}
