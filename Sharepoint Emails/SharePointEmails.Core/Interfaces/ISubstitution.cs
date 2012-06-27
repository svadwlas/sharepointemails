using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public interface ISubstitutionWorker
    {
        string Process(string data, ProcessMode mode);
    }

    public interface ISubstitution
    {
        string Pattern
        {
            get;
        }

        string Description
        {
            get;
        }

        string Process(string text, ISubstitutionContext context, ProcessMode mode);
    }
}
