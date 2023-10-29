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
    /// <c>NullModel</c> class used to return a nullModel object or to return other error
    /// values from functions in order to avoid returning null.
    /// </summary>
    public class NullModel : Model
    {
        public NullModel()
        {
            baseWord = new List<char>();
            foundWords = new List<string>();
            // Starting player points.
            playerPoints = 0;
            // Initial total possible points.
            maxPoints = 0;
            requiredLetter = '-';
        }

        /// <summary>
        /// Retrieves the current score of the player.
        /// </summary>
        public override int GetCurrentScore()
        {
            return -1;
        }

        /// <summary>
        /// Generates valid words for the current puzzle based on the database.
        /// </summary>
        public override void GenerateValidWords()
        { }

        /// <summary>
        /// Selects a random word to be used as the base for the puzzle.
        /// </summary>
        public override void SelectRandomWordForPuzzle()
        { }

        /// <summary>
        /// Sets the base word for the puzzle from a given word.
        /// </summary>
        public override Model SetBaseWordForPuzzle(string word)
        {
            return this;
        }

        /// <summary>
        /// Shuffles the letters of the base word. Also makes sure that the required letter is the first element.
        /// </summary>
        public override void ShuffleBaseWord()
        { }

        /// <summary>
        /// Checks if a given word is valid for the current puzzle.
        /// </summary>
        public override bool IsValidWord(string word)
        { 
            return false;
        }

        /// <summary>
        /// Determines if a word has already been found by the player.
        /// </summary>
        public override bool IsWordAlreadyFound(string word)
        {
            return false;
        }

        /// <summary>
        /// Adds a found word to the list of discovered words and updates player's points.
        /// </summary>
        public override void AddFoundWord(string word)
        { }

        /// <summary>
        /// Calculates the points required for the player to achieve the next rank.
        /// </summary>
        public override int GetNextRankThreshold()
        {
            return -1;
        }

        /// <summary>
        /// Determines if a puzzle has been started.
        /// </summary>
        public override bool Active()
        {
            return false;
        }

        /// <summary>
        /// Saves the current game state to a specified file.
        /// </summary>
        public override bool SaveCurrentGameState(string saveName)
        {
            return false;
        }

        /// <summary>
        /// Saves the current puzzle to a specified file.
        /// </summary>
        public override bool SaveCurrentPuzzleState(string saveName)
        {
            return false;
        }

        /// <summary>
        /// Assigns properties and fields from a loaded game to the current instance.
        /// </summary>
        public override void AssignFrom(Model loadedGame)
        { }

        /// <summary>
        /// Returns true if the player has found all of the possible valid words
        /// of the puzzle.
        /// <return>True if foundWords == validWords, false otherwise.</return>
        /// </summary>
        public override bool wonTheGame()
        {
            return false;
        }

        /// <summary>
        /// Retrieves the required letter of the current puzzle.
        /// </summary>
        public override char GetRequiredLetter()
        {
            return '-';
        }

        /// <summary>
        /// Retrieves the player's current points.
        /// </summary>
        public override int GetPlayerPoints()
        {
            return -1;
        }

        /// <summary>
        /// Retrieves the status titles with their associated point thresholds.
        /// </summary>
        public override List<KeyValuePair<string, int>> GetStatusTitles()
        {
            return new List<KeyValuePair<string, int>>();
        }

        /// <summary>
        /// Retrieves a list of words found by the player.
        /// </summary>
        public override IEnumerable<string> GetFoundWords()
        {
            return new List<string>();
        }

        /// <summary>
        /// Retrieves the maximum points achievable for the current puzzle.
        /// </summary>
        public override int GetMaxPoints()
        {
            return -1;
        }

        /// <summary>
        /// Retrieves the valid words for the current puzzle.
        /// </summary>
        public override List<String> GetValidWords()
        {
            return new List<string>();
        }

        /// <summary>
        /// Calculates the ranks and the points required to reach them
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, int> GetAllRanks()
        {
            return new Dictionary<string, int>();
        }

        /// <summary>
        /// Creates a string for the hint
        /// </summary>
        /// <returns>
        /// A string that contains the hint information
        /// </returns>
        public override string PrintHintTable()
        {
            return "";
        }

        public override bool IsNull()
        {
            return true;
        }

    }
}
