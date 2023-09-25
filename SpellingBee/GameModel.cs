﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.IO;

namespace SpellingBee
{
/*
 * GameModel Class - SpellingBee
 * 
 * Purpose:
 * --------
 * The GameModel class encapsulates the core game logic and data for the Spelling Bee game.
 * It provides methods for game-related operations like creating puzzles, validating guesses,
 * managing game progress, saving/loading game states, and other utility functions.
 * 
 * Main Features:
 * --------------
 * - GenerateValidWords: Fetches valid words for the current puzzle from the database.
 * - SelectRandomWordForPuzzle: Chooses a random pangram to form a puzzle.
 * - AddFoundWord: Updates the list of found words and adjusts the player's points accordingly.
 * - SaveCurrentGameState/SaveCurrentPuzzleState: Allows game or puzzle state to be saved.
 * - LoadGameState: Retrieves saved game states.
 * 
 * Dependencies:
 * -------------
 * This class interacts with the GameView to provide visual feedback and with the GameController
 * to process user commands. It also interfaces with an SQLite database to fetch word lists and
 * validate puzzle words.
 * 
 * External Libraries:
 * -------------------
 * - Newtonsoft.Json: Used for serializing and deserializing game states for saving/loading.
 * - Microsoft.Data.Sqlite: Allows interaction with an SQLite database.
 * 
 * Usage:
 * ------
 * GameModel should be instantiated and used along with GameView and GameController to form
 * the MVC pattern and enable a complete game flow.
 */

    public class GameModel
    {
        [JsonProperty] private List<char> baseWord;
        [JsonProperty] private List<string> foundWords;
        [JsonProperty] private int playerPoints;
        [JsonProperty] private char requiredLetter;
        [JsonProperty] private int maxPoints;
        private Random rand;
        private List<string> validWords;
        private List<KeyValuePair<string, int>> statusTitles;
        private List<string> PangramWords;
        

        public List<char> BaseWord => baseWord;
        public char RequiredLetter => requiredLetter;
        public int PlayerPoints => playerPoints;
        public List<KeyValuePair<string, int>> StatusTitles => statusTitles;
        public IEnumerable<string> FoundWords => foundWords;

        private const string DatabaseConnectionString = "Data Source=SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

        public void Exit()
        {
            Console.WriteLine("Thank you for playing Spelling Bee. Goodbye!");
            System.Environment.Exit(0);
        }

        public GameModel()
        {
            baseWord = new List<char>();
            foundWords = new List<string>();
            validWords = new List<string>();
            rand = new Random();

            // Status titles with associated point thresholds
            statusTitles = new List<KeyValuePair<string, int>>
    {
        new KeyValuePair<string, int>("Beginner", 0),
        new KeyValuePair<string, int>("Good Start", 2),
        new KeyValuePair<string, int>("Moving Up", 5),
        new KeyValuePair<string, int>("Good", 8),
        new KeyValuePair<string, int>("Solid", 15),
        new KeyValuePair<string, int>("Nice", 25),
        new KeyValuePair<string, int>("Great", 40),
        new KeyValuePair<string, int>("Amazing", 50),
        new KeyValuePair<string, int>("Genius", 70),
        new KeyValuePair<string, int>("Queen Bee", 100)
    };

            requiredLetter = 'a'; // Default required letter
            playerPoints = 0;    // Starting player points
            maxPoints = 0; // Initial total possible points

            PangramWords = PangramList(); // Fetch the list of pangrams from the database
        }

        public List<string> PangramList()
        {
            string query = $"select word from pangrams";
            string connectionString = "Data Source=SetUpSpellingBee\\Database\\SpellingBeeWords.db;";
            List<string> words = new List<string>();

            try
            {
                using (SqliteConnection con = new SqliteConnection(connectionString))
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = query;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string word = reader.GetString(0);
                                words.Add(word);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return words;
        }

        public void GenerateValidWords()
        {
            List<string> tableNames = new List<string>
    {
        "four_letter_words", "five_letter_words", "six_letter_words", "seven_letter_words",
        "eight_letter_words", "nine_letter_words", "ten_letter_words", "eleven_letter_words",
        "twelve_letter_words", "thirteen_letter_words", "fourteen_letter_words", "fifteen_letter_words"
    };

            StringBuilder queryBuilder = new StringBuilder();
            foreach (string tableName in tableNames)
            {
                queryBuilder.AppendLine($"SELECT word FROM {tableName} WHERE word LIKE '%{requiredLetter}%' AND word NOT GLOB '*[^{(new string(baseWord.ToArray()))}]*'");

                // Add UNION between queries, except for the last one
                if (tableNames.IndexOf(tableName) < tableNames.Count - 1)
                {
                    queryBuilder.AppendLine("UNION");
                }
            }

            string query = queryBuilder.ToString();
            string connectionString = "Data Source=SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

            try
            {
                using (SqliteConnection con = new SqliteConnection(connectionString))
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = query;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string word = reader.GetString(0);
                                validWords.Add(word);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Calculate maxPoints based on validWords
            foreach (var word in validWords)
            {
                int uniqueLetterCount = word.Distinct().Count(); // Count of unique letters in the word
                int wordLength = word.Length;
                int points = 0;

                if (wordLength == 4) points = 1;
                else if (wordLength == 5 || wordLength == 6) points = wordLength;
                else if (wordLength > 6) points = wordLength + (uniqueLetterCount == 7 ? 7 : 0);

                maxPoints += points;
            }
        }

        public void SelectRandomWordForPuzzle()
        {
            string selectedWord = PangramWords[rand.Next(PangramWords.Count)];
            baseWord = new List<char>(selectedWord.Distinct().ToArray());
            ShuffleBaseWord();
            requiredLetter = baseWord[0];
        }

        public void SetBaseWordForPuzzle(string word)
        {
            string bWord = word.ToLower();
            while (!PangramWords.Contains(bWord))
            {
                Console.WriteLine("This word is not valid. Please enter a new word: ");
                bWord = Console.ReadLine().ToLower();
            }

            baseWord = new List<char>(bWord.Distinct().ToArray());
            ShuffleBaseWord();
            requiredLetter = baseWord[0];
        }

        public void ShuffleBaseWord()
        {
            int n = baseWord.Count;
            for (int i = n - 1; i > 0; i--)
            {
                // Get a random index up to i inclusive
                int j = rand.Next(i + 1);

                // Swap elements
                char temp = baseWord[i];
                baseWord[i] = baseWord[j];
                baseWord[j] = temp;
            }
        }

        public bool IsValidWord(string word)
        {
            return validWords.Contains(word);
        }

        public bool IsWordAlreadyFound(string word)
        {
            return foundWords.Contains(word);
        }

        public void AddFoundWord(string word)
        {
            if (IsValidWord(word) && !IsWordAlreadyFound(word))
            {
                foundWords.Add(word);
                UpdatePlayerPointsForFoundWord(word);
            }
        }

        public void Reset()
        {
            baseWord.Clear();
            requiredLetter = 'a';
            validWords.Clear();
            foundWords.Clear();
            playerPoints = 0;
            maxPoints = 0;
            PangramWords = PangramList();
        }

        private void UpdatePlayerPointsForFoundWord(string word)
        {
            int uniqueLetterCount = word.Distinct().Count(); // Count of unique letters in the word
            int wordLength = word.Length;
            int points = 0;

            if (wordLength == 4)
                points = 1;
            else if (wordLength == 5 || wordLength == 6)
                points = wordLength;
            else if (wordLength > 6)
                points = wordLength + (uniqueLetterCount == 7 ? 7 : 0);

            playerPoints += points; // Add the points to the player's total score.
        }

        public bool Active()
        {
            return baseWord.Count > 0;
        }

        public void SaveCurrentGameState(string saveName)
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"));
            string fileName = saveName;
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Error: no file name");
                return;
            }
            fileName += ".json";
            var jsonString = JsonConvert.SerializeObject(this);
            File.WriteAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"), fileName), jsonString);
        }

        public void SaveCurrentPuzzleState(string saveName)
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"));
            string fileName = saveName;
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Error: no file name");
                return;
            }
            fileName += ".json";
            GameModel temp = new GameModel
            {
                requiredLetter = this.requiredLetter,
                baseWord = new List<char>(this.baseWord)
            };
            var jsonString = JsonConvert.SerializeObject(temp);
            File.WriteAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"), fileName), jsonString);
        }

        public GameModel LoadGameState(string fileId)
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"));
            var fileList = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"));

            if (fileList.Length == 0)
            {
                Console.WriteLine("No Saved Games");
                return null;
            }

            if (int.TryParse(fileId, out int id) && id < fileList.Length)
            {
                StreamReader fileContents = new StreamReader(File.OpenRead(fileList[id]));
                string openedFile = fileContents.ReadToEnd();
                GameModel loadedGame = JsonConvert.DeserializeObject<GameModel>(openedFile);
                loadedGame.GenerateValidWords();
                return loadedGame;
            }
            else
            {
                Console.WriteLine("Invalid file Id");
                return null;
            }
        }

        public List<string> GetAvailableSaveFiles()
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"));
            return Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "saves\\")).ToList();
        }

        public GameModel LoadGameStateFromFile(int fileId)
        {
            var fileList = GetAvailableSaveFiles();

            if (fileList.Count > fileId)
            {
                string filePath = fileList[fileId];
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<GameModel>(jsonData);
            }

            return null; // File not found or invalid ID.
        }

        public void AssignFrom(GameModel loadedGame)
        {
            // Assigning each field and property from the loaded game to this instance
            baseWord = new List<char>(loadedGame.baseWord);
            foundWords = new List<string>(loadedGame.foundWords);
            playerPoints = loadedGame.playerPoints;
            requiredLetter = loadedGame.requiredLetter;
            validWords = new List<string>(loadedGame.validWords);
            maxPoints = loadedGame.maxPoints;
        }

        public int GetMaxPoints()
        {
            return maxPoints;
        }
    }
}
