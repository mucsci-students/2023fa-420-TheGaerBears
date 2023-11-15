using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpellingBee
{
    internal class HighScores
    {
        private const string HighScoresDir = "HighScores/";

        public HighScores()
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), HighScoresDir));
        }

        private string FormatBaseWordForFileName(string baseWord)
        {
            char firstLetter = baseWord[0];
            char[] characters = baseWord.Substring(1).ToCharArray();
            Array.Sort(characters);
            string sortedRest = new string(characters);

            return firstLetter + sortedRest;
        }

        public string UpdateOrCreateHighScore(string baseWord, string playerName, int score)
        {

            string formattedBaseWord = FormatBaseWordForFileName(baseWord);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), HighScoresDir, formattedBaseWord + ".json");

            List<KeyValuePair<string, int>> highScores;

            if (File.Exists(filePath))
            {
                // Read existing scores
                string json = File.ReadAllText(filePath);
                highScores = JsonConvert.DeserializeObject<List<KeyValuePair<string, int>>>(json);
            }
            else
            {
                // If no file exists, create a new list
                highScores = new List<KeyValuePair<string, int>>();
            }

            // Update the list with the new score
            highScores = UpdateHighScores(highScores, new KeyValuePair<string, int>(playerName, score));

            // Save the updated scores back to the file
            string updatedJson = JsonConvert.SerializeObject(highScores, Formatting.Indented);
            File.WriteAllText(filePath, updatedJson);

            string output = "High Scores!\n";
            foreach (KeyValuePair<string,int> pair in highScores)
            {
                output += pair.Key + ": " + pair.Value + "\n";
            }
            return output;
        }


        private List<KeyValuePair<string, int>> UpdateHighScores(List<KeyValuePair<string, int>> highScores, KeyValuePair<string, int> newScore)
        {
            // Add the new score
            highScores.Add(newScore);

            // Order by score in descending order, then by name if scores are equal
            highScores = highScores.OrderByDescending(pair => pair.Value).ThenBy(pair => pair.Key).ToList();

            // If we exceed the number of high scores, remove the last
            if (highScores.Count > 10)
            {
                highScores.RemoveAt(highScores.Count - 1);
            }
            return highScores;
        }

        public List<KeyValuePair<string, int>> GetHighScores(string baseWord)
        {
            string formattedBaseWord = FormatBaseWordForFileName(baseWord);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), HighScoresDir, formattedBaseWord + ".json");

            List<KeyValuePair<string, int>> highScores;

            if (File.Exists(filePath))
            {
                // Read existing scores
                string json = File.ReadAllText(filePath);
                highScores = JsonConvert.DeserializeObject<List<KeyValuePair<string, int>>>(json);
            }
            else
            {
                // If no file exists, create a new list
                highScores = new List<KeyValuePair<string, int>>();
            }

            return highScores;
        }
    }
}
