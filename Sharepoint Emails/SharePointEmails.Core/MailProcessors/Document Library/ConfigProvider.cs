using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core.MailProcessors.Document_Library
{
    class ConfigProvider
    {
        public bool AddAttachment { get; set; }

        public bool SaveInitialMessage { get; set; }

        public bool AllowAnonym { get; set; }

        public ConfigProvider()
        {
            AddAttachment = true;
            SaveInitialMessage = true;
            AllowAnonym = true;
        }
    }
}
