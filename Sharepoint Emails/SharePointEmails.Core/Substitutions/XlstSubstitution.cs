using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using SharePointEmails.Logging;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.Core.Exceptions;
using SharePointEmails.Core.Extensions;
namespace SharePointEmails.Core.Substitutions
{
    public class XlstSubstitution : BaseSubstitution
    {
        public string Pattern
        {
            get { return "XSLT"; }
        }

        public string Description
        {
            get { return "eXtensible Stylesheet Language Transformations"; }
        }

        public override string Process(string text, ISubstitutionContext context)
        {
            try
            {
                string res = string.Empty;
                var xml = context.GetXML();
                var log = new StringBuilder();
                log.AppendLine("Applying XSLT");
                log.AppendLine("Template");
                log.AppendLine(text);

                
                if (!text.IsXslt())
                {
                    log.AppendLine("Template is wrong");
                    res = text;
                }
                else
                {
                    log.AppendLine("Incoming XML");
                    log.AppendLine(xml);
                    res = xml.ApplyXslt(text, context.GetTemplateLibrary(), Worker.OnPartLoaded);
                    log.AppendLine("Result:");
                    log.AppendLine(res);
                }
                Logger.WriteTrace(log.ToString(), SeverityEnum.Verbose);
                return res;
            }
            catch (XsltException ex)
            {
                Logger.WriteTrace("Cannot parse or transform template. maybe because it is not xslt template", SeverityEnum.Warning);
                Logger.WriteTrace(ex, SeverityEnum.Warning);
                return text;
            }
            catch (Exception ex)
            {
                Logger.WriteTrace("ERROR DURIN|G GEMERATING OUTPUT HTML", SeverityEnum.CriticalError);
                Logger.WriteTrace(ex, SeverityEnum.CriticalError);
                return text;
            }
        }
    }
}
