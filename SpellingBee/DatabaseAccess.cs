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
    /// <c>DatabaseAccess</c> class.
    /// </summary>
    public class DatabaseAccess : IDatabase
    {
        public DatabaseAccess()
        {

        }

        private const string DatabaseConnectionString = "Data Source=../../../../SpellingBee/SetUpSpellingBee/Database/SpellingBeeWords.db;";
        private const string DatabaseConnectionString_Two = "Data Source=./SetUpSpellingBee/Database/SpellingBeeWords.db";

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
        public List<string> GenerateValidWords(List<char> baseWord, char requiredLetter)
        {
            List<string> validWords = new List<string>();
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

            return validWords;
        }
    }
}

