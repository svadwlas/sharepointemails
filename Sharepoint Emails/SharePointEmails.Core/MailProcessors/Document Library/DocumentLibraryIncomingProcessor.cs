using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.MailProcessors.Document_Library
{
    class DocumentLibraryIncomingProcessor : IIncomingMessageProcessor
    {
        internal DocumentLibraryIncomingProcessor(SPDocumentLibrary library, SEMessage message)
        {
        }

        public void Process()
        {
            throw new NotImplementedException();
        }
    }
}
