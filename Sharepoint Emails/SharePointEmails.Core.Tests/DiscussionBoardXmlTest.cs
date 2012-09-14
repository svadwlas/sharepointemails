using SharePointEmails.Core.Substitutions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.SharePoint;
using System.Xml.Linq;
using Microsoft.SharePoint.Moles;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.IO;
namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for DiscussionBoardXmlTest and is intended
    ///to contain all DiscussionBoardXmlTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DiscussionBoardXmlTest
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

        void Validate(string xml, string xsd, string namespac)
        {
            var doc = XDocument.Parse(xml);
            Assert.AreEqual(namespac,doc.Root.GetDefaultNamespace().NamespaceName);
            var shcemas = new XmlSchemaSet();
            shcemas.Add(namespac, XmlReader.Create(new StringReader(xsd)));
            doc.Validate(shcemas, (s, e) => { throw e.Exception; });
        }

        const string namespa = "urn:sharepointemail-discussionboard";

        /// <summary>
        ///A test for GetElement
        ///</summary>
        [TestMethod()]
        [HostType("Moles")]
        public void GetElementTest()
        {

            Dictionary<Guid, object> fields = new Dictionary<Guid, object>();
            fields[SPBuiltInFieldId.Body] = "&lt;div class=\"ExternalClassDECDB3D2480C445D8F958DFE7B787791\"&gt;&lt;p&gt;Discussion body​&lt;/p&gt;&lt;/div&gt;";
            MSPListItem listItem = new MSPListItem();
            listItem.BehaveAsDefaultValue();
            var uniqueId = Guid.NewGuid();
            listItem.UniqueIdGet = () => uniqueId;
            listItem.ContentTypeIdGet = () => SPBuiltInContentTypeId.Discussion;
            listItem.ItemGetGuid = (g) => fields[g];
            var fieldCol=new MSPFieldCollection();
            fieldCol.ContainsGuid = (g) => fields.ContainsKey(g);
            listItem.FieldsGet = () => fieldCol;
            var actual = DiscussionBoardXml.Create().GetElement(listItem);
            Validate(actual.ToString(), SharepointEmails.Properties.Resources.DiscussionBoardSchema, namespa);

            var clearText = actual.Element(XName.Get("Discussion", namespa)).Element(XName.Get("Body", namespa)).Element(XName.Get("ClearValue", namespa)).Value;
            Assert.IsTrue(clearText.StartsWith("Discussion body"));
        }
    }
}
