using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

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
                    if (!UseParse)
                    {
                        return GetElementUseList(listItem);
                    }
                    else
                    {
                        return GetElementUseParse(listItem);
                    }
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
            var element = new XElement("DiscussionBoard");
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
                        
                        curent.SetAttributeValue("xmlns", "urn:sharepointemail-discussionboard");
                        element.Add(curent);
                        var subjElement = new XElement("Subject");
                        var bodyElement = new XElement("Body");

                        var subjText = new XElement("Value")
                        {
                            Value = (item.Fields.Contains(SPBuiltInFieldId.DiscussionTitle) ? (item[SPBuiltInFieldId.DiscussionTitle] as string) ?? string.Empty : string.Empty)
                        };

                        var clearSubjText = new XElement("ClearValue") { Value = GetDiscussionSubjText(subjText.Value) };

                        var bodyText = new XElement("Value")
                        {
                            Value = (item.Fields.Contains(SPBuiltInFieldId.Body) ? (item[SPBuiltInFieldId.Body] as string) ?? string.Empty : string.Empty)
                        };

                        var clearBodyText = new XElement("ClearValue") { Value = GetDiscussionBodyText(bodyText.Value) };

                        subjElement.Add(subjText, clearSubjText);
                        bodyElement.Add(bodyText, clearBodyText);
                        curent.Add(subjElement, bodyElement);
                    }
                    else
                    {
                        curent = new XElement("Message");
                        if (item.UniqueId == listItem.UniqueId)
                        {
                            curent.SetAttributeValue("Current", true);
                        }

                        var bodyElement = new XElement("Body");
                        var bodyText = new XElement("Value")
                        {
                            Value = (item.Fields.Contains(SPBuiltInFieldId.Body) ? (item[SPBuiltInFieldId.Body] as string) ?? string.Empty : string.Empty)
                        };

                        var clearBodyText = new XElement("ClearValue") { Value = GetMessageBodyText(bodyText.Value) };

                        bodyElement.Add(bodyText, clearBodyText);
                        curent.Add(bodyElement);
                    }
                    if (parent != null)
                    {
                        parent.Add(curent);
                    }
                    parent = curent;
                }

                return element;
           
        }

        private string GetMessageBodyText(string p)
        {
            return "clear message body text";
        }

        private string GetDiscussionSubjText(string p)
        {
            return "clear discussion subject text";
        }

        private string GetDiscussionBodyText(string p)
        {
            return "clear discussion body text";
        }

        private List<SPListItem> GetDescedantsForMessage(SPListItem message)
        {
            var res = new List<SPListItem>();
            if (message.Fields.Contains(SPBuiltInFieldId.ParentFolderId))
            {
                var parentId = (int)message[SPBuiltInFieldId.ParentFolderId];
                var parent = message.ParentList.GetItemById(parentId);
                res.Add(parent);
                res.Add(message);
            }
            return res;
        }
    }
}
