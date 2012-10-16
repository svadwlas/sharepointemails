using SharePointEmails.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using SharePointEmails.Core.Extensions;
using SPMocksBuilder;
namespace SharePointEmails.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for ResourcesTest and is intended
    ///to contain all ResourcesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResourcesTest
    {
        [TestMethod()]
        public void ContextXMLTest_IsValidXML()
        {
            XDocument.Parse(Resources.TestContextXML);
        }

        [TestMethod()]
        public void subjXsltTest()
        {
            Assert.IsTrue(Resources.subjXslt.IsXslt());
        }

        [TestMethod()]
        public void testbodyTest()
        {
            Assert.IsTrue(Resources.BodyTemplate.IsXslt());
            Assert.IsTrue(Resources.BodyTemplateForDiscussionBoard.IsXslt());
            Assert.IsTrue(Resources.ListAddressTemplate.IsXslt());
            Assert.IsTrue(Resources.AdminAddressTemplate.IsXslt());
        }

        SPDocumentLibrary GetLibrary()
        {
            var filesinLibrary = new Dictionary<string, string> { { "Utils.xslt", Resources.Utils } };

            var vList = new VList(new VDocumentLibrary())
            {
                Items = filesinLibrary.Select(p => new VListItem()
                {
                    File = new VFile(Encoding.Default.GetBytes(p.Value), p.Key)
                }).ToArray()
            };

            return (SPDocumentLibrary)vList.List;
        }

        [TestMethod()]
        [HostType("Moles")]
        public void testBodyTemplateForDiscussion()
        {
            var s = Properties.Resources.TestReplyOnReply.ApplyXslt(Resources.BodyTemplateForDiscussionBoard, GetLibrary());
            Assert.IsTrue(s.Contains("Disc subj"));
           // Assert.IsTrue(s.Contains("Disc body"));
            Assert.IsTrue(s.Contains("first message text"));
            Assert.IsTrue(s.Contains("second message text"));
            Assert.IsTrue(s.Contains("third message text"));
        }
    }
}
