namespace spelling_bee_solver;

internal class WordFinder
{
    // TODO add words containing E and R to sanity check

    private enum WordStatus
    {
        HasQ,
        HasS,
        TooManyLetters,
        Valid
    }

    private const string WORDS_PATH = @"..\..\..\words.txt";
    private const string VALID_WORDS_PATH = @"..\..\..\update.txt";

    private readonly IEnumerable<string> _words;

    public WordFinder()
    {
        _words = File.ReadLines(WORDS_PATH);
    }

    public List<string> FindWords(char center, HashSet<char> letters)
    {
        var result = new List<string>();
        var allLetters = new HashSet<char>(letters);
        allLetters.Add(center);
        foreach (var word in _words)
        {
            if (!word.Contains(center))
                continue;
            var isMatch = true;
            foreach(var letter in word)
            {
                if (!allLetters.Contains(letter))
                    isMatch = false;
            }
            if (isMatch)
                result.Add(word);
        }
        return result;
    }

    public void CreateSaneWordsFile()
    {
        File.WriteAllLines(VALID_WORDS_PATH, ValidWords());
    }

    private IEnumerable<string> ValidWords()
    {
        foreach (var word in _words)
            if (GetWordStatus(word) == WordStatus.Valid)
                yield return word;
    }

    private static WordStatus GetWordStatus(string word)
    {
        if (word.Contains('q'))
            return WordStatus.HasQ;

        if (word.Contains('s'))
            return WordStatus.HasS;

        var distinct = new HashSet<char>();
        foreach (var c in word)
            distinct.Add(c);
        if (distinct.Count > 7)
            return WordStatus.TooManyLetters;

        return WordStatus.Valid;

    }

    public void SanityCheck()
    {
        var qCount = 0;
        var sCount = 0;
        var distinctLetterCount = 0;
        foreach (var word in _words)
        {
            var status = GetWordStatus(word);
            switch (status)
            {
                case WordStatus.HasQ:
                    qCount++;
                    break;
                case WordStatus.HasS:
                    sCount++;
                    break;
                case WordStatus.TooManyLetters:
                    distinctLetterCount++;
                    break;
                case WordStatus.Valid:
                    break;
                default:
                    throw new Exception($"Unhandled case: {status}");
            }
        }
        Console.WriteLine($"{qCount} words containing Q.");
        Console.WriteLine($"{sCount} words containing S.");
        Console.WriteLine($"{distinctLetterCount} words with more than 7 different letters.");
    }

    public void CommonWords()
    {
        foreach (var word in _words)
        {
            var distinct = new HashSet<char>();
            foreach (var c in word)
                distinct.Add(c);
            if (distinct.Count <= 3)
                Console.WriteLine(word);
        }
    }
}
