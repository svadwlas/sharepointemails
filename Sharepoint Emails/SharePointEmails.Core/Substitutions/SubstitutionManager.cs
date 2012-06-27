using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public class SubstitutionManager
    {
        public ISubstitutionWorker GetWorker(ISubstitutionContext context, WorkerType type)
        {
            switch (type)
            {
                case WorkerType.ForBody:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), context, new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ComplexSubstitution(),
                                                                                                                        new ContextSubstitution(),
                                                                                                                        new XlstSubstitution()
                                                                                                                    });
                case WorkerType.ForSubject:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), context, new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ContextSubstitution(),
                                                                                                                        new XlstSubstitution()
                                                                                                                    });
                case WorkerType.ForFrom:
                case WorkerType.ForReplay:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), context, new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ContextSubstitution(),
                                                                                                                        new XlstSubstitution()
                                                                                                                    });
                default: return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), context, new List<ISubstitution>());
            }
        }

        public enum WorkerType
        {
            ForBody,ForSubject,ForFrom,ForReplay
        }
    }

}
