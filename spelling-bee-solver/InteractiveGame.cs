namespace spelling_bee_solver;

internal class InteractiveGame
{
    // TODO: specific prompt for pangram

    public void PlayGame()
    {
        var finder = new WordFinder();

        Console.WriteLine("Welcome to spelling-bee-solver. This program will help you come");
        Console.WriteLine("up with answers for the daily NY Times Spelling Bee puzzle.");

        var center = PromptCenterLetter();
        var letters = PromptLetters(center);

        var matchingWords = finder.FindWords(center, letters);
        matchingWords.Sort();
        Console.WriteLine($"Thank you. {matchingWords.Count} words have been found.");

        while (true)
        {
            var wordLen = PromptWordLength(matchingWords);
            var startingLetters = PromptStartingLetters();
            OutputMatchingWords(matchingWords, wordLen, startingLetters);
        }
    }

    private static char PromptCenterLetter()
    {
        while (true)
        {
            Console.Write("Which is the center letter? ");
            var entry = Console.ReadLine()?.Trim();
            if (entry == null || entry.Length != 1 || !Char.IsLetter(entry[0]))
                Console.WriteLine("Please enter just the single letter in the center.");
            else
                return Char.ToLower(entry[0]);
        }
    }

    private static HashSet<char> PromptLetters(char center)
    {
        while (true)
        {
            Console.Write("Enter the other six letters (no spaces): ");
            var entry = Console.ReadLine();
            if (entry != null)
            {
                entry = entry.Trim();
                if (entry.Contains(center))
                {
                    Console.WriteLine($"You do not need to include the center letter ({center}).");
                    continue;
                }
                if (entry.Length < 6)
                {
                    Console.WriteLine($"Please enter all six letters.");
                    continue;
                }
                else
                {
                    var result = new HashSet<char>();
                    foreach (char c in entry)
                        result.Add(Char.ToLower(c));
                    if (result.Count == 6)
                        return result;
                    Console.WriteLine("Six unique letters not found");
                }
            }
        }
    }

    private static int? PromptWordLength(List<String> words)
    {
        var min = words.Min(x => x.Length);
        var max = words.Max(x => x.Length);

        while (true)
        {
            Console.Write("Which word length do you want to see? Leave blank for everything: ");
            var entry = Console.ReadLine()?.Trim();
            if (entry == null || entry.Length == 0)
                return null;
            if (!int.TryParse(entry, out int len))
            {
                Console.WriteLine("Please enter digits, or nothing at all.");
                continue;
            }
            if (len < min || len > max)
                Console.WriteLine($"No words of that length. Word lengths are between {min} and {max}.");
            else
                return len;
        }
    }

    private static string? PromptStartingLetters()
    {
        while (true)
        {
            Console.Write("Which starting letter(s) do you want answers for. Leave blank for everything: ");
            var entry = Console.ReadLine()?.Trim();
            return entry;
        }
    }

    private static void OutputMatchingWords(
        List<String> words, 
        int? wordLen,
        string? startingLetters)
    {
        var filtered = words.Where(w => wordLen == null || w.Length == wordLen)
            .Where(w => startingLetters == null || w.StartsWith(startingLetters))
            .ToList();
        var wordFormat = filtered.Count == 1 ? "word" : "words";
        Console.WriteLine($"{filtered.Count} {wordFormat} found.");
        foreach (var word in filtered)
            Console.WriteLine(word);
    }
}
