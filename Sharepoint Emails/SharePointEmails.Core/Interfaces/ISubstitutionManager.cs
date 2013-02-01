using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Enums;

namespace SharePointEmails.Core.Interfaces
{
    public interface ISubstitutionManager
    {
        ISubstitutionWorker GetWorker(ISubstitutionContext context, MessageFieldType type);
    }
}
