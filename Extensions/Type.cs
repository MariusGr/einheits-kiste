using System;

namespace EinheitsKiste
{
    public static class Types
    {
        public static string GetNameWithNamespace(this Type type) => $"{type.Namespace}.{type.Name}";
    }
}
