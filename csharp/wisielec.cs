// Dawid Sobota 78618
// Gra Wisielec IT w C# (klasy WordBank, Player, Game + plik slowa.txt)

using System;
using System.Collections.Generic;
using System.IO;

public class WordBank
{
    private readonly List<string> words;
    private readonly Random random = new Random();

    public WordBank(string filename = "slowa.txt")
    {
        words = LoadWords(filename);
    }

    private List<string> LoadWords(string filename)
    {
        // Szuka pliku w bieżącym folderze lub poziom wyżej (gdy uruchamiamy z folderu csharp/)
        string[] candidates =
        {
            filename,
            Path.Combine("..", filename),
            Path.Combine(AppContext.BaseDirectory, filename),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", filename)
        };

        foreach (string path in candidates)
        {
            if (File.Exists(path))
            {
                var loaded = new List<string>();
                foreach (string line in File.ReadAllLines(path))
                {
                    string word = line.Trim().ToLower();
                    if (!string.IsNullOrEmpty(word))
                    {
                        loaded.Add(word);
                    }
                }
                if (loaded.Count > 0)
                {
                    return loaded;
                }
            }
        }

        throw new FileNotFoundException($"Nie znaleziono pliku ze słowami: {filename}");
    }

    public string GetRandomWord()
    {
        int index = random.Next(words.Count);
        return words[index].ToLower();
    }
}

public class Player
{
    public string Name { get; }
    public List<char> GuessedLetters { get; }
    public int Mistakes { get; private set; }

    public Player(string name)
    {
        Name = name;
        GuessedLetters = new List<char>();
        Mistakes = 0;
    }

    public bool HasGuessed(char letter)
    {
        return GuessedLetters.Contains(letter);
    }

    public void AddGuess(char letter)
    {
        if (!GuessedLetters.Contains(letter))
        {
            GuessedLetters.Add(letter);
        }
    }

    public void AddMistake()
    {
        Mistakes++;
    }
}

public class Game
{
    private readonly WordBank wordBank;
    public Player Player { get; }
    public int MaxMistakes { get; }
    public string SecretWord { get; }

    public Game(WordBank wordBank, Player player, int maxMistakes = 6)
    {
        this.wordBank = wordBank;
        Player = player;
        MaxMistakes = maxMistakes;
        SecretWord = this.wordBank.GetRandomWord();
    }

    public string GetMaskedWord()
    {
        // Odsłania odgadnięte litery, pozostałe zastępuje znakiem '_'
        var znaki = new List<string>();
        foreach (char ch in SecretWord)
        {
            znaki.Add(Player.HasGuessed(ch) ? ch.ToString() : "_");
        }
        return string.Join(" ", znaki);
    }

    public bool IsWon()
    {
        foreach (char ch in SecretWord)
        {
            if (!Player.HasGuessed(ch))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsLost()
    {
        return Player.Mistakes >= MaxMistakes;
    }

    public bool IsGameOver()
    {
        return IsWon() || IsLost();
    }

    public string GetState()
    {
        if (IsWon()) return "Wygrana";
        if (IsLost()) return "Przegrana";
        return "W trakcie";
    }

    public void GuessLetter(char letter)
    {
        letter = char.ToLower(letter);

        if (IsGameOver())
        {
            Console.WriteLine("Gra jest już zakończona.");
            return;
        }

        if (Player.HasGuessed(letter))
        {
            Console.WriteLine($"Litera '{letter}' była już podana!");
            return;
        }

        Player.AddGuess(letter);

        if (SecretWord.Contains(letter))
        {
            Console.WriteLine($"Dobrze! Litera '{letter}' znajduje się w haśle.");
        }
        else
        {
            Player.AddMistake();
            Console.WriteLine($"Pudło! Brak litery '{letter}' w haśle.");
        }
    }

    public void DisplayStatus()
    {
        Console.WriteLine("\n----------------------------");
        Console.WriteLine($"Gracz: {Player.Name} | Stan gry: {GetState()}");
        Console.WriteLine($"Hasło: {GetMaskedWord()}");
        Console.WriteLine($"Błędy: {Player.Mistakes} / {MaxMistakes}");
        string podane = Player.GuessedLetters.Count > 0
            ? string.Join(", ", Player.GuessedLetters)
            : "brak";
        Console.WriteLine($"Podane litery: {podane}");
    }

    public void Play()
    {
        Console.WriteLine($"Witaj w grze Wisielec IT, {Player.Name}!");

        while (!IsGameOver())
        {
            DisplayStatus();
            Console.Write("Podaj literę: ");
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input) || input.Length != 1 || !char.IsLetter(input[0]))
            {
                Console.WriteLine("Wpisz dokładnie jedną literę!");
                continue;
            }

            GuessLetter(input[0]);
        }

        DisplayStatus();
        if (IsWon())
        {
            Console.WriteLine($"Gratulacje! Odgadłeś hasło: {SecretWord}");
        }
        else
        {
            Console.WriteLine($"Koniec gry! Ukryte hasło to: {SecretWord}");
        }
    }
}

class Program
{
    static void Main()
    {
        var bank = new WordBank("slowa.txt");
        Console.Write("Podaj imię gracza: ");
        string? imie = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(imie))
        {
            imie = "Gracz";
        }

        var player = new Player(imie);
        var game = new Game(bank, player, maxMistakes: 6);
        game.Play();
    }
}
