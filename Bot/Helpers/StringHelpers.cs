using System;

namespace RatingsBot.Helpers
{
    public static class StringHelpers
    {
        public static bool StartsWithCI(this string s, string value)
        {
            return !string.IsNullOrEmpty(value) && s.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}