using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;


namespace SpellingBee
{
	public class CreatePuzzle
	{
        public static char[] NewPuzzle(string baseWord = "-")
        {
            string selectedWord = "";
            //string query = "SELECT COUNT() FROM table_name WHERE word = '{baseWord}';";
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
                                string word = reader.GetString(1);
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

            if (baseWord.Equals("-"))
            {
                //Picks a random pangram
                Random rand = new Random();
                selectedWord = words[rand.Next(words.Count())];
            }
            else
            {
                selectedWord = NewPuzzleBaseWord(baseWord, words);
            }
            char[] q = selectedWord.Distinct().ToArray();
            Array.Sort(q);
            string qPangram = new string(q);


            return shuffle(qPangram).ToCharArray();
        }

        public static string shuffle(string selectedWord)
        {
            //Shuffles the word when returning it
            string shuffled;
            do
            {
                shuffled = new string(
                                        selectedWord
                                            .OrderBy(character => Guid.NewGuid())
                                            .ToArray()
                                        );
            } while (shuffled == selectedWord);
            return shuffled;
        }
        /*public static string NewPuzzle(string baseWord = "-")
        {
            string selectedWord;
            if (baseWord.Equals("-"))
            {


                //List of tablenames
                List<string> tableNames = new List<string> {"seven_letter_words", "eight_letter_words", "nine_letter_words",
                "ten_letter_words", "eleven_letter_words", "twelve_letter_words", "thirteen_letter_words",
                "fourteen_letter_words", "fifteen_letter_words" };

                //SQL code to get list of all pangrams in dictionary
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append($"WITH RECURSIVE SplitLetters AS (SELECT 1 AS n UNION ALL SELECT n + 1 FROM SplitLetters WHERE n < 15)");
                foreach (string tablename in tableNames)
                {
                    queryBuilder.AppendLine($"SELECT DISTINCT word, GROUP_CONCAT(DISTINCT SUBSTRING(word, n, 1)) AS word_no_duplicates FROM {tablename} JOIN " +
                        $"SplitLetters ON n <= LENGTH(word) GROUP BY word HAVING LENGTH(word_no_duplicates) = 13");

                    if (tableNames.IndexOf(tablename) < tableNames.Count - 1)
                    {
                        queryBuilder.AppendLine("UNION");
                    }
                }

                string query = queryBuilder.ToString();

                //string connectionString = "Data Source=C:\\Users\\noahi\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";
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
                                    string word = reader.GetString(1);
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

                //Picks a random pangram
                Random rand = new Random();
                selectedWord = words[rand.Next(words.Count())];

                //Removes commas from string
                selectedWord = selectedWord.Replace(",", string.Empty);
            }
            else
            {
                selectedWord = NewPuzzleBaseWord(baseWord);
            }
            //Shuffles the word when returning it
            string shuffled;
            do
            {
                shuffled = new string(
                                        selectedWord
                                            .OrderBy(character => Guid.NewGuid())
                                            .ToArray()
                                        );
            } while (shuffled == selectedWord);

            return shuffled;
        }*/

        public static string NewPuzzleBaseWord(string baseWord, List<string> words)
        {
            string bWord = baseWord;
            while (!words.Contains(bWord))
            {
                Console.WriteLine("This word is not valid. Please enter a new word: ");
                bWord = Console.ReadLine().ToLower();
            }
            return bWord;
        }

        /*public static string NewPuzzleFromBaseWord(string baseWord)
        {
            //Check if word is pangram
            //List of tablenames
            List<string> tableNames = new List<string> {"seven_letter_words", "eight_letter_words", "nine_letter_words",
                "ten_letter_words", "eleven_letter_words", "twelve_letter_words", "thirteen_letter_words",
                "fourteen_letter_words", "fifteen_letter_words" };

            //SQL code to get list of all pangrams in dictionary
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append($"WITH RECURSIVE SplitLetters AS (SELECT 1 AS n UNION ALL SELECT n + 1 FROM SplitLetters WHERE n < 16)");
            foreach (string tablename in tableNames)
            {
                queryBuilder.AppendLine($"SELECT DISTINCT word, GROUP_CONCAT(DISTINCT SUBSTRING(word, n, 1)) AS word_no_duplicates FROM {tablename} JOIN " +
                    $"SplitLetters ON n <= LENGTH(word) GROUP BY word HAVING LENGTH(word_no_duplicates) = 13");

                if (tableNames.IndexOf(tablename) < tableNames.Count - 1)
                {
                    queryBuilder.AppendLine("UNION");
                }
            }

            string query = queryBuilder.ToString();

            //string connectionString = "Data Source=C:\\Users\\noahi\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";
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
                                string word = reader.GetString(1);
                                word = word.Replace(",", string.Empty);
                                char[] characters = word.ToCharArray();
                                Array.Sort(characters);
                                words.Add(new string(characters));
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            //Changes the entered word into the pangram version of itself
            char[] q;
            string qPangram = "";

            //Checks the pangram list to see if the entered word is in it
            while (!words.Contains(qPangram.ToLower()))
            {
                Console.WriteLine("This word is not valid. Please enter a new word: ");
                string bWord = Console.ReadLine().ToLower();
                q = bWord.Distinct().ToArray();
                Array.Sort(q);
                qPangram = new string(q);
            }
            return qPangram;
        }*/

        public static void ShowFoundWords(List<string> foundWords)
        {
            Console.WriteLine("The words you have found are:");
            foreach (string word in foundWords)
            {
                if (foundWords.IndexOf(word) < foundWords.Count - 1)
                    Console.Write(word + ", ");
                
                else
                    Console.WriteLine(word);
            }
        }
    }
}
