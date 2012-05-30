using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public interface ITemplate
    {
        string ProcessedText
        {
            get;
            set;
        }

        TemplateStateEnum State { set; get; }

        int Type { set; get; }

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

        ITemplateOwner Owner
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        int Property
        {
            get;
            set;
        }

        void Update();
        void SaveTo(SPListItem item);
    }
}
