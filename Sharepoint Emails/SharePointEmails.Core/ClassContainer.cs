using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;
using Microsoft.Practices.Unity;

namespace SharePointEmails.Core
{
    public class ClassContainer
    {
        static ClassContainer _Instance = null;

        public static ClassContainer Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_Instance)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new ClassContainer();
                        }
                    }
                }
                return _Instance;
            }
        }

        private UnityContainer m_Container = null;

        private void Register(UnityContainer container)
        {
            container.RegisterInstance<ILogger>(new DefaultLogger());
            container.RegisterInstance<ITemplatesManager>(new DefaultTemplatesManager(new DefaultLogger()));
            container.RegisterInstance<ISiteManager>(new DefaultSiteManager());
        }

        public UnityContainer Container
        {
            get
            {
                if (m_Container == null)
                {
                    lock (m_Container)
                    {
                        if (m_Container==null)
                        {
                            m_Container = new UnityContainer();
                            Register(m_Container);
                        }
                    }
                }
                return m_Container;
            }
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
