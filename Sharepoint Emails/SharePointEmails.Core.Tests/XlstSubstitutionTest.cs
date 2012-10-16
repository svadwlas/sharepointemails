using SharePointEmails.Core.Substitutions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SharePointEmails.Core;
using System.Diagnostics;
using SharePointEmails.Logging;
using Moq;

namespace SharePointEmails.Core.Tests
{


    /// <summary>
    ///This is a test class for XlstSubstitutionTest and is intended
    ///to contain all XlstSubstitutionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XlstSubstitutionTest
    {
        [TestInitialize()]
        public void MyTestInitialize()
        {
            ClassContainer.Instance = null;
            Application.Current = null;
            ClassContainer.mockLogger = new Mock<ILogger>().Object;
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            ClassContainer.mockLogger = null;
        }

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
            var xlst = Properties.Resources.test;
            var context = new SubstitutionContext(Properties.Resources.EventDataFileAdded);
            var xml = context.GetXML();
            var subs = new XlstSubstitution();
            var res = subs.Process(xlst, context);
            Debug.WriteLine(res);
            Assert.IsNotNull(res);
            //  Assert.IsTrue(res.Contains("TABLE"));
        }
    }
}
