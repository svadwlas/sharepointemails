using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core
{
    public interface ITemplate
    {
        string ProcessedText
        {
            get;
            set;
        }

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

        void Update();
    }
}
