using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SharePointEmails;

namespace SharePointEmails.SwitchWebPart
{
    [ToolboxItemAttribute(false)]
    public class SwitchWebPart : WebPart
    {
        const string HIDENFIELDNAME = "SwitchesInformation";

        public string Info { set; get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Page.ClientScript.RegisterClientScriptInclude("jquery-1.7.2.min.js", "_layouts/Switcher/jquery-1.7.2.min.js");
            Page.ClientScript.RegisterClientScriptInclude("FieldSwitches.js", "_layouts/Switcher/FieldSwitches.js");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (SPContext.Current != null && SPContext.Current.FormContext != null && !string.IsNullOrEmpty(Info))
            {
                var controls = SPContext.Current.FormContext.FieldControlCollection;
                var options = FieldsSwitches.Create(Info, controls);
                if (options != null)
                {
                    var json = options.ToJson();
                    Page.ClientScript.RegisterHiddenField(HIDENFIELDNAME, json);
                }
            }
        }
    }
}
