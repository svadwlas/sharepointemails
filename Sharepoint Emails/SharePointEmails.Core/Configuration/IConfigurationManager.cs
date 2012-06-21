using System;
namespace SharePointEmails.Core.Configuration
{
    public interface IConfigurationManager
    {
        WebConfiguration GetConfig(Microsoft.SharePoint.SPWeb web);
        FarmConfiguration GetConfigOrdefault(Microsoft.SharePoint.Administration.SPFarm farm);
        WebConfiguration GetConfigOrdefault(Microsoft.SharePoint.SPWeb web);
        void SetConfig(FarmConfiguration config, Microsoft.SharePoint.Administration.SPFarm farm);
        void SetConfig(WebConfiguration config, Microsoft.SharePoint.SPWeb web);
    }
}
