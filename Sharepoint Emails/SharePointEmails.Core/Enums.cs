using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePointEmails.Core
{
    public enum TemplateTypeEnum
    {
        Unknown=1, AllItemEvents=28, ItemAdded=4, ItemRemoved=8, ItemUpdated=16
    }
    public enum TemplateStateEnum
    {
        Unknown, Draft, Published
    }

    public static class EnumConverter
    {
        public static TemplateStateEnum ToState(string state)
        {
            if (string.IsNullOrEmpty(state)) return TemplateStateEnum.Unknown;
            if (state.ToLower() == TemplateCT.StateChoices.Published.ToLower())
                return TemplateStateEnum.Published;
            else if (state.ToLower() == TemplateCT.StateChoices.Draft.ToLower())
                return TemplateStateEnum.Draft;
            else
                return TemplateStateEnum.Unknown;
        }

        public static string StateToValue(TemplateStateEnum value)
        {
            if (value == TemplateStateEnum.Published)
                return TemplateCT.StateChoices.Published;
            else
                return TemplateCT.StateChoices.Draft;
        }

        public static int ToType(string[] strs)
        {
            int res = 0;
            if (strs == null) return (int)TemplateTypeEnum.Unknown;
            foreach (var s in strs)
            {
                if (s.ToLower() == TemplateCT.TypeChoices.All.ToLower())
                    res = res | (int)TemplateTypeEnum.AllItemEvents;
                else if (s.ToLower() == TemplateCT.TypeChoices.ItemAdded.ToLower())
                    res = res | (int)TemplateTypeEnum.ItemAdded;
                else if (s.ToLower() == TemplateCT.TypeChoices.ItemRemoved.ToLower())
                    res = res | (int)TemplateTypeEnum.ItemRemoved;
                else if (s.ToLower() == TemplateCT.TypeChoices.ItemUpdated.ToLower())
                    res = res | (int)TemplateTypeEnum.ItemUpdated;
            }
            if (res == 0)
                return (int)TemplateTypeEnum.Unknown;
            else
                return res;
        }

        public static int ToType(SPFieldMultiChoiceValue value)
        {
            var list = new List<string>();
            for (int i = 0; i < value.Count; i++)
            {
                list.Add(value[i]);
            }
            return ToType(list.ToArray());
        }

        public static SPFieldMultiChoiceValue TypeToValue(int value)
        {
            var res = new SPFieldMultiChoiceValue();

            if (((value & (int)TemplateTypeEnum.AllItemEvents) == (int)TemplateTypeEnum.AllItemEvents))
                res.Add(TemplateCT.TypeChoices.All);
            else
            {
                if (((value & (int)TemplateTypeEnum.ItemAdded) == (int)TemplateTypeEnum.ItemAdded))
                    res.Add(TemplateCT.TypeChoices.ItemAdded);
                if (((value & (int)TemplateTypeEnum.ItemRemoved) == (int)TemplateTypeEnum.ItemRemoved))
                    res.Add(TemplateCT.TypeChoices.ItemRemoved);
                if (((value & (int)TemplateTypeEnum.ItemUpdated) == (int)TemplateTypeEnum.ItemUpdated))
                    res.Add(TemplateCT.TypeChoices.ItemUpdated);
            }

            return res;
        }
    }
}
