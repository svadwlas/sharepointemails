using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core
{
    public interface ISubstitutionWorker
    {
        void Process(string data);
    }

    public interface ISubstitution
    {
        string Pattern
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        string TestValue
        {
            get;
            set;
        }

        string Process(string text, ISubstitutionContext context);
    }
}
