using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SharePointEmails.Core;

namespace SharepointEmails.Layouts.SharepointEmails
{
    public partial class PreviewPage : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = int.Parse(Context.Request.QueryString["ID"]);
            var templatelist = Web.Lists[Constants.TemplateListName];
            var item = templatelist.GetItemById(id);
            var template = new Template(item);
            div_Body.InnerHtml = template.Body;
            div_Subj.InnerHtml = template.Subject;
        }
    }
}
