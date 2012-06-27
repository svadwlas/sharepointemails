using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharePointEmails.Core
{
    public static class XmlExtensions
    {
        public static string GetAtributeValue(this XElement el, XName attName)
        {
            return (el.Attribute(attName) == null) ? null : el.Attribute(attName).Value;
        }
    }
}
