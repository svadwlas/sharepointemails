using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Logging
{
    public interface ILogger
    {
        void WriteTrace(string text, SeverityEnum severety, Category area=Category.Default);
        void WriteTrace(Exception ex, SeverityEnum severety, Category area=Category.Default);
    }
}
