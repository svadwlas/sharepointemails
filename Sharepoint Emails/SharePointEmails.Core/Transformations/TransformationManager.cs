using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Enums;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Transformations
{
    class TransformationManager
    {
        public IMianTemplateTransformation GetTransformation(MessageFieldType type, ISubstitutionContext context)
        {
            return new XsltTransformation();
        }
    }
}
