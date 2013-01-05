using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.SharePoint.Administration;

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

        private DiagnosticService Local
        {
            get
            {
                if (_local == null)
                {
                    _local = DiagnosticService.Local;
                }
                if (_local == null) throw new NullReferenceException("DiagnosticService is null");
                return _local;
            }
        }DiagnosticService _local;

        public void WriteTrace(string text, SeverityEnum severety, Category area)
        {
            Local.WriteTrace(0, Local[area], Get(severety), text);

            //var line=string.Format("{0,24}: {1,20} {2} {3}"+Environment.NewLine, CurTime, severety, area, text);
            //Debug.Write(text);
            //var desktop=Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //var log=Path.Combine(desktop, "log.txt");
            //if(!File.Exists(log))File.WriteAllText(log,"");
            //File.AppendAllText(log, line);

        }

        TraceSeverity Get(SeverityEnum severity)
        {
            switch (severity)
            {
                case SeverityEnum.CriticalError: return TraceSeverity.Unexpected;
                case SeverityEnum.Error: return TraceSeverity.High;
                case SeverityEnum.Information: 
                case  SeverityEnum.Warning:
                    return TraceSeverity.Monitorable;
                case SeverityEnum.Trace: return TraceSeverity.Verbose;
                case SeverityEnum.Verbose: return TraceSeverity.VerboseEx;
                default: return TraceSeverity.VerboseEx;
            }
        }

        public void WriteTrace(Exception ex, SeverityEnum severety, Category area)
        {
            WriteTrace(ex.Message + Environment.NewLine + ex.StackTrace, severety, area);
        }
    }
}
