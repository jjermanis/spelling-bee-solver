namespace spelling_bee_solver;

internal class InteractiveGame
{
    public void PlayGame()
    {
        var finder = new WordFinder();

        Console.WriteLine("Welcome to spelling-bee-solver. This program will help you");
        Console.WriteLine("come up with answers for the dailing NY Times Spelling Bee");
        Console.WriteLine("puzzle.");

        var center = PromptCenterLetter();
        var letters = PromptLetters(center);

        var matchingWords = finder.FindWords(center, letters);
        matchingWords.Sort();
        Console.WriteLine("Here are the matching words:");
        foreach (var word in matchingWords)
            Console.WriteLine(word);
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
}
