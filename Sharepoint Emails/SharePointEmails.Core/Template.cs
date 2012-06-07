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
            this.Pattern = "No body";
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
            this.Pattern = m_Item[SEMailTemplateCT.TemplateBody] as string;
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
        }

        public void SaveTo(SPListItem item)
        {
            m_Item = item;
            Update();
        }

        public void Update()
        {
            m_Item[SEMailTemplateCT.TemplateName] = this.Name;
            m_Item[SEMailTemplateCT.TemplateBody] = this.Pattern;
            m_Item[SEMailTemplateCT.TemplateType] = EnumConverter.TypeToValue(this.EventTypes);
            m_Item[SEMailTemplateCT.TemplateState] = EnumConverter.StateToValue(this.State);
            m_Item.Update();
            Refresh();
        }

        public string GetProcessedText(ISubstitutionContext context)
        {
            var manager = ClassContainer.Instance.Resolve<SubstitutionManager>();
            if (manager != null)
            {
                var worker = manager.GetWorker(context);
                if (worker != null)
                {
                    var res = worker.Process(Pattern);
                    return res;
                }
                else
                {
                    return "no worker";
                }
            }
            else
            {
                return "no manager";
            }
        }

        public string Pattern
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

    
        public AssociationConfiguration Config
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
    }
}
