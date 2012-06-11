using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public interface ISubstitutionContext
    {
        string GetField(string fieldName);
        List<string> GetAvailableFields();
    }
}
