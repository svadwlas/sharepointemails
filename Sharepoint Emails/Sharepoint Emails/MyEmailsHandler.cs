using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.SharePoint;

namespace SharePointEmails
{
    class MyEmailsHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    Guid id = Guid.Empty;
                    if (TryGuid(context.Request.QueryString["ID"], out id))
                    {
                        var file = EmailStorage.GetFile(id);
                        if (file.Exists)
                        {
                            context.Response.WriteFile(file.FullName);
                        }
                        else
                        {
                            context.Response.Write("Maybe your email was deleted on server");
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                });
        }

        bool TryGuid(string str, out Guid guid)
        {
            guid = Guid.Empty;
            try
            {
                guid = new Guid(str);
                return true;
            }
            catch { }
            return false;
        }
    }


    class EmailStorage
    {
        static string FolderPath
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), "Emails");
            }
        }



        static string ProcessKey(Guid key)
        {
            return key.ToString().Replace("{", "").Replace("}", "").Replace("-", "") + ".htm";
        }

        static void CreateFile(Guid id, string html)
        {
            File.WriteAllText(Path.Combine(FolderPath, ProcessKey(id)), html);
        }

        static void CreateTempDir()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }

        static public void Add(Guid id, string html)
        {
            try
            {
                CreateFile(id, html);
            }
            catch (DirectoryNotFoundException)
            {
                CreateTempDir();
                CreateFile(id, html);
            }
        }

        static public FileInfo GetFile(Guid id)
        {
            return new FileInfo(Path.Combine(FolderPath, ProcessKey(id)));
        }
    }
}
