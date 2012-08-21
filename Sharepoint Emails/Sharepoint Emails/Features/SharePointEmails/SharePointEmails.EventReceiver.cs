using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using SharePointEmails.Core;
using System.Text;
using SharePointEmails.Core.Associations;
using Microsoft.SharePoint.WebPartPages;

namespace SharepointEmails.Features.SharePointEmails
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
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            if (properties.Feature.Parent is SPSite)
            {
                var site = (SPSite)properties.Feature.Parent;
                UploadBasicFiles(site.RootWeb);
                RegisterWebParts(site.RootWeb);
            }
        }

        private void RegisterWebParts(SPWeb sPWeb)
        {
            try
            {
                string tohide = "[TemplateBodyUseFile{true:TemplateBody;false:TemplateBodyFile;}]"
                               +"[TemplateSubjectUseFile{true:TemplateSubject;false:TemplateSubjectFile;}]"
                               +"[TemplateReplayUseFile{true:TemplateReplay;false:TemplateReplayFile;}]"
                               +"[TemplateFromUseFile{true:TemplateFrom;false:TemplateFromFile;}]";
                var templates = sPWeb.Lists[Constants.TemplateListName];
                var wm = sPWeb.GetLimitedWebPartManager(templates.DefaultEditFormUrl, System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
                var wp=new SharepointEmails.SwitchWebPart.SwitchWebPart() {Info=tohide };
                wm.AddWebPart(wp, null, 0);
                wm.SaveChanges(wp);
                wm = sPWeb.GetLimitedWebPartManager(templates.DefaultNewFormUrl, System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
                wp=new SharepointEmails.SwitchWebPart.SwitchWebPart() {Info=tohide };
                wm.AddWebPart(wp, null, 0);
                wm.SaveChanges(wp);
            }
            catch (Exception ex)
            {
                Application.Current.Logger.Write(ex, global::SharePointEmails.Logging.SeverityEnum.CriticalError);
            }
        }

        private void UploadBasicFiles(SPWeb sPWeb)
        {
            try
            {
                var list = sPWeb.Lists[Constants.XsltLibrary] as SPDocumentLibrary;
                list.RootFolder.Files.Add("subj.xslt", Encoding.Default.GetBytes(Properties.Resources.subjXslt));
                list.RootFolder.Files.Add("body.xslt", Encoding.Default.GetBytes(Properties.Resources.testbody));
                list.RootFolder.Files.Add("BodyTemplate.xslt", Encoding.Default.GetBytes(Properties.Resources.BodyTemplate));
                list.RootFolder.Files.Add("BodyTemplateForDiscussionBoard.xslt", Encoding.Default.GetBytes(Properties.Resources.BodyTemplateForDiscussionBoard));
                list.RootFolder.Files.Add("ListAddressTemplate.xslt", Encoding.Default.GetBytes(Properties.Resources.ListAddressTemplate));
                list.RootFolder.Files.Add("AdminAddressTemplate.xslt", Encoding.Default.GetBytes(Properties.Resources.AdminAddressTemplate));
                list.Update();

                foreach (SPListItem item in list.Items)
                {
                    item["Title"] = item.File.Name;
                    item.Update();
                }

                var templates = sPWeb.Lists[Constants.TemplateListName];
                var templ = templates.AddItem();

                templ[TemplateCT.TemplateName] = "Default template";
                templ[TemplateCT.TemplateState] = TemplateCT.StateChoices.Published;
                templ[TemplateCT.TemplateType] = new SPFieldMultiChoiceValue(TemplateCT.TypeChoices.All);
                templ[TemplateCT.TemplateFromUseFile] = true;
                templ[TemplateCT.TemplateFromFile] = new SPFieldLookupValue(6, "AdminAddressTemplate.xslt"); ;
                templ[TemplateCT.TemplateSubjectUseFile] = true;
                templ[TemplateCT.TemplateBodyUseFile] = true;
                templ[TemplateCT.TemplateSubjectFile] = new SPFieldLookupValue(1, "subj.xslt");
                templ[TemplateCT.TemplateBodyFile] = new SPFieldLookupValue(3, "BodyTemplate.xslt");
                templ[TemplateCT.Associations] = new AssociationConfiguration
                {
                    new GroupAssociation
                    {
                        ItemType=GroupType.AllList,
                        Name="Default association"
                    }
                }.ToString();
                templ.Update();

                var temp2= templates.AddItem();

                temp2[TemplateCT.TemplateName] = "Default template for discussion board";
                temp2[TemplateCT.TemplateState] = TemplateCT.StateChoices.Published;
                temp2[TemplateCT.TemplateType] = new SPFieldMultiChoiceValue(TemplateCT.TypeChoices.All);
                temp2[TemplateCT.TemplateFromUseFile] = true;
                temp2[TemplateCT.TemplateFromFile] = new SPFieldLookupValue(5, "ListAddressTemplate.xslt"); ;
                temp2[TemplateCT.TemplateSubjectUseFile] = true;
                temp2[TemplateCT.TemplateBodyUseFile] = true;
                temp2[TemplateCT.TemplateSubjectFile] = new SPFieldLookupValue(1, "subj.xslt");
                temp2[TemplateCT.TemplateBodyFile] = new SPFieldLookupValue(4, "BodyTemplateForDiscussionBoard.xslt");
                temp2[TemplateCT.Associations] = new AssociationConfiguration
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
            }
            catch (Exception ex)
            {
                Application.Current.Logger.Write(ex, global::SharePointEmails.Logging.SeverityEnum.CriticalError);
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            if (properties.Feature.Parent is SPSite)
            {
                var site = (SPSite)properties.Feature.Parent;
                DeleteLists(site.RootWeb, Constants.TemplateListName);
                DeleteLists(site.RootWeb, Constants.XsltLibrary);
            }
        }

        private void DeleteLists(SPWeb web, string title)
        {
            var list = web.Lists.TryGetList(title);
            if (list != null)
            {
                list.Delete();
                web.Update();
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
    }
}
