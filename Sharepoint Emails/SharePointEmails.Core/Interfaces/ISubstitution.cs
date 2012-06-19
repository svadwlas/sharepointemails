﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core
{
    public interface ISubstitutionWorker
    {
        string Process(string data);
    }

    public interface ISubstitution
    {
        string Pattern
        {
            get;
        }

        string Description
        {
            get;
        }

        string Process(string text, ISubstitutionContext context);

        List<string> GetAvailableKeys(ISubstitutionContext context);
    }
}
