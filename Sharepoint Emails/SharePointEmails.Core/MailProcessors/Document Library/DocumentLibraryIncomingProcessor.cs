using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.Core.Interfaces.MailProcessor.Strategies;
using SharePointEmails.Core.Extensions;
namespace SharePointEmails.Core.MailProcessors.Document_Library
{
    class DocumentLibraryIncomingProcessor : BaseIncomingMailProcessor
    {
        IDocumentLibraryGetFile getFileStarategy;
        SEMessage message;
        ConfigProvider config;
        SPDocumentLibrary library;
        internal DocumentLibraryIncomingProcessor(SPDocumentLibrary library, SEMessage message, ConfigProvider config)
        {

        }

        public override void Process()
        {
            if (config.AddAttachment)
            {
                var file = getFileStarategy.GetFile(message);
                if (file != null)
                {
                    var item = library.AddItem();
                    if (config.SaveInitialMessage)
                    {
                        using (var stream = message.GetMessageStream())
                        {
                            item.Attachments.Add("initial message.eml", stream.ReadFully());
                        }
                    }

                    item.File.SaveBinary(file, false);
                }
            }
        }
    }
}
