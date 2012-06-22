using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

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

        void Refresh()
        {
            this.Name = m_Item[SEMailTemplateCT.TemplateName] as string;
            this.Subject = m_Item[SEMailTemplateCT.TemplateSubject] as string;
            this.Body = m_Item[SEMailTemplateCT.TemplateBody] as string;
            if (m_Item[SEMailTemplateCT.TemplateType] != null)
            {
                var val = new SPFieldMultiChoiceValue(m_Item[SEMailTemplateCT.TemplateType].ToString());
                this.EventTypes = EnumConverter.ToType(val);
            }
            else
            {
                this.EventTypes = (int)TemplateTypeEnum.AllItemEvents;
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
            m_Item[SEMailTemplateCT.TemplateName] = this.Name;
            m_Item[SEMailTemplateCT.TemplateSubject] = this.Subject;
            m_Item[SEMailTemplateCT.TemplateBody] = this.Body;
            m_Item[SEMailTemplateCT.TemplateType] = EnumConverter.TypeToValue(this.EventTypes);
            m_Item[SEMailTemplateCT.TemplateState] = EnumConverter.StateToValue(this.State);
            m_Item[SEMailTemplateCT.Associations] = Asses.ToString();
            m_Item.Update();
            Refresh();
        }

        public string GetProcessedText(ISubstitutionContext context)
        {
            var manager = ClassContainer.Instance.Resolve<SubstitutionManager>();
            var worker = manager.GetWorker(context);
            var res = worker.Process(Body);
            return res;
        }

        public string GetProcessedSubj(ISubstitutionContext context)
        {
            var manager = ClassContainer.Instance.Resolve<SubstitutionManager>();
            var worker = manager.GetWorker(context);
            var res = worker.Process(Subject);
            return res;
        }

        public string Subject { set; get; }

        public string Body
        {
            get;
            set;
        }

        public bool IsValid
        {
            get;
            set;
        }

        public Guid Id
        {
            get;
            set;
        }

        public string LastModifiedBy
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
