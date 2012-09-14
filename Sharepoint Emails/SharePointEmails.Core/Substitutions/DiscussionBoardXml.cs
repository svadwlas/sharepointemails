using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Text.RegularExpressions;

namespace SharePointEmails.Core.Substitutions
{
    public class DiscussionBoardXml
    {
        const string NameSpace = "urn:sharepointemail-discussionboard";

        public bool UseParse=true;
        public static DiscussionBoardXml Create()
        {
            return new DiscussionBoardXml();
        }
        private DiscussionBoardXml() { }

        public XElement GetElement(SPListItem listItem)
        {
            if (listItem != null)
            {
                if (listItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Message) || listItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Discussion))
                {
                    return GetElementUseList(listItem);
                }
            }
            return null;
        }

        private XElement GetElementUseParse(SPListItem listItem)
        {
            XNamespace nsp = XNamespace.Get(NameSpace);
            var res = new XElement(nsp+"DiscussionBoard");
            var discussion = new XElement(nsp+"Discussion");
            var body = new XElement(nsp + "Body");
            var value=new XElement(nsp+"Value");
            var clearValue = new XElement(nsp + "ClearValue");
            value.Value = GetFieldValue<string>(listItem, SPBuiltInFieldId.Body,string.Empty);
            clearValue.Value = GetFieldValue<string>(listItem, SPBuiltInFieldId.Body, string.Empty);

            body.Add(value, clearValue);
            discussion.Add(body);
            res.Add(discussion);
            return res;
        }

        T GetFieldValue<T>(SPListItem item, Guid fieldGuid, T def=default(T))
        {
            return item.Fields.Contains(fieldGuid) ? ((T)item[fieldGuid]) : def;
        }

        private XElement GetElementUseList(SPListItem listItem)
        {
            XNamespace nsp = XNamespace.Get(NameSpace);
            var element = new XElement(nsp+"DiscussionBoard");
                List<SPListItem> chain = new List<SPListItem>();
                if (listItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Message))
                {
                    chain.AddRange(GetDescedantsForMessage(listItem));
                }
                else
                {
                    chain.Add(listItem);
                }

                XElement parent = null;
                XElement curent = null;
                foreach (SPListItem item in chain)
                {
                    if (item.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Discussion))
                    {
                        curent = new XElement(nsp + "Discussion");
                        element.Add(curent);
                        var subjElement = new XElement(nsp + "Subject");
                        var bodyElement = new XElement(nsp + "Body");

                        var subjText = new XElement(nsp + "Value")
                        {
                            Value =  item.GetFieldValue<string>(SPBuiltInFieldId.DiscussionTitle, string.Empty)
                        };

                        var clearSubjText = new XElement(nsp + "ClearValue") { Value = GetClearDiscussionSubjText(subjText.Value) };

                        var bodyText = new XElement(nsp + "Value")
                        {
                            Value = item.GetFieldValue<string>(SPBuiltInFieldId.Body, string.Empty)
                        };

                        var clearBodyText = new XElement(nsp + "ClearValue") { Value = GetClearDiscussionBodyText(item.GetFieldValue<string>(SPBuiltInFieldId.TrimmedBody, string.Empty)) };

                        subjElement.Add(subjText, clearSubjText);
                        bodyElement.Add(bodyText, clearBodyText);
                        curent.Add(subjElement, bodyElement);
                    }
                    else
                    {
                        curent = new XElement(nsp + "Message");
                        var bodyElement = new XElement(nsp + "Body");
                        var bodyText = new XElement(nsp + "Value")
                        {
                            Value = item.GetFieldValue<string>(SPBuiltInFieldId.Body, string.Empty)
                        };

                        var clearBodyText = new XElement(nsp + "ClearValue") { Value = GetClearMessageBodyText(item.GetFieldValue<string>(SPBuiltInFieldId.TrimmedBody, string.Empty)) };

                        bodyElement.Add(bodyText, clearBodyText);
                        curent.Add(bodyElement);
                    }
                    if (item.UniqueId == listItem.UniqueId)
                    {
                        curent.SetAttributeValue("Current", true);
                    }
                    if (item.ParentList != null && item.ParentList.ParentWeb != null)
                    {
                        var createdBy = new SPFieldUserValue(item.ParentList.ParentWeb, item[SPBuiltInFieldId.Author].ToString());
                        curent.SetAttributeValue("User", createdBy.User.LoginName);
                    }
                    if (parent != null)
                    {
                        parent.Add(curent);
                    }
                    parent = curent;
                }

                return element;
           
        }

        string RemoveTags(string text)
        {
            var res = Regex.Replace(text, @"&lt;(.|\n)*?&gt;|<(.|\n)*?>", string.Empty);
            return res;
        }

        private string GetClearMessageBodyText(string p)
        {
            return RemoveTags(p);
        }

        private string GetClearDiscussionSubjText(string p)
        {
            return RemoveTags(p);
        }

        private string GetClearDiscussionBodyText(string p)
        {
            return RemoveTags(p);
        }

        private List<SPListItem> GetDescedantsForMessage(SPListItem message)
        {
            var res = new List<SPListItem>();
            var threadIndex=message.GetFieldValue<string>(SPBuiltInFieldId.ThreadIndex,"");
            var threadTopic=message.GetFieldValue<string>(SPBuiltInFieldId.ThreadTopic);
            var threading=message.GetFieldValue<string>(SPBuiltInFieldId.Threading);
            var d = new Dictionary<int, SPListItem>();
            foreach (SPListItem item in message.ParentList.Items)
            {
                if (item.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Discussion)) continue;
                var index = item.GetFieldValue<string>(SPBuiltInFieldId.ThreadIndex);
                var topic = item.GetFieldValue<string>(SPBuiltInFieldId.ThreadTopic);
                var thr = item.GetFieldValue<string>(SPBuiltInFieldId.Threading);
                if (!string.IsNullOrEmpty(threadIndex) && threadIndex.StartsWith(index))
                {
                    d.Add(index.Length, item);
                }
            }
            res = d.OrderBy(p => p.Key).Select(p=>p.Value).ToList();
            if (message.Fields.Contains(SPBuiltInFieldId.ParentFolderId))
            {
                var parentId = (int)message[SPBuiltInFieldId.ParentFolderId];
                var parent = message.ParentList.GetItemById(parentId);
                res.Insert(0, parent);
                //res.Add(message);
            }
            return res;
        }
    }
}
