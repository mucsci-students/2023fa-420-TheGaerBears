using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpellingBee.SetUpSpellingBee.src.main
{
    public class CreateDatabase
    {
        public string word { get; set; }
    
        public void LoadDataFromJsonFile(string jsonFilePath, string tableName)
        {
            // Read JSON file into a string
            string jsonString = File.ReadAllText(jsonFilePath);
            List<CreateDatabase> wordEntries = JsonConvert.DeserializeObject<List<CreateDatabase>>(jsonString);


            //"Data Source=C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\Database\\SpellingBeeWords.db;";
            // Define the connection string
            //C:\\Path\\To\\Your\\Database\\SpellingBeeWords.db;
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

                // Count records in the table
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                    var count = (long)cmd.ExecuteScalar();
                    Console.WriteLine($"Number of records in '{tableName}': {count}");

                    // You can uncomment this section to display table contents
                    /*cmd.CommandText = $"SELECT * FROM {tableName}";
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"Contents of '{tableName}' table:");

                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader.GetInt32(0)}, Word: {reader.GetString(1)}");
                        }
                    }*/
                }
            }
        }
    }
}
