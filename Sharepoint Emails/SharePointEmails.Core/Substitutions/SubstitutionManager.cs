using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;
using SharePointEmails.Core.Substitutions;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    public class SubstitutionManager
    {
        public ISubstitution GetWorker(ISubstitutionContext context, WorkerType type)
        {
            switch (type)
            {
                case WorkerType.ForBody:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(),  new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ComplexSubstitution(),
                                                                                                                        new ContextVarsSubstitution(),
                                                                                                                        new XlstSubstitution()
                                                                                                                    });
                case WorkerType.ForSubject:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ContextVarsSubstitution(),
                                                                                                                        new XlstSubstitution(),
                                                                                                                        new RemoveXmlTagsSubstitution(),
                                                                                                                        new OneLineSubstitution()
                                                                                                                    });
                case WorkerType.ForFrom:
                case WorkerType.ForReplay:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ContextVarsSubstitution(),
                                                                                                                        new XlstSubstitution(),
                                                                                                                        new RemoveXmlTagsSubstitution(),
                                                                                                                        new OneLineSubstitution()
                                                                                                                    });
                default: return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), new List<ISubstitution>());
            }
        }

        public enum WorkerType
        {
            ForBody, ForSubject, ForFrom, ForReplay
        }
    }

}
