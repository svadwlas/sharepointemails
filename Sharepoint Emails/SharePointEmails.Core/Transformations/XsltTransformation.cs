using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.Core.Extensions;
using SharePointEmails.Logging;
namespace SharePointEmails.Core.Transformations
{
    class XsltTransformation : MainTransformation
    {
        public string GetTemplateData(ISubstitutionContext context)
        {
            return context.GetXML();
        }
        public override string Transform(string template, ISubstitutionContext context, Func<string,string> partIncludedCallback)
        {
            try
            {
                string res = string.Empty;
                var xml = GetTemplateData(context);
                var log = new StringBuilder();
                log.AppendLine("Applying XSLT");
                log.AppendLine("Template:");
                log.AppendLine(template);


                if (!template.IsXslt())
                {
                    log.AppendLine("Template is not xslt");
                    res = template;
                }
                else
                {
                    log.AppendLine("Incoming XML");
                    log.AppendLine(xml);
                    res = xml.ApplyXslt(template, context, partIncludedCallback);
                    log.AppendLine("Result:");
                    log.AppendLine(res);
                }
                Logger.WriteTrace(log.ToString(), SeverityEnum.Verbose);
                return res;
            }
            catch (XsltException ex)
            {
                Logger.WriteTrace("Cannot parse or transform template. maybe because it is not xslt template", ex, SeverityEnum.Warning);
                return template;
            }
            catch (Exception ex)
            {
                Logger.WriteTrace("ERROR DURIN|G GEMERATING OUTPUT HTML", ex, SeverityEnum.CriticalError);
                return template;
            }
        } 
    }
}
