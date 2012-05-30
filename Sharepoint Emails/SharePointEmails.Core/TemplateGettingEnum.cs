using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core
{
    public enum TemplateGettingEnum
    {
        Default,
        ExceptionIfMoreThenOne,
        /// <summary>
        /// From item
        /// </summary>
        FirstFromEnd,
        /// <summary>
        /// from site
        /// </summary>
        FirstFromStart,
    }
}
