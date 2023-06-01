namespace spelling_bee_solver;

internal class WordFinder
{
    // TODO: most of this class is dictionary cleanup. Separate into new class?
    // TODO: profile performance. Start up takes about 1 second.

    private enum WordStatus
    {
        HasS,
        HasEandR,
        TooManyLetters,
        NeverInPangram,
        KnownInvalid,
        Valid
    }

    private const string WORDS_PATH = @"..\..\..\words.txt";
    private const string UPDATE_WORDS_PATH = @"..\..\..\update.txt";
    private const string INVALID_WORDS_PATH = @"..\..\..\invalid.txt";

    private readonly List<string> _words;
    private readonly HashSet<string> _invalid_words;
    private readonly HashSet<string> _all_pangrams;
    private readonly HashSet<string> _used_pangrams;

    public WordFinder()
    {
        _words = File.ReadLines(WORDS_PATH).ToList();
        _invalid_words = new HashSet<string>(File.ReadLines(INVALID_WORDS_PATH));
        _all_pangrams = AllPangrams();
        _used_pangrams = new HashSet<string>();
    }

    public List<string> FindWords(char center, HashSet<char> letters)
    {
        var result = new List<string>();
        var allLetters = new HashSet<char>(letters);
        allLetters.Add(center);
        foreach (var word in _words)
        {
            if (_invalid_words.Contains(word))
                continue;
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
        var validWords = ValidWords().ToList();
        if (validWords.Count != _words.Count)
            File.WriteAllLines(UPDATE_WORDS_PATH, ValidWords());
    }

    private IEnumerable<string> ValidWords()
    {
        foreach (var word in _words)
            if (GetWordStatus(word) == WordStatus.Valid)
                yield return word;
    }

    private WordStatus GetWordStatus(string word)
    {
        if (word.Contains('s'))
            return WordStatus.HasS;

        if (word.Contains('e') && word.Contains('r'))
            return WordStatus.HasEandR;

        var distinct = new HashSet<char>();
        foreach (var c in word)
            distinct.Add(c);
        if (distinct.Count > 7)
            return WordStatus.TooManyLetters;

        if (!IsInPangram(word))
            return WordStatus.NeverInPangram;

        if (_invalid_words.Contains(word))
            return WordStatus.KnownInvalid;

        return WordStatus.Valid;
    }

    public void SanityCheck()
    {
        var sCount = 0;
        var erCount = 0;
        var distinctLetterCount = 0;
        foreach (var word in _words)
        {
            var status = GetWordStatus(word);
            switch (status)
            {
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
        Console.WriteLine($"{sCount} words containing S.");
        Console.WriteLine($"{erCount} words containing E and R.");
        Console.WriteLine($"{distinctLetterCount} words with more than 7 different letters.");
    }

    public void CommonWords()
    {
        foreach (var word in _words)
        {
            var distinct = new HashSet<char>();
            foreach (var c in word)
                distinct.Add(c);
            /*
            */
            if (distinct.Count <= 3)
                Console.WriteLine(word);
            /*
            if (distinct.Count <= 4 && word.Length >= 8)
                Console.WriteLine(word);
            if (distinct.Count <= 5 && word.Length >= 10)
                Console.WriteLine(word);
            if (distinct.Count <= 6 && word.Length >= 12)
                Console.WriteLine(word);
            */
        }
    }

    public void InvalidWordsCheck()
    {
        foreach (var word in _words)
            if (GetWordStatus(word) != WordStatus.Valid)
                Console.WriteLine(word);
    }

    private bool IsInPangram(string word)
    {
        if (_all_pangrams.Contains(word))
            return true;

        foreach (var pangram in _all_pangrams)
            if (IsWordInPangram(word, pangram))
            {
                _used_pangrams.Add(pangram);
                return true;
            }
        return false;
    }

    private static bool IsWordInPangram(string word, string pangram)
    {
        foreach (var c in word)
            if (!pangram.Contains(c))
                return false;
        return true;
    }

    private HashSet<string> AllPangrams()
    {
        var result = new HashSet<string>();
        foreach (var word in _words)
        {
            var currLetters = new HashSet<char>();
            foreach (var c in word)
                currLetters.Add(c);
            if (currLetters.Count == 7)
                result.Add(word);
        }
        return result;
    }
}
