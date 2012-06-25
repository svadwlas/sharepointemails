using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharePointEmails.Logging;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using System.Collections;

namespace SharePointEmails.Core.Substitutions
{
    public class ResourceSubstitution:ISubstitution
    {
        ILogger Logger;
        public ResourceSubstitution()
        {
            Logger = ClassContainer.Instance.Resolve<ILogger>();
        }

        public string Pattern
        {
            get { return "{$Resources:<File name>, <Resource Key>}"; }
        }

        public string Description
        {
            get { return "resources"; }
        }

        public string Process(string text, ISubstitutionContext context)
        {
            string res = text;
            var mc = Regex.Matches(res, @"\{\$.+?\}");

            foreach (Match m in mc)
            {
                try
                {
                    Logger.Write("Processing resorce " + m.Value, SeverityEnum.Verbose);
                    var resource = m.Value.Trim('{', '}');
                    //$Resources:core,Alert_icon_margin_right
                    var source = resource.Substring(resource.IndexOf(':') + 1, resource.IndexOf(',') - resource.IndexOf(':') - 1);
                    var name = resource.Substring(resource.IndexOf(',') + 1);
                    var lcid = (uint)context.getDestinationCulture().LCID;
                    var fieldTextValue = SPUtility.GetLocalizedString("$Resources:" + name, source, lcid);
                    if (fieldTextValue != null && fieldTextValue.StartsWith("$Resources:"))
                    {
                        Logger.Write(string.Format("{0} - is not localized for LCID={1}", resource, lcid), SeverityEnum.Warning);
                    }
                    if (!string.IsNullOrEmpty(m.Value))
                    {
                        res = res.Replace(m.Value, fieldTextValue ?? "no value");
                    }
                    else
                    {
                        throw new Exception("Wrong Resource farmat");
                    }
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
            throw new NotImplementedException();
        }
    }
}
