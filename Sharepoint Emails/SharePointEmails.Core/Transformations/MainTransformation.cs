using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.Transformations
{
    abstract class MainTransformation : IMianTemplateTransformation
    {
          protected ILogger Logger;
          public MainTransformation()
        {
               Logger = Application.Current.Logger;
        }
          public abstract string Transform(string template, ISubstitutionContext context, Func<string, string> partIncludedCallback);
    }
}
