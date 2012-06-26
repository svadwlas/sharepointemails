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
                 if (this.StaticName == SEMailTemplateCT.TemplateBody)
                 {
                     shouldbevalidated = this.ShoulBeValidated(SEMailTemplateCT.TemplateBodyUseFile);
                 }
                 else if (this.StaticName == SEMailTemplateCT.TemplateSubject)
                 {
                     shouldbevalidated = this.ShoulBeValidated(SEMailTemplateCT.TemplateSubjectUseFile);
                 }
                 if (shouldbevalidated)
                 {
                     if ((this.Required == true) && (value.ToString() == ""))
                     {
                         throw new SPFieldValidationException(this.Title
                             + " must have a value.");
                     }
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
