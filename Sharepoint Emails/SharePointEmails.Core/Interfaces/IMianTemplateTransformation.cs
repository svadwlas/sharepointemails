using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core.Interfaces
{
    public interface IMianTemplateTransformation
    {
        string Transform(string template, ISubstitutionContext context, Func<string, string> partIncludedCallback);
    }
}
