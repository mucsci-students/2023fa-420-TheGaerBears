using Avalonia;
using Avalonia.Controls.Documents;
using DynamicData.Kernel;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellingBee
{

    public class CliController : Controller
    {
        public GameModel _model;
        private readonly GameView _view;
        private string _lastMessage = "";

        private static CliController _instance;

        /// <summary>
        /// Initializes a new instance of the <c>CliController</c> class with the provided GameModel and GameView.
        /// </summary>
        private CliController(GameModel model, GameView view) : base(model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public static CliController GetInstance(GameModel model, GameView view)
        {
            if (_instance == null)
            {
                _instance = new CliController(model, view);
            }
            return _instance;
        }

        public GameModel GetModelInstance()
        {
            return _model;
        }

        public static CliController GetExistingInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Starts a new puzzle using random base word.
        /// </summary>
        public override void NewPuzzle()
        {
            _model.Reset();
            _model.SelectRandomWordForPuzzle();
            _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
            _model.GenerateValidWords();
        }

        /// <summary>
        /// Starts a new puzzle using a specified base word.
        /// </summary>
        public override void NewPuzzleBaseWord(string word)
        {
            Model temp = _model.SetBaseWordForPuzzle(word);
            if (temp.IsNull())
            {
                _view.DisplayMessage("This is not a valid pangram");
                return;
            }
            _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
            _model.GenerateValidWords();
        }

        /// <summary>
        /// Processes the guessed word by the user.
        /// </summary>
        public override void Guess(string word)
        {
            if (_model.IsValidWord(word))
            {
                if (_model.IsWordAlreadyFound(word))
                {
                    _lastMessage = $"You have already found the word \"{word}\"!";
                }
                else
                {
                    _lastMessage = "Word found!";
                    _model.AddFoundWord(word);
                    _view.DisplayScore(_model.GetPlayerPoints());
                }
            }
            else
            {
                _lastMessage = $"{word} is not a valid guess.";
            }

            _view.DisplayMessage(_lastMessage);
        }

        /// <summary>
        /// Shuffles the current base word and updates the view.
        /// </summary>
        public override void ShuffleBaseWord()
        {
            _model.ShuffleBaseWord();
            _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
        }

        /// <summary>
        /// Saves the current game state prompting the user for a filename.
        /// </summary>
        public override void SaveCurrent(string saveName)
        {
            if (_model.SaveCurrentGameState(saveName))
                _view.DisplayMessage("Successfully saved game with progress");
            else
                _view.DisplayMessage("Error: No file name");
        }

        /// <summary>
        /// Saves the current puzzle prompting the user for a filename.
        /// </summary>
        public override void SavePuzzle(string saveName)
        {
            if (_model.SaveCurrentPuzzleState(saveName))
                _view.DisplayMessage("Successfully saved game with progress");
            else
                _view.DisplayMessage("Error: No file name");
        }

        /// <summary>
        /// Loads a saved game from a list of available save files.
        /// </summary>
        public override void Load(string fileName)
        {
            var fileList = _model.GetAvailableSaveFiles();

            if (fileList.Count == 0)
            {
                _view.DisplayMessage("No Saved Games");
                return;
            }

            _view.DisplaySaveFilesList(fileList);

            int fileId = _view.GetFileIdFromUser();

            Model loadedGame = _model.LoadGameStateFromFile(fileId);

            if (!loadedGame.IsNull() )
            {
                _model.AssignFrom((GameModel) loadedGame);
                _view.DisplayMessage("This is the loaded puzzle");
                _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
            }
            else
            {
                _view.DisplayMessage("Invalid file Id");
            }
        }

        /// <summary>
        /// Submits high score for the current puzzle
        /// </summary>
        /// <param name="name"></param>
        public void submitHighScore(string name)
        {
            if (name != null && name != "" && !_model.IsNull())
            {
                HighScores scores = new HighScores();
                string temp = "";
                var word = GetBaseWord();
                for (int i = 0; i < word.Count; ++i)
                {
                    temp += word[i];
                }
                _view.DisplayMessage(scores.UpdateOrCreateHighScore(temp, name, _model.GetCurrentScore()).ToString());
            }
            else
            {
                _view.DisplayMessage("Please enter a valid name");
            }
        }

        /// <summary>
        /// Lists the high scores for the current puzzle
        /// </summary>
        public void viewHighScores()
        {
            HighScores scores = new HighScores();
            string temp = "";
            var word = GetBaseWord();
            for (int i = 0; i < word.Count; ++i)
            {
                temp += word[i];
            }

            List<KeyValuePair<string, int>> highScores = scores.GetHighScores(temp);

            if (highScores == null || highScores.Count == 0)
            {
                _view.DisplayMessage("There are no high scores for this game yet");
            }
            else
            {

                foreach (var score in highScores)
                {
                    string display = score.Value.ToString();
                    display = display.PadRight(10);
                    display += score.Key.ToString();
                    _view.DisplayMessage(display);
                }
            }

        }

        /// <summary>
        /// Displays the initial game screen.
        /// </summary>
        public void BeginScreen()
        {
            _view.BeginScreen();
        }

        /// <summary>
        /// Processes user input and executes the corresponding command.
        /// </summary>
        public void HandleCommand(string input)
        {
            switch (input)
            {
                case "-hint":
                    if (_model.Active())
                    {
                        //var hintToPrint = _model.LettersInWord();
                        //PrintHintTable(hintToPrint);
                        _view.DisplayMessage(_model.PrintHintTable());
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;
                case "-exit":
                    _view.Exit();
                    _model.Exit();
                    break;

                case "-help":
                    _view.Help();
                    break;

                case "-load":
                case "-load puzzle":
                    Load(new string("fileName"));
                    break;

                case "-new":
                case "-new game":
                    NewPuzzle();
                    break;

                case "-new game from word":
                    _view.DisplayMessage("Please enter a valid pangram: ");
                    string pang = _view.GetInput();
                    NewPuzzleBaseWord(pang);
                    break;

                case "-save current":
                    if (_model.Active())
                    {
                        _view.DisplayMessage("Enter Save File Name:");
                        string saveName = _view.GetInput();
                        SaveCurrent(saveName);
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-save puzzle":
                    if (_model.Active())
                    {
                        _view.DisplayMessage("Enter Save File Name:");
                        string saveName = _view.GetInput();
                        SavePuzzle(saveName);
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-found words":
                case "-show found words":
                    if (_model.Active())
                    {
                        _view.ShowFoundWords(_model.GetFoundWords());
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-puzzle":
                case "-show puzzle":
                    if (_model.Active())
                    {
                        _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-status":
                case "-show status":
                    if (_model.Active())
                    {
                        _view.ShowStatus(_model.GetPlayerPoints(), _model.GetMaxPoints(), _model.GetStatusTitles(), _model.GetAllRanks());
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-shuffle":
                    if (_model.Active())
                    {
                        ShuffleBaseWord();
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-save score":
                    if (_model.Active())
                    {
                        _view.DisplayMessage("Please enter a name for the score: ");
                        string name = _view.GetInput();
                        submitHighScore(name);
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-view scores":
                    if (_model.Active())
                    {
                        viewHighScores();
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                default:
                    if (input.Equals(""))
                    {
                        //Do Nothing
                    }
                    else if (_model.Active() && !input[0].Equals('-'))
                    {
                        Guess(input);
                    }
                    else
                    {
                        _view.DisplayMessage("This is not a valid command. Please use -help to see the list of commands.");
                    }
                    break;
            }
        }
    }
}
