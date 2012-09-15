using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public class SubstitutionWorker : ISubstitution
    {
        private List<ISubstitution> m_substitutions;
        private List<ISubstitution> m_alreadyProcessed;

        private ILogger m_Logger;

        private ISubstitutionContext m_context;

        public SubstitutionWorker(ILogger logger, List<ISubstitution> sustitutions)
        {
            m_Logger = logger;
            m_substitutions = sustitutions ?? new List<ISubstitution>(); ;
            m_alreadyProcessed = new List<ISubstitution>();
        }

        string ProcessIncludes(string includes)
        {
            return Process(includes, m_alreadyProcessed, m_context);
        }

        string Process(string res, IList<ISubstitution> substitutions, ISubstitutionContext context, Action<ISubstitution> processedCallback=null)
        {
            if (m_substitutions != null)
            {
                foreach (var substitution in substitutions)
                {
                    m_Logger.Write("START SUBSTITUTION : " + substitution.GetType().Name + "with template :" + Environment.NewLine + res, SeverityEnum.Verbose);
                    m_Logger.Write("START SUBSTITUTION : " + substitution.GetType().Name, SeverityEnum.Verbose);
                    try
                    {
                        res = substitution.Process(res, context, ProcessIncludes);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Write(ex, SeverityEnum.CriticalError);
                    }
                    finally
                    {
                        if (processedCallback != null)
                        {
                            processedCallback(substitution);
                        }
                    }
                }
            }
            return res;
        }

        public string Process(string data, ISubstitutionContext context, Func<string, string> processIncludes=null)
        {
            var res = data ?? "";
            m_alreadyProcessed.Clear();
            m_context = context;
            res = Process(data, m_substitutions, context, (s) => m_alreadyProcessed.Add(s));
            return (string.IsNullOrEmpty(res)) ? "empty" : res;
        }
    }
}
