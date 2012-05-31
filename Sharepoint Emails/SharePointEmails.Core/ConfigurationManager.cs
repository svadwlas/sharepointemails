using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Serialization;
using System.IO;

namespace SharePointEmails.Core
{
    public class ConfigurationManager
    {
        const string SE_WEB_CONFIG_PROP = "SEWEBPROP";

        XmlSerializer Searilizer
        {

            get
            {
                if (_Serializer == null)
                {
                    _Serializer = new XmlSerializer(typeof(WebConfiguration));
                }
                return _Serializer;
            }
        }XmlSerializer _Serializer;

        WebConfiguration GetConfig(SPWeb web)
        {
            if (web == null) return null;
            if (web.Properties.ContainsKey(SE_WEB_CONFIG_PROP))
            {
                var value = web.Properties[SE_WEB_CONFIG_PROP];
                using (var reader = new StringReader(value))
                {
                    return (WebConfiguration)Searilizer.Deserialize(reader);
                }
            }
            else
            {
                return new WebConfiguration();
            }
        }

        bool SetConfig(SPWeb web, WebConfiguration config)
        {
            if (web == null) return false;
            var value = new StringBuilder();
            using (var writer = new StringWriter(value))
            {
                Searilizer.Serialize(writer, config);
            }
            web.Properties[SE_WEB_CONFIG_PROP] = value.ToString();
            web.Update();
            return true;
        }

        public void AddAssociation(SPWeb web, Association ass)
        {
            var config = GetConfig(web);
            config.Associations.Add(ass);
            SetConfig(web, config);
        }

        public bool RemoveAssociation(SPWeb web, Guid assId)
        {
            var config = GetConfig(web);
            if(config.Associations!=null)
            {
                var asses = config.Associations.Where(p => p.ID == assId);
                foreach (var ass in asses)
                {
                    config.Associations.Remove(ass);
                }
                return asses.Any();
            }
            return false;
        }

        public WebConfiguration Get(SPWeb web)
        {
            return GetConfig(web);
        }

        public ICollection<Association> GetAllAssociations(SPWeb web)
        {
            return GetConfig(web).Associations;
        }
    }
}
