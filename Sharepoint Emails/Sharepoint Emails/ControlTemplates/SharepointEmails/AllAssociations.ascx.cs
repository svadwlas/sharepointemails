using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SharePointEmails.Core;
using System.Collections.Generic;

namespace SharepointEmails.ControlTemplates.SharepointEmails
{
    public partial class UserControl1 : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SPContext.Current != null && SPContext.Current.Web != null)
            {
                var manager = ClassContainer.Instance.Resolve<ConfigurationManager>();
                if (manager != null)
                {
                    var list = manager.GetAllAssociations(SPContext.Current.Web);
                    //GridView1.DataSource = list;
                    GridView1.DataSource = new List<Association>
                    {
                        new IDAssociation()
                    };
                    GridView1.DataBind();
                }
            }
        }
    }
}
