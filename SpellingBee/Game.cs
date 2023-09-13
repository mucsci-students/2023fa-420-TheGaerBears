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
        private List<char> baseWord;
        private char requiredLetter;
        private Random rand;
        [JsonIgnore] private List<string> validWords;
        private List<string> foundWords;
        private List<KeyValuePair<string, int>> statusTitles;
        private int playerPoints;
        private int totalPossiblePoints;
        private List<string> PangramWords;

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

        public void GenerateValidWords()
        {
            List<string> tableNames = new List<string> {"four_letter_words",
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
                                                           "fifteen_letter_words"};

            StringBuilder queryBuilder = new StringBuilder();
            foreach (string tableName in tableNames)
            {
                queryBuilder.AppendLine($"SELECT word FROM {tableName} WHERE word LIKE '%{requiredLetter}%' AND word NOT GLOB '*[^{baseWord}]*'");

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
        }

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
        /// Selects a random Pangram
        /// </summary>
        public void NewPuzzle()
        {
            //Picks a random pangram
            string selectedWord = PangramWords[rand.Next(PangramWords.Count())];


            char[] q = selectedWord.Distinct().ToArray();
            baseWord = new List<char>(q);

            //Shuffles the letters without printing them
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
            //Choose required letter
            requiredLetter = q[0];
            ShowPuzzle();
            GenerateValidWords();
        }

        /// <summary>
        /// Takes a starting word for the puzzle and checks its validity
        /// If valid it starts the puzzle with the pangrams letters
        /// If not it asks for a new word
        /// </summary>
        public void NewPuzzleBaseWord(string word)
        {
            string bWord = word.ToLower();
            while (!PangramWords.Contains(bWord))
            {
                Console.WriteLine("This word is not valid. Please enter a new word: ");
                bWord = Console.ReadLine().ToLower();
            }

            char[] q = bWord.Distinct().ToArray();
            baseWord = new List<char>(q);

            //Shuffles the letters without printing them
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
            //Choose required letter
            requiredLetter = q[0];
            GenerateValidWords();
            ShowPuzzle();
            foundWords.Add(bWord);
            PuzzleRank();
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
            Console.WriteLine("Shuffled letters:");

            foreach (char letter in baseWord)
            {
                Console.Write(letter + " ");
            }
            Console.WriteLine("\n");
        }
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
        /// if valid saves it in foundWords, if invalid shows corresponding error message
        /// </summary>
        public void Guess()
        {
            Console.WriteLine("Enter a word: ");
            try
            {
                String guess = Console.ReadLine();
            
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

        
        public void DisplayScore()
        {
            Console.WriteLine($"Your current score is: {playerPoints}");
        }
        public void SaveCurrent()
        {
            //Creates the save folder if it doesnt exist
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
        }

    }
}
