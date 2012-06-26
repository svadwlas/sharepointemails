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
   public class XlstSubstitution:ISubstitution
    {
        ILogger Logger;
        public XlstSubstitution()
        {
            Logger = ClassContainer.Instance.Resolve<ILogger>();
        }
        public string Pattern
        {
            get { return "XSLT"; }
        }

        public string Description
        {
            get { return "eXtensible Stylesheet Language Transformations"; }
        }

        public string Process(string text, ISubstitutionContext context,ProcessMode mode)
        {
            try
            {
                var res = new StringBuilder();
                var c = new System.Xml.Xsl.XslCompiledTransform(true);
                var contextXML =(mode==ProcessMode.Test)?SubstitutionContext.GetTestXML(): context.GetXML();
                using (var xsltReader = XmlReader.Create(new StringReader(text)))
                {
                    c.Load(xsltReader);
                }

                using (var xmlreader = XmlReader.Create(new StringReader(contextXML)))
                {
                    using (var resultWriter = XmlWriter.Create(new StringWriter(res)))
                    {
                        c.Transform(xmlreader, resultWriter);
                    }
                }
                return res.ToString();
            }
            catch (Exception ex)
            {
                Logger.Write("ERROR DURIN|G GEMERATING OUTPUT HTML", SeverityEnum.CriticalError);
                Logger.Write(ex, SeverityEnum.CriticalError);
                return text;
            }
      }

        public List<string> GetAvailableKeys(ISubstitutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
