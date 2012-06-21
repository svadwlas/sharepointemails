using SharePointEmails.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.SharePoint;
using SharePointEmails.Logging;
using Microsoft.SharePoint.Moles;
using SharePointEmails.Core.Configuration.Moles;
using SharePointEmails.Core.Configuration;
using Moq;
using System.Collections.Generic;
using SharePointEmails.Core.Associations;
using SharePointEmails.Core.Exceptions;

namespace SharePointEmails.Core.Tests
{


    /// <summary>
    ///This is a test class for DefaultTemplatesManagerTest and is intended
    ///to contain all DefaultTemplatesManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DefaultTemplatesManagerTest
    {
        List<ITemplate> TemplateList;
        Mock<IConfigurationManager> configManager;
        Mock<ILogger> logger;

        MSPList moleSourceList;

        void CreateSourceListAllOnTheRootSite(string itemCTId, int itenId)
        {
            MSPWeb web = new MSPWeb();
            MSPSite site = new MSPSite();

            web.IDGet = () => new Guid("{C04453C1-848A-4434-AD72-DF09FF6E62ED}");
            web.SiteGet = () => site;
            web.ParentWebGet = () => null;

            site.IDGet = () => new Guid("{591F1D25-295F-47B9-AC62-D2414C210B75}");
            site.RootWebGet = () => web;

            MSPListItem item = new MSPListItem();
            item.ContentTypeIdGet = () => new SPContentTypeId(itemCTId);
            moleSourceList = new MSPList();
            moleSourceList.GetItemByIdInt32 = (id) =>
            {
                if (id == itenId) return item;
                else return new MSPListItem();
            };

            moleSourceList.ParentWebGet = () => web;

            var lists = new MSPListCollection();

            var hiddentList = new MSPList();
            lists.TryGetListString = (s) =>
            {
                if (s == Constants.TemplateListName) return hiddentList;
                else return new MSPList();
            };

            web.ListsGet = () => lists;

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

        public ITemplate CreateTemplate(int eventTypes, TemplateStateEnum templateState, AssociationConfiguration asses)
        {
            var res = new Mock<ITemplate>() { DefaultValue = DefaultValue.Mock };
            res.Setup(p => p.Name).Returns(Guid.NewGuid().ToString());
            res.Setup(p => p.IsValid).Returns(true);
            res.Setup(p => p.Id).Returns(Guid.NewGuid());
            res.Setup(p => p.EventTypes).Returns(eventTypes);
            res.Setup(p => p.State).Returns(templateState);
            res.Setup(p => p.Asses).Returns(asses);
            return res.Object;
        }

        private void Test_ByContentTypeAss(int TemplateEvents, TemplateStateEnum templateState, string AssCTId, bool IncludeChild, string ItemCTId, SPEventType eventType, bool shouldFound)
        {
            CreateSourceListAllOnTheRootSite(ItemCTId, 1);
            var expected = CreateTemplate(TemplateEvents, templateState,
                                                new AssociationConfiguration
                                                {
                                                    new ContentTypeAssociation()
                                                    {
                                                        ContentTypeID=AssCTId,
                                                        IncludingChilds=IncludeChild
                                                    }
                                                });
            TemplateList.Add(expected);
            DefaultTemplatesManager target = new DefaultTemplatesManager(logger.Object, configManager.Object);
            ISearchContext context = SearchContext.Create(moleSourceList, 1, Properties.Resources.EventDataTskAdded, SPEventType.Add);
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
            Test_ByContentTypeAss(
                (int)TemplateTypeEnum.AllItemEvents,
                TemplateStateEnum.Published,
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                false,
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Add,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId2()
        {
            Test_ByContentTypeAss(
                (int)TemplateTypeEnum.ItemAdded,
                TemplateStateEnum.Published,
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                false,
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Add,
                true);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId3()
        {
            Test_ByContentTypeAss(
                (int)TemplateTypeEnum.ItemRemoved,
                TemplateStateEnum.Published,
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                false,
                "0x0100429BDB95B7C9334BBABA404B60E119C3",
                SPEventType.Add,
                false);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId4()
        {
            Test_ByContentTypeAss(
             (int)TemplateTypeEnum.ItemAdded,
             TemplateStateEnum.Published,
             "0x0100429BDB95B7C9334BBABA404B60E119C3",
             false,
             "0x01",
             SPEventType.Add,
             false);
        }

        [TestMethod()]
        [HostType("Moles")]
        public void GetTemplate_AssByCTId5()
        {
            Test_ByContentTypeAss(
           (int)TemplateTypeEnum.ItemAdded,
           TemplateStateEnum.Published,
           "0x0100429BDB95B7C9334BBABA404B60E119C3",
           true,
           "0x01",
           SPEventType.Add,
           true);
        }
    }
}
