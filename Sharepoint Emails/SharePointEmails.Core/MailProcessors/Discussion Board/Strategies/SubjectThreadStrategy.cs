using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using SharePointEmails.Logging;

namespace SharePointEmails.Core.MailProcessors
{
    internal class SubjectThreadStrategy:IThreadStrategy
    {
        ILogger m_Logger = Application.Current.Logger;

        public int GetRelatedItemId(SEMessage mail)
        {
            var res = -1;
            var subject = mail.Subject ?? string.Empty;
            var matches = Regex.Matches(subject, @"\(([0-9]+)\)");
            if (matches.Count > 0)
            {
                var value = matches[0].Groups[1].Value;
                subject = subject.Replace(value, "");
                mail.Subject = subject;
                res = int.Parse(value);
            }
            return res;
        }

        public void OnMailSending(SEMessage mail, int ItemId)
        {
            mail.Subject = mail.Subject + string.Format("({0})", ItemId);
        }
    }
}
