using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Substitutions;
using System.Globalization;

namespace SharePointEmails.Core
{
    public interface ISubstitutionContext
    {
        List<FieldChange> Changes { get; }
        string GetField(string fieldName, ModifiersCollection modifiers);
        List<string> GetAvailableFields();
        string GetContextValue(string value, ModifiersCollection modifiers=null);
        CultureInfo getDestinationCulture();

        string GetXML();
    }
}
