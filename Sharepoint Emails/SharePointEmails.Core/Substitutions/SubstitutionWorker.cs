using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;
using SharePointEmails.Core.Substitutions;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    public class SubstitutionWorker : ISubstitution, ISubstitutionWorker
    {
        private List<ISubstitution> m_substitutions; //all substitutions
        private List<ISubstitution> m_alreadyProcessed;//substitutions which have been already used

        private ILogger m_Logger;

        private ISubstitutionContext m_currentContext;

        public SubstitutionWorker(ILogger logger, List<ISubstitution> sustitutions)
        {
            m_Logger = logger;
            m_substitutions = sustitutions ?? new List<ISubstitution>(); ;
            m_alreadyProcessed = new List<ISubstitution>();
        }

        public string OnPartLoaded(string part)//some substitutions can include new parts to template so we need process them with already processed substitutions
        {
            return Process(part, m_alreadyProcessed, m_currentContext, null);
        }

        string Process(string res, IList<ISubstitution> substitutions, ISubstitutionContext context, Action<ISubstitution> processedCallback)
        {
            if (m_substitutions != null)
            {
                foreach (var substitution in substitutions)
                {
                    m_Logger.Write("START SUBSTITUTION : " + substitution.GetType().Name + "with template :" + Environment.NewLine + res, SeverityEnum.Verbose);
                    m_Logger.Write("START SUBSTITUTION : " + substitution.GetType().Name, SeverityEnum.Verbose);
                    try
                    {
                        m_currentContext = context;
                        substitution.Worker = this;
                        res = substitution.Process(res, context);
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

        public string Process(string data, ISubstitutionContext context)
        {
            var res = data ?? "";
            m_alreadyProcessed.Clear();
            res = Process(data, m_substitutions, context, (s) => m_alreadyProcessed.Add(s));
            return (string.IsNullOrEmpty(res)) ? "empty" : res;
        }


        public ISubstitutionWorker Worker
        {
            get;set;
        }
    }
}
