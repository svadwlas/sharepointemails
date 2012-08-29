using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Interfaces;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Text.RegularExpressions;
using SharePointEmails.Logging;
using System.IO;

namespace SharePointEmails.MailProcessors
{
    public class IncomingDiscussionBoardProcessor : IIncomingMessageProcessor
    {
        ILogger m_Logger = null;

        SPList m_list = null;
        SPEmailMessage m_message = null;
        SPUser m_user = null;

        string m_body = string.Empty;
        string m_subject = string.Empty;
        bool m_itemIdPresented = false;

        public IncomingDiscussionBoardProcessor(SPList list, SPEmailMessage message, ILogger logger)
        {
            this.m_list = list;
            this.m_message = message;
            this.m_Logger = logger;
        }

        private void Parse()
        {
            m_subject = m_message.Headers["subject"];
            m_body = m_message.PlainTextBody;
        }

        bool GetOption(string option, bool defaultvalue)
        {
            bool flag = defaultvalue;
            object obj2 = m_list.RootFolder.Properties[option];
            if (obj2 is int)
            {
                flag = 1 == ((int)obj2);
            }
            return flag;
        }

        bool SaveOriginalMessage
        {
            get
            {
                return GetOption("vti_emailsaveoriginal", true);
            }
        }

        bool SaveAttachments
        {
            get
            {
                return GetOption("vti_emailsaveattachments", false);
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        void AddEmail(SPListItem item)
        {
            using (var stream = m_message.GetMessageStream())
            {
                item.Attachments.Add("OriginalMessage.eml", ReadFully(stream));
            }
        }

        public void Process()
        {
            m_user = m_list.ParentWeb.SiteUsers.GetByEmail(m_message.Sender);
            if (m_user == null)
            {
                m_Logger.Write("No user with email=" + m_message.Sender, SeverityEnum.Warning);
            }
            Parse();
            SPListItem relatedItem = GetRelatedListItem();
            SPListItem newItem = null;
            if (relatedItem != null)
            {
                m_Logger.Write("Related item was found", SeverityEnum.Trace);
                if (relatedItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Discussion))
                {
                    m_Logger.Write("Related item is discussion", SeverityEnum.Trace);
                    newItem = AddMessageToDiscussion(relatedItem);
                }
                else if (relatedItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Message))
                {
                    var parentId = (int)relatedItem[SPBuiltInFieldId.ParentFolderId];
                    var disc = m_list.GetItemById(parentId);
                    newItem = AddMessageToDiscussion(disc);
                    m_Logger.Write("Related item is message", SeverityEnum.Trace);
                }
                else
                {
                    m_Logger.Write("Related item has some wrong content type " + relatedItem.ContentTypeId, SeverityEnum.CriticalError);
                }
            }
            else
            {
                m_Logger.Write("Related item was not found", SeverityEnum.Trace);
                if (m_itemIdPresented)
                {
                    m_Logger.Write("Item id was present", SeverityEnum.Trace);

                }
                else
                {
                    m_Logger.Write("Item id was not present", SeverityEnum.Trace);
                    newItem = AddNewDiscussion();
                }
            }
            if (newItem != null)
            {
                if (SaveOriginalMessage)
                {
                    AddEmail(newItem);
                }
                if (SaveAttachments)
                {
                    AddAttachments(newItem);
                }
                if (m_user != null)
                {
                    var value = new SPFieldUserValue(m_list.ParentWeb, m_user.ID, m_user.LoginName);
                    newItem[SPBuiltInFieldId.Editor] = value;
                    newItem[SPBuiltInFieldId.Author] = value;
                }
                newItem.Update();
                m_Logger.Write("Item SUCCESSFULY added", SeverityEnum.Trace);
            }
            else
            {
                m_Logger.Write("No new item", SeverityEnum.Warning);
            }
        }

        private void AddAttachments(SPListItem newItem)
        {
            foreach(SPEmailAttachment att in m_message.Attachments )
            {
                newItem.Attachments.Add(att.FileName ?? Path.GetTempFileName(), ReadFully(att.ContentStream));
            }
        }

        private SPListItem AddNewDiscussion()
        {
            var item = SPUtility.CreateNewDiscussion(m_list, m_subject);
            item[SPBuiltInFieldId.Body] = m_body;      
            return item;
        }

        private SPListItem AddMessageToDiscussion(SPListItem relatedItem)
        {
            var reply = SPUtility.CreateNewDiscussionReply(relatedItem);
            reply[SPBuiltInFieldId.Body] = m_message.PlainTextBody;
            reply[SPBuiltInFieldId.Title] = m_message.PlainTextBody;
            return reply;
        }

        int GetItemId()
        {
            m_Logger.Write("try get id from subj=" + m_subject, SeverityEnum.Trace);
            var matches = Regex.Matches(m_subject, @"\(([0-9]+)\)");
            if (matches.Count > 0)
            {
                var value = matches[0].Groups[1].Value;
                m_subject = m_subject.Replace(value, "");
                return int.Parse(value);
            }
            else
            {
                return -1;
            }
        }

        private SPListItem GetRelatedListItem()
        {
            var id = GetItemId();
            m_itemIdPresented = id != -1;
            if (m_itemIdPresented)
            {
                try
                {
                    var item = m_list.GetItemById(id);

                    return item;
                }
                catch (ArgumentException ex)
                {
                    m_Logger.Write("Item with ID=" + id + " not found", SeverityEnum.Warning);
                    m_Logger.Write(ex, SeverityEnum.Warning);
                }
            }
            return null;
        }
    }
}
