using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SharepointEmails.Layouts.SharepointEmails
{
    public partial class ListSettings : LayoutsPageBase
    {
        const string SE_ENABLED = "SharePointEmailsEnabled";

        protected void Page_Load(object sender, EventArgs e)
        {
            cb_Enabled.CheckedChanged += new EventHandler(cb_Enabled_CheckedChanged);
            cb_Enabled.Checked = Registered;
        }

        void cb_Enabled_CheckedChanged(object sender, EventArgs e)
        {
            Register(cb_Enabled.Checked);
        }

        bool Registered
        {
            set
            {
                if (SPContext.Current.List != null)
                {
                    SPContext.Current.List.RootFolder.Properties[SE_ENABLED] = value;
                    SPContext.Current.List.RootFolder.Update();
                   // SPContext.Current.List.EventReceivers.Add(SPEventReceiverType.EmailReceived, typeof(EmailReceiver).Assembly.FullName, typeof(EmailReceiver).FullName);
                }
            }

            get
            {
                if (SPContext.Current.List != null)
                {
                    return SPContext.Current.List.RootFolder.Properties.ContainsKey(SE_ENABLED) ? (bool)SPContext.Current.List.RootFolder.Properties[SE_ENABLED] : false;
                }
                return false;
            }
        }

        void Register(bool register)
        {
           
        }
    }
}
