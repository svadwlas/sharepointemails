using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SharePointEmails.Core;

namespace SharepointEmails.Layouts.SharepointEmails
{
    public partial class WebSettings : LayoutsPageBase
    {
        Presenter presenter;

        public WebConfiguration Configuration { set; get; }

        public event EventHandler OnFirstLoad;
         
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (OnFirstLoad != null) OnFirstLoad(this, EventArgs.Empty);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            presenter = new Presenter(this, new WebSettingsModel());
        }
    }

    class Presenter
    {
        WebSettings m_view;
        WebSettingsModel m_model;

        public Presenter(WebSettings view, WebSettingsModel model)
        {
            m_model = model;
            m_view = view;

            m_view.OnFirstLoad += new EventHandler(m_view_OnFirstLoad);
        }

        void m_view_OnFirstLoad(object sender, EventArgs e)
        {
            var m_config=m_model.LoadConfig();
            m_view.Configuration = m_config;
        }
    }

    class WebSettingsModel
    {
        ConfigurationManager Configmanager { get{return ClassContainer.Instance.Resolve<ConfigurationManager>();}}

        public WebConfiguration LoadConfig()
        {
            if (SPContext.Current != null)
            {
                return Configmanager.Get(SPContext.Current.Web);
            }
            else
            {
                return null;
            }

        }
    }

}
