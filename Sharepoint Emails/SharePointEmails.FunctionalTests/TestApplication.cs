using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint;
using SharePointEmails.Core;
using System.IO;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using System.Diagnostics;

namespace SharePointEmails.FunctionalTests
{
    [TestClass]
    public class TestApplication
    {
        static readonly Uri TestSiteCollection = new Uri("http://dev/sites/TestSiteCollection/");
        static readonly string User1Login = "user1";
        static readonly string User2Login = "user2";
        static readonly string User3Login = "user3";
        static readonly string AdministratorLogin = "administrator";

        static SPUser user1;
        static SPUser user2;
        static SPUser user3;
        static SPSite testSite = null;
        static SPWeb rootWeb = null;
        static SPList root_discussionBoard = null;
        static SPListItem root_discussionBoard_discussionItem;
        static SPListItem root_discussionBoard_discussionItem_message1;
        [ClassInitialize]
        public static void ClassStart(TestContext testContext)
        {
            if (!SPSite.Exists(TestSiteCollection))
            {
                Install();
            }
            testSite = new SPSite(TestSiteCollection.ToString());
            rootWeb = testSite.RootWeb;
            root_discussionBoard = rootWeb.Lists["Team Discussion"];
            user1 = rootWeb.SiteUsers[User1Login];
            user1 = rootWeb.SiteUsers[User2Login];
            user1 = rootWeb.SiteUsers[User3Login];
        }

        private static void Install()
        {
            var rootUri = new Uri(TestSiteCollection.Scheme + "://" + TestSiteCollection.Host);
            var root = new SPSite(rootUri.ToString());
            var webApp = root.WebApplication;
            webApp.Sites.Add(TestSiteCollection.ToString(), "Site for tests", "", (uint)CultureInfo.CurrentCulture.LCID, "Team Site", AdministratorLogin, AdministratorLogin, "");
        }
      
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            if (testSite != null)
            {
                testSite.Dispose();
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
            Application.Current.GetMessageForItem(root_discussionBoard, root_discussionBoard_discussionItem.ID, SPEventType.Add, "", user1.LoginName, user2.Email, user1.ID);
        }
    }
}
