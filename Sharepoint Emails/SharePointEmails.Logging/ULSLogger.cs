using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
namespace SharePointEmails.Logging
{
    internal class DiagnosticService : SPDiagnosticsServiceBase
    {
        private static string DiagnosticsAreaName = "SharePointEmails";

        public DiagnosticService()
        { 
        }

        public DiagnosticService(string name, SPFarm farm)
            :base(name, farm)
        {

        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsCategory> categories = new List<SPDiagnosticsCategory>();
            foreach (string catName in Enum.GetNames(typeof(Category)))
            {
                uint catId = (uint)(int)Enum.Parse(typeof(Category), catName);
                categories.Add(new SPDiagnosticsCategory(catName, TraceSeverity.Verbose, EventSeverity.Error, 0, catId));
            }

            yield return new SPDiagnosticsArea(DiagnosticsAreaName, categories);
        }

        public static DiagnosticService Local
        {
            get
            {
                return SPDiagnosticsServiceBase.GetLocal<DiagnosticService>();
            }
        }

        public SPDiagnosticsCategory this[Category id]
        {
            get
            {
                return Areas[DiagnosticsAreaName].Categories[id.ToString()];
            }
        }
    }
}
