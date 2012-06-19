using SharePointEmails.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Rhino.Mocks;
namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for FieldSubstitutionTest and is intended
    ///to contain all FieldSubstitutionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FieldSubstitutionTest
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
        ///A test for FieldSubstitution Constructor
        ///</summary>
        [TestMethod()]
        public void FieldSubstitutionConstructorTest()
        {
            FieldSubstitution target = new FieldSubstitution();
        }

        /// <summary>
        ///A test for Process
        ///</summary>
        [TestMethod()]
        public void ProcessTest()
        {
            FieldSubstitution target = new FieldSubstitution();
            var context = MockRepository.GenerateMock<ISubstitutionContext>();
            context.Expect(p => p.GetField("field1")).Return("field1text");
            context.Expect(p => p.GetField("field2")).Return("field2text");
            var text = "text1 [field1] text2 [field2] text3 [field1]";
            var expected = "text1 field1text text2 field2text text3 field1text";
            var actual = target.Process(text, context);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Pattern
        ///</summary>
        [TestMethod()]
        public void PatternTest()
        {
            FieldSubstitution target = new FieldSubstitution(); // TODO: Initialize to an appropriate value
            var actual = target.Pattern;
            Assert.AreEqual("[FieldName]", actual);
        }
    }
}