using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

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
            var res = new StringBuilder();
            var c = new System.Xml.Xsl.XslCompiledTransform();

            using (var xsltReader = XmlReader.Create(new StringReader(xslt)))
            {
                c.Load(xsltReader);
            }

            using (var xmlreader = XmlReader.Create(new StringReader(xml)))
            {
                using (var resultWriter = XmlWriter.Create(new StringWriter(res)))
                {
                    c.Transform(xmlreader, resultWriter);
                }
            }
            return res.ToString();
        }
    }
}
