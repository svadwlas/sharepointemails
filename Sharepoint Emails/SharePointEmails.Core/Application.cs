using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Logging;
using SharePointEmails.Core.Configuration;
using Microsoft.SharePoint.Administration;

namespace SharePointEmails.Core
{
    public class Application
    {
        static Application _Curent;
        static object _CurentLock=new object();
        static object _LoggerLock=new object();
        public static Application Current
        {
            get
            {
                if (_Curent == null)
                {
                    lock (_CurentLock)
                    {
                        if (_Curent == null)
                        {
                            _Curent = new Application();
                        }
                    }
                }
                return _Curent;
            }
        }

        private FarmConfiguration FarmConfig
        {
            get
            {
                var configManager=ClassContainer.Instance.Resolve<ConfigurationManager>();
                var res=configManager.GetConfigOrdefault(SPFarm.Local);
                return res;
            }
        }

        private WebConfiguration WebConfig(SPWeb web)
        {
            var configManager = ClassContainer.Instance.Resolve<ConfigurationManager>();
            var res = configManager.GetConfigOrdefault(web);
            return res;
        }

        public bool IsDisabledForFarm()
        {
            return false;
            return FarmConfig.Disabled;
        }

        public bool IsDisabledForWeb(SPWeb web)
        {
            return false;
            var temp = web;
            while (temp != null)
            {
                var config = WebConfig(temp);
                if (config != null)
                {
                    if (temp.Equals(web) && ((config.Disabled)))
                        return true;
                    else if (config.Disabled && config.DisableIncludeChilds)
                        return true;
                }
            }
            return false;
        }

        public ILogger Logger
        {
            get
            {
                if (_Logger == null)
                {
                    lock (_LoggerLock)
                    {
                        if (_Logger == null)
                        {
                            _Logger = ClassContainer.Instance.Resolve<ILogger>();
                        }
                    }
                }
                return _Logger;
            }
        }
        ILogger _Logger;

        ITemplatesManager GetTemplateManager()
        {
            return ClassContainer.Instance.Resolve<ITemplatesManager>();
        }

        TemplateTypeEnum get(SPEventType type)
        {
            switch (type)
            {
                case SPEventType.All: return TemplateTypeEnum.AllItemEvents;
            }
            return TemplateTypeEnum.AllItemEvents;
        }

        public Message GetMessageForItem(SPList list, int ItemID, SPEventType type, string eventXML)
        {
            var manager = GetTemplateManager();
            ISearchContext search = null;
            search = SearchContext.Create(list, ItemID, eventXML, get(type));
            if (search != null)
            {
                var res = manager.GetTemplate(search);
                if (res != null)
                {
                    return new Message
                        {
                            Body = res.GetProcessedText(new SubstitutionContext(eventXML)),
                            Subject = "generated message " + DateTime.Now.ToLongTimeString()
                        };
                }
            }
            return null;
        }
    }
}
