using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core;
using Microsoft.SharePoint.Utilities;
using System.Runtime.InteropServices;

namespace SharepointEmails
{
    public class SPAssociationFieldType : SPFieldMultiLineText
    {

        public SPAssociationFieldType(SPFieldCollection fields, string fieldName)
            : base(fields, fieldName)
        { }

        public SPAssociationFieldType(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
        }

        public override Microsoft.SharePoint.WebControls.BaseFieldControl FieldRenderingControl
        {
            get
            {
                return new SPAssociationControl() { FieldName = this.InternalName };
            }
        }
        public override string GetFieldValueAsHtml(object value)
        {
            try
            {
                if (value != null)
                {
                    var res = SPHttpUtility.ConvertSimpleHtmlToText(value.ToString(), -1);
                    return AssociationConfiguration.ParseOrDefault(res).Count+" asses" ;
                }
                return "";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}