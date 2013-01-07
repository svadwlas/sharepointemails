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

        AssociationCollection Associations { get; set; }

        IEnumerable<string> SendDraftToAdresses { get; set; }

        void SaveChanges();
    }
}
