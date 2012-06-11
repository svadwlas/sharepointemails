using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using System.Runtime.InteropServices;
namespace SharePointEmails.Core.Configuration
{
    [Serializable]
    public class FarmConfiguration
    {
        public bool Disabled { get; set; }
        
    }
}
