using SharePointEmails.Core.Substitutions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SharePointEmails.Core;
using Moq;
using System.Globalization;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for ResourceSubstitutionTest and is intended
    ///to contain all ResourceSubstitutionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResourceSubstitutionTest
    {
        [ClassInitialize()]
        public static void MyTestInitialize(TestContext testContext)
        {
            ClassContainer.mockLogger = new Mock<ILogger>().Object;
        }

        [TestMethod(),Ignore]
        public void ProcessTest()
        {
            ResourceSubstitution target = new ResourceSubstitution();
            var c = new Mock<ISubstitutionContext>();
            c.Setup(p => p.getDestinationCulture()).Returns(CultureInfo.CurrentCulture);
            var actual = target.Process("aa {$Resources:core, listedit_fieldsdescription_part1}", c.Object);
            Assert.AreEqual("", actual);
        }
    }
}
