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
using Microsoft.SharePoint;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Extensions
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

        public static string ApplyXslt(this string xml, string xslt, ISubstitutionContext context, Func<string, string> processIncludes=null)
        {
            var temp = Path.GetTempFileName();
            var res = new StringBuilder();
            try
            {
                var c = new System.Xml.Xsl.XslCompiledTransform(true);
                File.WriteAllText(temp, xslt);
                c.Load(temp, new XsltSettings(true, true), new Resolver(context, temp, processIncludes));

                using (var xmlreader = XmlReader.Create(new StringReader(xml)))
                {
                    using (var resultWriter = XmlWriter.Create(new StringWriter(res), new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
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

            string tempXslt;

            ISubstitutionContext m_context;
            Func<string, string> processIncludes;
            public Resolver(ISubstitutionContext context, string tempXslt, Func<string, string> processIncludes)
            {
                Logger = Application.Current.Logger;

                if (Logger == null) throw new InvalidProgramException("No logger configured");
                this.tempXslt = tempXslt;
                this.processIncludes = processIncludes;
                this.m_context = context;
            }

            public override System.Net.ICredentials Credentials
            {
                set { }
            }

            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                if (string.Equals(absoluteUri.LocalPath, tempXslt))
                {
                    var local = absoluteUri.LocalPath;
                    if (!string.IsNullOrEmpty(local))
                    {
                        return File.OpenRead(local);
                    }
                }
                else
                {

                    if (m_context != null)
                    {
                        using (var includeReader = new StreamReader(m_context.GetTemplateFile(Path.GetFileName(absoluteUri.LocalPath))))
                        {
                            var include = includeReader.ReadToEnd();
                            if (processIncludes != null)
                            {
                                include = processIncludes(include);
                            }
                            return new MemoryStream(Encoding.Default.GetBytes(include));
                        }

                    }
                }

                Logger.WriteTrace("Need resolve type " + ((ofObjectToReturn != null) ? ofObjectToReturn.Name : "no type") + " uri=" + absoluteUri, SeverityEnum.Verbose);
                return null;
            }
        }
    }
}
