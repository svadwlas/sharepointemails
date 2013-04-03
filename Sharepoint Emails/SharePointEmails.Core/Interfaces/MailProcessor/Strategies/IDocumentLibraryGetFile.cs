using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.MailProcessors;

namespace SharePointEmails.Core.Interfaces.MailProcessor.Strategies
{
    interface IDocumentLibraryGetFile
    {
        byte[] GetFile(SEMessage message);
    }
}
