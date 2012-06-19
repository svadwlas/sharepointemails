using SharePointEmails.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for SubstitutionContextTest and is intended
    ///to contain all SubstitutionContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubstitutionContextTest
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
        ///A test for SubstitutionContext Constructor
        ///</summary>
        [TestMethod()]
        public void SubstitutionContextConstructorTest()
        {
            string eventData = Properties.Resources.EventDataFileAdded;
            SubstitutionContext target = new SubstitutionContext(eventData);
        }

        /// <summary>
        ///A test for GetField
        ///</summary>
        [TestMethod()]
        public void GetFieldTest()
        {
            string eventData = Properties.Resources.EventDataFileAdded;
            SubstitutionContext target = new SubstitutionContext(eventData);
            Assert.AreEqual(@"TEST\administrator",target.GetField("Document Created By", ":N")) ;
            Assert.AreEqual(null,target.GetField("Document Created By", ":O"));
            Assert.AreEqual(null,target.GetField("Author", ":O"));
            Assert.AreEqual(@"TEST\administrator",target.GetField("Author", ":N"));
            Assert.AreEqual(@"TEST\administrator",target.GetField("Editor", ":N"));
            Assert.AreEqual(null,target.GetField("Editor", ":O"));
        }
    }
}
