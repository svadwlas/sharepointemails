using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SharePointEmails.Logging;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    class RemoveXmlTagsSubstitution : BaseSubstitution
    {
        public override string Process(string text, ISubstitutionContext context)
        {
            try
            {
                var doc = XDocument.Parse(text);
                return doc.Root.Value;
            }
            catch (Exception ex)
            {
                Logger.WriteTrace("Cannot remove tags. maybe they are not existed" + ex.Message, SeverityEnum.Verbose);
                return text;
            }
        }
    }
}
