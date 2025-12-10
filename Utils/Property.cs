namespace EinheitsKiste
{
    public static class Property
    {
        public static string GetBackingFieldName(string propertyName) => $"<{propertyName}>k__BackingField";
    }
}
