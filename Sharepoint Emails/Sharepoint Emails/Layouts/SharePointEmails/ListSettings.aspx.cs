using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Linq;
using SharePointEmails.MailProcessors;
using SharePointEmails.Core.MailProcessors;
namespace SharePointEmails.Layouts.SharePointEmails
{
    public partial class ListSettings : LayoutsPageBase
    {
        SPList m_list = null;

        Type ReceiverType { get { return typeof(EmailReceiver); } }

        ProcessorsManager Manager { get { return ProcessorsManager.Instance; } }

        void UpdateView()
        {
            if (m_list != null)
            {
                lbl_ListName.Text = m_list.Title;
                
                var emailReceivers = Manager.GetReceivers(m_list);
                cb_Enabled.Checked = Manager.IsIntegrationEnabled(m_list);
                if (emailReceivers.Any())
                {
                    cb_Enabled.ToolTip = emailReceivers.Aggregate((s1, s2) => s1 + Environment.NewLine + s2);
                }
                else
                {
                    cb_Enabled.ToolTip = "No receivers";
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_list = SPContext.Current.List;
            if (m_list != null)
            {
                btn_Save.Click += new EventHandler(btn_Save_Click);              
            }
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            if (cb_Enabled.Checked)
            {
                Manager.EnableDiscussionBoardProcessing(m_list);
                lbl_Message.Text = "Registered";
            }
            else
            {
                Manager.DisableDiscussionBoardProcessing(m_list);
                lbl_Message.Text = "UnRegistered";
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateView();
        }
    }
}
