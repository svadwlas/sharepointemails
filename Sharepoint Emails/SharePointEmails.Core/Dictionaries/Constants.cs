using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core
{
    public static class Constants
    {
        static public string TemplateListName
        {
            get
            {
                return "HiddenTemplatesList";
            }
        }

        static public Guid FeatureId
        {
            get
            {
                return new Guid("{50e13f0b-69b1-43cf-8627-d782e7efa4cc}");
            }
        }
    }
}
