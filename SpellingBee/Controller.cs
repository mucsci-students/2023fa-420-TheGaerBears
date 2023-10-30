using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpellingBee
{
    public abstract class Controller
    {
        public Model _model;

        public string _lastMessage = "";
        public Controller(Model model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public abstract void NewPuzzle();

        public abstract void NewPuzzleBaseWord(string word);

        public abstract void Guess(string word);

        /// <summary>
        /// Method <c>GetLastMessage</c> returns the string currently stored in <c>_lastMessage</c>. 
        /// <returns>The string message.</returns>
        /// </summary>
        public string GetLastMessage()
        {
            return _lastMessage;
        }

        public abstract void ShuffleBaseWord();

        /// <summary>
        /// Method <c>GetBaseWord</c> returns the baseword of the puzzle.
        /// /// <returns>The list of characters that make up the baseword.</returns>
        /// </summary>
        public List<char> GetBaseWord()
        {
            return _model.GetBaseWord();
        }

        public abstract void SaveCurrent(string saveName);

        public abstract void SavePuzzle(string saveName);

        public abstract void Load(string fileName);

    }
}
