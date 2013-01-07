using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Globalization;
using SharePointEmails.Core;
using Microsoft.SharePoint.Utilities;
using System.Xml.Linq;
using SharePointEmails.Core.Exceptions;
using SharePointEmails.Core.Configuration;
using System.Diagnostics;

namespace Test
{
    class Program
    {
//        const string SUCCESS = "SUCCESS";
//        const string FAIL= "FAIL!!!";
//        static readonly Uri TestSiteCollection = new Uri("http://dev/sites/TestSiteCollection");
//        static readonly string SiteName = "TestSiteCollection";
//        static readonly string domain = "test\\";
//        static readonly string User1Login = domain+"user1";
//        static readonly string User1Name = "user1";
//        static readonly string User1Email = "spuser1@mail.ru";
//        static readonly string User2Login =domain+ "user2";
//        static readonly string User2Name= "user2";
//        static readonly string User2Email = "spuser2@mail.ru";
//        static readonly string User3Login =domain+ "user3";
//        static readonly string User3Name = "user3";
//        static readonly string User3Email = "spuser3@mail.ru";
//        static readonly string AdministratorLogin = "administrator";

//        static SPUser user1;
//        static SPUser user2;
//        static SPUser user3;
//        static SPSite testSite = null;
//        static SPWeb rootWeb = null;
//        static SPList root_discussionBoard = null;
//        public static void ClassStart()
//        {
//            if (!SPSite.Exists(TestSiteCollection))
//            {
//                testSite=Install();
//            }
//            else
//            {
//                testSite = new SPSite(TestSiteCollection.ToString());
//            }

//            if (testSite.Features.Any(p => p.DefinitionId == Constants.FeatureId))
//            {
//                testSite.Features.Remove(Constants.FeatureId);
//            }
//            testSite.Features.Add(Constants.FeatureId);
//            rootWeb = testSite.RootWeb;
//            root_discussionBoard = rootWeb.Lists["Team Discussion"];
//            user1 = rootWeb.SiteUsers[User1Login];
//            user2 = rootWeb.SiteUsers[User2Login];
//            user3 = rootWeb.SiteUsers[User3Login];
//        }

//        private static SPSite Install()
//        {
//            var rootUri = new Uri(TestSiteCollection.Scheme + "://" + TestSiteCollection.Host);
//            var root = new SPSite(rootUri.ToString());
//            var webApp = root.WebApplication;
//            var site=webApp.Sites.Add("/sites/"+SiteName, "Site for tests", "", (uint)CultureInfo.CurrentCulture.LCID, "STS#0", AdministratorLogin, AdministratorLogin, "");
//            var web=site.RootWeb;
//            var user1 = web.EnsureUser(User1Login);
//            var user2 = web.EnsureUser(User2Login);
//            var user3 = web.EnsureUser(User3Login);
//            foreach (SPGroup group in web.SiteGroups)
//            {
//                group.AddUser(user1);
//                group.AddUser(user2);
//                group.AddUser(user3);
//            }

//            foreach (SPGroup group in web.Groups)
//            {
//                group.AddUser(user1);
//                group.AddUser(user2);
//                group.AddUser(user3);
//            }
//            return site;
//        }

//        public static void ClassCleanUp()
//        {
//            if (testSite != null)
//            {
//                testSite.Dispose();
//            }
//        }


//        #region tests

//        public static void Test_NotTemplateException()
//        {
//            Console.WriteLine("TestMethod1");
//            var item = SPUtility.CreateNewDiscussion(root_discussionBoard, "Test Discussion");
//            item.Update();

//            //remove all templates
//            var removed = new List<Guid>();
//            var templates = rootWeb.Lists[Constants.TemplateListName];
//            foreach (SPListItem template in templates.Items)
//            {
//                removed.Add(template.Recycle());
//            }

//            string res = FAIL;

//            try
//            {
//                Application.Current.GetMessageForItem(root_discussionBoard, item.ID, SPEventType.Add, XML(item), user1.LoginName, user2.Email, user1.ID);
//            }
//            catch (SeTemplateNotFound)
//            {
//                res = SUCCESS;
//            }
//            finally
//            {
//                item.Delete();

//                //restore all templates
//                rootWeb.RecycleBin.Restore(removed.ToArray());
//            }
//            Console.WriteLine(res);
//        }

//        public static void Test_DefaultTemplates()
//        {
//            Console.WriteLine("Test_DefaultTemplates");
//            var item = SPUtility.CreateNewDiscussion(root_discussionBoard, "Test Discussion Text");
//            item.Update();
//            try
//            {
//                var message = Application.Current.GetMessageForItem(root_discussionBoard, item.ID, SPEventType.Add, XML(item), user1.LoginName, user2.Email, user1.ID);
//                Debug.Assert(message != null);
//                Debug.Assert(message.Body.Contains("Test Discussion Text"));
//                Debug.Assert(message.Body.Contains("Discussion Subject"));
//            }
//            finally
//            {
//                item.Delete();
//            }
//        }

//        #endregion

//        public static void AddTestTemplates()
//        {
//            var list = rootWeb.Lists.TryGetList(Constants.TemplateListName);
//            if (list != null)
//            {
//            }
//        }

//        public static void Test_Find()
//        {
//            Console.WriteLine("TestMethod1");
//            var item = SPUtility.CreateNewDiscussion(root_discussionBoard, "Test Discussion");
//            item.Update();

//            string res = FAIL;

//            try
//            {
//                Application.Current.GetMessageForItem(root_discussionBoard, item.ID, SPEventType.Add, XML(item), user1.LoginName, user2.Email, user1.ID);
//            }
//            catch (SeTemplateNotFound)
//            {
//                res = SUCCESS;
//            }
//            finally
//            {
//                item.Delete();
//            }
//            Console.WriteLine(res);
//        }

//        private static string XML(SPListItem item)
//        {
            
//            XElement fields=new XElement("Fields");
//            foreach(SPField field in item.Fields)
//            {
//                var fieldEl=new XElement("Field");
//                if (field.Id == SPBuiltInFieldId.ContentTypeId)
//                {
//                    fieldEl.SetAttributeValue("Old", item.ContentTypeId.ToString());
//                }
//                else
//                {
//                    fieldEl.SetAttributeValue("Old", item[field.Id]);
//                }
//                fieldEl.SetAttributeValue("Name",field.InternalName);
//                fieldEl.SetAttributeValue("DisplayName",field.Title);
//                fieldEl.SetAttributeValue("Type",field.Type.ToString());
//                fieldEl.SetAttributeValue("Hidden",field.Hidden);
                

//                fields.Add(fieldEl);
//            }
//            return fields.ToString();
//        }

////        <Fields>
//// <Field Name="ContentTypeId" Type="string" Hidden="true" DisplayName="Content Type ID" New="0x010100ACE5C8B4725EF143A968E58355698311" />
//// <Field Name="Author" Type="user" DisplayName="Created By" New="1" LookupNewF="TEST\administrator" />
//// <Field Name="Editor" Type="user" DisplayName="Modified By" New="1" LookupNewF="TEST\administrator" />
//// <Field Name="Modified_x0020_By" Type="string" Hidden="true" DisplayName="Document Modified By" New="TEST\administrator" />
//// <Field Name="Created_x0020_By" Type="string" Hidden="true" DisplayName="Document Created By" New="TEST\administrator" />
//// <Field Name="File_x0020_Type" Type="string" Hidden="true" DisplayName="File Type" New="txt" />
//// <Field Name="_Level" Type="null" Hidden="true" DisplayName="Level" New="1" />
//// <Field Name="FileLeafRef" Type="string" Hidden="true" Old="TestDoc7.txt" />
//// <Field Name="FileRef" Type="string" Hidden="true" Old="Shared Documents/TestDoc7.txt" />
//// <Field Name="FileDirRef" Type="string" Hidden="true" Old="Shared Documents" />
//// <Field Name="_DraftVisibility" Type="integer" Hidden="true" New="0" />
////</Fields>
        static void Main(string[] args)
        {


     //       ClassStart();

     //       Test_NotTemplateException();

     ////       Test_DefaultTemplates();

     //       ClassCleanUp();

            //Application.Current.Logger.WriteTrace("1" + Environment. + "2", SharePointEmails.Logging.SeverityEnum.CriticalError);
        }
    }
}
