using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellingBee
{
    /// <summary>
    /// Class <c>GuiController</c> is responsible for controlling the game flow and 
    /// interaction between the GameModel and the GUI.
    /// </summary>
    public class GuiController : Controller
    {
        // A GameModel object used by the GuiController.
        private readonly GameModel _model;

        // A string used to store a message and which is used by <c>MainWindowViewModel</c>.
        private string _lastMessage = "";

        /// <summary>
        /// This constructor initializes a <c>GuiController</c> from 
        /// a <c>GameModel</c> as long as model is not null.
        /// /// <param name="model">the GameModel to initialize the GuiController.</param>
        /// </summary>
        public GuiController(GameModel model) : base (model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// Method <c>NewPuzzle</c> starts a new puzzle game.
        /// <para>
        /// Calls <c>Reset</c> to clear progress, then
        /// gets, shuffles, and displays a valid pangram, then
        /// generates valid words to guess from.
        /// </para>
        /// </summary>
        public override void NewPuzzle()
        {
            _model.Reset();
            _model.SelectRandomWordForPuzzle();
            _model.GenerateValidWords();
        }

        /// <summary>
        /// Method <c>NewPuzzleBaseWord</c> starts a new puzzle game from an inputted baseword.
        /// <para>
        /// Calls <c>Reset</c> to clear progress, then
        /// gets, shuffles, and displays a valid pangram, then
        /// generates valid words from the new baseword to guess from.
        /// </para>
        /// <param name="word">the new baseword to start the puzzle.</param>
        /// </summary>
        public override void NewPuzzleBaseWord(string word)
        {
            if (!_model.SetBaseWordForPuzzle(word))
            {
                _lastMessage = "Not a valid pangram";
                return;
            }
            _lastMessage = "";
            _model.GenerateValidWords();
        }

        /// <summary>
        /// Method <c>Guess</c> allows the user to guess a word.
        /// <para>
        /// A new game must be started to guess. 
        /// If a guess is invalid, message "invalid guess" is displayed.
        /// If the guess is valid and not a duplicate: 
        /// the word is added to the <c>foundWords</c> list,
        /// the equivalent Points are added to <c>playerPoints</c>,
        /// and the Points used in the calculating <c>pointsToNextRank</c>.
        /// </para>
        /// <param name="word">the word guessed by the user.</param>
        /// </summary>
        public override void Guess(string word)
        {  
            if (_model.GetBaseWord().Count == 0)
            {
                _lastMessage = "Start a game first!";
            }
            else if (_model.IsValidWord(word))
            {
                if (_model.IsWordAlreadyFound(word))
                {
                    _lastMessage = $"You have already found the word \"{word}\"!";
                }
                else
                {
                    _lastMessage = "Word found!";
                    _model.AddFoundWord(word);
                }
            }
            else
            {
                _lastMessage = $"{word} is not a valid guess.";
            }
        }

        /// <summary>
        /// Method <c>GetLastMessage</c> returns the string currently stored in <c>_lastMessage</c>. 
        /// <returns>The string message.</returns>
        /// </summary>
        /*public string GetLastMessage()
        {
            return _lastMessage;
        }*/

        /// <summary>
        /// Method <c>ShuffleBaseWord</c> is used in the creation of a new game
        /// to shuffle the letters of the pangram before displaying them.
        /// A game must be started.
        /// </summary>
        public override void ShuffleBaseWord()
        {
            if (_model.GetBaseWord().Count == 0)
            {
                _lastMessage = "Start a game first!";
            }
            else
            {
                _lastMessage = "";
                _model.ShuffleBaseWord();
            }
        }

        /// <summary>
        /// Method <c>GameStarted</c> returns whether a game has started or not. 
        /// Used in <c>MainWindowViewModel</c> to ensure some methods are only
        /// ran when a game has started.
        /// <returns>True if the baseword has letters in it, false otherwise.</returns>
        /// </summary>
        public bool GameStarted()
        {
            return !(_model.GetBaseWord().Count == 0);
        }

        /// <summary>
        /// Method <c>GetBaseWord</c> returns the baseword of the puzzle.
        /// /// <returns>The list of characters that make up the baseword.</returns>
        /// </summary>
        /*public List<char> GetBaseWord()
        {
            return _model.GetBaseWord();
        }*/

        /// <summary>
        /// Method <c>Shuffle</c> swaps the letters of the baseword while
        /// a game is in progress. 
        /// A game must be started.
        /// </summary>
        public void Shuffle()
        {
            if (_model.GetBaseWord().Count == 0)
            {
                _lastMessage = "Start a game first!";
            }
            else
            {
                _model.ShuffleBaseWord();
            }
        }

        /// <summary>
        /// Method <c>SaveCurrent</c> allows the user to save the current game state.
        /// <para>
        /// The name of the save file is first typed in,
        /// and then the user clicks "Save Current" to save the game
        /// into that save file. 
        /// </para>
        /// <param name="saveName">the name of the save file the user types.</param>
        /// </summary>
        public override void SaveCurrent(string saveName)
        {
            if (_model.GetBaseWord().Count == 0)
            {
                _lastMessage = "Start a game first!";
            }
            else
            {
                if (!_model.SaveCurrentGameState(saveName))
                    _lastMessage = "Invalid save name";
                else
                    _lastMessage = "File saved";
            }
        }

        /// <summary>
        /// Gets the hint table from the model and retu
        /// </summary>
        /// <returns></returns>
        public void Hint()
        {
            if (_model.GetBaseWord().Count == 0)
            {
                _lastMessage = "Start a game first!";
            }
            else
            {
                _lastMessage = _model.PrintHintTable();
            }
        }

        /// <summary>
        /// Method <c>SavePuzzle</c> allows the user to save the current puzzle without progress.
        /// <para>
        /// The name of the save file is first typed in,
        /// and then the user clicks "Save Puzzle" to save the game
        /// into that save file. 
        /// </para>
        /// <param name="saveName">the name of the save file the user types.</param>
        /// </summary>
        public override void SavePuzzle(string saveName)
        {
            if (_model.GetBaseWord().Count == 0)
            {
                _lastMessage = "Start a game first!";
            }
            else
            {
                

                if (!_model.SaveCurrentPuzzleState(saveName))
                    _lastMessage = "Invalid save name";
                else
                    _lastMessage = "File saved";
            }
        }

        /// <summary>
        /// Method <c>Load</c> allows the user to load a saved puzzle or game.
        /// <param name="saveName">the name of the saved file.</param>
        /// </summary>
        public override void Load(string fileName)
        {
            int fileId = -1;
            var files = _model.GetAvailableSaveFiles();
            for (int i = 0; i < files.Count; i++)
            {
                if (System.IO.Path.GetFileNameWithoutExtension(files[i]).Equals(System.IO.Path.GetFileNameWithoutExtension(fileName)))
                {
                    fileId = i;
                    break;
                }
            }

            GameModel loadedGame = _model.LoadGameStateFromFile(fileId);
            
            if (loadedGame != null)
            {
                _model.AssignFrom(loadedGame);
                _lastMessage = "File Loaded";
            }
            else
            {
                _lastMessage = "Invalid file Id";
            }
        }

        /// <summary>
        /// Method <c>GetFoundWords</c> returns a list of the valid words guessed so far.
        /// <return>A list of the valid found words.</return>
        /// </summary>
        public List<string> GetFoundWords()
        { 
            return _model.GetFoundWords().ToList();
        }

        /// <summary>
        /// Method <c>GetCurrentRank</c> returns the user's Rank calculated from 
        /// their <c>playerPoints</c> and the <c>MaxPoints</c> of the current puzzle.
        /// <returns>A string representing their current Rank.</returns>
        /// </summary>
        public string GetCurrentRank()
        {   
            int currentPoints = _model.GetPlayerPoints();
            string rank = "Beginner";
            foreach (var rankPair in _model.GetStatusTitles())
            {
                if (currentPoints >= (int)(rankPair.Value * _model.GetMaxPoints() * .01))
                {
                    rank = rankPair.Key;
                }
            }
            return rank;
        }

        /// <summary>
        /// Method <c>Help</c> displays an explanation of the available buttons.
        /// <return>The description of the buttons.</return>
        /// </summary>
        public string GetHelp()
        {
            //Warning do not add tabs or it will make the display incorrect! 
            return @"Welcome to the Spelling Bee game!

New Game: Starts a new puzzle game.
New Game From Word: Starts a new puzzle game with the word currently written at the bottom of the screen.
Load Game: Load a saved game or puzzle.
Save Word: Save the current game with progress with the name currently written at the bottom of the screen.
Save Puzzle: Save the current puzzle with progress with the name currently written at the bottom of the screen. 
Found Words: Display the words you have found.
Shuffle: Shuffle the puzzle letters.
Help: Show this list of commands.
Toggle Colorblind: Adjusts game to be more Red-Green colorblind friendly!

You can also simply type in a word to make a guess.
Remember, all words must contain the required letter!";
        }

        //// Method can be removed during after next code review, since it is not being implemented
        /// originally implemented <c>MainWindowViewModel</c>
        public string GetNthLetter(int n)
        {
            return _model.GetBaseWord()[n].ToString();
        }

        /// <summary>
        /// Method <c>GetCurrentScore</c> is used in <c>MainWindowViewModel</c> to update the
        /// player's current score.
        /// <return>The int current score.</return>
        /// </summary>
        public int GetCurrentScore()
        {
            return _model.GetCurrentScore();
        }

        /// <summary>
        /// Method <c>GetNextRank</c> is used in <c>MainWindowViewModel</c> to update the
        /// player's Points until their next Rank.
        /// <return>The int Points until the next Rank.</return>
        /// </summary>
        public int GetNextRank()
        {
            return _model.PointsToNextRank();
        }
    }
}
