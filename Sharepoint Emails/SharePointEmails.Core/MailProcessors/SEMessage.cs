using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Microsoft.SharePoint.Utilities;
using System.IO;

namespace SharePointEmails.Core.MailProcessors
{
    public class SEMessage
    {
        public const string SUBJECT = "subject";
        public const string REPLYTO = "reply-to";
        public const string FROM = "from";

        public static SEMessage Create(Guid eventID, GeneratedMessage message, StringDictionary stringDictionary, string body)
        {
            var mail = new SEMessage();
            mail.HtmlBody = body;
            mail.EventID = eventID;
            foreach (string key in stringDictionary.Keys)
            {
                mail[key] = stringDictionary[key];
            }
            if (message.Subject != null)
            {
                mail[SEMessage.SUBJECT] = message.Subject;
            }
            if (message.Body != null)
            {
                mail.HtmlBody = message.Body;
            }
            if (message.Replay != null)
            {
                mail[REPLYTO] = message.Replay;
            }
            if (message.From != null)
            {
                mail[FROM] = message.From;
            }

            return mail;
        }

        public static SEMessage Create(SPEmailMessage mail)
        {
            var res = new SEMessage();
            res.inner = mail;
            foreach (var i in mail.Headers)
            {
                res.headers.Add(i.Name, i.Value);
            }
            res.PlainBody = mail.PlainTextBody;
            res.HtmlBody = mail.HtmlBody;
            res.Sender = mail.Sender;
            return res;
        }


        public string this[string name]
        {
            get
            {
                return headers[name];
            }
            set
            {
                headers[name] = value;
            }
        }

        public StringDictionary headers = new StringDictionary();

        public Guid EventID{ set; get; }
        public string PlainBody { set; get; }
        public string HtmlBody { set; get; }
        public string Sender { set; get; }
        public string Subject {
            get
            {
                return this[SEMessage.SUBJECT];
            }
            set
            {
                headers[SEMessage.SUBJECT] = value;
            }
        }

        SPEmailMessage inner;
        public Stream GetMessageStream()
        {
            return inner.GetMessageStream();
        }
        public SPEmailAttachmentCollection Attachments
        {
            get
            {
                return inner.Attachments;
            }
        }
    }
}
