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
             if ((this.Required == true) && (value.ToString() == ""))
             {
                 throw new SPFieldValidationException(this.Title
                     + " must have a value.");
             }
             try
             {
                 var shouldbevalidated = true;
                 if (SPContext.Current.FormContext != null)
                 {
                     foreach (BaseFieldControl field in SPContext.Current.FormContext.FieldControlCollection)
                     {
                         if (this.StaticName == SEMailTemplateCT.TemplateBody)
                         {
                             if (field.FieldName == SEMailTemplateCT.TemplateBodyUseFile && (bool)field.Value)
                             {
                                 shouldbevalidated = false;
                             }
                         }
                         else if (this.StaticName == SEMailTemplateCT.TemplateSubject)
                         {
                             if (field.FieldName == SEMailTemplateCT.TemplateSubjectUseFile && (bool)field.Value)
                             {
                                 shouldbevalidated = false;
                             }
                         }
                     }
                 }
                 else
                 {
                     shouldbevalidated = false;
                 }
                 if (shouldbevalidated)
                 {
                     string val = value.ToString();
                     var content = SPContext.Current.ListItem.GetAttachmentContent(val);
                     if (content != null)
                     {
                         val = content;
                     }
                     var compiler = new XslCompiledTransform(true);

                     using (var xsltReader = XmlReader.Create(new StringReader(val.ToString())))
                     {
                         compiler.Load(xsltReader);
                     }
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
             return base.GetValidatedString(value);
         }
    }
}
