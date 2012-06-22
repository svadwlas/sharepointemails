using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public interface ISubstitutionContext
    {
        string GetField(string fieldName, ModifiersCollection modifiers);
        List<string> GetAvailableFields();
        string GetContextValue(string value, ModifiersCollection modifiers);
    }
}
