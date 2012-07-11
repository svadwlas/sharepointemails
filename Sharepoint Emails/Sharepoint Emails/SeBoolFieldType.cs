using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using System.Text.RegularExpressions;
using System.Collections;

namespace SharepointEmails
{
    class SeBoolFieldType : SPFieldBoolean
    {
        public SeBoolFieldType(SPFieldCollection fields, string fieldName)
            : base(fields, fieldName) { GetFieldsHoHide(); }

        public SeBoolFieldType(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName) { GetFieldsHoHide(); }

        public override Microsoft.SharePoint.WebControls.BaseFieldControl FieldRenderingControl
        {
            get
            {
                var control = (BaseFieldControl)base.FieldRenderingControl;
                control.PreRender += OnPrerender;
                return control;
            }
        }

        void GetFieldsHoHide()
        {
           
            var str = GetCustomProperty("FieldsToHide") as string;
            if (!string.IsNullOrEmpty(str))
            {
                foreach (Match byVal in Regex.Matches(str, "(.+?):(.+?);"))
                {
                    var val=byVal.Groups[1].Value;
                    if (!fieldsToHide.ContainsKey(val)) fieldsToHide[val] = new List<string>();
                    foreach (string field in byVal.Groups[2].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!fieldsToHide[val].Contains(field))
                        {
                            fieldsToHide[val].Add(field);
                        }
                    }
                }
            }
        }

        string sufix(BaseFieldControl field)
        {
            if (field is BaseTextField)
                return "_ctl00_TextField";
            else if (field is LookupField)

                return "_Lookup";
            else if (field is BooleanField)
            {
                return "_ctl00_BooleanField";
            }
            else
                return string.Empty;
        }

        Dictionary<string, List<string>> fieldsToHide = new Dictionary<string, List<string>>();

        string GetForScript(ArrayList controls)
        {
            var res = string.Empty;

            foreach (var p in fieldsToHide)
            {
                if (p.Value.Count > 0)
                {
                    res += p.Key + ":";
                    foreach (BaseFieldControl c in controls)
                    {
                        foreach (var f in p.Value)
                        {
                            if (c.FieldName == f)
                            {
                                res += c.ClientID + sufix(c) + ",";
                            }
                        }
                    }
                    res.Trim(',');
                    res += ";";
                }
            }
            return res;
        }

        public void OnPrerender(object sender, EventArgs e)
        {

            if (SPContext.Current.FormContext != null)
            {
                var c = (BaseFieldControl)sender;
                c.Page.ClientScript.RegisterStartupScript(GetType(), "ChangeField" + this.StaticName,
                    @"InitSeBool('" + c.ClientID + sufix(c) + "','" + GetForScript(SPContext.Current.FormContext.FieldControlCollection) + "')", true);
            }
        }
    }
}
