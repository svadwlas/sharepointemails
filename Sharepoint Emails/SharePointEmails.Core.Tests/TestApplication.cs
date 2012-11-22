using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPMocksBuilder;
using Moq;
using SharePointEmails.Logging;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Moles;
using Microsoft.SharePoint.Behaviors;

namespace SharePointEmails.Core.Tests
{
    [TestClass()]
    public class TestApplication
    {
        [TestMethod]
        [HostType("Moles")]
        public void TestCtor()
        {
            Application.Current = null;
            ClassContainer.Instance = null;
            ClassContainer.mockLogger = new Mock<ILogger>().Object;           

            var list = TestUtils.CreateWithTemplates();
            var vSite = new VSite()
            {
                RootWeb = new VWeb()
                {
                    Lists = new[] { 
                        list,
                        new VList()
                        {
                            Title="Test List 1",
                            ContentTypes=new []{
                                new VContentType(SPBuiltInContentTypeId.Item)
                                {
                                    Fields=new []{
                                        new VField("List1_Field1_Text")
                                    }
                                }
                            },
                            Items=new []
                            {
                                new VListItem()
                                {
                                    ValuesByName=new Dictionary<string,object>()
                                    {
                                        {"List1_Field1_Text","List1_Field1_Text_Value"}
                                    }
                                }
                            }
                        }
                    }
                },
                Users=new []{
                    new VUser("domain\\receiver","receiver@domain.com"),
                    new VUser("domain\\modifier","modifier@domain.com"),
                    new VUser("domain\\creator","creator@domain.com")
                }
            };

            var items = new List<VListItem>();
            var templates = new[]
                       {
                           new Template()
                           {
                               Name="Template1",
                               Subject="template1Subject",
                               Body="Template1Body",
                               From="template1From@host.com",
                               Replay="template1Reply@host.com",
                               EventTypes=(int)TemplateTypeEnum.AllItemEvents,
                           }
                       };

            foreach (var template in templates)
            {
                items.Add(new VListItem());
            }

            list.Items = items;

            for (int i = 0; i < templates.Length; i++)
            {
                templates.ToArray()[i].SaveTo(items[i].Item);
            }

            var alert = new MSPAlert();
            alert.ListIDGet = () => list.ID;
            alert.UserIdGet = () => 3;//creator of alert
            Application.Current.OnNotification(vSite.Site.RootWeb, new Microsoft.SharePoint.SPAlertHandlerParams()
                {
                    eventData = new Microsoft.SharePoint.SPAlertEventData[]
                    {
                        new SPAlertEventData()
                        {
                            itemId=1,                           
                            eventType=(int)SPEventType.Add,    
                            modifiedBy="domain\\modifier"
                        },
                    },
                    headers = new System.Collections.Specialized.StringDictionary()
                    {
                        {"to", "receiver@domain.com"}
                    },
                    a = alert
                });
        }
    }
}
