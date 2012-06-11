using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace SharePointEmails.Core.Configuration
{
    public class SerializeHelper
    {
        public static T ParseOrDefault<T>(string str)
        {
            try
            {
                var s = new XmlSerializer(typeof(T));
                using (var reader = new StringReader(str))
                {
                    return (T)s.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                return Activator.CreateInstance<T>();
            }
        }

        public static string ToXmlString<T>(T obj)
        {
            try
            {
                var s = new XmlSerializer(typeof(T));
                var b = new StringBuilder();
                using (var reader = new StringWriter(b))
                {
                    s.Serialize(reader, obj);
                }
                return b.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
