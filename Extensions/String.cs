using System.Text.RegularExpressions;

namespace EinheitsKiste
{
    public static class Strings
    {
        public static string[] SplitPascalCase(this string input) => Regex.Split(input, @"(?<!^)(?=[A-Z])");
    }
}
