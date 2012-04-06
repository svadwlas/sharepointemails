using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace SharepointEmails.WebServices.SharePointEmailsWebApplication
{
    [ServiceContract]
    interface ISharePointEmailsService
    {
        [OperationContract]
        string Hello(string name);
    }
}
