using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SharePointEmails.Core;
using SharePointEmails.Logging;
using System.Configuration;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.MailProcessors;
using System.ComponentModel;

namespace SharepointEmails
{
    [Description("Processor of incoming emails")]
    [DisplayName("SharePointEmails email receiver")]
    public class EmailReceiver : SPEmailEventReceiver
    {
        ILogger m_Logger = null;

        public EmailReceiver()
        {
            m_Logger = ClassContainer.Instance.Resolve<ILogger>();
            if (m_Logger == null) throw new ConfigurationErrorsException("No logger configured");
        }

        public override void EmailReceived(SPList list, SPEmailMessage emailMessage, string receiverData)
        {
            m_Logger.Write("List " + list.Title + " recieved mail from " + emailMessage.EnvelopeSender, SeverityEnum.Trace);
            try
            {             
                var processor = CreateIncomingProcessor(list, emailMessage);
                if (processor != null)
                {
                    m_Logger.Write(processor.GetType().FullName + " was found as incoming processor for list " + list.Title, SeverityEnum.Trace);
                    try
                    {
                        processor.Process();
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Write("Error during processing of message", SeverityEnum.CriticalError);
                        m_Logger.Write(ex, SeverityEnum.CriticalError);
                    }
                }
                else
                {
                    m_Logger.Write("No incoming processor found for list " + list.Title, SeverityEnum.Trace, AreasEnum.IncomingMessages);
                }
            }
            catch (Exception ex)
            {
                m_Logger.Write("Error in the handler", SeverityEnum.CriticalError);
                m_Logger.Write(ex, SeverityEnum.CriticalError);
            }
        }

        IIncomingMessageProcessor CreateIncomingProcessor(SPList list, SPEmailMessage message)
        {
            try
            {
                if (list.BaseTemplate == SPListTemplateType.DiscussionBoard)
                {
                    return new IncomingDiscussionBoardProcessor(list, message, m_Logger);
                }
            }
            catch (Exception ex)
            {
                m_Logger.Write("Error during processor creating", SeverityEnum.CriticalError);
                m_Logger.Write(ex, SeverityEnum.CriticalError);
            }
            return null;
        }
    }
}
