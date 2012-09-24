using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SharePointEmails.Core.Configuration;
using SharePointEmails.Core;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls.WebParts;
using System.IO;
namespace SharePointEmails.Layouts.SharePointEmails
{
    public partial class WebSettings : LayoutsPageBase
    {
        WebConfiguration getExisted()
        {
            return global::SharePointEmails.Core.Application.Current.WebConfig(Web);
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
                if (_configManager == null)
                {
                    _configManager = global::SharePointEmails.Core.Application.Current.GetConfigurationManager();
                }
                return _configManager;
            }
        }

        ConfigurationManager _configManager = null;

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
            btn_GetXml.Click += new EventHandler(btn_GetXml_Click);
        }

        void btn_GetXml_Click(object sender, EventArgs e)
        {
            var temp = Path.ChangeExtension(Path.GetTempFileName(), ".xml");
            File.WriteAllText(temp, Properties.Resources.TestContextXML);
            Response.AppendHeader("Content-Disposition", "attachment; filename=context.xml");
            Response.TransmitFile(temp);
            Response.End();
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
