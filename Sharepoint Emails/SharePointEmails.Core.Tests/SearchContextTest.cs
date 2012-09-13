using SharePointEmails.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Moles;

namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for SearchContextTest and is intended
    ///to contain all SearchContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SearchContextTest
    {
        [TestMethod]
        [HostType("Moles")]
        public void TestCtor()
        {
            var context = (SearchContext)SearchContext.Create(new MSPList(), 1, Properties.Resources.EventDataFileAdded, SPEventType.Modify,"a@a.com");
            Assert.AreEqual(new SPContentTypeId("0x010100ACE5C8B4725EF143A968E58355698311"),context.ItemContentTypeId);
            Assert.AreEqual(context.ItemId, 1);
        }
    }
}
