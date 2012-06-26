using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePointEmails.Core.Substitutions
{
    public class Modifier
    {
        public readonly static Modifier Old = new Modifier(":O", "Old value");
        public readonly static Modifier New = new Modifier(":N", "New value");
        public readonly static ModifiersCollection AllModifiers = new ModifiersCollection()
        {
            Old,New
        };

        public Modifier(string pattern,string description)
        {
            Pattern=pattern;
            Description=description;
        }
    
        public string Pattern { get; set; }
        public string Description { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is Modifier)
            {
                return Pattern == ((Modifier)obj).Pattern;
            }
            return false;
        }
    }

    public class ModifiersCollection:List<Modifier>
    {
        public static ModifiersCollection Empty { get { return new ModifiersCollection(); } }
        public static ModifiersCollection Parse(string modifiers)
        {
            var res = new ModifiersCollection();
            if (string.IsNullOrEmpty(modifiers)) return res;
            foreach (var s in modifiers.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var m = Modifier.AllModifiers.Where(p => p.Pattern != null && p.Pattern.Trim(':') == s).FirstOrDefault();
                if (m != null && !res.Contains(m)) res.Add(m);
            }
            return res;
        }
    }
}
