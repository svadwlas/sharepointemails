using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharePointEmails.Logging;
using SharePointEmails.Core.Exceptions;

namespace SharePointEmails.Core
{
    public class DefaultTemplatesManager : ITemplatesManager
    {
        ILogger Logger { set; get; }

        public DefaultTemplatesManager(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            Logger = logger;
        }

        public Exception LastError
        {
            get;
            set;
        }

        private SPList EnsureList(SPWeb web)
        {
            SPList list = null;
            if (web != null)
            {
                list = web.Lists.TryGetList(Constants.TemplateListName);
                if (list == null)
                {
                    Logger.Write("Try to get siteManager",SeverityEnum.Trace);
                    var siteManager = ClassContainer.Instance.Resolve<ISiteManager>();
                    if (siteManager != null)
                    {
                        list = siteManager.CreateHiddenTemplatesList(web);
                    }
                    else
                    {
                        Logger.Write("Site manager is null", SeverityEnum.CriticalError);
                        throw new SeBaseException("No Site Manager");
                    }
                }
            }
            if (list != null)
                return list;
            else
                throw new SeBaseException("Cannot get list");

        }


        SPList GetTemplatesList(Guid siteId)
        {
            Logger.Write("try to get template list",SeverityEnum.Trace,AreasEnum.Default);
            if (siteId == Guid.Empty)
            {
                Logger.Write("Site Id is Empty",SeverityEnum.Error);
                return null;
            }
            else
            {
                
                try
                {
                    using (var site = new SPSite(siteId))
                    {
                        using (var web = site.RootWeb)
                        {
                            var list = EnsureList(web);
                            if (list != null)
                            {
                                return list;
                            }
                            else
                            {
                                throw new Exception("list is null");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex, SeverityEnum.CriticalError);
                    throw new SeBaseException(ex.Message,ex);
                }
            }
        }

        public ITemplate GetTemplate(ITemplateOwner owner, TemplateGettingEnum settings)
        {
            SPList list = GetTemplatesList(OwnersManager.Instanse.GetSiteId(owner));
            if (list != null)
            {
                SPListItemCollection items;

                switch (settings)
                {
                    case TemplateGettingEnum.Default:
                        {
                            var query = GetQueryList(owner);
                            items = list.GetItems(query);
                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException("settings");
                        }
                }
                if (items != null)
                {
                    if (items.Count == 0)
                        return null;
                    else
                    {
                        if (settings == TemplateGettingEnum.ExceptionIfMoreThenOne)
                            return TemplateFromItem(items[0], owner);
                        else
                            throw new SeBaseException("More then one template");
                    }
                }
                else
                {
                    return null;
                }
                
            }
            else
            {
                return null;
            }
        }



        public void AddTemplate(ITemplate template, ITemplateOwner owner)
        {
            try
            {
                var list = GetTemplatesList(OwnersManager.Instanse.GetSiteId(owner));
                if (list != null)
                {
                    var item = list.AddItem();
                    item["Title"] = string.IsNullOrEmpty(template.Name) ? Guid.NewGuid().ToString() : template.Name;
                    item.Update();
                }
                else
                {
                    throw new SeBaseException("List is null");
                }
            }
            catch (Exception ex)
            {
                throw new SeBaseException(ex.Message, ex);
            }
        }

        public List<ITemplate> GetAllTemplates(ITemplateOwner owner)
        {
            var res = new List<ITemplate>();
              SPList list = GetTemplatesList(OwnersManager.Instanse.GetSiteId(owner));
              if (list != null)
              {
                  foreach (SPListItem item in list.Items)
                  {
                      try
                      {
                          res.Add(TemplateFromItem(item,owner));
                      }
                      catch (Exception ex)
                      {
                          Logger.Write("Cannot parse template " + item.ID + ex.Message, SeverityEnum.CriticalError);
                      }
                  }
                  
              }
              return res;
        }

        ITemplate TemplateFromItem(SPListItem item,ITemplateOwner owner)
        {
            return new Template(item);
        }

        #region Queries

        SPQuery GetQueryList(ITemplateOwner owner)
        {
            var query = new SPQuery();

            return query;
        }

        #endregion Queries
    }
}
