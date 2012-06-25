using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public class SubstitutionWorker : ISubstitutionWorker
    {
        private List<ISubstitution> m_substitutions;

        private ILogger m_Logger;

        private ISubstitutionContext m_context;

        public SubstitutionWorker(ILogger logger, ISubstitutionContext context)
        {
            m_Logger = logger;
            m_context = context;
            m_substitutions = new List<ISubstitution>
            {
                new ResourceSubstitution(),
                new FieldSubstitution(),
                new ContextSubstitution()
            };
        }

        public string Process(string data)
        {
            var res = data;
            if (m_substitutions != null)
            {
                foreach (var substitution in m_substitutions)
                {
                    try
                    {
                        res = substitution.Process(res, m_context);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Write(ex, SeverityEnum.CriticalError);
                    }
                }
            }
            if (string.IsNullOrEmpty(res))
                return "empty";
            else
                return res;
        }
    }
}
