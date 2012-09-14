using SharepointEmails.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Moles;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for ResourcesTest and is intended
    ///to contain all ResourcesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResourcesTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for TestContextXML
        ///</summary>
        [TestMethod()]
        public void ContextXMLTest_IsValidXML()
        {
            XDocument.Parse(Resources.TestContextXML);
        }

        /// <summary>
        ///A test for subjXslt
        ///</summary>
        [TestMethod()]
        public void subjXsltTest()
        {
            Assert.IsTrue(Resources.subjXslt.IsXslt());
        }

        /// <summary>
        ///A test for testbody
        ///</summary>
        [TestMethod()]
        public void testbodyTest()
        {
            Assert.IsTrue(Resources.BodyTemplate.IsXslt());
            Assert.IsTrue(Resources.BodyTemplateForDiscussionBoard.IsXslt());
            Assert.IsTrue(Resources.ListAddressTemplate.IsXslt());
            Assert.IsTrue(Resources.AdminAddressTemplate.IsXslt());
        }

        SPDocumentLibrary GetLibrary()
        {
            var doc = new MSPDocumentLibrary();
            var list=new MSPList(doc);
            var files = new List<SPListItem>();

            foreach (var file in new Dictionary<string, string> { { "Utils.xslt", Resources.Utils } })
            {
                var item = new MSPListItem();
                var f = new MSPFile();
                f.NameGet = () => file.Key;
                item.FileGet = () => f;
                f.OpenBinaryStream = () => new MemoryStream(Encoding.Default.GetBytes(file.Value));
                files.Add(item);
            }

            var items = new MSPListItemCollection();
            items.GetEnumerator = () => files.GetEnumerator();
            list.ItemsGet = () => items;
            return (SPDocumentLibrary)list;
        }

        /// <summary>
        ///A test for testbody
        ///</summary>
        [TestMethod()]
        [HostType("Moles")]
        public void testBodyTemplateForDiscussion()
        {
            var s = Properties.Resources.TestReplyOnReply.ApplyXslt(Resources.BodyTemplateForDiscussionBoard, GetLibrary());
            Assert.IsTrue(s.Contains("Disc subj"));
           // Assert.IsTrue(s.Contains("Disc body"));
            Assert.IsTrue(s.Contains("first message text"));
            Assert.IsTrue(s.Contains("second message text"));
            Assert.IsTrue(s.Contains("third message text"));
        }
    }
}
