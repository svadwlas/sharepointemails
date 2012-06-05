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
            this.Type = (int)TemplateTypeEnum.Unknown;
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
                this.Type = EnumConverter.ToType(val);
            }
            else
            {
                this.Type = (int)TemplateTypeEnum.All;
            }
            this.State = EnumConverter.ToState(m_Item[SEMailTemplateCT.TemplateState] as string);
        }

        public void SaveTo(SPListItem item)
        {
            m_Item = item;
            Update();
        }

        public ISubstitutionContext Context { set; get; }

        public void Update()
        {
            m_Item[SEMailTemplateCT.TemplateName] = this.Name;
            m_Item[SEMailTemplateCT.TemplateBody] = this.Pattern;
            m_Item[SEMailTemplateCT.TemplateType] = EnumConverter.TypeToValue(this.Type);
            m_Item[SEMailTemplateCT.TemplateState] = EnumConverter.StateToValue(this.State);
            m_Item.Update();
            Refresh();
        }

        public string ProcessedText
        {
            get
            {
                var manager = ClassContainer.Instance.Resolve<SubstitutionManager>();
                if (manager != null)
                {
                    var worker = manager.GetWorker(Context);
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

        public ISearchContext Owner
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

        public int Type
        {
            get;
            set;
        }

        public int Property
        {
            get;
            set;
        }
    }
}
