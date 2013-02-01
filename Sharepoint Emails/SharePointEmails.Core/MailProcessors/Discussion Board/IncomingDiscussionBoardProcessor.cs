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
using SharePointEmails.Core;
using SharePointEmails.Core.MailProcessors;
using SharePointEmails.Core.MailProcessors.Strategies;
using SharePointEmails.Core.Extensions;
namespace SharePointEmails.MailProcessors
{
    internal class IncomingDiscussionBoardProcessor : IIncomingMessageProcessor
    {
        IThreadStrategy m_threadStarategy = null;

        ITextParserStrategy m_TextStartegy = null;

        ILogger m_Logger = null;

        SPList m_list = null;
        SEMessage m_message = null;
        SPUser m_user = null;

        bool m_itemIdPresented = false;

        public IncomingDiscussionBoardProcessor(SPList list, SEMessage message, ILogger logger, IThreadStrategy threadStrategy, ITextParserStrategy textStrategy)
        {
            this.m_list = list;
            this.m_message = message;
            this.m_Logger = logger;
            this.m_threadStarategy = threadStrategy;
            this.m_TextStartegy = textStrategy;
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

        void AddEmail(SPListItem item)
        {
            using (var stream = m_message.GetMessageStream())
            {
                item.Attachments.Add("OriginalMessage.eml",stream.ReadFully());
            }
        }

        public void Process()
        {
            m_user = m_list.ParentWeb.SiteUsers.GetByEmail(m_message.Sender);
            if (m_user != null)
            {
                using (var site = new SPSite(m_list.ParentWeb.Site.ID, m_user.UserToken))
                {
                    using (var web = site.OpenWeb(m_list.ParentWeb.ID))
                    {
                        m_list = web.Lists.GetList(m_list.ID, false, true);
                        SPListItem relatedItem = GetRelatedListItem();
                        SPListItem newItem = null;
                        if (relatedItem != null)
                        {
                            m_Logger.WriteTrace("Related item was found", SeverityEnum.Trace);
                            if (relatedItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Discussion) || relatedItem.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Message))
                            {
                                newItem = AddReplyToItem(relatedItem);
                            }
                            else
                            {
                                m_Logger.WriteTrace("Related item has some wrong content type " + relatedItem.ContentTypeId, SeverityEnum.CriticalError);
                            }
                        }
                        else
                        {
                            m_Logger.WriteTrace("Related item was not found", SeverityEnum.Trace);
                            if (m_itemIdPresented)
                            {
                                m_Logger.WriteTrace("Item id was present", SeverityEnum.Trace);

                            }
                            else
                            {
                                m_Logger.WriteTrace("Item id was not present", SeverityEnum.Trace);
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
                            newItem.Update();
                            m_Logger.WriteTrace("Item SUCCESSFULY added", SeverityEnum.Trace);
                        }
                        else
                        {
                            m_Logger.WriteTrace("No new item", SeverityEnum.Warning);
                        }
                    }
                }
            }
            else
            {
                m_Logger.WriteTrace("No user with email=" + m_message.Sender, SeverityEnum.Warning);
            }
        }

        void AddAttachments(SPListItem newItem)
        {
            foreach (SPEmailAttachment att in m_message.Attachments)
            {
                newItem.Attachments.Add(att.FileName ?? Path.GetTempFileName(),att.ContentStream.ReadFully());
            }
        }

        SPListItem AddNewDiscussion()
        {
            return m_TextStartegy.CreateDiscussion(m_list, m_message);
        }

        SPListItem AddReplyToItem(SPListItem relatedItem)
        {
            return m_TextStartegy.CreateReply(relatedItem, m_message);
        }

        int GetItemId()
        {
            return m_threadStarategy.GetRelatedItemId(m_message);
        }

        SPListItem GetRelatedListItem()
        {
            var id = GetItemId();
            m_itemIdPresented = id != -1;
            if (m_itemIdPresented)
            {
                try
                {
                    return m_list.GetItemById(id);
                }
                catch (ArgumentException ex)
                {
                    m_Logger.WriteTrace("Item with ID=" + id + " not found", SeverityEnum.Warning);
                    m_Logger.WriteTrace(ex, SeverityEnum.Warning);
                }
            }
            return null;
        }
    }
}
