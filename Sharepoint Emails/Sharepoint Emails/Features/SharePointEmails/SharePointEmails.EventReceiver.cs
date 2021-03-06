using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using SharePointEmails.Core;
using System.Text;
using SharePointEmails.Core.Associations;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using SharePointEmails.SwitchWebPart;
using SharePointEmails.Logging;

namespace SharePointEmails.Features.SharePointEmails
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("63938aa4-f966-4e4b-ba49-48a5f2125e47")]
    public class SharePointEmailsEventReceiver : SPFeatureReceiver
    {
        
        const string TEMP_FILE = "SE_tmp_template_file";
        const int SERVICE_NOT_STARTED = 2;

        /// <summary>
        /// Updates alert templates file for site
        /// </summary>
        /// <param name="site"></param>
        /// <param name="alertsFile"></param>
        void ExecuteUpdate(SPSite site, string alertsFile)
        {
            //update alert templates file
            var bin = SPUtility.GetGenericSetupPath(@"BIN\");
            ExecuteShellCommand(Path.Combine(bin, "stsadm.exe"), GetUpdateAlertsParams(site, alertsFile));
            //restart timer
            ExecuteShellCommand("net", "stop SPTimerV4", new int[] { SERVICE_NOT_STARTED });
            ExecuteShellCommand("net", "start SPTimerV4");
        }

        string GetUpdateAlertsParams(SPSite site, string alertsFile)
        {
            return string.Format("-o updatealerttemplates -url {0} -f \"{1}\"", site.Url, alertsFile);
        }

        void ExecuteShellCommand(string exe, string param)
        {
            ExecuteShellCommand(exe, param, new int[0]);
        }

        /// <summary>
        /// Execute shell command
        /// </summary>
        /// <param name="exe"></param>
        /// <param name="param"></param>
        /// <param name="ignoreCode"></param>
        void ExecuteShellCommand(string exe, string param, int[] ignoreCode)
        {
            
            Process process = new Process();
            process.StartInfo.FileName = exe;
            process.StartInfo.Arguments = param;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            if (process.WaitForExit(30000))
            {
                if (process.ExitCode != 0&&!ignoreCode.Contains(process.ExitCode))
                {
                    var console = process.StandardOutput.ReadToEnd();
                    throw new Exception(string.Format("Process {0} with params {1} returns {2} \n {3}", process.StartInfo.FileName, process.StartInfo.Arguments, process.ExitCode, console));
                }
            }
            else
            {
                var console = process.StandardOutput.ReadToEnd();
                try
                {
                    process.Kill();
                }
                catch { }
                throw new Exception(string.Format("Process timeout {0} with params {1} returns {2} \n {3}", process.StartInfo.FileName, process.StartInfo.Arguments, process.ExitCode, console));

            }
        }

        string GetOrCreateDefaultAlertFile()
        {
            var path = SPUtility.GetGenericSetupPath(@"TEMPLATE\XML\");
            var alertsFile = Path.Combine(path, "alerttemplates.xml");
            if (!File.Exists(alertsFile))
            {
                File.WriteAllText(alertsFile, Properties.Resources.alerttemplates);
            }
            return alertsFile;
        }

        /// <summary>
        /// register custom alert handler for site
        /// </summary>
        /// <param name="site"></param>
        private void RegisterAlertHandler(SPSite site)
        {
            DeleteTMPFile(site);
            var alertFile = GetOrCreateDefaultAlertFile();
            string changedFile;
            AlertHandler.RegisterForAll(alertFile, out changedFile);
            site.RootWeb.Properties[TEMP_FILE] = changedFile;
            site.RootWeb.Properties.Update();
            ExecuteUpdate(site, changedFile);
        }

        private void UnRegisterAlertHandler(SPSite site)
        {
            DeleteTMPFile(site);
            var alertsFile = GetOrCreateDefaultAlertFile();
            ExecuteUpdate(site, alertsFile);
        }

        /// <summary>
        /// register Web Part for switching of fields
        /// </summary>
        /// <param name="web"></param>
        private void RegisterWebParts(SPWeb web)
        {
            try
            {
                string tohide = "[TemplateBodyUseFile{true:TemplateBody;false:TemplateBodyFile;}]"
                               +"[TemplateSubjectUseFile{true:TemplateSubject;false:TemplateSubjectFile;}]"
                               +"[TemplateReplayUseFile{true:TemplateReplay;false:TemplateReplayFile;}]"
                               +"[TemplateFromUseFile{true:TemplateFrom;false:TemplateFromFile;}]";
                var templates = web.Lists[Constants.TemplateListName];
                var wm = web.GetLimitedWebPartManager(templates.DefaultEditFormUrl, System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
                var wp = new SwitchWebPart.SwitchWebPart() { Info = tohide };
                wm.AddWebPart(wp, null, 0);
                wm.SaveChanges(wp);
                wm = web.GetLimitedWebPartManager(templates.DefaultNewFormUrl, System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
                wp=new SwitchWebPart.SwitchWebPart() {Info=tohide };
                wm.AddWebPart(wp, null, 0);
                wm.SaveChanges(wp);
            }
            catch (Exception ex)
            {
                Application.Current.Logger.WriteTrace(ex, global::SharePointEmails.Logging.SeverityEnum.CriticalError);
            }
        }

        /// <summary>
        /// Upload default XSLT files and add default email  templates to template list
        /// </summary>
        /// <param name="web"></param>
        private void UploadBasicFiles(SPWeb web)
        {
            try
            {
                var files = new List<DefaultTemplateFile>
                {
                    new DefaultTemplateFile {Name="subj.xslt",Bytes=Encoding.Default.GetBytes(Properties.Resources.subjXslt)},
                    new DefaultTemplateFile{Name="BodyTemplate.xslt",Bytes= Encoding.Default.GetBytes(Properties.Resources.BodyTemplate)},
                    new DefaultTemplateFile{Name="BodyTemplateForDiscussionBoard.xslt",Bytes= Encoding.Default.GetBytes(Properties.Resources.BodyTemplateForDiscussionBoard)},
                    new DefaultTemplateFile{Name="ListAddressTemplate.xslt", Bytes=Encoding.Default.GetBytes(Properties.Resources.ListAddressTemplate)},
                    new DefaultTemplateFile{Name="AdminAddressTemplate.xslt", Bytes=Encoding.Default.GetBytes(Properties.Resources.AdminAddressTemplate)},
                    new DefaultTemplateFile{Name="Utils.xslt", Bytes=Encoding.Default.GetBytes(Properties.Resources.Utils)},
                    new DefaultTemplateFile{Name="EmailHeader.xslt", Bytes=Encoding.Default.GetBytes(Properties.Resources.EmailHeader)},
                    new DefaultTemplateFile{Name="Styles.xslt", Bytes=Encoding.Default.GetBytes(Properties.Resources.Styles)},
                    new DefaultTemplateFile{Name="BodyTemplateForTasks.xslt", Bytes=Encoding.Default.GetBytes(Properties.Resources.BodyTemplateForTasks)}
                };

                Func<string, SPFieldLookupValue> getId = (s) =>
                    {
                        return new SPFieldLookupValue(files.IndexOf(files.Single(p => string.Equals(p.Name, s, StringComparison.CurrentCultureIgnoreCase))) + 1, s);
                    };

                var list = web.Lists[Constants.XsltLibrary] as SPDocumentLibrary;
                foreach (var p in files)
                {
                    list.RootFolder.Files.Add(p.Name, p.Bytes);
                }
                list.Update();

                foreach (SPListItem item in list.Items)
                {
                    item["Title"] = item.File.Name;
                    item.Update();
                }

                var templates = web.Lists[Constants.TemplateListName];
                var templ = templates.AddItem();

                templ[TemplateCT.TemplateName] = "Default template";
                templ[TemplateCT.TemplateState] = TemplateCT.StateChoices.Published;
                templ[TemplateCT.TemplateType] = new SPFieldMultiChoiceValue(TemplateCT.TypeChoices.All);
                templ[TemplateCT.TemplateFromUseFile] = true;
                templ[TemplateCT.TemplateFromFile] = getId("AdminAddressTemplate.xslt");
                templ[TemplateCT.TemplateSubjectUseFile] = true;
                templ[TemplateCT.TemplateBodyUseFile] = true;
                templ[TemplateCT.TemplateSubjectFile] = getId("subj.xslt");
                templ[TemplateCT.TemplateBodyFile] = getId("BodyTemplate.xslt");
                templ[TemplateCT.Associations] = new AssociationCollection
                {
                    new GroupAssociation
                    {
                        ItemType=GroupType.AllList,
                        Name="Default Lists association"
                    },
                    new GroupAssociation
                    {
                        ItemType=GroupType.AllDocumentLibrary,
                        Name="Default Libraries association"
                    }
                }.ToString();
                templ.Update();

                var temp2= templates.AddItem();

                temp2[TemplateCT.TemplateName] = "Default template for discussion board";
                temp2[TemplateCT.TemplateState] = TemplateCT.StateChoices.Published;
                temp2[TemplateCT.TemplateType] = new SPFieldMultiChoiceValue(TemplateCT.TypeChoices.All);
                temp2[TemplateCT.TemplateFromUseFile] = true;
                temp2[TemplateCT.TemplateFromFile] = getId("ListAddressTemplate.xslt"); 
                temp2[TemplateCT.TemplateReplayUseFile] = true;
                temp2[TemplateCT.TemplateReplayFile] = getId("ListAddressTemplate.xslt"); 
                temp2[TemplateCT.TemplateSubjectUseFile] = true;
                temp2[TemplateCT.TemplateBodyUseFile] = true;
                temp2[TemplateCT.TemplateSubjectFile] = getId( "subj.xslt");
                temp2[TemplateCT.TemplateBodyFile] = getId("BodyTemplateForDiscussionBoard.xslt");
                temp2[TemplateCT.Associations] = new AssociationCollection
                {
                    new GroupAssociation
                    {
                        ItemType=GroupType.AllDiscusions,
                        Name="Default association for all discussions"
                    },
                     new GroupAssociation
                    {
                        ItemType=GroupType.AllMessages,
                        Name="Default association for all messages"
                    },
                }.ToString();
                temp2.Update();

                var temp3 = templates.AddItem();

                temp3[TemplateCT.TemplateName] = "Default template for tasks";
                temp3[TemplateCT.TemplateState] = TemplateCT.StateChoices.Published;
                temp3[TemplateCT.TemplateType] = new SPFieldMultiChoiceValue(TemplateCT.TypeChoices.All);
                temp3[TemplateCT.TemplateFromUseFile] = true;
                temp3[TemplateCT.TemplateFromFile] = getId("ListAddressTemplate.xslt");
                temp3[TemplateCT.TemplateReplayUseFile] = true;
                temp3[TemplateCT.TemplateReplayFile] = getId("ListAddressTemplate.xslt");
                temp3[TemplateCT.TemplateSubjectUseFile] = true;
                temp3[TemplateCT.TemplateBodyUseFile] = true;
                temp3[TemplateCT.TemplateSubjectFile] = getId("subj.xslt");
                temp3[TemplateCT.TemplateBodyFile] = getId("BodyTemplateForTasks.xslt");
                temp3[TemplateCT.Associations] = new AssociationCollection
                {
                    new GroupAssociation
                    {
                        ItemType=GroupType.AllTasks,
                        Name="Default association for all discussions"
                    }
                }.ToString();
                temp3.Update();
            }
            catch (Exception ex)
            {
                Application.Current.Logger.WriteTrace(ex, global::SharePointEmails.Logging.SeverityEnum.CriticalError);
            }
        }

        private void DeleteList(SPWeb web, string title)
        {
            var list = web.Lists.TryGetList(title);
            if (list != null)
            {
                list.Delete();
                web.Update();
            }
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            Application.Current.Logger.WriteTrace("Feature activating", SeverityEnum.Information);
            if (properties.Feature.Parent is SPSite)
            {
                var site = (SPSite)properties.Feature.Parent;
                UploadBasicFiles(site.RootWeb);
                RegisterWebParts(site.RootWeb);
                RegisterAlertHandler(site);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            if (properties.Feature.Parent is SPSite)
            {
                var site = (SPSite)properties.Feature.Parent;
                DeleteList(site.RootWeb, Constants.TemplateListName);
                DeleteList(site.RootWeb, Constants.XsltLibrary);
                UnRegisterAlertHandler(site);
            }
        }

        void DeleteTMPFile(SPSite site)
        {
            var web = site.RootWeb;
            if (web.Properties.ContainsKey(TEMP_FILE))
            {
                try
                {
                    File.Delete(web.Properties[TEMP_FILE]);
                }
                catch { }
                web.Properties.Remove(TEMP_FILE);
                web.Properties.Update();
            }
        }

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}

        class DefaultTemplateFile
        {
            public string Name;
            public byte[] Bytes;
        }
    }
}
