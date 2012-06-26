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

        public SPList GetHiddenList(SPWeb web, bool create=false)
        {
            var list = web.Lists.TryGetList(Constants.TemplateListName);
            if (list == null && create)
            {
                var manager = ClassContainer.Instance.Resolve<ISiteManager>();
                return manager.CreateHiddenTemplatesList(web);
            }
            else
            {
                return list;
            }
        }

        public SPList CreateHiddenTemplateLibrary(SPWeb web, bool create = true)
        {
            var list = web.Lists.TryGetList(Constants.XsltLibrary);
            if (list == null && create)
            {
                var manager = ClassContainer.Instance.Resolve<ISiteManager>();
                return manager.CreateXsltTemplatesList(web);
            }
            else
            {
                return list;
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

        public WebConfiguration WebConfig(SPWeb web)
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

        public bool IsDisabledForSite(SPSite site)
        {
            return !site.Features.Any(p => p.DefinitionId == Constants.FeatureId);
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

        ITemplatesManager Manager
        {
            get
            {
                return ClassContainer.Instance.Resolve<ITemplatesManager>();
            }
        }

        public Message GetMessageForItem(SPList list, int ItemID, SPEventType type, string eventXML,string modifierName,string toEmail,int createUserId)
        {
            ISearchContext search = SearchContext.Create(list, ItemID, eventXML, type);
            var res = Manager.GetTemplate(search);
            if (res != null)
            {
                Logger.Write("Found template:", SeverityEnum.Verbose);
                Logger.Write(res.ToString(), SeverityEnum.Verbose);
                var substitutionContext = new SubstitutionContext(eventXML, list, ItemID, modifierName, toEmail, createUserId);
                return new Message
                    {
                        Body = res.GetProcessedText(substitutionContext),
                        Subject = res.GetProcessedSubj(substitutionContext)
                    };
            }
            else
            {
                Logger.Write("Found template is null", SeverityEnum.Error);
            }
            return null;
        }
    }
}
