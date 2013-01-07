using SharePointEmails.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SharePointEmails.Core.Substitutions;
using Microsoft.SharePoint;
using SPMocksBuilder;

namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for SubstitutionContextTest and is intended
    ///to contain all SubstitutionContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubstitutionContextTest
    {
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
            Assert.AreEqual(@"TEST\administrator", target.GetCurrentFieldValue("Document Created By", new ModifiersCollection { Modifier.New }));
            Assert.AreEqual(null, target.GetCurrentFieldValue("Document Created By", new ModifiersCollection { Modifier.Old }));
            Assert.AreEqual(null, target.GetCurrentFieldValue("Author", new ModifiersCollection { Modifier.Old }));
            Assert.AreEqual(@"TEST\administrator", target.GetCurrentFieldValue("Author", new ModifiersCollection { Modifier.New }));
            Assert.AreEqual(@"TEST\administrator", target.GetCurrentFieldValue("Editor", new ModifiersCollection { Modifier.New }));
            Assert.AreEqual(null, target.GetCurrentFieldValue("Editor", new ModifiersCollection { Modifier.Old }));
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetContextValue()
        {
            var vsite = new VSite()
            {
                Url = "siteurl",
                RootWeb = new VWeb
                {
                    Title = "webtitle",
                    Lists = new[]
                    {
                        new VList
                        {
                            Title="TitleText",
                            Description="descriptiontext"
                        }
                    }
                }
            };

            SubstitutionContext target = new SubstitutionContext(Properties.Resources.EventDataFileAdded, vsite.Site.RootWeb.Lists[0]);
            Assert.AreEqual("TitleText",target.GetContextValue("SList.Title",new ModifiersCollection()));
            Assert.AreEqual("descriptiontext", target.GetContextValue("SList.Description",new ModifiersCollection()));
            Assert.AreEqual("siteurl",target.GetContextValue("SSite.Url",new ModifiersCollection()));
            Assert.AreEqual("webtitle", target.GetContextValue("SWeb.Title", new ModifiersCollection()));
        }

      
    }
}
