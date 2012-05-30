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
        public Template(SPListItem item) 
        {
            m_Item = item;
        }

        public string ProcessedText
        {
            get
            {
                return string.Empty;
            }
            set
            {
                
            }
        }

        public string Pattern
        {
            get
            {
                return string.Empty;
            }
            set
            {
                
            }
        }

        public bool IsValid
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        public Guid Id
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }


        public ITemplateOwner Owner
        {
            get
            {
                if (m_owner == null)
                {
                 
                }
                return m_owner;
            }
            set
            {

                m_owner = value;
            }
        }ITemplateOwner m_owner;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_Name))
                {
                    if (m_Item != null)
                    {
                        m_Name = m_Item.Title;
                    }
                    else
                    {
                        m_Name = "Noname";
                    }
                }
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }string m_Name;

        public void Update()
        {
            m_Item.Update();
        }
    }
}
