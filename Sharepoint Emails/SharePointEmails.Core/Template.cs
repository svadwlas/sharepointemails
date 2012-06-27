using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.IO;
using SharePointEmails.Core.Substitutions;

namespace SharePointEmails.Core
{
    public class Template : ITemplate
    {
        private SPListItem m_Item = null;
        public Template()
        {
            this.Name = "Noname";
            this.Body = "No body";
            this.EventTypes = (int)TemplateTypeEnum.Unknown;
            this.State = TemplateStateEnum.Unknown;
        }
        public Template(SPListItem item)
        {
            m_Item = item;
            Refresh();
        }

        private bool BodyAttached { set; get; }
        private bool SubjectAttached { set; get; }
        private bool UseFileForSubject { set; get; }
        private bool UseFileForBody { set; get; }


        public string Body { get; set; }

        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public TemplateStateEnum State
        {
            get;
            set;
        }

        public int EventTypes
        {
            get;
            set;
        }


        public AssociationConfiguration Asses
        {
            get
            {
                return AssociationConfiguration.ParseOrDefault(_Config);
            }
            set
            {
                _Config = (value ?? AssociationConfiguration.Empty).ToString();
            }
        }
        string _Config = null;

        void Refresh()
        {
            this.Name = m_Item[SEMailTemplateCT.TemplateName] as string;
            this.UseFileForSubject = (bool)m_Item[SEMailTemplateCT.TemplateSubjectUseFile];
            this.UseFileForBody = (bool)m_Item[SEMailTemplateCT.TemplateBodyUseFile];

            if (!this.UseFileForBody)
            {
                this.Body = m_Item[SEMailTemplateCT.TemplateBody] as string;
                var content = m_Item.GetAttachmentContent(this.Body);
                if (content != null)
                {
                    BodyAttached = true;
                    this.Body = content;
                }
            }
            else
            {
                this.Body = m_Item.GetLookupFileContent(SEMailTemplateCT.TemplateBodyFile) ?? "";
            }

            if (!this.UseFileForSubject)
            {
                this.Subject = m_Item[SEMailTemplateCT.TemplateSubject] as string;
                var content = m_Item.GetAttachmentContent(this.Subject);
                if (content != null)
                {
                    SubjectAttached = true;
                    this.Subject = content;
                }
            }
            else
            {
                this.Subject = m_Item.GetLookupFileContent(SEMailTemplateCT.TemplateSubjectFile) ?? "";
            }
            if (m_Item[SEMailTemplateCT.TemplateType] != null)
            {
                var val = new SPFieldMultiChoiceValue(m_Item[SEMailTemplateCT.TemplateType].ToString());
                this.EventTypes = EnumConverter.ToType(val);
            }
            else
            {
                this.EventTypes = (int)TemplateTypeEnum.Unknown;
            }
            this.State = EnumConverter.ToState(m_Item[SEMailTemplateCT.TemplateState] as string);
            this.Asses = AssociationConfiguration.ParseOrDefault(m_Item[SEMailTemplateCT.Associations] as string);
        }

        public void SaveTo(SPListItem item)
        {
            m_Item = item;
            Update();
        }

        public void Update()
        {
            //TODO update other fields
            m_Item[SEMailTemplateCT.TemplateName] = this.Name;
            m_Item[SEMailTemplateCT.TemplateSubject] = this.Subject;
            m_Item[SEMailTemplateCT.TemplateBody] = this.Body;
            m_Item[SEMailTemplateCT.TemplateType] = EnumConverter.TypeToValue(this.EventTypes);
            m_Item[SEMailTemplateCT.TemplateState] = EnumConverter.StateToValue(this.State);
            m_Item[SEMailTemplateCT.Associations] = Asses.ToString();
            m_Item.Update();
            Refresh();
        }

        public string GetProcessedBody(ISubstitutionContext context, ProcessMode mode)
        {
            var manager = ClassContainer.Instance.Resolve<SubstitutionManager>();
            var worker = manager.GetWorker(context, SubstitutionManager.WorkerType.ForBody);
            return worker.Process(Body, mode); ;
        }

        public string GetProcessedSubj(ISubstitutionContext context, ProcessMode mode)
        {
            var manager = ClassContainer.Instance.Resolve<SubstitutionManager>();
            var worker = manager.GetWorker(context, SubstitutionManager.WorkerType.ForSubject);
            return worker.Process(Subject, mode);
        }

        public string Subject { set; get; }


        public override string ToString()
        {
            var s =
                "Name: " + Name + Environment.NewLine +
                "EventTypes: " + EventTypes + Environment.NewLine +
                "State: " + State + Environment.NewLine +
                "Pattern: " + Body + Environment.NewLine;
            foreach (var ass in Asses)
            {
                s += "Ass: " + Environment.NewLine + ass.ToString() + Environment.NewLine;
            }
            return s;
        }
    }
}
