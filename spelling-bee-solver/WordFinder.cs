namespace spelling_bee_solver;

internal class WordFinder
{
    private enum WordStatus
    {
        HasQ,
        HasS,
        HasEandR,
        TooManyLetters,
        KnownInvalid,
        Valid
    }

    private const string VALID_WORDS_PATH = @"..\..\..\words.txt";
    private const string UPDATE_WORDS_PATH = @"..\..\..\update.txt";
    private const string INVALID_WORDS_PATH = @"..\..\..\invalid.txt";

    private readonly IEnumerable<string> _valid_words;
    private readonly HashSet<string> _invalid_words;

    public WordFinder()
    {
        _valid_words = File.ReadLines(VALID_WORDS_PATH);
        _invalid_words = new HashSet<string>(File.ReadLines(INVALID_WORDS_PATH));
    }

    public List<string> FindWords(char center, HashSet<char> letters)
    {
        var result = new List<string>();
        var allLetters = new HashSet<char>(letters);
        allLetters.Add(center);
        foreach (var word in _valid_words)
        {
            if (!word.Contains(center))
                continue;
            var isMatch = true;
            foreach (var letter in word)
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
        File.WriteAllLines(UPDATE_WORDS_PATH, ValidWords());
    }

    private IEnumerable<string> ValidWords()
    {
        foreach (var word in _valid_words)
            if (GetWordStatus(word) == WordStatus.Valid)
                yield return word;
    }

    private WordStatus GetWordStatus(string word)
    {
        if (word.Contains('q'))
            return WordStatus.HasQ;

        if (word.Contains('s'))
            return WordStatus.HasS;

        if (word.Contains('e') && word.Contains('r'))
            return WordStatus.HasEandR;

        var distinct = new HashSet<char>();
        foreach (var c in word)
            distinct.Add(c);
        if (distinct.Count > 7)
            return WordStatus.TooManyLetters;

        if (_invalid_words.Contains(word))
            return WordStatus.KnownInvalid;

        return WordStatus.Valid;
    }

    public void SanityCheck()
    {
        var qCount = 0;
        var sCount = 0;
        var erCount = 0;
        var distinctLetterCount = 0;
        foreach (var word in _valid_words)
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
                case WordStatus.HasEandR:
                    erCount++;
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
        Console.WriteLine($"{erCount} words containing E and R.");
        Console.WriteLine($"{distinctLetterCount} words with more than 7 different letters.");
    }

    public void CommonWords()
    {
        foreach (var word in _valid_words)
        {
            var distinct = new HashSet<char>();
            foreach (var c in word)
                distinct.Add(c);
            if (distinct.Count <= 3)
                Console.WriteLine(word);
        }
    }

    public void InvalidWordsCheck()
    {
        foreach (var word in _valid_words)
        {
            if (_invalid_words.Contains(word))
                Console.WriteLine(word);
        }
    }
}
