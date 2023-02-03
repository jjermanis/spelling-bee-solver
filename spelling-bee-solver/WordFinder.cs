namespace spelling_bee_solver;

internal class WordFinder
{
    private const string WORDS_PATH = @"..\..\..\words.txt";

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
}
