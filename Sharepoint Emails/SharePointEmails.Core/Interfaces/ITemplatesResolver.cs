﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core.Interfaces
{
    public interface ITemplatesManager
    {
        ITemplate GetTemplate(ISearchContext owner);
    }
}
