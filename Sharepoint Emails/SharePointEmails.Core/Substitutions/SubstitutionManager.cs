using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Logging;
using SharePointEmails.Core.Substitutions;
using SharePointEmails.Core.Interfaces;
using SharePointEmails.Core.Enums;

namespace SharePointEmails.Core.Substitutions
{
    public class SubstitutionManager:ISubstitutionManager
    {
        public ISubstitutionWorker GetWorker(ISubstitutionContext context, MessageFieldType type)
        {
            switch (type)
            {
                case MessageFieldType.ForBody:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ComplexSubstitution(),
                                                                                                                        new ContextVarsSubstitution(),
                                                                                                                    },
                                                                                            new List<ISubstitution>
                                                                                                                    {
                                                                                                                    });
                case MessageFieldType.ForSubject:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ContextVarsSubstitution(),
                                                                                                                        
                                                                                                                    },
                                                                                            new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new RemoveXmlTagsSubstitution(),
                                                                                                                        new OneLineSubstitution()
                                                                                                                    });
                case MessageFieldType.ForFrom:
                case MessageFieldType.ForReplay:
                    return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new ResourceSubstitution(),
                                                                                                                        new FieldSubstitution(),
                                                                                                                        new ContextVarsSubstitution(),
                                                                                                                    },
                                                                                            new List<ISubstitution>
                                                                                                                    {
                                                                                                                        new RemoveXmlTagsSubstitution(),
                                                                                                                        new OneLineSubstitution()
                                                                                                                    });
                default: return new SubstitutionWorker(ClassContainer.Instance.Resolve<ILogger>(), new List<ISubstitution>(), new List<ISubstitution>());
            }
        }

       
    }

}
