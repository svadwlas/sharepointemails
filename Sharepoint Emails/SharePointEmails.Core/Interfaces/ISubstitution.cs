using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public interface ISubstitution
    {
        string Process(string text, ISubstitutionContext context, Func<string, string> IncludeResolver=null);
    }
}
