using SharePointEmails.Core.Substitutions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.SharePoint;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using SPMocksBuilder;
using System.Diagnostics;
namespace SharePointEmails.Core.Tests
{


    /// <summary>
    ///This is a test class for DiscussionBoardXmlTest and is intended
    ///to contain all DiscussionBoardXmlTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DiscussionBoardXmlTest
    {

        void Validate(string xml, string xsd, string namespac)
        {
            var doc = XDocument.Parse(xml);
            Assert.AreEqual(namespac, doc.Root.GetDefaultNamespace().NamespaceName);
            var shcemas = new XmlSchemaSet();
            shcemas.Add(namespac, XmlReader.Create(new StringReader(xsd)));
            Debug.Write(xml);
            Debug.Write(xsd);
            doc.Validate(shcemas, (s, e) => { throw e.Exception; });
        }

        const string namespa = "urn:sharepointemail-discussionboard";

        private SPSite GetSiteWithDiscussionBoard(ICollection<VListItem> items)
        {
            var vSite = new VSite()
            {
                RootWeb = new VWeb
                {
                    Lists = new[]
                         {
                             new VList()
                             {
                                 ContentTypes=new []
                                 {
                                       VContentType.CreateForDiscussion(),
                                       VContentType.GetForDiscussionMessage(),
                                 },
                                 Items= items
                             }
                         }
                },
                Users = new[]
                {
                    new VUser("User1","User1@com.com","User1Name"),
                    new VUser("User2","User2@com.com","User2Name"),
                    new VUser("User3","User3@com.com","User3Name")
                }
            };

            return vSite.Site;
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetElementFromDiscussionItem()
        {
            var site = GetSiteWithDiscussionBoard(new[]
                                 {
                                     new VListItem()
                                     {
                                         ContentTypeName="Discussion",
                                         Values=new Dictionary<Guid,object>
                                         {
                                             {SPBuiltInFieldId.Body,"&lt;div class=\"ExternalClassDECDB3D2480C445D8F958DFE7B787791\"&gt;&lt;p&gt;Discussion body​&lt;/p&gt;&lt;/div&gt;"},
                                             {SPBuiltInFieldId.Author,"1;#User1"},
                                             {SPBuiltInFieldId.Created,DateTime.Now}
                                         }
                                     }
                                 });

            var actual = DiscussionBoardXml.Create().GetElement(site.RootWeb.Lists[0].Items[0]);
            Validate(actual.ToString(), SharePointEmails.Properties.Resources.DiscussionBoardSchema, namespa);

            var clearText = actual.Element(XName.Get("Discussion", namespa)).Element(XName.Get("Body", namespa)).Element(XName.Get("ClearValue", namespa)).Value;
            Assert.IsTrue(clearText.StartsWith("Discussion body"));
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetElementFromMessageItem()
        {
            var site = GetSiteWithDiscussionBoard(new[]
                                 {
                                     new VListItem()
                                     {
                                         ContentTypeId=SPBuiltInContentTypeId.Discussion,
                                         Values=new Dictionary<Guid,object>
                                         {
                                             {SPBuiltInFieldId.Body,"&lt;div class=\"ExternalClassDECDB3D2480C445D8F958DFE7B787791\"&gt;&lt;p&gt;Discussion body​ from User1&lt;/p&gt;&lt;/div&gt;"},
                                             {SPBuiltInFieldId.Author,"1;#User1"},
                                             {SPBuiltInFieldId.Created,DateTime.Now},
                                             {SPBuiltInFieldId.ThreadIndex,"1"},
                                         },
                                         Folder=new VFolder()
                                     },

                                       new VListItem()
                                     {
                                          ContentTypeId=SPBuiltInContentTypeId.Message,
                                         Values=new Dictionary<Guid,object>
                                         {
                                             {SPBuiltInFieldId.Body,"&lt;div class=\"ExternalClassDECDB3D2480C445D8F958DFE7B787791\"&gt;&lt;p&gt;reply from user2&lt;/p&gt;&lt;/div&gt;"},
                                             {SPBuiltInFieldId.Author,"2;#User2"},
                                             {SPBuiltInFieldId.Created,DateTime.Now},
                                             {SPBuiltInFieldId.ThreadIndex,"123"},
                                             {SPBuiltInFieldId.ParentFolderId,1}
                                         }
                                     },
                                       new VListItem()
                                     {
                                          ContentTypeId=SPBuiltInContentTypeId.Message,
                                         Values=new Dictionary<Guid,object>
                                         {
                                             {SPBuiltInFieldId.Body,"&lt;div class=\"ExternalClassDECDB3D2480C445D8F958DFE7B787791\"&gt;&lt;p&gt;reply from user3​&lt;/p&gt;&lt;/div&gt;"},
                                             {SPBuiltInFieldId.Author,"3;#User3"},
                                             {SPBuiltInFieldId.Created,DateTime.Now},
                                             {SPBuiltInFieldId.ThreadIndex,"123456"},
                                             {SPBuiltInFieldId.ParentFolderId,1}
                                         }
                                     }
                                 });

            var actual = DiscussionBoardXml.Create().GetElement(site.RootWeb.Lists[0].Items[2]);
            Validate(actual.ToString(), SharePointEmails.Properties.Resources.DiscussionBoardSchema, namespa);

            var dicElement = actual.Element(XName.Get("Discussion", namespa));
            var firstMessage = actual.Element(XName.Get("Discussion", namespa)).Element(XName.Get("Message", namespa));
            var secondMessage=firstMessage.Element(XName.Get("Message", namespa));

            Assert.IsTrue(dicElement.Element(XName.Get("Body", namespa)).Element(XName.Get("ClearValue", namespa)).Value.StartsWith("Discussion body​ from User1"));
            Assert.IsTrue(firstMessage.Element(XName.Get("Body",namespa)).Element(XName.Get("ClearValue",namespa)).Value.StartsWith("reply from user2"));
            Assert.IsTrue(secondMessage.Element(XName.Get("Body",namespa)).Element(XName.Get("ClearValue",namespa)).Value.StartsWith("reply from user3"));

            Assert.IsNull(dicElement.Attribute("Current"));
            Assert.IsNull(firstMessage.Attribute("Current"));
            Assert.AreEqual(secondMessage.Attribute("Current").Value,"true");
        }
    }
}
