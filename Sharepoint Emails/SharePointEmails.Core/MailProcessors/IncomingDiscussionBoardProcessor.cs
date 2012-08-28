using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Interfaces;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SharePointEmails.MailProcessors
{
    public class IncomingDiscussionBoardProcessor : IIncomingMessageProcessor
    {
        SPList m_list = null;
        SPEmailMessage m_message = null;
        bool isParsed = false;
        public IncomingDiscussionBoardProcessor(SPList list, SPEmailMessage message)
        {
            this.m_list = list;
            this.m_message = message;
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

        public void Process()
        {
            Parse();
        }
    }
}
