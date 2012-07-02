using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using SharePointEmails.Core;
using Microsoft.SharePoint.WebControls;
namespace SharepointEmails
{
    public class SPXsltField : SPFieldMultiLineText
    {
         public SPXsltField(SPFieldCollection fields, string fieldName)
            : base(fields, fieldName){ }

         public SPXsltField(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName){}

         public override string GetValidatedString(object value)
         {
             try
             {
                 var shouldbevalidated = false;
                 if (this.StaticName == TemplateCT.TemplateBody)
                 {
                     shouldbevalidated = this.ShoulBeValidated(TemplateCT.TemplateBodyUseFile);
                 }
                 else if (this.StaticName == TemplateCT.TemplateSubject)
                 {
                     shouldbevalidated = this.ShoulBeValidated(TemplateCT.TemplateSubjectUseFile);
                 }
                 if (shouldbevalidated)
                 {
                     if ((this.Required == true) && (value.ToString() == ""))
                     {
                         throw new SPFieldValidationException(this.Title
                             + " must have a value.");
                     }
                     
                     string val = (value??"").ToString();
                     if (!string.IsNullOrEmpty(val))
                     {
                         var content = SPContext.Current.ListItem.GetAttachmentContent(val);
                         if (content != null)
                         {
                             val = content;
                         }
                         if (val.IsXslt())
                         {
                             val.ValidateXslt();
                         }
                     }
                     return base.GetValidatedString(value);
                 }
             }
             catch (XsltCompileException ex)
             {
                 throw new SPFieldValidationException(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
             }
             catch (XsltException ex)
             {
                 throw new SPFieldValidationException(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
             }
             catch (Exception ex)
             {
                 throw new SPFieldValidationException(ex.Message);
             }
             return null;
         }
    }
}
