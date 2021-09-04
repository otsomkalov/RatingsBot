using System;

namespace RatingsBot.Helpers
{
    public static class StringHelpers
    {
        public static bool EqualsCI(this string s1, string s2)
        {
            return s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool StartsWithCI(this string s, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return s.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}