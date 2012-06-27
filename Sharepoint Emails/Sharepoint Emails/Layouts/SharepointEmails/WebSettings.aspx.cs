using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SharePointEmails.Core.Configuration;
using SharePointEmails.Core;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls.WebParts;

namespace SharepointEmails.Layouts.SharepointEmails
{
    public partial class WebSettings : LayoutsPageBase
    {
        WebConfiguration getExisted()
        {
            return SharePointEmails.Core.Application.Current.WebConfig(Web);
        }

        WebConfiguration Config
        {
            get
            {
                var web = getExisted();

                UpdateModel(web);

                return web;
            }

            set
            {
                UpdateView(value);
            }
        }

        ConfigurationManager ConfigurationManager
        {
            get
            {
                return ClassContainer.Instance.Resolve<ConfigurationManager>();
            }
        }

        public void UpdateModel(WebConfiguration config)
        {
            config.Disabled = cb_Disabled.Checked;
            config.DisableIncludeChilds = cb_IncludeChilds.Checked;
        }

        public void UpdateView(WebConfiguration config)
        {
            cb_Disabled.Checked = config.Disabled;
            cb_IncludeChilds.Checked = config.DisableIncludeChilds;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Config = getExisted();
            }

            btn_Save.Click += new EventHandler(btn_Save_Click);
            btn_Exit.Click += new EventHandler(btn_Exit_Click);
        }

        void btn_Exit_Click(object sender, EventArgs e)
        {
            Context.Response.Redirect("/_layouts/settings.aspx", true);
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            ConfigurationManager.SetConfig(Config, Web);
            Config = getExisted();
            lbl_Message.Text = "Saved";
        }
    }
}
