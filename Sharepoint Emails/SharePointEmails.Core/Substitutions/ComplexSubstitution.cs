﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharePointEmails.Logging;
using SharePointEmails.Core.Interfaces;

namespace SharePointEmails.Core.Substitutions
{
    class ComplexSubstitution:BaseSubstitution
    {
        public override string Process(string text, ISubstitutionContext context)
        {
            var res=text;
            foreach (Match m in Regex.Matches(res, @"\{\$(.+)\}"))
            {
                try
                {
                    string value = null;
                    switch (m.Groups[1].Value.ToLower())
                    {
                        case "changes": value = GetGhanges(context); break;
                        default: value = "no " + m.Value; break;
                    }
                    res = res.Replace(m.Value, value);
                }
                catch (Exception ex)
                {
                    Logger.WriteTrace(ex, SeverityEnum.Error);
                }
            }
            return res;
        }

        private string GetGhanges(ISubstitutionContext context)
        {
            var sb = new StringBuilder();
            List<FieldChange> toDisplay = context.FieldsValues.Where(p => p.IsChanged).ToList();
            sb.Append("<table class='changes-table'>");
            if (toDisplay.Count > 0)
            {
                sb.Append(string.Format("<tr><th>Field Name</th><th>Field Old Value</th><th>Field New Value</th></tr>"));
                foreach (var item in toDisplay)
                {
                    sb.Append(string.Format(@"
                    <tr>
                        <td>{0}</td>
                        <td>{1}</td>
                        <td>{2}</td>
                    </tr>", item.FieldDisplayName, item.GetText(new ModifiersCollection { Modifier.Old }), item.GetText(new ModifiersCollection { Modifier.New })));
                }
            }
            else
            {
                sb.Append(string.Format("<tr><td>No data to display</td></tr>"));
            }
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}
