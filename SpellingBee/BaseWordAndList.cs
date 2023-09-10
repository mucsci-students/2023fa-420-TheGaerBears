using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace SpellingBee
{
    public class BaseWordAndList
    {
        public string IdentifyBaseWord(char mustUseLetter)
        {
            Random random = new Random();
            string pangram = mustUseLetter.ToString();

            string letters = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 1; i < 7; i++)
            {
                char randomLetter = letters[random.Next(letters.Length)];
                pangram += randomLetter;
            }
            return pangram;
        }

        public List<string> GenerateWordList(string pangram, int minLength = 4)
        {
            List<string> words = new List<string>();
            if (string.IsNullOrEmpty(pangram))
            {
                throw new ArgumentException("Pangram cannot be null or empty.", nameof(pangram));
            }

            string query = $"SELECT word FROM your_table WHERE LENGTH(word) >= {minLength} AND word LIKE '{pangram}%'";
            string connectionString = "Data Source=C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

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
        public bool IsInputValid(string userInput, List<string> wordList)
        {
            return wordList.Contains(userInput);
        }
    }
}
