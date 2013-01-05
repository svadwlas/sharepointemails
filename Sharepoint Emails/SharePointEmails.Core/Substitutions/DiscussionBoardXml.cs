using System;
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
        public static DiscussionBoardXml Create()
        {
            return new DiscussionBoardXml();
        }

        private DiscussionBoardXml() { }

        public static XName L(string name)
        {
            return SubstitutionContext.L(name);
        }

        public XElement GetElement(SPListItem listItem)
        {
            if (listItem != null && (listItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Message) || listItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Discussion)))
            {
                var element = new XElement(L("DiscussionBoard"));
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
                        curent = new XElement(L("Discussion"));
                        
                        element.Add(curent);
                        var subjElement = new XElement(L("Subject"));
                        var bodyElement = new XElement(L("Body"));
                        var subjText = new XElement(L("Value"))
                        {
                            Value = item.GetFieldValue<string>(SPBuiltInFieldId.Title, string.Empty)
                        };

                        var clearSubjText = new XElement(L("ClearValue")) { Value = GetClearDiscussionSubjText(subjText.Value) };

                        var bodyText = new XElement(L("Value"))
                        {
                            Value = item.GetFieldValue<string>(SPBuiltInFieldId.Body, string.Empty)
                        };

                        var clearBodyText = new XElement(L("ClearValue")) { Value = GetClearDiscussionBodyText(bodyText.Value) };

                        subjElement.Add(subjText, clearSubjText);
                        bodyElement.Add(bodyText, clearBodyText);
                        curent.Add(subjElement, bodyElement);
                    }
                    else
                    {
                        curent = new XElement(L("Message"));
                        var bodyElement = new XElement(L("Body"));
                        var bodyText = new XElement(L("Value"))
                        {
                            Value = item.GetFieldValue<string>(SPBuiltInFieldId.Body, string.Empty)
                        };

                        var clearBodyText = new XElement(L("ClearValue")) { Value = GetClearMessageBodyText(bodyText.Value) };

                        bodyElement.Add(bodyText, clearBodyText);
                        curent.Add(bodyElement);
                    }
                    if (item.UniqueId == listItem.UniqueId)
                    {
                        curent.SetAttributeValue("Current", true);
                    }
                    if (item.Fields.Contains(SPBuiltInFieldId.Created))
                    {
                        curent.SetAttributeValue("Created", item.GetFieldValue<DateTime>(SPBuiltInFieldId.Created).ToUniversalTime());
                    }
                    if (item.ParentList != null && item.ParentList.ParentWeb != null)
                    {
                        var createdBy = new SPFieldUserValue(item.ParentList.ParentWeb, item[SPBuiltInFieldId.Author].ToString());
                        curent.SetAttributeValue("User", createdBy.User.LoginName);
                        curent.SetAttributeValue("UserName", createdBy.User.Name);
                        curent.SetAttributeValue("UserProfileUrl", "/_layouts/userdisp.aspx?ID="+createdBy.User.ID);
                        curent.SetAttributeValue("UserProfileUrl", "a");
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
                    if (threadIndex.StartsWith(index) && item.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Message))
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
