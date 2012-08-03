using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.WebControls;
using System.Collections;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace SharepointEmails
{
    class FieldsSwitches : List<FieldSwitch>
    {
        const string PROP_NAME = "FieldsToHide";

        public static FieldsSwitches Create(ArrayList controls)
        {
            if (controls == null) return null;
            var res = new FieldsSwitches();
            var ids = controls.GetClientIds();
            foreach (var c in controls.OfType<BaseFieldControl>())
            {
                if (c.Field != null)
                {
                    var s = c.Field.GetCustomProperty(PROP_NAME) as string;
                    if (string.IsNullOrEmpty(s))
                    {
                        var prop = XDocument.Parse(c.Field.SchemaXml).Descendants("Property").Where(p => p.Element("Name") != null && p.Element("Name").Value == PROP_NAME).FirstOrDefault();
                        if (prop != null)
                        {
                            var val = prop.Elements("Value").FirstOrDefault();
                            if (val != null)
                            {
                                s = val.Value;
                            }
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(s))
                    {
                        var sw = FieldSwitch.Parse(s, c.FieldName, ids);
                        if (sw != null)
                        {
                            res.Add(sw);
                        }
                    }
                }
            }
            return res;
        }

        public string ToJson()
        {
            var sb = new StringBuilder();
            var s = new JavaScriptSerializer();
            s.Serialize(this, sb);

            return sb.ToString();
        }
    }

    class FieldSwitch
    {
        public Field field { set; get; }
        public List<Switch> switches { set; get; }
        public static FieldSwitch Parse(string toparse, string fieldName, Dictionary<string, string> controlIds)
        {
            if (!controlIds.ContainsKey(fieldName)) return null;
            var res = new FieldSwitch()
            {
                switches = new List<Switch>(),
                field = new Field
                {
                    fieldId = controlIds[fieldName],
                    fieldName = fieldName
                }
            };

            if (!string.IsNullOrEmpty(toparse))
            {
                foreach (Match byVal in Regex.Matches(toparse, "(.+?):([^;]+);*"))
                {
                    var val = byVal.Groups[1].Value.Trim();
                    var sw = res.switches.Where(p => p.value == val).SingleOrDefault() ?? new Switch();
                    sw.value = val;
                    if (sw.fields == null) sw.fields = new List<Field>();
                    foreach (string field in byVal.Groups[2].Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (controlIds.ContainsKey(field))
                        {
                            if (!sw.fields.Any(p => p.fieldName == field))
                            {

                                sw.fields.Add(new Field { fieldName = field, fieldId = controlIds[field] });
                            }
                        }
                    }

                    if (!res.switches.Contains(sw) && sw.fields.Count > 0)
                    {
                        res.switches.Add(sw);
                    }
                }
            }
            return res.switches.Count == 0 ? null : res;
        }
    }

    class Switch
    {
        public string value { set; get; }
        public List<Field> fields;
    }
    class Field
    {
        public string fieldName { set; get; }
        public string fieldId { set; get; }
    }

    public static class FieldControlsExt
    {
        static string sufix(BaseFieldControl field)
        {
            if (field is BaseTextField)
                return "_ctl00_TextField";
            else if (field is LookupField)

                return "_Lookup";
            else if (field is BooleanField)
            {
                return "_ctl00_BooleanField";
            }
            else
                return string.Empty;
        }
        public static Dictionary<string, string> GetClientIds(this ArrayList controls)
        {
            var res = new Dictionary<string, string>();
            foreach (var control in controls.OfType<BaseFieldControl>())
            {
                res.Add(control.FieldName, control.ClientID + sufix(control));
            }
            return res;
        }
    }
}
