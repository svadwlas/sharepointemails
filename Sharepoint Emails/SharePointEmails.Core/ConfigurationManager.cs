using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Remoting.Contexts;

namespace SharePointEmails.Core
{
    [Synchronization]
    public class ConfigurationManager : ContextBoundObject 
    {
        const string SE_WEB_CONFIG_PROP = "SEWEBPROP";

        XmlSerializer Searilizer
        {

            get
            {
                if (_Serializer == null)
                {
                    _Serializer = new XmlSerializer(typeof(TemplateConfiguration));
                }
                return _Serializer;
            }
        }XmlSerializer _Serializer;

        TemplateConfiguration GetConfig(SPWeb web)
        {
            if (web == null) return null;
            if (web.Properties.ContainsKey(SE_WEB_CONFIG_PROP) && (!string.IsNullOrEmpty( web.Properties[SE_WEB_CONFIG_PROP])))
            {
                var value = web.Properties[SE_WEB_CONFIG_PROP];
                using (var reader = new StringReader(value))
                {
                    return (TemplateConfiguration)Searilizer.Deserialize(reader);
                }
            }
            else
            {
                return new TemplateConfiguration();
            }
        }

        bool SetConfig(SPWeb web, TemplateConfiguration config)
        {
            if (web == null) return false;
            var value = new StringBuilder();
            using (var writer = new StringWriter(value))
            {
                Searilizer.Serialize(writer, config);
            }
            web.Properties[SE_WEB_CONFIG_PROP] = value.ToString();
            web.Properties.Update();
            web.Update();
            return true;
        }

        public void AddAssociation(SPWeb web, Association ass)
        {
            var config = GetConfig(web);
            config.Associations.Add(ass);
            SetConfig(web, config);
        }

        public bool RemoveAssociation(SPWeb web, string assId)
        {
            var config = GetConfig(web);
            if(config.Associations!=null)
            {
                var asses = config.Associations.Where(p => p.ID == assId).ToList();
                foreach (var ass in asses)
                {
                    config.Associations.Remove(ass);
                }
                if (asses.Any())
                {
                    SetConfig(web, config);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public TemplateConfiguration Get(SPWeb web)
        {
            return GetConfig(web);
        }

        public ICollection<Association> GetAllAssociations(SPWeb web)
        {
            return GetConfig(web).Associations;
        }
    }
}
