using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    public abstract class BaseSubstitution:ISubstitution
    {
        protected ILogger Logger;
        public BaseSubstitution()
        {
               Logger = Application.Current.Logger;
        }

        public abstract string Process(string text, ISubstitutionContext context);

        public virtual ISubstitutionWorker Worker
        {
            get;
            set;
        }
    }
}
