using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Substitutions;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Interfaces
{
    public interface ISubstitution
    {
        string Process(string text, ISubstitutionContext context);
        ISubstitutionWorker Worker { get; set; }
    }
}
