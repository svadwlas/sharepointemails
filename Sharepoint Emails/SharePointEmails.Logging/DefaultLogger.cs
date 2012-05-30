using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SharePointEmails.Logging
{
    public class DefaultLogger : ILogger
    {
        string CurTime
        {
            get
            {
                return DateTime.Now.ToString("hh:mm:ss dd mm yyyy");
            }
        }

        public void Write(string text, SeverityEnum severety, AreasEnum area)
        {
            Debug.Write(string.Format("{0,24}: {1,20} {2} {3}", CurTime, severety, area, text));
        }

        public void Write(Exception ex, SeverityEnum severety, AreasEnum area)
        {
            Write(ex.Message + Environment.NewLine + ex.StackTrace, severety, area);
        }
    }
}
