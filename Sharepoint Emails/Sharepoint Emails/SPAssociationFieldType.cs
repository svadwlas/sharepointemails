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
            : base(fields, fieldName){ }

        public SPAssociationFieldType(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName){}

        public override Microsoft.SharePoint.WebControls.BaseFieldControl FieldRenderingControl
        {
            get
            {
                return new SPAssociationControl()
                {
                    FieldName = this.InternalName
                };
            }
        }

        public override string GetFieldValueAsHtml(object value)
        {
            try
            {
                var config=AssociationConfiguration.ParseOrDefault(SPHttpUtility.ConvertSimpleHtmlToText(value.ToString(), -1));
                int count = config.Count;
                if (count > 0)
                {
                    var sb = new StringBuilder();
                    sb.Append("<table>");
                    foreach (var a in config)
                    {
                        sb.Append(string.Format("<tr><td>{0}) </td><td>{1}</td><td>{2}</td></tr>",config.IndexOf(a)+1,a.Name,a.Type));
                    }
                    sb.Append("</table>");
                    return sb.ToString();
                }
                else
                {
                    return "no associations";
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}