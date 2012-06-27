using SharePointEmails.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SharePointEmails.Core.Substitutions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Moles;

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
            Assert.AreEqual(@"TEST\administrator", target.GetField("Document Created By", new ModifiersCollection { Modifier.New }));
            Assert.AreEqual(null, target.GetField("Document Created By", new ModifiersCollection { Modifier.Old }));
            Assert.AreEqual(null, target.GetField("Author", new ModifiersCollection { Modifier.Old }));
            Assert.AreEqual(@"TEST\administrator", target.GetField("Author", new ModifiersCollection { Modifier.New }));
            Assert.AreEqual(@"TEST\administrator", target.GetField("Editor", new ModifiersCollection { Modifier.New }));
            Assert.AreEqual(null, target.GetField("Editor", new ModifiersCollection { Modifier.Old }));
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetContextValue()
        {
            MSPList sList = new MSPList();
            MSPSite sSite = new MSPSite();
            MSPWeb sWeb=new MSPWeb();

            sList.ParentWebGet = () => sWeb;
            sWeb.SiteGet = () => sSite;

            sWeb.TitleGet = () => "webtitle";
            sSite.UrlGet = () => "siteurl";
            sList.DescriptionGet = () => "descriptiontext";
            sList.TitleGet=()=>"TitleText";

            SubstitutionContext target = new SubstitutionContext(Properties.Resources.EventDataFileAdded, sList);
            var xlm = SubstitutionContext.GetTestXML();
            Assert.AreEqual("TitleText",target.GetContextValue("SList.Title",new ModifiersCollection()));
            Assert.AreEqual("descriptiontext", target.GetContextValue("SList.Description",new ModifiersCollection()));
            Assert.AreEqual("siteurl",target.GetContextValue("SSite.Url",new ModifiersCollection()));
            Assert.AreEqual("webtitle", target.GetContextValue("SWeb.Title", new ModifiersCollection()));
        }

      
    }
}
