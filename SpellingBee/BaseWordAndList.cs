using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace SpellingBee
{
    public class BaseWordAndList
    {
        public List<string> tableNames = new List<string> {"four_letter_words",
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
                                                           "fifteen_letter_words",};
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

        public List<string> GenerateWordList(char mustUseLetter, string pangram, int minLength = 4)
        {
            List<string> words = new List<string>();
            if (!char.IsLetter(mustUseLetter))
            {
                throw new ArgumentException("Must use letter must be a valid letter.", nameof(mustUseLetter));
            }

            StringBuilder queryBuilder = new StringBuilder();
            foreach (string tableName in tableNames)
            {
                queryBuilder.AppendLine($"SELECT word FROM {tableName} WHERE LENGTH(word) >= {minLength} AND word LIKE '%{mustUseLetter}%' AND word GLOB '*[{pangram}]*'");

                // Add UNION between queries, except for the last one
                if (tableNames.IndexOf(tableName) < tableNames.Count - 1)
                {
                    queryBuilder.AppendLine("UNION");
                }
            }

            string query = queryBuilder.ToString();

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

    }
}
