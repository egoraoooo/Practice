namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        var filterchar = input.ToLower().Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c)).ToArray();

        if (filterchar.Length == 0)
        {
            return false;
        }

        return filterchar.SequenceEqual(filterchar.Reverse());
    }
}