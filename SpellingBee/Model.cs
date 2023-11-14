using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.IO;
using Avalonia.Media.TextFormatting;
using DynamicData.Aggregation;
using System.Security.Cryptography;

namespace SpellingBee
{
    /// <summary>
    /// <c>Game Model</c> class.
    /// </summary>
    public abstract class Model
    {
        [JsonProperty] public List<char> baseWord;
        [JsonProperty] public List<string> foundWords;
        [JsonProperty] public int playerPoints;
        [JsonProperty] public char requiredLetter;
        [JsonProperty] public int maxPoints;
        [JsonProperty] public List<string> wordlist;
        [JsonProperty] public string author;
        [JsonProperty] public string encrypted;

        /// <summary>
        /// Retrieves the current score of the player.
        /// </summary>
        public abstract int GetCurrentScore();

        /// <summary>
        /// Exits the game application.
        /// </summary>
        public void Exit()
        {
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Generates valid words for the current puzzle based on the database.
        /// </summary>
        public abstract void GenerateValidWords();

        /// <summary>
        /// Selects a random word to be used as the base for the puzzle.
        /// </summary>
        public abstract void SelectRandomWordForPuzzle();

        /// <summary>
        /// Sets the base word for the puzzle from a given word.
        /// </summary>
        public abstract Model SetBaseWordForPuzzle(string word);

        /// <summary>
        /// Shuffles the letters of the base word. Also makes sure that the required letter is the first element.
        /// </summary>
        public abstract void ShuffleBaseWord();

        /// <summary>
        /// Checks if a given word is valid for the current puzzle.
        /// </summary>
        public abstract bool IsValidWord(string word);

        /// <summary>
        /// Determines if a word has already been found by the player.
        /// </summary>
        public abstract bool IsWordAlreadyFound(string word);

        /// <summary>
        /// Adds a found word to the list of discovered words and updates player's points.
        /// </summary>
        public abstract void AddFoundWord(string word);

        /// <summary>
        /// Resets the game model to its initial state.
        /// </summary>
        public virtual void Reset()
        {
            baseWord.Clear();
            foundWords.Clear();
            playerPoints = 0;
            maxPoints = 0;
            requiredLetter = (char)0;
        }

        /// <summary>
        /// Calculates the points required for the player to achieve the next rank.
        /// </summary>
        public abstract int GetNextRankThreshold();

        /// <summary>
        /// Determines if a puzzle has been started.
        /// </summary>
        public abstract bool Active();

        /// <summary>
        /// Saves the current game state to a specified file.
        /// </summary>
        public abstract bool SaveCurrentGameState(string saveName);

        /// <summary>
        /// Saves the current puzzle to a specified file.
        /// </summary>
        public abstract bool SaveCurrentPuzzleState(string saveName);


        /// <summary>
        /// Retrieves a list of available save files.
        /// </summary>
        public List<string> GetAvailableSaveFiles()
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "saves/"));
            return Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "saves/")).ToList();
        }

        /// <summary>
        /// Loads a game state from a specified file ID.
        /// </summary>
        public Model? LoadGameStateFromFile(int fileId)
        {
            var fileList = GetAvailableSaveFiles();
            if (fileList.Count > fileId)
            {
                string filePath = fileList[fileId];
                string jsonData = File.ReadAllText(filePath);
				GameModel loadedGame = JsonConvert.DeserializeObject<GameModel>(jsonData);
				if (loadedGame.author != "GaerBears" && loadedGame.encrypted == "secretwordlist")
				{
                    return new NullModel();
				}
				if (loadedGame.encrypted == "secretwordlist")
				{
					for (int i = 0; i < loadedGame.wordlist.Count; i++)
                    {
                        string output = "";
                        foreach (char c in loadedGame.wordlist[i])
                        {
                            output += (char)((((c + 13) - 'a') % 26) + 'a');
                        }
                        loadedGame.wordlist[i] = output;
                    }
				}
                
                return loadedGame;
            }
            // File not found or invalid ID.
            return new NullModel();
        }

        /// <summary>
        /// Assigns properties and fields from a loaded game to the current instance.
        /// </summary>
        public abstract void AssignFrom(Model loadedGame);

        /// <summary>
        /// Returns true if the player has found all of the possible valid words
        /// of the puzzle.
        /// <return>True if foundWords == validWords, false otherwise.</return>
        /// </summary>
        public abstract bool wonTheGame();

        /// <summary>
        /// Retrieves the base word of the current puzzle.
        /// </summary>
        public List<char> GetBaseWord()
        {
            return baseWord;
        }

        /// <summary>
        /// Retrieves the required letter of the current puzzle.
        /// </summary>
        public abstract char GetRequiredLetter();

        /// <summary>
        /// Retrieves the player's current points.
        /// </summary>
        public abstract int GetPlayerPoints();

        /// <summary>
        /// Retrieves the status titles with their associated point thresholds.
        /// </summary>
        public abstract List<KeyValuePair<string, int>> GetStatusTitles();

        /// <summary>
        /// Retrieves a list of words found by the player.
        /// </summary>
        public abstract IEnumerable<string> GetFoundWords();

        /// <summary>
        /// Retrieves the maximum points achievable for the current puzzle.
        /// </summary>
        public abstract int GetMaxPoints();

        /// <summary>
        /// Retrieves the valid words for the current puzzle.
        /// </summary>
        public abstract List<String> GetValidWords();

        /// <summary>
        /// Calculates the ranks and the points required to reach them
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<string, int> GetAllRanks();

        /// <summary>
        /// Creates a string for the hint
        /// </summary>
        /// <returns>
        /// A string that contains the hint information
        /// </returns>
        public abstract string PrintHintTable();

        /// <summary>
        /// Checks to see if this is the null version of the model
        /// </summary>
        /// <returns></returns>
        public abstract bool IsNull();

    }
}

