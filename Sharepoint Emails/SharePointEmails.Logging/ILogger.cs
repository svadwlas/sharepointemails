using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Logging
{
    public interface ILogger
    {
        void Write(string text, SeverityEnum severety, AreasEnum area=AreasEnum.DefaultArea);
        void Write(Exception ex, SeverityEnum severety, AreasEnum area=AreasEnum.DefaultArea);
    }
}
