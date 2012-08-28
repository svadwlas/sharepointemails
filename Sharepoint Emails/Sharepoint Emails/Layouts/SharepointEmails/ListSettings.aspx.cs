using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Linq;
using SharePointEmails.MailProcessors;
namespace SharepointEmails.Layouts.SharepointEmails
{
    public partial class ListSettings : LayoutsPageBase
    {
        SPList m_list = null;

        Type ReceiverType { get { return typeof(EmailReceiver); } }

        void UpdateView()
        {
            if (m_list != null)
            {
                lbl_ListName.Text = m_list.Title;
                var emailReceivers = GetReceivers();
                cb_Enabled.Checked = emailReceivers.Any(p => p == ReceiverType.FullName);
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
            //System.Diagnostics.Debugger.Launch();
            m_list = SPContext.Current.List;
            if (m_list != null)
            {
                btn_Save.Click += new EventHandler(btn_Save_Click);              
            }
        }

        IEnumerable<string> GetReceivers()
        {
            return m_list.EventReceivers.OfType<SPEventReceiverDefinition>().Where(p => p.Type == SPEventReceiverType.EmailReceived).Select(p => p.Class);
        }

        void UnRegister(string type)
        {
            SPEventReceiverDefinition receiver = null;
            do
            {
                receiver = m_list.EventReceivers.OfType<SPEventReceiverDefinition>().Where(p => p.Class == type).FirstOrDefault();
                if (receiver != null)
                {
                    receiver.Delete();
                }

            } while (receiver != null);
        }

        void Register(Type type)
        {
            var def = m_list.EventReceivers.Add();
            def.Assembly = type.Assembly.FullName;
            def.Class = type.FullName;
            def.Name = "SharePointEmails Email Receiver";
            def.Type = SPEventReceiverType.EmailReceived;
            def.SequenceNumber = 999;
            def.Synchronization = SPEventReceiverSynchronization.Synchronous;
            def.Update();
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            
                UnRegister(ReceiverType.FullName);
                if (cb_Enabled.Checked)
                {
                    Register(ReceiverType);
                    lbl_Message.Text = "Registered";
                }
                else
                {
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
