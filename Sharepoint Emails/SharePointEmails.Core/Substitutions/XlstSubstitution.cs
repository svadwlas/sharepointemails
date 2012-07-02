using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.Substitutions
{
    public class XlstSubstitution : ISubstitution
    {
        ILogger Logger;
        public XlstSubstitution()
        {
            Logger = Application.Current.Logger;
        }
        public string Pattern
        {
            get { return "XSLT"; }
        }

        public string Description
        {
            get { return "eXtensible Stylesheet Language Transformations"; }
        }

        public string Process(string text, ISubstitutionContext context, ProcessMode mode)
        {
            try
            {
                if (!text.IsXslt()) return text;
                var contextXML = (mode == ProcessMode.Test) ? SubstitutionContext.GetTestXML() : context.GetXML();
                return contextXML.ApplyXslt(text);
            }
            catch (XsltException ex)
            {
                Logger.Write("Cannot parse or transform template. maybe because it is not xslt template", SeverityEnum.Warning);
                Logger.Write(ex, SeverityEnum.Warning);
                return text;
            }
            catch (Exception ex)
            {
                Logger.Write("ERROR DURIN|G GEMERATING OUTPUT HTML", SeverityEnum.CriticalError);
                Logger.Write(ex, SeverityEnum.CriticalError);
                return text;
            }
        }
    }
}
