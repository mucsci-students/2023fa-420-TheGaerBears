using System.Data.SQLite;
using System.Text.Json;
using System.IO;

namespace SpellingBee
{
    internal class ParsingJson
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

    }
}