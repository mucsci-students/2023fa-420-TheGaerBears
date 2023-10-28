using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellingBee
{
    /// <summary>
    /// <c>NullModel</c> class used to return a nullModel object or to return other error
    /// values from functions in order to avoid returning null.
    /// </summary>
    public class NullModel : GameModel
    {
        public NullModel() 
        {
        }
        /// <summary>
        /// Retrieves the current score of the player.
        /// </summary>
        public override int GetCurrentScore()
        {
            return -1;
        }

        public override List<string> PangramList()
        {
            return new List<string>();
        }

        public override int PointsToNextRank()
        {
            return -1;
        }
        public override string PrintHintTable()
        {
            return "this is null.";
        }

        /// <summary>
        /// Retrieves the valid words for the current puzzle.
        /// </summary>
        public override List<String> GetValidWords()
        {
            return new List<String>();
        }

        public override int GetMaxPoints()
        {
            return -1;
        }

        public override List<string> GetAvailableSaveFiles()
        {
            return new List<string>();
        }

        public override GameModel LoadGameStateFromFile(int fileId)
        {
            return new NullModel();

        }
        /// <summary>
        /// Retrieves the base word of the current puzzle.
        /// </summary>
        public override List<char> GetBaseWord()
        {
            return new List<char>();
        }

        /// <summary>
        /// Retrieves the required letter of the current puzzle.
        /// </summary>
        public override char GetRequiredLetter()
        {
            return '!';
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
            return new List<String>();
        } 

    }
}
