// See https://aka.ms/new-console-template for more information
using System.Data.SQLite;
using System.Text.Json;
using System.IO;

namespace SpellingBee
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fileName = "C:\Users\skyfa\source\repos\SpellingBee\4-letter-words.json";

            if (File.Exists(fileName))
            {
                byte[] data = File.ReadAllBytes(fileName);
                Utf8JsonReader reader = new Utf8JsonReader(data);

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.StartObject:
                            Console.WriteLine("-------------");
                            break;
                        case JsonTokenType.EndObject:
                            break;
                        case JsonTokenType.StartArray:
                        case JsonTokenType.EndArray:
                            break;
                        case JsonTokenType.PropertyName:
                            Console.Write($"{reader.GetString()}: ");
                            break;
                        case JsonTokenType.String:
                            Console.WriteLine(reader.GetString());
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }

                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("The JSON file does not exist in the current directory.");
            }
        }



            /*//Starting with a single JSON file right now
            string cs = @"URI=file:";

            //Creating a SQLite connection object
            using var con = new SQLiteConnection(cs);
            //Open connection
            con.Open();

            //SQLiteCommmand is a obj used to execute query on db
            //param is connection object
            using var cmd = new SQLiteCommand(con);

            //If table already exists, do not create
            cmd.CommandText = "DROP TABLE IF EXISTS cars";
            //We do not want a result set for DROP/INSERT/DELETE statements.
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE four letter words(id INTEGER PRIMARY KEY,
                word TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO four letter words(word) VALUES(@word)";

            cmd.Parameters.AddWithValue("@word", "rock");
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Console.WriteLine("row inserted");*/
 
    }
}