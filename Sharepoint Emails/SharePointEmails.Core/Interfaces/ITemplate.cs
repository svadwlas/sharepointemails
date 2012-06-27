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
        TemplateStateEnum State { set; get; }

        int EventTypes { set; get; }

        string From { set; get; }

        string Replay { set; get; }

        string Subject { set; get; }

        string Body { get; set; }

        Guid Id { get; set; }

        string Name { get; set; }

        AssociationConfiguration Asses { get; set; }

        void SaveChanges();

        void SaveTo(SPListItem item);

        string GetProcessedBody(ISubstitutionContext context, ProcessMode mode);

        string GetProcessedSubj(ISubstitutionContext context, ProcessMode mode);
        string GetProcessedFrom(ISubstitutionContext context, ProcessMode mode);
        string GetProcessedReplay(ISubstitutionContext context, ProcessMode mode);
    }
}
