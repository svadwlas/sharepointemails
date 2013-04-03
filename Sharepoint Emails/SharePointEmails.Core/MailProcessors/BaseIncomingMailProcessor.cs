using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.MailProcessors
{
    abstract class BaseIncomingMailProcessor : IIncomingMessageProcessor
    {
        abstract public void Process();
    }
}
