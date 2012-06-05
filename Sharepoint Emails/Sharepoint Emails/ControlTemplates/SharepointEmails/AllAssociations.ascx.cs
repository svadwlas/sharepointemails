using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SharePointEmails.Core;
using System.Collections.Generic;
using System.Linq;
namespace SharepointEmails.ControlTemplates.SharepointEmails
{
    public partial class UserControl1 : UserControl
    {
        const int GUID_COLUMN_INDEX = 0;

        List<AssInfo> Items { set; get; }

        ConfigurationManager Manager
        {
            get
            {
                return ClassContainer.Instance.Resolve<ConfigurationManager>();
            }
        }

        SPWeb Web { get { return SPContext.Current.Web; } }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void btn_Delete_Click(object sender, EventArgs e)
        {
            if (GridView1.SelectedValue != null)
            {
                Manager.RemoveAssociation(Web, (string)GridView1.SelectedValue);
            }
        }

        

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Items=Manager.GetAllAssociations(Web).Select(p => new AssInfo { Name = p.Name, Type = p.Type.ToString(),ID=p.ID.ToString() }).ToList();
            GridView1.DataSource = Items;
            //GridView1.DataSource = new List<Association>
            //{
            //    new IDAssociation()
            //};
            GridView1.DataBind();  
        }
    }

    class AssInfo
    {
        public string Name { set; get; }
        public string Type { set; get; }
        public Association Ass { set; get; }
        public string ID { set; get; }
    }
}
