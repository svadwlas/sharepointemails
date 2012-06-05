using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SharepointEmails.Layouts.SharepointEmails
{
    public partial class ChooseItemPage : LayoutsPageBase
    {
        SPWeb Web { get { return SPContext.Current.Web; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            TreeViewDataSource1.DataBinding += new EventHandler(TreeViewDataSource1_DataBinding);
            tv_Elements.TreeNodeDataBound += new System.Web.UI.WebControls.TreeNodeEventHandler(tv_Elements_TreeNodeDataBound);
        }

        void tv_Elements_TreeNodeDataBound(object sender, System.Web.UI.WebControls.TreeNodeEventArgs e)
        {
        }

        void TreeViewDataSource1_DataBinding(object sender, EventArgs e)
        {
            
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }
    }
}
