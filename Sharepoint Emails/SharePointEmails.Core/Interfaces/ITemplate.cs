using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Substitutions;
using SharePointEmails.Core.Associations;

namespace SharePointEmails.Core.Interfaces
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

        IEnumerable<string> SendDraftToAdresses { get; set; }

        void SaveChanges();

        void SaveTo(SPListItem item);

        string GetProcessedBody(ISubstitutionContext context);

        string GetProcessedSubj(ISubstitutionContext context);
        string GetProcessedFrom(ISubstitutionContext context);
        string GetProcessedReplay(ISubstitutionContext context);
    }
}
