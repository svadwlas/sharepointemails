using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Core;
using Microsoft.SharePoint.Utilities;

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

        public override object GetFieldValue(string value)
        {
            return base.GetFieldValue(value);
            
        }
    }
}