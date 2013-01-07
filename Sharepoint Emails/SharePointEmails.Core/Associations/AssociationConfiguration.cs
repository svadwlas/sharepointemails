using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Serialization;
using System.IO;
using SharePointEmails.Core.Associations;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.Associations
{
    [Serializable]
    public class AssociationCollection:List<Association>
    {
        static XmlSerializer Searilizer
        {

            get
            {
                if (_Serializer == null)
                {
                    _Serializer = new XmlSerializer(typeof(AssociationCollection));
                }
                return _Serializer;
            }
        }static XmlSerializer _Serializer;

        public static AssociationCollection Parse(string str)
        {
            using (var reader = new StringReader(str))
            {
                return (AssociationCollection)Searilizer.Deserialize(reader);
            }
        }

        public static AssociationCollection ParseOrDefault(string str)
        {
            try
            {
                return Parse(str);
            }
            catch (Exception ex)
            {
                Application.Current.Logger.WriteTrace("Cannot parse config. empty will be returned", SeverityEnum.Error);
                Application.Current.Logger.WriteTrace(ex.ToString(), SeverityEnum.Error);
                return AssociationCollection.Empty;
            }
        }

        public static AssociationCollection Empty { get { return new AssociationCollection(); } }

        public override string ToString()
        {
            var value = new StringBuilder();
            using (var writer = new StringWriter(value))
            {
                Searilizer.Serialize(writer, this);
            }
            return value.ToString();
        }
    }

  

   

  
    
}
