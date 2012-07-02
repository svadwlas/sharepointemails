using SharepointEmails.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

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
            Assert.IsTrue(Resources.testbody.IsXslt());
        }
    }
}
