﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Substitutions;
using System.Globalization;

namespace SharePointEmails.Core.Interfaces
{
    public interface ISubstitutionContext
    {
        List<FieldChange> Changes { get; }

        string GetField(string fieldName, ModifiersCollection modifiers);
        string GetContextValue(string value, ModifiersCollection modifiers = null);

        CultureInfo GetDestinationCulture();

        string GetXML();

        SPDocumentLibrary GetTemplateLibrary();
    }
}
