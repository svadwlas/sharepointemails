using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;

namespace SharepointEmails
{
    class SeBoolFieldType : SPFieldBoolean
    {
        public SeBoolFieldType(SPFieldCollection fields, string fieldName)
            : base(fields, fieldName) { }

        public SeBoolFieldType(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName) { }

        public override Microsoft.SharePoint.WebControls.BaseFieldControl FieldRenderingControl
        {
            get
            {

                var control = (BooleanField)base.FieldRenderingControl;
                control.PreRender += OnPrerender;
                return control;
            }
        }

        string OnChecked
        {
            get
            {
                if (this.StaticName == SEMailTemplateCT.TemplateSubjectUseFile)
                    return SEMailTemplateCT.TemplateSubjectFile;
                else if (this.StaticName == SEMailTemplateCT.TemplateBodyUseFile)
                    return SEMailTemplateCT.TemplateBodyFile;
                else
                    return string.Empty;
            }
        }
        string OnUnChecked
        {
            get
            {
                if (this.StaticName == SEMailTemplateCT.TemplateSubjectUseFile)
                    return SEMailTemplateCT.TemplateSubject;
                else if (this.StaticName == SEMailTemplateCT.TemplateBodyUseFile)
                    return SEMailTemplateCT.TemplateBody;
                else
                    return string.Empty;
            }
        }

        string sufix(BaseFieldControl field)
        {
            if (field is BaseTextField)
                return "_ctl00_TextField";
            else if (field is LookupField)

                return "_Lookup";
            else
                return string.Empty;
        }

        public void OnPrerender(object sender, EventArgs e)
        {
            if (SPContext.Current.FormContext != null)
            {
                string oncheckedId = string.Empty;
                string onUncheckedId = string.Empty;
                foreach (BaseFieldControl control in SPContext.Current.FormContext.FieldControlCollection)
                {
                    if (control.FieldName == OnUnChecked) { onUncheckedId = control.ClientID + sufix(control); }
                    if (control.FieldName == OnChecked) { oncheckedId = control.ClientID + sufix(control); }
                }
                var c = (SPControl)sender;
                c.Page.ClientScript.RegisterStartupScript(GetType(), "boolChange" + this.StaticName,
                    @"InitSeBool('" + c.ClientID + "_ctl00_BooleanField" + "','" + oncheckedId + "','" + onUncheckedId + "')", true);
            }
        }
    }
}
