using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.IO;
using Avalonia.Media.TextFormatting;
using DynamicData.Aggregation;

namespace SpellingBee
{
    /// <summary>
    /// <c>Game Model</c> class.
    /// </summary>
    public class GameModel
    {
        [JsonProperty] private List<char> baseWord;
        [JsonProperty] private List<string> foundWords;
        [JsonProperty] private int playerPoints;
        [JsonProperty] private char requiredLetter;
        [JsonProperty] private int maxPoints;

        private readonly Random rand;
        private readonly List<string> validWords;
        private readonly List<KeyValuePair<string, int>> statusTitles;
        private List<string> PangramWords;

        private const string DatabaseConnectionString = "Data Source=../../../../SpellingBee/SetUpSpellingBee/Database/SpellingBeeWords.db;";
        private const string DatabaseConnectionString_Two = "Data Source=./SetUpSpellingBee/Database/SpellingBeeWords.db";

        /// <summary>
        /// Retrieves the current score of the player.
        /// </summary>
        public int GetCurrentScore()
        {
            return playerPoints; 
        }

        /// <summary>
        /// Exits the game application.
        /// </summary>
        public void Exit()
        {
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Initializes a new instance of the <c>GameModel</c> class, setting up the base game state.
        /// </summary>
        public GameModel()
        {
            baseWord = new List<char>();
            foundWords = new List<string>();
            validWords = new List<string>();
            rand = new Random();

            // Status titles with associated point thresholds.
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
            // Starting player points.
            playerPoints = 0;    
            // Initial total possible points.
            maxPoints = 0; 

            // Fetch the list of pangrams from the database.
            PangramWords = PangramList(); 
        }

        /// <summary>
        /// Retrieves a list of pangrams from the database.
        /// </summary>
        public List<string> PangramList()
        {
            string query = $"select word from pangrams";
            string connectionString = DatabaseConnectionString;
            List<string> words = new();

            try
            {
                using SqliteConnection con = new(connectionString);
                con.Open();
                using var cmd = con.CreateCommand();
                cmd.CommandText = query;
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string word = reader.GetString(0);
                    words.Add(word);
                }
            }
            catch (Exception ex)
            {
                // Nested try statement to attempt both connection strings.
                connectionString = DatabaseConnectionString_Two;
                try
                {
                    using SqliteConnection con = new(connectionString);
                    con.Open();
                    using var cmd = con.CreateCommand();
                    cmd.CommandText = query;
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string word = reader.GetString(0);
                        words.Add(word);
                    }
                }
                catch 
                {
                    Console.WriteLine($"An error occurred: {ex.Message}"); 
                }
                
            }

            return words;
        }

        /// <summary>
        /// Generates valid words for the current puzzle based on the database.
        /// </summary>
        public void GenerateValidWords()
        {
            List<string> tableNames = new()
            {
                "four_letter_words", "five_letter_words", "six_letter_words", "seven_letter_words",
                "eight_letter_words", "nine_letter_words", "ten_letter_words", "eleven_letter_words",
                "twelve_letter_words", "thirteen_letter_words", "fourteen_letter_words", "fifteen_letter_words"
            };

            StringBuilder queryBuilder = new();
            foreach (string tableName in tableNames)
            {
                queryBuilder.AppendLine($"SELECT word FROM {tableName} WHERE word LIKE '%{requiredLetter}%' AND word NOT GLOB '*[^{(new string(baseWord.ToArray()))}]*'");

                // Add UNION between queries, except for the last one.
                if (tableNames.IndexOf(tableName) < tableNames.Count - 1)
                {
                    queryBuilder.AppendLine("UNION");
                }
            }

            string query = queryBuilder.ToString();
            string connectionString = DatabaseConnectionString;

            try
            {
                using SqliteConnection con = new(connectionString);
                con.Open();
                using var cmd = con.CreateCommand();
                cmd.CommandText = query;
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string word = reader.GetString(0);
                    validWords.Add(word);
                }
            }
            catch (Exception ex)
            {
                // Nested try statement.
                connectionString = DatabaseConnectionString_Two;
                try
                {
                    using SqliteConnection con = new(connectionString);
                    con.Open();
                    using var cmd = con.CreateCommand();
                    cmd.CommandText = query;
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string word = reader.GetString(0);
                        validWords.Add(word);
                    }
                }
                catch
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            // Calculate maxPoints based on validWords.
            foreach (var word in validWords)
            {
                // Count of unique letters in the word.
                int uniqueLetterCount = word.Distinct().Count(); 
                int wordLength = word.Length;
                int points = 0;

                if (wordLength == 4) points = 1;
                else if (wordLength == 5 || wordLength == 6) points = wordLength;
                else if (wordLength > 6) points = wordLength + (uniqueLetterCount == 7 ? 7 : 0);

                maxPoints += points;
            }
        }

        /// <summary>
        /// Selects a random word to be used as the base for the puzzle.
        /// </summary>
        public void SelectRandomWordForPuzzle()
        {
            string selectedWord = PangramWords[rand.Next(PangramWords.Count)];
            baseWord = new List<char>(selectedWord.Distinct().ToArray());
            ShuffleBaseWord();
            requiredLetter = baseWord[0];
        }

        /// <summary>
        /// Sets the base word for the puzzle from a given word.
        /// </summary>
        public bool SetBaseWordForPuzzle(string word)
        {
            string bWord = word.ToLower();
            if (!PangramWords.Contains(bWord))
            {
                return false;
            }
            Reset();
            baseWord = new List<char>(bWord.Distinct().ToArray());
            ShuffleNewWord();
            requiredLetter = baseWord[0];
            return true;
        }

        /// <summary>
        /// Shuffles the letters of the base word.
        /// </summary>
        private void ShuffleNewWord()
        {
            int n = baseWord.Count;
            for (int i = n - 1; i > 0; i--)
            {
                // Get a random index up to i inclusive.
                int j = rand.Next(i + 1);

                // Use tuple to swap elements.
                (baseWord[i], baseWord[j]) = (baseWord[j], baseWord[i]);
            }
        }

        /// <summary>
        /// Shuffles the letters of the base word. Also makes sure that the required letter is the first element.
        /// </summary>
        public void ShuffleBaseWord()
        {
            int n = baseWord.Count;
            for (int i = n - 1; i > 0; i--)
            {
                // Get a random index up to i inclusive.
                int j = rand.Next(i + 1);

                // Swap elements.
                (baseWord[i], baseWord[j]) = (baseWord[j], baseWord[i]);
            }

            // Ensure the required letter is at the 0th index.
            int requiredIndex = baseWord.IndexOf(requiredLetter);
            if (requiredIndex != 0 && requiredIndex >= 0 && baseWord.Count > 1)
            {
                (baseWord[0], baseWord[requiredIndex]) = (baseWord[requiredIndex], baseWord[0]);
            }
        }

        /// <summary>
        /// Checks if a given word is valid for the current puzzle.
        /// </summary>
        public bool IsValidWord(string word)
        {
            return validWords.Contains(word);
        }

        /// <summary>
        /// Determines if a word has already been found by the player.
        /// </summary>
        public bool IsWordAlreadyFound(string word)
        {
            return foundWords.Contains(word);
        }

        /// <summary>
        /// Adds a found word to the list of discovered words and updates player's points.
        /// </summary>
        public void AddFoundWord(string word)
        {
            if (IsValidWord(word) && !IsWordAlreadyFound(word))
            {
                foundWords.Add(word);
                UpdatePlayerPointsForFoundWord(word);
            }
        }

        /// <summary>
        /// Resets the game model to its initial state.
        /// </summary>
        public void Reset()
        {
            baseWord.Clear();
            validWords.Clear();
            foundWords.Clear();
            playerPoints = 0;
            maxPoints = 0;
            PangramWords = PangramList();
        }

        /// <summary>
        /// Calculates the points required for the player to achieve the next rank.
        /// </summary>
        public int PointsToNextRank()
        {
            // Default status
            int status = 0; 

            for (int i = 0; i < statusTitles.Count; ++i)
            {
                if (playerPoints >= (int)(statusTitles[i].Value * maxPoints* .01))
                {
                    if (i == statusTitles.Count - 1)
                        status = GetMaxPoints();
                    else 
                        status = (int)(statusTitles[i + 1].Value * maxPoints * .01);
                }
            }
            return (status - playerPoints);
        }

        private void UpdatePlayerPointsForFoundWord(string word)
        {
            // Count of unique letters in the word.
            int uniqueLetterCount = word.Distinct().Count(); 
            int wordLength = word.Length;
            int points = 0;

            if (wordLength == 4)
                points = 1;
            else if (wordLength == 5 || wordLength == 6)
                points = wordLength;
            else if (wordLength > 6)
                points = wordLength + (uniqueLetterCount == 7 ? 7 : 0);

            // Add the points to the player's total score.
            playerPoints += points; 
        }

        /// <summary>
        /// Determines if a puzzle has been started.
        /// </summary>
        public bool Active()
        {
            return baseWord.Count > 0;
        }

        /// <summary>
        /// Saves the current game state to a specified file.
        /// </summary>
        public bool SaveCurrentGameState(string saveName)
        {
            string fileName = saveName;
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves/"));
            fileName += ".json";
            var jsonString = JsonConvert.SerializeObject(this);
            File.WriteAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "saves/"), fileName), jsonString);
            return true;
        }

        /// <summary>
        /// Saves the current puzzle to a specified file.
        /// </summary>
        public bool SaveCurrentPuzzleState(string saveName)
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves/"));
            string fileName = saveName;
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            fileName += ".json";
            GameModel temp = new()
            {
                requiredLetter = this.requiredLetter,
                baseWord = new List<char>(this.baseWord),
                maxPoints = this.maxPoints
            };
            var jsonString = JsonConvert.SerializeObject(temp);
            File.WriteAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "saves/"), fileName), jsonString);
            return true;
        }

        /// <summary>
        /// Retrieves a list of available save files.
        /// </summary>
        public List<string> GetAvailableSaveFiles()
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves/"));
            return Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "saves/")).ToList();
        }

        /// <summary>
        /// Loads a game state from a specified file ID.
        /// </summary>
        public GameModel? LoadGameStateFromFile(int fileId)
        {
            var fileList = GetAvailableSaveFiles();
            if (fileList.Count > fileId)
            {
                string filePath = fileList[fileId];
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<GameModel>(jsonData);
            }
            // File not found or invalid ID.
            return null; 
        }

        /// <summary>
        /// Assigns properties and fields from a loaded game to the current instance.
        /// </summary>
        public void AssignFrom(GameModel loadedGame)
        {
            // Assigning each field and property from the loaded game to this instance
            baseWord = new List<char>(loadedGame.baseWord);
            foundWords = new List<string>(loadedGame.foundWords);
            playerPoints = loadedGame.playerPoints;
            requiredLetter = loadedGame.requiredLetter;
            //validWords = new List<string>(loadedGame.validWords);
            GenerateValidWords();
            maxPoints = loadedGame.maxPoints;
        }

        /// <summary>
        /// Returns true if the player has found all of the possible valid words
        /// of the puzzle.
        /// <return>True if foundWords == validWords, false otherwise.</return>
        /// </summary>
        public bool wonTheGame()
        {
            return (foundWords.Count == validWords.Count);
        }

        /// <summary>
        /// Retrieves the base word of the current puzzle.
        /// </summary>
        public List<char> GetBaseWord()
        {
            return baseWord;
        }

        /// <summary>
        /// Retrieves the required letter of the current puzzle.
        /// </summary>
        public char GetRequiredLetter()
        {
            return requiredLetter;
        }

        /// <summary>
        /// Retrieves the player's current points.
        /// </summary>
        public int GetPlayerPoints()
        {
            return playerPoints;
        }

        /// <summary>
        /// Retrieves the status titles with their associated point thresholds.
        /// </summary>
        public List<KeyValuePair<string, int>> GetStatusTitles()
        {
            return statusTitles;
        }

        /// <summary>
        /// Retrieves a list of words found by the player.
        /// </summary>
        public IEnumerable<string> GetFoundWords()
        {
            return foundWords;
        }

        /// <summary>
        /// Retrieves the maximum points achievable for the current puzzle.
        /// </summary>
        public int GetMaxPoints()
        {
            return maxPoints;
        }

        /// <summary>
        /// Retrieves the valid words for the current puzzle.
        /// </summary>
        public List<String> GetValidWords()
        {
            return validWords;
        }

        /// <summary>
        /// Returns a dictionary that maps each starting letter of words in validWords to an array.
        /// The array's indices represent word lengths (4 to 15) and the values are the counts of words with that starting letter and length.
        /// </summary>
        /// <returns>A dictionary of chars to arrays of ints.</returns>
        public Dictionary<char, int[]> LettersInWord()
        {
            // Initialize a dictionary to hold the starting letter of words and an array to represent counts of word lengths.
            Dictionary<char, int[]> countOfLetters = new();

            // Iterate over each word in the validWords list.
            foreach (var word in validWords)
            {
                // Convert the word into a char array for easy access to individual letters.
                var w = word.ToCharArray();

                // If the dictionary does not already contain an entry for the first letter of the word, add it.
                if (!countOfLetters.ContainsKey(w[0]))
                {
                    countOfLetters[w[0]] = new int[12]; // Initialize a new array of 12 integers (for word lengths 4 to 15).
                }

                // Calculate the length of the current word.
                int length = word.Length;


                // Increment the appropriate index in the array associated with the starting letter of the word.
                // Subtract 4 from the word length to get the correct index (for length 4, we want index 0; for length 5, index 1, etc.).
                countOfLetters[w[0]][length - 4]++;

            }

            // Return the populated dictionary.
            return countOfLetters;
        }

        /// <summary>
        /// Generates a list of two letter strings that represent the starting two letters of each word
        /// and puts a count of how many times they show up in the valid word list
        /// </summary>
        /// <returns>
        /// Dictionary of the two letter string and their count
        /// </returns>
        public Dictionary<string, int> TwoLetterList()
        {
            // Initialize a dictionary to hold the two letter strings and their counts
            Dictionary<string, int> twoLetters = new();

            // Loop through valid words
            foreach (var word in validWords)
            {
                // Add the valid word to dictionary or add 1 to its count if already there
                string twoLetterSub = word.Substring(0, 2);
                try
                {
                    twoLetters.Add(twoLetterSub, 1);
                }
                catch (ArgumentException)
                {
                    twoLetters[twoLetterSub] += 1;
                }
            }
            return twoLetters;
        }

        /// <summary>
        /// Creats a tuple where the first int is the number of pangrams in the valid word set
        /// and the second int is the number of perfect pangrams in the valid word set
        /// </summary>
        /// <returns>
        /// Tuple of two ints
        /// </returns>
        public (int, int) PangramCount()
        {
            int pangram = 0;
            int perfPangram = 0;
            // Counts the number of perfect and regular pangrams in validWords
            foreach(var word in validWords)
            {
                if (word.Distinct().Count() == 7 && word.Count() == 7)
                {
                    perfPangram++;
                    pangram++;
                }
                else if (word.Distinct().Count() == 7)
                    pangram++;
            }
            return (pangram, perfPangram);
        }

        /// <summary>
        /// Creates a string for the hint
        /// </summary>
        /// <returns>
        /// A string that contains the hint information
        /// </returns>
        public string PrintHintTable()
        {
            // Getting data for hints
            Dictionary<char, int[]> data = LettersInWord();
            // Define the range
            int[] wordLengths = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            int arrayLength = wordLengths.Length;



            // Calculate totals for each column
            int[] columnTotals = new int[arrayLength];
            foreach (var entry in data)
            {
                for (int i = 0; i < arrayLength; i++)
                {
                    columnTotals[i] += entry.Value[i];
                }
            }

            List<int> removedColumns = new List<int>();

            // Start Hint
            StringBuilder build = new StringBuilder();
            build.Append("\nRequired letter is first.\n\n");
            build.Append(new string(baseWord.ToArray()));
            build.Append("\n\n");

            (int, int) pangramCount = PangramCount();
            build.Append($"Words: {validWords.Count()}, Points: {GetMaxPoints()}, Pangrams: {pangramCount.Item1}");
            if (pangramCount.Item2 != 0) 
                build.Append($" ({pangramCount.Item2} Perfect)");
            if (data.Count() == 7)
                build.Append($", BINGO");
            build.Append("\n\n");

            // Create header for table
            build.Append("    ");
            for (int i = 0; i < arrayLength; i++)
            {
                if (columnTotals[i] != 0)
                {
                    build.Append($"{wordLengths[i], 3} ");
                }
                else
                {
                    removedColumns.Add(i);
                }
            }
            //Added for -1 so list isn't empty
            removedColumns.Add(-1);

            build.Append(" tot\n");

            

            // Create rows
            foreach (var entry in data)
            {
                build.Append($"{entry.Key}   ");
                int rowTotal = 0;
                for (int i = 0; i < arrayLength; i++)
                {
                    if (!removedColumns.Contains(i))
                    { 
                        if (entry.Value[i] == 0)
                        {
                            build.Append("  - ");
                        }
                        else
                        {
                            build.Append($"{entry.Value[i],3} ");
                            rowTotal += entry.Value[i];
                        }
                    }
                }
                build.Append($"{rowTotal,4}\n");
            }

            // Print bottom totals
            build.Append("tot ");
            for (int i = 0; i < arrayLength; i++)
            {
                if (!removedColumns.Contains(i))
                {
                    build.Append($"{columnTotals[i],3} ");
                }
            }
            build.Append($"{validWords.Count(),4}");

            // Create the two letter list
            build.Append("\n\nTwo Letter List:\n\n");
            Dictionary<string, int> twoLetter = TwoLetterList();
            string curLet = twoLetter.Keys.First().Substring(0, 1);

            foreach (var two in twoLetter)
            {
                if (!curLet.Equals(two.Key.Substring(0, 1)))
                {
                    curLet = two.Key.Substring(0, 1);
                    build.Append("\n");
                }
                build.Append(two.Key + "-" + two.Value.ToString() + " ");
            }

            return build.ToString();
        }

    }
}

