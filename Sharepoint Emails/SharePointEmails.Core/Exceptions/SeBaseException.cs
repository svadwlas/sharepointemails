using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core.Exceptions
{
    public class SeBaseException:Exception
    {
        public SeBaseException(string message):base(message)
        {
        }
        public SeBaseException(string message, Exception inner):base(message,inner)
        {
        }
    }
}
