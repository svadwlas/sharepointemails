using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core.Interfaces
{
    public interface ISubstitutionWorker
    {
        string PreProcess(string data, ISubstitutionContext context);
        string PostProcess(string data, ISubstitutionContext context);
        string OnPartLoaded(string part);
    }
}
