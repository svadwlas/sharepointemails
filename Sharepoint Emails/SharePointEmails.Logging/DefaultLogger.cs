using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

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
            var line=string.Format("{0,24}: {1,20} {2} {3}"+Environment.NewLine, CurTime, severety, area, text);
            Debug.Write(text);
            var desktop=Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var log=Path.Combine(desktop, "log.txt");
            if(!File.Exists(log))File.WriteAllText(log,"");
            File.AppendAllText(log, line);
        }

        public void Write(Exception ex, SeverityEnum severety, AreasEnum area)
        {
            Write(ex.Message + Environment.NewLine + ex.StackTrace, severety, area);
        }
    }
}
