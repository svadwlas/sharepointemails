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
            Console.WriteLine("Register (R) or Unregister (U) ? (R/U)");
            var mode = Console.ReadLine().ToLower();
            if (mode == "r" || mode == "u")
            {
                Console.WriteLine("IIS will be reseted and SharePoint Timer Service will be restarted. Continue? (y/n)");
                if (Console.ReadLine().ToLower() == "y")
                {
                    Console.WriteLine("Modifying alerttemplates.xml");
                    var path = SPUtility.GetGenericSetupPath(@"TEMPLATE\XML\");
                    var bin = SPUtility.GetGenericSetupPath(@"BIN\");
                    var file = Path.Combine(path, "alerttemplates.xml");
                    if (mode == "r")
                    {
                        Console.WriteLine("registering custom AlertHandler");
                        AlertHandler.RegisterForAll(file);
                    }
                    else
                    {
                        Console.WriteLine("Unregistering custom AlertHandler");
                        AlertHandler.UnRegisterForAll(file);
                    }
                    Console.WriteLine("Updating alerts");
                    Process.Start(Path.Combine(bin, "stsadm.exe"), "-o updatealerttemplates -url http://localhost/").WaitForExit();
                    Console.WriteLine("IIS reseting");
                    Process.Start("iisreset").WaitForExit();
                    Console.WriteLine("timer restarting");
                    Process.Start("net", "stop SPTimerV4").WaitForExit();
                    Process.Start("net", "start SPTimerV4").WaitForExit();
                    Console.WriteLine("Finished");
                }
                else
                {
                    Console.WriteLine("cancelled");
                }
            }
            else
            {
                Console.WriteLine("Wrong mode");
            }

            Console.WriteLine("Any key...");
            Console.ReadKey();
        }
    }
}
