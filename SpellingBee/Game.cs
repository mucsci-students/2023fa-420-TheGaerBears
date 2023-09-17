using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using SQLitePCL;

namespace SpellingBee
{
    internal class Game
    {
        [JsonProperty] private List<char> baseWord;
        [JsonProperty] private List<string> foundWords;
        [JsonProperty] private int playerPoints;
        [JsonProperty] private char requiredLetter;
        private Random rand;
        private List<string> validWords;
        private List<KeyValuePair<string, int>> statusTitles;
        private List<string> PangramWords;
        private int totalPossiblePoints;

        public Game()
        {
            baseWord = new List<char>();
            requiredLetter = 'a';
            rand = new Random();
            validWords = new List<string>();
            foundWords = new List<string>();
            statusTitles = new List<KeyValuePair<string, int>>()
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
            playerPoints = 0;
            totalPossiblePoints = 0;
            PangramWords = PangramList();
        }

        /// <summary>
        /// At game start, generates the list of valid words from the baseword and required letter
        /// </summary>
        public void GenerateValidWords()
        {
            List<string> tableNames = new List<string> 
            {
                "four_letter_words",
                "five_letter_words",
                "six_letter_words",
                "seven_letter_words",
                "eight_letter_words",
                "nine_letter_words",
                "ten_letter_words",
                "eleven_letter_words",
                "twelve_letter_words",
                "thirteen_letter_words",
                "fourteen_letter_words",
                "fifteen_letter_words"
            };

            StringBuilder queryBuilder = new StringBuilder();
            foreach (string tableName in tableNames)
            {
                queryBuilder.AppendLine($"SELECT word FROM {tableName} WHERE word LIKE '%{requiredLetter}%' " +
                    $"AND word NOT GLOB '*[^{(new string(baseWord.ToArray()))}]*'");

                // Add UNION between queries, except for the last one
                if (tableNames.IndexOf(tableName) < tableNames.Count - 1)
                {
                    queryBuilder.AppendLine("UNION");
                }
            }

            string query = queryBuilder.ToString();

            string connectionString = "Data Source=..\\..\\..\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

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

            // Calculate totalPoints
            for (int i = 0; i < validWords.Count(); ++i)
            { 
                // Count of unique letters in the word
                int uniqueLetterCount = validWords[i].Distinct().Count(); 
                int wordLength = validWords[i].Count();
                int points = 0;

                if (wordLength == 4)
                {
                    points = 1;
                }
                else if (wordLength == 5 || wordLength == 6)
                {
                    points = wordLength;
                }
                else if (wordLength > 6)
                {
                    points = wordLength + (uniqueLetterCount == 7 ? 7 : 0);
                }
                // Add the points to the player's total score.
                totalPossiblePoints += points; 
            }

            
        }

        /// <summary>
        /// Adds the database pangrams table to a list of pangrams
        /// </summary>
        private List<string> PangramList()
        {
            string query = $"select word from pangrams";
            string connectionString = "Data Source=..\\..\\..\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

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

        /// <summary>
        /// Creates a new puzzle when called
        /// Selects a random pangram and required letter, shows it to the user
        /// GenerateValidWords() is called
        /// </summary>
        public void NewPuzzle()
        {
            // Reset the values for the new puzzle
            Reset();
            // Picks a random pangram
            string selectedWord = PangramWords[rand.Next(PangramWords.Count())];

            char[] q = selectedWord.Distinct().ToArray();
            baseWord = new List<char>(q);

            // Shuffles the letters without printing them
            int n = baseWord.Count();

            for (int i = n - 1; i > 0; i--)
            {
                // Get a random index up to i inclusive
                int j = rand.Next(i + 1);

                // Swap elements
                char temp = baseWord[i];
                baseWord[i] = baseWord[j];
                baseWord[j] = temp;
            }

            // Choose required letter
            requiredLetter = baseWord[0];
            ShowPuzzle();
            Console.WriteLine();
            Console.WriteLine("Guess a word followed by 'Enter'. Good Luck!");
            Console.WriteLine();
            GenerateValidWords();
        }

        /// <summary>
        /// Takes a starting word for the puzzle and checks its validity
        /// If valid it starts the puzzle with the pangrams letters
        /// If not it asks for a new word
        /// GenerateValidWords() is called
        /// </summary>
        public void NewPuzzleBaseWord(string word)
        {
            // Reset the values for the new puzzle
            Reset();

            string bWord = word.ToLower();
            while (!PangramWords.Contains(bWord))
            {
                Console.WriteLine("This word is not valid. Please enter a new word: ");
                bWord = Console.ReadLine().ToLower();
            }

            char[] q = bWord.Distinct().ToArray();
            baseWord = new List<char>(q);

            // Shuffles the letters without printing them
            int n = baseWord.Count();

            for (int i = n - 1; i > 0; i--)
            {
                // Get a random index up to i inclusive
                int j = rand.Next(i + 1);

                // Swap elements
                char temp = baseWord[i];
                baseWord[i] = baseWord[j];
                baseWord[j] = temp;
            }

            // Choose required letter
            requiredLetter = baseWord[0];
            ShowPuzzle();
            Console.WriteLine();
            Console.WriteLine("Guess a word followed by 'Enter'. Good Luck!");
            Console.WriteLine();
            GenerateValidWords();
        }

        /// <summary>
        /// Resets the values of baseWord, requiredLetter, validWords, foundWords, playerPoints, and totalPossiblePoints
        /// </summary>
        private void Reset()
        {
            baseWord = new List<char>();
            requiredLetter = 'a';
            validWords = new List<string>();
            foundWords = new List<string>();
            playerPoints = 0;
            totalPossiblePoints = 0;
        }

        /// <summary>
        /// Checks if a game is running or not
        /// </summary>
        public bool Active()
        {
            if (baseWord.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Prints out the words found by the user, in one line
        /// </summary>
        /// <param name="foundWords"></param>
        public void ShowFoundWords()
        {
            Console.WriteLine("The words you have found are: ");
            foreach (string word in foundWords)
            {
                if (foundWords.IndexOf(word) < foundWords.Count - 1)
                    Console.Write(word + ", ");

                else
                    Console.WriteLine(word);
            }
        }

        /// <summary>
        /// Exits the console app
        /// </summary>
        public void Exit()
        {
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Displays command information for player
        /// </summary>
        public void Help()
        {
            Console.WriteLine("" +
                "How to Play: enter a word containing letters from the 7 given letters.\n" +
                "This word must be at least 4 letters long, and contain the indicated required letter.\n" +
                "Commands:\n" +
                "-help: Lists instructions and all available commands\n" +
                "-exit: Quits the current game without saving anything.\n" +
                "-save puzzle: Saves the game with no progress.\n" +
                "-save current: Saves the game along with progress.\n" +
                "-load puzzle: Loads a previously saved game.\n" +
                "-show puzzle: Displays letters and required letter of current puzzle.\n" +
                "-show found words: Displays list of all words found so far\n" +
                "-shuffle: Shuffles the order letters are displayed in.\n" +
                "-show status: Shows the player their current rank.\n" +
                "-new game: Generates a game with a random board without saving.\n" +
                "-new game from word: Generates a game with a board based on an entered pangram.\n");
        }

        /// <summary>
        /// Changes the order of the baseword letters and reprints in a new order, along with required letter
        /// </summary>
        public void Shuffle()
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
            Console.WriteLine("Shuffled letters: ");

            foreach (char letter in baseWord)
            {
                Console.Write(letter + " ");
            }
            Console.WriteLine("\n");
            Console.WriteLine("Required Letter: " + requiredLetter);
            Console.WriteLine("\n");
        }

        /// <summary>
        /// Shows the users current rank and points
        /// </summary>
        public void ShowStatus()
        {
            double ratio = (double)playerPoints / totalPossiblePoints;
            double percentageAsDecimal = ratio * 100;
            int percentage = (int)Math.Round(percentageAsDecimal);

            string status = "Beginner"; // Default status

            foreach (var title in statusTitles)
            {
                if (percentage >= title.Value)
                {
                    status = title.Key;
                }
            }

            Console.WriteLine($"Your current points: {playerPoints}");
            Console.WriteLine($"Your status: {status}");
        }

        /// <summary>
        /// Prints the intro page for the game
        /// </summary>
        public void BeginScreen()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            centerText("Spelling Bee Puzzle Game");
            Console.WriteLine();
            centerText("Enter a Command: (-New Game, -Load, -Help)");
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Displays the baseword letters and the required letter to the user
        /// </summary>
        public void ShowPuzzle()
        {
            Console.Write("Puzzle Letters: ");
            foreach (char letter in baseWord)
            {
                Console.Write(letter + " ");
            }
            Console.WriteLine();

            Console.WriteLine($"The required letter is: {requiredLetter}");
        }

        /// <summary>
        /// Allows user to Guess a word 
        /// If valid saves it in foundWords, if invalid shows corresponding error message
        /// </summary>
        public void Guess(string word)
        {
            try
            {
                String guess = word;
            
                if (validWords.Contains(guess))
                {
                    if (foundWords.Contains(guess))
                    {
                        Console.WriteLine($"You have already found the word \"{guess}\"!");
                    }
                    else
                    {
                        Console.WriteLine("Word found!");
                        foundWords.Add(guess);
                        PuzzleRank();
                    }
                }
                else
                {
                    Console.WriteLine($"{guess} is not a valid guess.");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        /// <summary>
        /// Saves the current state of the game
        /// </summary>
        public void SaveCurrent()
        {
            // Creates the save folder if it doesnt exist
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"));

            Console.WriteLine("Enter new save name");
            string fileName = Console.ReadLine();

            if (fileName == null)
            {
                Console.WriteLine("Error: no file name");
                return;
            }

            fileName += ".json";
            var jsonString = JsonConvert.SerializeObject(this);
            File.WriteAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"), fileName), jsonString);

            Console.WriteLine("Successfully saved game with progress");
        }

        /// <summary>
        /// Calculates the points for each word and adds it to the players total playerPoints
        /// </summary>
        public void PuzzleRank()
        {
            // Count of unique letters in the word
            int uniqueLetterCount = foundWords.Last().Distinct().Count(); 
            int wordLength = foundWords.Last().Length;
            int points = 0;

            if (wordLength == 4)
            {
                points = 1;
            }
            else if (wordLength == 5 || wordLength == 6)
            {
                points = wordLength;
            }
            else if (wordLength > 6)
            {
                points = wordLength + (uniqueLetterCount == 7 ? 7 : 0);
            }
            // Add the points to the player's total score.
            playerPoints += points;
        }

        /// <summary>
        /// Saves only the puzzle with no progress
        /// </summary>
        public void SavePuzzle()
        {
            // Creates the save folder if it doesnt exist
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"));

            Console.WriteLine("Enter new save name");
            string fileName = Console.ReadLine();

            if (fileName == null)
            {
                Console.WriteLine("Error: no file name");
                return;
            }

            fileName += ".json";
            Game temp = new Game();
            temp.requiredLetter = this.requiredLetter;
            temp.baseWord = this.baseWord;
            var jsonString = JsonConvert.SerializeObject(this);

            File.WriteAllText(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "saves\\"), fileName), jsonString);

            Console.WriteLine("Successfully saved puzzle");
        }

        /// <summary>
        /// Loads a saved game from a saved folder
        /// </summary>
        public void Load(ref Game game)
        {
            // Creates the save folder if it doesnt exist
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves"));
            var fileList = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "saves"));

            // Makes sure there are saved files
            if (fileList.Length == 0)
            {
                Console.WriteLine("No Saved Games");
                return;
            }

            Console.WriteLine("Which save file id would you like to load?");

            int i = 0;
            // Prints all files in the save folder
            foreach (string file in fileList)
            {
                Console.WriteLine(i + ": " + Path.GetFileNameWithoutExtension(file));
                ++i;
            }

            // Reads user input as an integer
            int fileId = Convert.ToInt32(Console.ReadLine());

            // If it is a valid id then it loads the game
            if (fileList.Length > fileId)
            {
                StreamReader fileContents = new StreamReader(File.OpenRead(fileList[fileId]));
                string openedFile = fileContents.ReadToEnd();
                game.Reset();

                game = JsonConvert.DeserializeObject<Game>(openedFile);
                game.GenerateValidWords();

                Console.WriteLine("This is the loaded puzzle");
                game.ShowPuzzle();
            }
            else
            {
                Console.WriteLine("Invalid file Id");
            }
        }

        /// <summary>
        /// Centers the text on the screen
        /// </summary>
        public void centerText(String text)
        {
            Console.Write(new string(' ', (Console.WindowWidth - text.Length) / 2));
            Console.WriteLine(text);
        }
    }
}
