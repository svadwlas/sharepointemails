using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Xsl;
using SharePointEmails.Logging;

namespace SharePointEmails.Core
{
    public static class XmlExtensions
    {
        public static string GetAtributeValue(this XElement el, XName attName)
        {
            return (el.Attribute(attName) == null) ? null : el.Attribute(attName).Value;
        }
        public static bool IsXslt(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            return Regex.IsMatch(str, "stylesheet", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        public static void ValidateXslt(this string str)
        {
            var com = new System.Xml.Xsl.XslCompiledTransform();
            using (var reader = XmlReader.Create(new StringReader(str)))
            {
                com.Load(reader);
            }
        }

        public static string ApplyXslt(this string xml, string xslt)
        {
            var temp = Path.GetTempFileName();
            var res = new StringBuilder();
            try
            {
                var c = new System.Xml.Xsl.XslCompiledTransform();
                File.WriteAllText(temp, xslt);
                c.Load(temp, new XsltSettings(true, true), new Resolver());

                using (var xmlreader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
                {
                    using (var resultWriter = XmlWriter.Create(new StringWriter(res)))
                    {
                        c.Transform(xmlreader, resultWriter);
                    }
                }
            }
            finally
            {
                try { File.Delete(temp); }
                catch { }
            }
            return res.ToString();
        }

        class Resolver : XmlResolver
        {
            ILogger Logger { set; get; }
            public Resolver()
            {
                Logger = ClassContainer.Instance.Resolve<ILogger>();
                if (Logger == null) throw new InvalidProgramException("No logger configured");
            }

            public override System.Net.ICredentials Credentials
            {
                set { }
            }

            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                var local = absoluteUri.LocalPath;
                if (!string.IsNullOrEmpty(local))
                {
                    return File.OpenRead(local);
                }

                Logger.Write("Need resolve type " + ((ofObjectToReturn != null) ? ofObjectToReturn.Name : "no type") + " uri=" + absoluteUri, SeverityEnum.Verbose);
                return null;
            }
        }
    }
}
