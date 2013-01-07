using SharePointEmails.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.SharePoint;
using SharePointEmails.Logging;
using SharePointEmails.Core.Configuration;
using Moq;
using System.Collections.Generic;
using SharePointEmails.Core.Associations;
using SharePointEmails.Core.Exceptions;
using SharePointEmails.Core.Interfaces;
using SPMocksBuilder;

namespace SharePointEmails.Core.Tests
{
    /// <summary>
    ///The simplest test for GetTemplate. Exist only one template with one ass and in the root web
    ///</summary>
    [TestClass()]
    public class DefaultTemplatesManagerSimpleTest
    {
        List<ITemplate> TemplateList;
        Mock<IConfigurationManager> configManager;
        Mock<ILogger> logger;

        SPList CreateSourceListAllOnTheRootSite(string itemCTId, int itenId)
        {
            var vSite = new VSite
            {
                RootWeb = new VWeb
                {
                    Lists = new[]
                    {                        
                        new VList()
                        {
                            Title=Constants.TemplateListName,
                            ContentTypes=new []
                            {
                                new VContentType(new SPContentTypeId(itemCTId))
                            },
                            Items=new []
                            {
                                new VListItem(){}
                            }
                        }
                    }
                }
            };
            return vSite.Site.RootWeb.Lists[0];
        }

        ITemplate CreateTemplate(int eventTypes, TemplateStateEnum templateState, AssociationCollection asses)
        {
            var res = new Mock<ITemplate>() { DefaultValue = DefaultValue.Mock };
            res.Setup(p => p.Name).Returns(Guid.NewGuid().ToString());
            res.Setup(p => p.Id).Returns(Guid.NewGuid());
            res.Setup(p => p.EventTypes).Returns(eventTypes);
            res.Setup(p => p.State).Returns(templateState);
            res.Setup(p => p.Associations).Returns(asses);
            return res.Object;
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TemplateList = new List<ITemplate>();
            TemplatesList.mock = TemplateList;
            logger = new Mock<ILogger>();
            configManager = new Mock<IConfigurationManager>();
            configManager.Setup(p => p.GetConfig(It.IsAny<SPWeb>())).Returns(new WebConfiguration()
          {
              Disabled = false,
              DisableIncludeChilds = false
          });
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TemplatesList.mock = null;
        }

        #region ByCTId

        private void Test_WithOneAssAndOneTemplateTypeAss(int TemplateEvents, TemplateStateEnum templateState, Association ass, string ItemCTId, SPEventType eventType, bool shouldFound)
        {
            var moleSourceList=CreateSourceListAllOnTheRootSite(ItemCTId, 1);
            var expected = CreateTemplate(TemplateEvents, templateState,
                                                new AssociationCollection
                                                {
                                                   ass
                                                });
            TemplateList.Add(expected);
            DefaultTemplatesManager target = new DefaultTemplatesManager(logger.Object, configManager.Object);
            ISearchContext context = SearchContext.Create(moleSourceList, 1, Properties.Resources.EventDataTskAdded, eventType,"a@a.com");
            if (shouldFound)
            {
                var actual = target.GetTemplate(context);
                Assert.AreSame(expected, actual);
            }
            else
            {
                try
                {
                    var actual = target.GetTemplate(context);
                    Assert.Fail("shouldn't found any templates");
                }
                catch (SeTemplateNotFound)
                {
                }
            }
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId1()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.AllItemEvents,
                TemplateStateEnum.Published,
                new ContentTypeAssociation { ContentTypeID = "0x0100429BDB95B7C9334BBABA404B60E119C3", IncludingChilds = false },
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Add,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId2()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemAdded,
                TemplateStateEnum.Published,
                 new ContentTypeAssociation { ContentTypeID = "0x0100429BDB95B7C9334BBABA404B60E119C3", IncludingChilds = false },
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Add,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId3()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemRemoved,
                TemplateStateEnum.Published,
                 new ContentTypeAssociation { ContentTypeID = "0x0100429BDB95B7C9334BBABA404B60E119C3", IncludingChilds = false },
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Add,
                false);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId4()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
             (int)TemplateTypeEnum.ItemAdded,
             TemplateStateEnum.Published,
             new ContentTypeAssociation { ContentTypeID = "0x0100429BDB95B7C9334BBABA404B60E119C3", IncludingChilds = false },
             "0x01",
             SPEventType.Add,
             false);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId5()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
           (int)TemplateTypeEnum.ItemAdded,
           TemplateStateEnum.Published,
            new ContentTypeAssociation { ContentTypeID = "0x0100429BDB95B7C9334BBABA404B60E119C3", IncludingChilds = true },
           "0x01",
           SPEventType.Add,
           true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId6()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemRemoved | (int)TemplateTypeEnum.ItemAdded,
                TemplateStateEnum.Published,
                new ContentTypeAssociation { ContentTypeID = "0x0100429BDB95B7C9334BBABA404B60E119C3", IncludingChilds = false },
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Add,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId7()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemRemoved | (int)TemplateTypeEnum.ItemAdded,
                TemplateStateEnum.Draft,
                new ContentTypeAssociation { ContentTypeID = "0x0100429BDB95B7C9334BBABA404B60E119C3", IncludingChilds = false },
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Add,
                false);
        }

        #endregion

        #region ByGroup

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup1()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemRemoved,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllList },
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Delete,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup2()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemUpdated,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllDocumentLibrary },
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Modify,
                false);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup3()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemUpdated,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllDocumentLibrary },
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Modify,
                false);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup4()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemUpdated,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllTasks },
                "0x0108429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Modify,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup5()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemUpdated,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllDiscusions },
                "0x0120029BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Modify,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup6()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemUpdated,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllMessages },
                "0x0107029BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Modify,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup7()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemUpdated,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllBlogPosts },
                "0x0110029BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Modify,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup8()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemUpdated,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllBlogComments },
                "0x0111029BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Modify,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByGroup9()
        {
            Test_WithOneAssAndOneTemplateTypeAss(
                (int)TemplateTypeEnum.ItemUpdated,
                TemplateStateEnum.Published,
                new GroupAssociation { ItemType = GroupType.AllDocumentLibrary },
                "0x0101029BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Modify,
                true);
        }

        #endregion
    }
}
