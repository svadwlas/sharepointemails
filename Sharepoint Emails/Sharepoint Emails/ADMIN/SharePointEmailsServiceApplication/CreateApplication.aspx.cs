using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.ApplicationPages;
using System.Web.UI.WebControls;

namespace SharepointEmails.Layouts.SharepointEmailsServiceApplication
{
    public partial class CreateApplication : GlobalAdminPageBase
    {
        // Fields
        protected RequiredFieldValidator AppNameValidator;
        protected IisWebServiceApplicationPoolSection AppPoolSection;
        protected Button ButtonOk;
        protected CheckBox CheckBoxDefault;
        protected TextBox TextBoxAppName;
        protected CustomValidator UniqueNameValidator;

        // Methods
        protected void OkButton_Click(object sender, EventArgs e)
        {
            //ULS.SendTraceTag(0x39766663, UlsInformation.Management, ULSTraceLevel.Verbose, "ExcelServerCreateApplication.ButtonNext_Click: Entering ButtonNext_Click...");
            this.Page.Validate();
            if (this.Page.IsValid)
            {
                string applicationName = this.TextBoxAppName.Text.Trim();
                SPLongOperation.Begin(delegate(SPLongOperation longOperation)
                {
                    try
                    {
                        SharePointEmailsServiceApplication serviceApplication = null;
                        serviceApplication = SharePointEmailsService.Local.CreateApplication(applicationName, this.AppPoolSection.GetOrCreateApplicationPool());
                        serviceApplication.Update();
                        IAsyncResult asyncResult = serviceApplication.BeginProvision(null, null);
                        serviceApplication.EndProvision(asyncResult);
                        SharePointEmailsServiceApplicationProxy proxy = SharePointEmailsService.Local.CreateProxy(applicationName, serviceApplication);
                        proxy.Update();
                        proxy.AddToDefaultGroup(this.CheckBoxDefault.Checked);
                    }
                    catch (Exception)
                    {
                        //ULS.SendTraceTag(0x39766664, UlsInformation.Management, ULSTraceLevel.High, "ExcelServerCreateApplication.ButtonNext_Click: {0}", new object[] { exception.Message });
                        throw;
                    }
                    longOperation.EndScript("window.frameElement.commonModalDialogClose(1, null);");
                });
            }
            //ULS.SendTraceTag(0x39766665, UlsInformation.Management, ULSTraceLevel.Verbose, "ExcelServerCreateApplication.ButtonNext_Click: Exiting ButtonNext_Click.");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ((DialogMaster)this.Page.Master).OkButton.Click += new EventHandler(this.OkButton_Click);
        }

        protected void ValidateUniqueName(object sender, ServerValidateEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            SharePointEmailsServiceApplication applicationByName = SharePointEmailsServiceApplication.GetApplicationByName(this.TextBoxAppName.Text.Trim());
            eventArgs.IsValid = applicationByName == null;
        }
    }
}
