using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Interfaces;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Text.RegularExpressions;
using SharePointEmails.Logging;

namespace SharePointEmails.MailProcessors
{
    public class IncomingDiscussionBoardProcessor : IIncomingMessageProcessor
    {
        ILogger m_Logger = null;
        SPList m_list = null;
        SPEmailMessage m_message = null;

        bool isParsed = false;
        public IncomingDiscussionBoardProcessor(SPList list, SPEmailMessage message, ILogger logger)
        {
            this.m_list = list;
            this.m_message = message;
            this.m_Logger = logger;
        }

        private void Parse()
        {
            if (!isParsed)
            {
                try
                {
                    Parse();
                }
                finally
                {
                    isParsed = true;
                }
            }
            
        }

        string m_subject;
        string m_body;

        public void Process()
        {
            SPListItem m_item = GetRelatedListItem();
        }

        int GetItemId(string subject, string body)
        {
            m_Logger.Write("try get id from subj=" + subject, SeverityEnum.Trace);
            var matches = Regex.Matches(subject, @"\(([0-9]+)\)");
            if (matches.Count > 0)
            {
                return int.Parse(matches[0].Groups[1].Value);
            }
            else
            {
                return -1;
            }
        }

        private SPListItem GetRelatedListItem()
        {
            var subject = m_message.Headers["subject"];
            var body = m_message.PlainTextBody;
            var id = GetItemId(subject, body);

            if (id != -1)
            {
                try
                {
                    var item=m_list.GetItemById(id);

                    return item;
                }
                catch (ArithmeticException ex)
                {
                    m_Logger.Write("Item with ID=" + id + " not found", SeverityEnum.Warning);
                    m_Logger.Write(ex, SeverityEnum.Warning);
                }
            }
            return null;
        }
    }
}
