using System.Text.RegularExpressions;

namespace IOGameServer.Helpers;

public static partial class UsernameValidator
{
    public static async Task<string> Validate(string username, int stringLengthLimit = 16)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return await GetRandomWord();
        }

        username = LettersAndNumbersRegex().Replace(username, "");

        stringLengthLimit = username.Length <= stringLengthLimit
            ? username.Length
            : stringLengthLimit;

        return username[..stringLengthLimit].Trim();
    }

    private static async Task<string> GetRandomWord()
    {
        var words = await File.ReadAllLinesAsync(@"./Resources/words.txt");

        return words[Random.Shared.Next(words.Length - 1)];
    }

    [GeneratedRegex("[^a-zA-Z0-9\\s-?!&@]")]
    private static partial Regex LettersAndNumbersRegex();
}
