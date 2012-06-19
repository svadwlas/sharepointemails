using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharepointEmails;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Diagnostics;

namespace RegisterAlertHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = SPUtility.GetGenericSetupPath(@"TEMPLATE\XML\");
            var bin = SPUtility.GetGenericSetupPath(@"BIN\");
            var file = Path.Combine(path, "alerttemplates.xml");
            AlertHandler.RegisterForAll(file);
            Process.Start(Path.Combine(bin, "stsadm.exe"), "-o updatealerttemplates -url http://localhost/");
            Process.Start("iisreset");
        }
    }
}
