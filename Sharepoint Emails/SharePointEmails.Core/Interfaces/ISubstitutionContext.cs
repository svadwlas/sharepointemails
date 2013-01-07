using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePointEmails.Core.Substitutions;
using System.Globalization;
using System.IO;

namespace SharePointEmails.Core.Interfaces
{
    /// <summary>
    /// context for processing templates
    /// </summary>
    public interface ISubstitutionContext
    {
        /// <summary>
        /// Get values of fields at event moment
        /// </summary>
        List<FieldChange> FieldsValues { get; }

        /// <summary>
        /// get current value for items field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        string GetCurrentFieldValue(string fieldName, ModifiersCollection modifiers);

        /// <summary>
        /// get value of context object or its property
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        string GetContextValue(string value, ModifiersCollection modifiers = null);

        /// <summary>
        /// Get culture for destination user
        /// </summary>
        /// <returns></returns>
        CultureInfo GetDestinationCulture();

        /// <summary>
        /// Get context xml
        /// </summary>
        /// <returns></returns>
        string GetXML();

        /// <summary>
        /// Get template file from storage
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Stream GetTemplateFile(string fileName);
    }
}
