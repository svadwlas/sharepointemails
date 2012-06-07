using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Logging;

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

        public Message GetMessage(SPListItem item,SPEventType type)
        {
            var manager = GetTemplateManager();
            var search = SearchContext.Create(item,get(type));
            var res = manager.GetTemplate(search);
            if (res != null)
            {
                return new Message
                    {
                        Body = res.GetProcessedText(null),
                        Subject = "generated message " + DateTime.Now.ToLongTimeString()
                    };
            }
            else
            {
                return null;
            }
        }
    }
}
