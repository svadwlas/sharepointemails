using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Xsl;
using System.Xml;
using System.IO;

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
                 var compiler = new XslCompiledTransform(true);

                 using (var xsltReader = XmlReader.Create(new StringReader(value.ToString())))
                 {
                     compiler.Load(xsltReader);
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
