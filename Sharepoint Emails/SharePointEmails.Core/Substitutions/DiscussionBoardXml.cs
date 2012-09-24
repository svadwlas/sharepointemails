﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Text.RegularExpressions;
using SharePointEmails.Core.Extensions;
namespace SharePointEmails.Core.Substitutions
{
    public class DiscussionBoardXml
    {
        const string DiscussionBoardNameSpace = "urn:sharepointemail-discussionboard";

        public static DiscussionBoardXml Create()
        {
            return new DiscussionBoardXml();
        }

        private DiscussionBoardXml() { }

        public XElement GetElement(SPListItem listItem)
        {
            if (listItem != null && (listItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Message) || listItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Discussion)))
            {
                XNamespace nsp = XNamespace.Get(DiscussionBoardNameSpace);
                var element = new XElement(nsp + "DiscussionBoard");
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
                foreach (SPListItem item in chain)//first item in chain should be Discussion, last current message
                {
                    if (item.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Discussion))
                    {
                        curent = new XElement(nsp + "Discussion");
                        element.Add(curent);
                        var subjElement = new XElement(nsp + "Subject");
                        var bodyElement = new XElement(nsp + "Body");
                        var subjText = new XElement(nsp + "Value")
                        {
                            Value = item.GetFieldValue<string>(SPBuiltInFieldId.Title, string.Empty)
                        };

                        var clearSubjText = new XElement(nsp + "ClearValue") { Value = GetClearDiscussionSubjText(subjText.Value) };

                        var bodyText = new XElement(nsp + "Value")
                        {
                            Value = item.GetFieldValue<string>(SPBuiltInFieldId.Body, string.Empty)
                        };

                        var clearBodyText = new XElement(nsp + "ClearValue") { Value = GetClearDiscussionBodyText(bodyText.Value) };

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
                        curent.SetAttributeValue("UserName", createdBy.User.Name);
                    }
                    if (parent != null)
                    {
                        parent.Add(curent);
                    }
                    parent = curent;
                }

                return element;
            }
            return null;
        }

        string RemoveTags(string text)
        {
            return Regex.Replace(text, @"&lt;(.|\n)*?&gt;|<(.|\n)*?>", string.Empty);
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

        SPQuery GetQueryForChain(string threadIndex)
        {
            var sb = new StringBuilder();
            sb.Append("<Where>");
            sb.Append("<Contains>");
            sb.Append("<FieldRef Name='ThreadIndex'/>");
            sb.Append("<Value Type='Text'>");
            sb.Append(threadIndex);
            sb.Append("</Value>");
            sb.Append("</Contains>");
            sb.Append("</Where>");
            return new SPQuery()
            {
                Query= sb.ToString()
            };
        }

        SPQuery GetQueryForDiscussionItems(SPFolder folder)
        {
            return new SPQuery()
            {
                Folder = folder
            };
        }

        List<SPListItem> GetDescedantsForMessage(SPListItem message)
        {
            var res = new List<SPListItem>();
            var discussionItem = message.ParentList.GetItemById(message.GetFieldValue<int>(SPBuiltInFieldId.ParentFolderId));
            var threadIndex = message.GetFieldValue<string>(SPBuiltInFieldId.ThreadIndex, "");
            if (!string.IsNullOrEmpty(threadIndex))
            {
                var d = new Dictionary<int, SPListItem>();
                foreach (SPListItem item in message.ParentList.GetItems(GetQueryForDiscussionItems(discussionItem.Folder)))//cannot query only for thread
                {
                    var index = item.GetFieldValue<string>(SPBuiltInFieldId.ThreadIndex);
                    if (threadIndex.StartsWith(index))
                    {
                        d.Add(index.Length, item);
                    }
                }
                res.AddRange(d.OrderBy(p => p.Key)
                    .Where(p => p.Key <= threadIndex.Length)//to avoid new messages after this
                    .Select(p => p.Value));
            }
            else
            {
                res.Add(message);
            }
            res.Insert(0, discussionItem);
            return res;
        }
    }
}
