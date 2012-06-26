using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public interface ITemplate
    {
        string GetProcessedText(ISubstitutionContext context, ProcessMode mode);
        string GetProcessedSubj(ISubstitutionContext context, ProcessMode mode);

        TemplateStateEnum State { set; get; }

        int EventTypes { set; get; }

        string Subject { set; get; }

        string Body
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
