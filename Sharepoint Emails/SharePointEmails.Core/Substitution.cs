using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;

namespace SharePointEmails.Core
{
    public class SubstitutionWorker : ISubstitutionWorker
    {
        private List<ISubstitution> m_substitutions = new List<ISubstitution>();

        private ILogger m_Logger;

        private ISubstitutionContext m_context;

        public SubstitutionWorker(ILogger logger, ISubstitutionContext context)
        {
            m_Logger = logger;
            m_context = context;
        }

        public void Process(string data)
        {
            if (m_substitutions != null)
            {
                foreach (var substitution in m_substitutions)
                {
                    try
                    {
                        substitution.Process(data, m_context);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Write(ex,SeverityEnum.CriticalError);
                    }
                }
            }
        }
    }
}
