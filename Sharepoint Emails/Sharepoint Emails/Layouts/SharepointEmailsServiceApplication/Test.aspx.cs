using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SharepointEmails.Layouts.SharepointEmailsServiceApplication
{
    public partial class ApplicationPage1 : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            OKButton.Click += new EventHandler(OKButton_Click);
        }

        void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                SharePointEmailsServiceApplicationProxy proxy = SharePointEmailsServiceApplicationProxy.GetProxy(SPServiceContext.Current);
                SharePointEmailsServiceApplication app = proxy.Application;
                string endPoint = app.DefaultEndpoint.ToString();

                ResultLiteral.Text = app.Hello(InputFormTextBoxName.Text);

            }
            catch (Exception ex)
            {
                ResultLiteral.Text = ex.Message;
            }
        }

    }
}
