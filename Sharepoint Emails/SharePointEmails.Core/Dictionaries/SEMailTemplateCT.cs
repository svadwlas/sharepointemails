using System;
public static class SEMailTemplateCT
{
    public const string CTId = "0x01006230043d1f824022a80ad3e43a7b6da4";
    public const string FeatureId = "50e13f0b-69b1-43cf-8627-d782e7efa4cc";
    public const string TemplateName = "TemplateName";
    public const string TemplateBody = "TemplateBody";
    public const string TemplateType = "TemplateType";
    public const string TemplateState = "TemplateState";

    public static class StateChoices
    {
        public const string Draft = "Draft";
        public const string Published = "Published";
    }

    public static class TypeChoices
    {
        public const string ItemRemoved = "Item added";
        public const string ItemAdded = "Item removed";
        public const string ItemUpdated = "Item updated";
        public const string All = "All";
    }
}