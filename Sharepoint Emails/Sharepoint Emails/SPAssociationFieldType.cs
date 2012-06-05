using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

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
    }
}