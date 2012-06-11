using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public interface ITemplate
    {
        string GetProcessedText(ISubstitutionContext context);

        TemplateStateEnum State { set; get; }

        int EventTypes { set; get; }

        string Pattern
        {
            get;
            set;
        }

        bool IsValid
        {
            get;
            set;
        }

        Guid Id
        {
            get;
            set;
        }

        string LastModifiedBy
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        AssociationConfiguration Asses { get; set; }

        void Update();
        void SaveTo(SPListItem item);
    }
}
