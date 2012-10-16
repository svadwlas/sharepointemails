using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Moles;
using Microsoft.SharePoint;
using System.IO;
using Microsoft.SharePoint.Utilities.Moles;

namespace SPMocksBuilder
{
    public class VWeb
    {
        internal MSPWeb web = new MSPWeb();

        MSPListCollection mockLists = new MSPListCollection();
        MSPWebCollection websMock = new MSPWebCollection();
        public ICollection<VWeb> Webs
        {
            set
            {
                foreach (var web in value)
                {
                    AddWeb(web);
                }
            }
        }ICollection<VWeb> _Webs = new List<VWeb>();

        MSPPropertyBag properties = new MSPPropertyBag();

        public VWeb()
        {
            properties.BehaveAsDefaultValue();
            web.ListsGet = () => mockLists;
            mockLists.GetListByIdGuidBoolean = (g, b) => _Lists.FirstOrThrow(p => p.ID == g, new MoleNotFound()).list;
            mockLists.GetListByNameStringBoolean = (g, b) => _Lists.FirstOrThrow(p => p.Title == g, new MoleNotFound()).list;
            web.TitleGet = () => Title;
            web.SiteUsersGet = () => ParentSite.usersMocks;
            web.SiteGet = () => ParentSite.site;
            web.ParentWebGet = () => (ParentWeb == null) ? null : ParentWeb.web;
            web.PropertiesGet = () => null;
            

            websMock.CountGet = () => _Webs.Count;
            websMock.ItemGetGuid = (g) => _Webs.FirstOrThrow(w => w.ID == g).web;
            websMock.ItemGetInt32 = (g) => _Webs.ToList()[g].web;
            websMock.ItemGetString = (g) => _Webs.FirstOrThrow(w => w.Title == g).web;

            mockLists.ItemGetInt32 = (i) => _Lists.ToList()[i].list;
            mockLists.ItemGetString = (i) => _Lists.FirstOrThrow(p=>p.Title==i).list;
            mockLists.ItemGetGuid= (i) => _Lists.FirstOrThrow(p => p.ID == i).list;
            mockLists.TryGetListString = (i) => _Lists.FirstOrThrow(p => p.Title == i).list;
        }

        void AddWeb(VWeb web)
        {
            if (web.ID == Guid.Empty)
            {
                web.ID = Guid.NewGuid();
            }
            if (string.IsNullOrEmpty(web.Title))
            {
                web.Title = "WebName" + web.ID.ToString().Substring(5);
            }
            web.ParentSite = this.ParentSite;
            web.ParentWeb = this;
            _Webs.Add(web);
        }

        void AddList(VList list)
        {

            if (list.ID == Guid.Empty)
            {
                list.ID = Guid.NewGuid();
            }
            if (string.IsNullOrEmpty(list.Title))
            {
                list.Title = "ListName" + list.ID.ToString().Substring(5);
            }

            list.ParentWeb = this;

            _Lists.Add(list);
        }

        public ICollection<VList> Lists
        {
            set
            {
                foreach (var list in value)
                {
                    AddList(list);
                }
            }
            get
            {
                return _Lists;
            }
        }ICollection<VList> _Lists = new List<VList>();

        internal VSite ParentSite { set; get; }
        internal VWeb ParentWeb { set; get; }

        public string Title { get; set; }
        public Guid ID { get; set; }
    }
    public class VSite
    {
        internal MSPSite site = new MSPSite();

        public SPSite Site { get { return site; } }

        

        internal MSPUserCollection usersMocks = new MSPUserCollection();

        public VSite()
        {
            ID = Guid.NewGuid();
            site.IDGet = () => ID;
            site.RootWebGet = () => _RootWeb.web;
            site.OpenWeb = () => _RootWeb.web;
            usersMocks.GetByIDInt32 = (i) => _Users.FirstOrDefault(p => p.ID == i).user;
            usersMocks.GetByIDNoThrowInt32 = (i) => _Users.FirstOrDefault(p => p.ID == i).user;
            usersMocks.GetByLoginNoThrowString= (i) => _Users.FirstOrDefault(p => p.Login == i).user;
            usersMocks.GetByEmailString= (i) => _Users.FirstOrDefault(p => p.Email == i).user;
            site.UrlGet = () => Url;
        }

        public VWeb RootWeb { set { SetRootWeb(value); } get { return _RootWeb; } }VWeb _RootWeb;

        private void SetRootWeb(VWeb value)
        {
            value.ParentSite = this;
            value.ParentWeb = null;
            _RootWeb = value;
        }

        public ICollection<VUser> Users
        {
            set
            {
                foreach(var user in value)
                {
                    AddUser(user);
                }
            }
            get
            {
                return _Users;
            }
        }

        private void AddUser(VUser user)
        {
            user.ParentSite = this;
            _Users.Add(user);
        }

        ICollection<VUser> _Users = new List<VUser>();


        public Guid ID { private set; get; }

        public string Url { set; get; }
    }

    public class VUser
    {
        internal MSPUser user=new MSPUser();

        public VUser(string login,string email=null, string name = null)
        {
            Login = login;
            FullName = name;
            Email = email;
            user.SiteIdGet = () => ParentSite.ID;
            user.NameGet = () => FullName;
            user.LoginNameGet= () => Login;
            user.EmailGet = () => Email;
            user.IDGet = () => ID;
        }

        public string Login{private set;get;}
        public string FullName { private set; get; }
        public string Email { private set; get; }

        public int ID
        {
            get
            {
                return ParentSite.Users.ToList().IndexOf(this) + 1;
            }
        }

        internal VSite ParentSite{set;get;}

    }

    public class MoleNotFound : Exception
    {
    }

    public class VList
    {
        internal MSPList list = null;

        MSPFieldCollection fieldsMocks = new MSPFieldCollection();
        MSPListItemCollection itemsMocks = new MSPListItemCollection();

        ICollection<VField> _Fields
        {
            get
            {
                var res = new List<VField>();
                foreach (var ct in _ContentTypes)
                {
                    foreach (var field in ct.Fields)
                    {
                        if (!res.Any(f => f.ID == field.ID))
                        {
                            res.Add(field);
                        }
                    }
                }
                return res;
            }
        }

        public VList()
            : this(null)
        {
        }

        public VList(VDocumentLibrary library)
        {
            if (library != null)
            {
                list = new MSPList(library.library);
            }
            else
            {
                list = new MSPList();
            }
            list.FieldsGet = () => fieldsMocks;
            list.TitleGet = () => Title;
            list.DescriptionGet = () => Description;
            list.ParentWebGet = () => ParentWeb.web;
            list.ItemCountGet = () => _Items.Count;
            list.ItemsGet = () => itemsMocks;

            list.GetItemsSPQuery = (q) => itemsMocks;
            list.GetItemsSPQueryString = (q, s) => itemsMocks;
            list.GetItemsSPView = (view) => itemsMocks;
            list.GetItemsStringArray = (a) => itemsMocks;

            list.GetItemByIdInt32 = (i) => _Items.FirstOrThrow(p => p.ID == i).item;
            list.GetItemByIdInt32Boolean = (i, b) => _Items.FirstOrThrow(p => p.ID == i).item;

            fieldsMocks.ItemGetGuid = (g) => _Fields.FirstOrThrow(f => f.ID == g).field;
            fieldsMocks.ItemGetInt32 = (g) => _Fields.ToArray()[g].field;
            fieldsMocks.ItemGetString = (g) => _Fields.FirstOrThrow(f => f.Name == g).field;

            itemsMocks.ItemGetInt32 = (i) => _Items.ToArray()[i].item;
            itemsMocks.ItemGetGuid = (g) => _Items.FirstOrThrow(p => p.UniqueId == g).item;
            itemsMocks.ListGet = () => list;
            itemsMocks.GetItemByIdInt32 = (i) => _Items.FirstOrThrow(p => p.ID == i).item;
            itemsMocks.GetEnumerator = () => _Items.Select(p => (SPListItem)p.item).ToList().GetEnumerator();

        }


        ICollection<VListItem> _Items = new List<VListItem>();
        ICollection<VContentType> _ContentTypes = new List<VContentType>();

        internal VWeb ParentWeb
        {
            set;
            get;
        }


        void AddContentType(VContentType ct)
        {
            _ContentTypes.Add(ct);
        }

        private void AddItem(VListItem item)
        {
            item.ParentList = this;
            _Items.Add(item);
        }

        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<VContentType> ContentTypes
        {
            set
            {
                foreach (var ct in value)
                {
                    AddContentType(ct);
                }
            }
            get
            {
                return _ContentTypes;
            }
        }
        public ICollection<VListItem> Items
        {
            set
            {
                foreach (var item in value)
                {
                    AddItem(item);
                }
            }

            get
            {
                return _Items;
            }
        }
        public SPList List { get { return (SPList)list; } }
    }

    public class VDocumentLibrary
    {
        internal MSPDocumentLibrary library = new MSPDocumentLibrary();
        public VDocumentLibrary()
        {
        }
    }

    public class VContentType
    {
        ICollection<VField> _Fields = new List<VField>();

        MSPFieldCollection fieldsMocks = new MSPFieldCollection();

        internal MSPContentType ct = new MSPContentType();

        public VContentType(SPContentTypeId id)
            : this(null, id)
        { }

        public VContentType(string name, SPContentTypeId id)
        {
            Id = id;
            ID = Guid.NewGuid();
            Name = string.IsNullOrEmpty(name) ? "CTName" + ID.ToString().Substring(5) : name;
            ct.FieldsGet = () => fieldsMocks;
            ct.NameGet = () => Name;
            ct.IdGet = () => Id;

            fieldsMocks.ContainsFieldString = fieldsMocks.ContainsFieldWithStaticNameString = (s) => _Fields.Any(p => p.Name == s);
            fieldsMocks.ContainsGuid = (s) => _Fields.Any(p => p.ID == s);
        }


        public SPContentTypeId Id { private set; get; }
        public Guid ID { private set; get; }
        public string Name { private set; get; }

        internal VList ParentList { set; get; }

        public ICollection<VField> Fields
        {
            set
            {
                foreach (var field in value)
                {
                    AddField(field);
                }
            }
            get
            {
                return _Fields;
            }
        }

        private void AddField(VField field)
        {
            _Fields.Add(field);
        }


        public static VContentType CreateForDiscussion()
        {
            return new VContentType("Discussion", SPBuiltInContentTypeId.Discussion)
                                        {
                                            Fields = new[]
                                            {
                                                new VField("Body",SPBuiltInFieldId.Body,SPFieldType.Text),
                                                new VField("Author",SPBuiltInFieldId.Author,SPFieldType.User),
                                                new VField("Created",SPBuiltInFieldId.Created,SPFieldType.DateTime),
                                                new VField("ThreadIndex",SPBuiltInFieldId.ThreadIndex,SPFieldType.Text)
                                            }
                                        };
        }

        public static VContentType GetForDiscussionMessage()
        {
            return new VContentType("Message", SPBuiltInContentTypeId.Message)
            {
                Fields = new[]
                                            {
                                                new VField("Body",SPBuiltInFieldId.Body,SPFieldType.Text),
                                                new VField("Author",SPBuiltInFieldId.Author,SPFieldType.User),
                                                new VField("Created",SPBuiltInFieldId.Created,SPFieldType.DateTime),
                                                new VField("ParentFolderId",SPBuiltInFieldId.ParentFolderId,SPFieldType.Integer),
                                                new VField("ThreadIndex",SPBuiltInFieldId.ThreadIndex,SPFieldType.Text)
                                            }
            };
        }

    }

    public class VField
    {
        internal MSPField field = new MSPField();

        public Guid ID { get; private set; }
        public string Name { get; private set; }

        public VField(string name, Guid? id = null, SPFieldType type = SPFieldType.Text, bool Requeried = false, bool Hidden = false)
        {
            if (id == Guid.Empty || !id.HasValue)
            {
                id = Guid.NewGuid();
            }
            ID = id.Value;
            Name = string.IsNullOrEmpty(name) ? "FieldName" + ID.ToString().Substring(5) : name;
            field.IdGet = () => ID;
            field.InternalNameGet = () => Name;
            field.TitleGet = () => Name;
            field.StaticNameGet = () => Name;
            field.TypeGet = () => type;
            field.TypeGet = () => type;
            field.RequiredGet = () => Requeried;
            field.HiddenGet = () => Hidden;
        }
    }
    public class VListItem
    {
        internal MSPListItem item = new MSPListItem();

        public VListItem()
        {
            UniqueId = Guid.NewGuid();
            item.UniqueIdGet = () => UniqueId;
            item.IDGet = () => ParentList.Items.ToList().IndexOf(this) + 1;
            item.ParentListGet = () => ParentList.list;
            item.FolderGet = () => (Folder == null) ? null : Folder.folder;
            item.FileGet = () => (File == null) ? null : File.file;
            item.WebGet = () => ParentList.ParentWeb.web;
            item.FieldsGet = () => ContentType.Fields.Mock();
            item.ContentTypeGet = () => ContentType.ct;
            item.ContentTypeIdGet = () => ContentType.Id;
            item.ItemGetGuid = (g) =>
                {
                    if (_Values.Any(p => p.Key == g))
                    {
                        return _Values.FirstOrThrow(p => p.Key == g).Value;
                    }
                    else
                    {
                        return _ValuesByName.FirstOrThrow(p => p.Key == ContentType.Fields.FirstOrThrow(f => f.ID == g).Name).Value;
                    }
                };
            item.ItemGetInt32 = (g) => _Values.ToArray()[g].Value;
            item.ItemGetString = item.GetFieldValueString = (g) =>
                {
                    if (_ValuesByName.Any(p => p.Key == g))
                    {
                        return _ValuesByName.FirstOrThrow(p => p.Key == g).Value;
                    }
                    else
                    {
                        return _Values.FirstOrThrow(p => p.Key == ContentType.Fields.FirstOrThrow(f => f.Name == g).ID).Value;
                    }
                };

            item.Update = () => { };
            item.ItemSetStringObject = (s, v) =>
            {
                if (ContentType.Fields.Any(p => p.Name == s))
                {
                    AddValue(new KeyValuePair<Guid, object>(ContentType.Fields.First(p => p.Name == s).ID, v));
                }
                else
                {
                    throw new MoleNotFound();
                }
            };

            item.ItemSetGuidObject = (g, v) =>
            {
                if (ContentType.Fields.Any(p => p.ID == g))
                {
                    AddValue(new KeyValuePair<Guid, object>(g, v));
                }
                else
                {
                    throw new MoleNotFound();
                }
            };

            item.AttachmentsGet = () => null;
        }

        public VList ParentList
        {
            internal set;
            get;
        }


        public Guid UniqueId { get; private set; }

        public Dictionary<Guid, object> Values
        {
            set
            {
                foreach (var p in value)
                {
                    AddValue(p);
                }
            }
            get
            {
                return _Values;
            }
        }Dictionary<Guid, object> _Values = new Dictionary<Guid, object>();

        public Dictionary<string, object> ValuesByName
        {
            set
            {
                foreach (var p in value)
                {
                    AddValueByName(p);
                }
            }
            get
            {
                return _ValuesByName;
            }
        }Dictionary<string, object> _ValuesByName = new Dictionary<string, object>();

        private void AddValueByName(KeyValuePair<string, object> p)
        {
            _ValuesByName[p.Key] = p.Value;
        }

        private void AddValue(KeyValuePair<Guid, object> p)
        {
            _Values[p.Key] = p.Value;
        }


        public VContentType ContentType
        {
            get
            {
                if (!ContentTypeId.HasValue)
                {
                    if (!string.IsNullOrEmpty(ContentTypeName))
                    {
                        return ParentList.ContentTypes.FirstOrThrow(c => c.Name == ContentTypeName);
                    }
                    else
                    {
                        return ParentList.ContentTypes.FirstOrThrow(c => true);
                    }
                }
                else
                {
                    return ParentList.ContentTypes.FirstOrThrow(c => c.Id.CompareTo(ContentTypeId.Value) == 0);
                }
            }
        }

        /// <summary>
        /// ContentType of item. Should be presented in the List. If not specified, then first contentType of List. Less prioritized then ContentTypeId
        /// </summary>
        public string ContentTypeName { set; get; }

        /// <summary>
        /// Content type Id of item. It is more prioritized then ContentTypeName
        /// </summary>
        public SPContentTypeId? ContentTypeId{ set; get; }

        public int ID { get { return ParentList.Items.ToList().IndexOf(this) + 1; } }

        public VFolder Folder { set; get; }

        public VFile File { set; get; }

        public SPListItem Item { get { return (SPListItem)item; } }
    }

    public class VFolder
    {
        internal MSPFolder folder = new MSPFolder();
        public VFolder()
        {
        }
    }

    public class VFile
    {
        internal MSPFile file = new MSPFile();
        public VFile(byte[] bytes, string name)
        {
            Name = name;
            Content = bytes;
            file.NameGet = () => Name;
            file.OpenBinaryStream = () => new MemoryStream(Content);
        }

        public string Name { private set; get; }

        public byte[] Content { get; private set; }

    }

    static class Ext
    {
        public static C FirstOrThrow<C>(this IEnumerable<C> col, Func<C, bool> pr, Exception e = null)
        {
            var t = col.FirstOrDefault(pr);
            if (t == null)
            {
                throw e ?? new MoleNotFound();
            }
            return t;
        }

        public static MSPFieldCollection Mock(this ICollection<VField> fields)
        {
            var mock = new MSPFieldCollection();
            mock.GetFieldByDisplayNameStringBoolean = (s, b) => fields.FirstOrThrow(f => f.Name == s).field;
            mock.GetFieldByIdGuidBoolean = (s, b) => fields.FirstOrThrow(f => f.ID == s).field;
            mock.GetFieldByInternalNameString = (s) => fields.FirstOrThrow(f => f.Name == s).field;
            mock.GetFieldByInternalNameStringBoolean = (s, b) => fields.FirstOrThrow(f => f.Name == s).field;
            mock.GetFieldString = (s) => fields.FirstOrThrow(f => f.Name == s).field;
            mock.GetFieldStringBoolean = (s, b) => fields.FirstOrThrow(f => f.Name == s).field;
            mock.ContainsFieldString = (s) => fields.Any(p => p.Name == s);
            mock.ContainsFieldWithStaticNameString = (s) => fields.Any(p => p.Name == s);
            mock.ContainsGuid = (s) => fields.Any(p => p.ID == s);
            mock.CountGet = () => fields.Count;
            return mock;
        }
    }

}
