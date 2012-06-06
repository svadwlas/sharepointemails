using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;

namespace SharePointEmails.Core
{
    public class SubstitutionManager
    {
        public ISubstitutionWorker GetWorker(ISubstitutionContext context)
        {
            return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), context);
        }
    }
}
