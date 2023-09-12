using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpellingBee.SetUpSpellingBee.src.main
{
    public class CreateDatabase
    {
        public string word { get; set; }
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
                                                           "fifteen_letter_words"};

        public void AddPangramTable(string tableName)
        {
            string connectionString = "Data Source=C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

            using (SqliteConnection con = new SqliteConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
                        cmd.Parameters.AddWithValue("@tableName", tableName);

                        var existingTable = cmd.ExecuteScalar();
                        List<string> wordsToAdd = new List<string>();
                        if (existingTable == null)
                        {
                            // The table doesn't exist, create it
                            cmd.CommandText = $"CREATE TABLE {tableName} (id INTEGER PRIMARY KEY, word TEXT)";
                            cmd.ExecuteNonQuery();
                        }

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

                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string word = reader["word"].ToString();
                                wordsToAdd.Add(word);
                            }
                        }

                        string insert = "INSERT INTO pangrams (word) VALUES(@word)";
                        foreach (string pangram in wordsToAdd)
                        {
                            using (var insertCmd = new SqliteCommand(insert, con))
                            {
                                insertCmd.Parameters.AddWithValue("@word", pangram); // Use correct parameter name
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions here, e.g., log the error or display a message
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
                finally
                {
                    con.Close(); // Ensure connection is closed
                }
            }
        }

        public void PrintCount(string tableName)
        {
            string connectionString = "Data Source=C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

            using (SqliteConnection con = new SqliteConnection(connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                    var count = (long)cmd.ExecuteScalar();
                    Console.WriteLine($"Number of records in '{tableName}': {count}");
                }
            }
        }
        public void PrintContents(string tableName)
        {
            string connectionString = "Data Source=C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

            using (SqliteConnection con = new SqliteConnection(connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {

                    cmd.CommandText = $"SELECT * FROM {tableName}";
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"Contents of '{tableName}' table:");

                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader.GetInt32(0)}, Word: {reader.GetString(1)}");
                        }
                    }
                }
            }
        }

        public void LoadDataFromJsonFile(string jsonFilePath, string tableName)
        {
            // Read JSON file into a string
            string jsonString = File.ReadAllText(jsonFilePath);
            List<CreateDatabase> wordEntries = JsonConvert.DeserializeObject<List<CreateDatabase>>(jsonString);

            string connectionString = "Data Source=C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";

            using (SqliteConnection con = new SqliteConnection(connectionString))
            {
                con.Open();

                // Check if the table exists and create it if not
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
                    cmd.Parameters.AddWithValue("@tableName", tableName);

                    var existingTable = cmd.ExecuteScalar();

                    if (existingTable == null)
                    {
                        // The table doesn't exist, create it
                        cmd.CommandText = $"CREATE TABLE {tableName} (id INTEGER PRIMARY KEY, word TEXT)";
                        cmd.ExecuteNonQuery();
                    }
                }

                //Batching insertions of data
                int batchSize = 500;

                for (int i = 0; i < wordEntries.Count; i += batchSize)
                {
                    using (var transaction = con.BeginTransaction())
                    {
                        for (int j = i; j < Math.Min(i + batchSize, wordEntries.Count); j++)
                        {
                            using (var cmd = con.CreateCommand())
                            {
                                // Use INSERT OR IGNORE to avoid inserting duplicate words
                                cmd.CommandText = $"INSERT OR IGNORE INTO {tableName} (word) VALUES (@word)";
                                cmd.Parameters.AddWithValue("@word", wordEntries[j].word);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Commit the transaction to improve performance
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
