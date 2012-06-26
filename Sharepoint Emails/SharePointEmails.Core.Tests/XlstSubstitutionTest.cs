using SharePointEmails.Core.Substitutions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SharePointEmails.Core;

namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for XlstSubstitutionTest and is intended
    ///to contain all XlstSubstitutionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XlstSubstitutionTest
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
        ///A test for Process
        ///</summary>
            [TestMethod]
        public void Process()
        {
            var xlst = @"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>
                        <xsl:template match='Data'>
                            <HTML>
                            <BODY>
                                <TABLE BORDER='2'>
                                <TR>
                                    <TD>Field</TD>
                                    <TD>Old</TD>
                                    <TD>New</TD>
                                </TR>
                                <xsl:apply-templates select='EventData'/>
                                </TABLE>
                            </BODY>
                            </HTML>
                        </xsl:template>
                        <xsl:template match='EventData'>
                            <xsl:apply-templates select='Field'/>
                        </xsl:template>
                        <xsl:template match='Field'>
                            <tr><td><xsl:value-of select='@DisplayName'/>(<xsl:value-of select='@Name'/>)</td><td><xsl:value-of select='@Old'/></td><td>New:<xsl:value-of select='@New'/></td></tr>
                        </xsl:template>
                        </xsl:stylesheet>";
            var context = new SubstitutionContext(Properties.Resources.EventDataFileAdded);
            var xml = context.GetXML();
            var subs = new XlstSubstitution();
            var res = subs.Process(xlst, context);
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Contains("TABLE"));
        }

            [TestMethod]
            public void Test()
            {
                var xlst = @"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>
                        <xsl:template match='Data'>
                            <HTML>
                            <BODY>
                                <TABLE BORDER='2'>
                                <TR>
                                    <TD>Field</TD>
                                    <TD>Old</TD>
                                    <TD>New</TD>
                                </TR>
                                <xsl:apply-templates select='EventData'/>
                                </TABLE>
                            </BODY>
                            </HTML>
                        </xsl:template>
                        <xsl:template match='EventData'>
                            <xsl:apply-templates select='Field'/>
                        </xsl:template>
                        <xsl:template match='Field'>
                            <tr><td><xsl:value-of select='@DisplayName'/>(<xsl:value-of select='@Name'/>)</td><td><xsl:value-of select='@Old'/></td><td>New:<xsl:value-of select='@New'/></td></tr>
                        </xsl:template>
                        </xsl:stylesheet>";
                var context = new SubstitutionContext(Properties.Resources.EventDataFileAdded);
                var xml = context.GetXML();
                var subs = new XlstSubstitution();
                var res = subs.Process(xlst, context);
                Assert.IsNotNull(res);
                Assert.IsTrue(res.Contains("TABLE"));
            }
    }
}
