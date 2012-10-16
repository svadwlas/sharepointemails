using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPMocksBuilder;
using SharePointEmails.Core.Interfaces;
using Microsoft.SharePoint;

namespace SharePointEmails.Core.Tests
{
    static class TestUtils
    {
        static public VList CreateWithTemplates()
        {
            var vContentType = new VContentType(new SPContentTypeId(TemplateCT.CTId))
            {
                Fields = new[]
                {
                    new VField(TemplateCT.TemplateName),
                    new VField(TemplateCT.Associations),
                    new VField(TemplateCT.SendDraftTo,null,SPFieldType.User),
                    new VField(TemplateCT.TemplateBody),
                    new VField(TemplateCT.TemplateBodyFile),
                    new VField(TemplateCT.TemplateBodyUseFile),
                    new VField(TemplateCT.TemplateFrom),
                    new VField(TemplateCT.TemplateFromFile),
                    new VField(TemplateCT.TemplateFromUseFile),
                    new VField(TemplateCT.TemplateReplay),
                    new VField(TemplateCT.TemplateReplayFile),
                    new VField(TemplateCT.TemplateReplayUseFile),
                    new VField(TemplateCT.TemplateState),
                    new VField(TemplateCT.TemplateSubject),
                    new VField(TemplateCT.TemplateSubjectFile),
                    new VField(TemplateCT.TemplateSubjectUseFile),
                    new VField(TemplateCT.TemplateType),
                }
            };

            var vlist = new VList()
            {
                Title = Constants.TemplateListName,
                ContentTypes = new[] { vContentType },
            };

            return vlist;
        }
    }
}
