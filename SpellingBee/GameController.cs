using System;

namespace SpellingBee
{
/*
 * GameController Class - SpellingBee
 * 
 * Purpose:
 * --------
 * This class is responsible for controlling the game flow and interaction between the GameModel and GameView.
 * It acts as a mediator that handles user commands, processes game logic, and updates the view accordingly.
 * 
 * Main Features:
 * --------------
 * - BeginScreen: Displays the initial screen of the game.
 * - NewPuzzle: Initiates a new puzzle with random pangram word.
 * - NewPuzzleBaseWord: Initiates a new puzzle with a given base word.
 * - Guess: Allows the user to guess a word and checks its validity.
 * - Shuffle: Shuffles the letters in the current puzzle.
 * - SaveCurrent: Saves the current game state to a file.
 * - SavePuzzle: Saves only the current puzzle (without found words) to a file.
 * - Load: Loads a saved game or puzzle state from a file.
 * - HandleCommand: Processes various user commands like starting a new game, saving/loading, shuffling letters, and guessing words.
 * 
 * Dependencies:
 * -------------
 * - GameModel: Holds the data and game logic.
 * - GameView: Responsible for displaying game data to the user.
 * 
 * Usage:
 * ------
 * This class is typically instantiated with a GameModel and GameView, and then it listens to user input to handle various game commands.
 */

    internal class GameController
    {
        private readonly GameModel _model;
        private readonly GameView _view;

        public GameController(GameModel model, GameView view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public void NewPuzzle()
        {
            _model.Reset();
            _model.SelectRandomWordForPuzzle();
            _view.ShowPuzzle(_model.BaseWord, _model.RequiredLetter);
            _model.GenerateValidWords();
        }

        public void NewPuzzleBaseWord(string word)
        {
            _model.Reset();
            _model.SetBaseWordForPuzzle(word);
            _view.ShowPuzzle(_model.BaseWord, _model.RequiredLetter);
            _model.GenerateValidWords();
        }

        public void Guess(string word)
        {
            if (_model.IsValidWord(word))
            {
                if (_model.IsWordAlreadyFound(word))
                {
                    _view.DisplayMessage($"You have already found the word \"{word}\"!");
                }
                else
                {
                    _view.DisplayMessage("Word found!");
                    _model.AddFoundWord(word);
                    _view.DisplayScore(_model.PlayerPoints);
                }
            }
            else
            {
                _view.DisplayMessage($"{word} is not a valid guess.");
            }
        }

        public void Shuffle()
        {
            _model.ShuffleBaseWord();
            _view.ShowPuzzle(_model.BaseWord, _model.RequiredLetter);
        }

        public void SaveCurrent()
        {
            Console.WriteLine("Enter a name for your save file:");
            string saveName = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(saveName))
            {
                _view.DisplayMessage("Invalid save name. Please try again.");
                return;  // or loop until a valid name is given
            }
            _model.SaveCurrentGameState(saveName);
            _view.DisplayMessage("Successfully saved game with progress");
        }

        public void SavePuzzle()
        {
            Console.WriteLine("Enter a name for your puzzle save file:");
            string saveName = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(saveName))
            {
                _view.DisplayMessage("Invalid save name. Please try again.");
                return;  // or loop until a valid name is given
            }
            _model.SaveCurrentPuzzleState(saveName);
            _view.DisplayMessage("Successfully saved puzzle");
        }

        public void Load()
        {
            var fileList = _model.GetAvailableSaveFiles();

            if (fileList.Count == 0)
            {
                _view.DisplayMessage("No Saved Games");
                return;
            }

            _view.DisplaySaveFilesList(fileList);

            int fileId = _view.GetFileIdFromUser();

            GameModel loadedGame = _model.LoadGameStateFromFile(fileId);

            if (loadedGame != null)
            {
                _model.AssignFrom(loadedGame);
                _view.DisplayMessage("This is the loaded puzzle");
                _view.ShowPuzzle(_model.BaseWord, _model.RequiredLetter);
            }
            else
            {
                _view.DisplayMessage("Invalid file Id");
            }
        }

        public void BeginScreen()
        {
            _view.BeginScreen();
        }

        public void HandleCommand(string input)
        {
            switch (input)
            {
                case "-exit":
                    _model.Exit();
                    break;

                case "-help":
                    _view.Help();
                    break;

                case "-load":
                case "-load puzzle":
                    Load();
                    break;

                case "-new":
                case "-new game":
                    NewPuzzle();
                    break;

                case "-new game from word":
                    Console.WriteLine("Please enter a valid pangram: ");
                    string pang = Console.ReadLine().Trim();
                    NewPuzzleBaseWord(pang);
                    break;

                case "-save current":
                    if (_model.Active())
                    {
                        SaveCurrent();
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-save puzzle":
                    if (_model.Active())
                    {
                        SavePuzzle();
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
                        _view.ShowFoundWords(_model.FoundWords);
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
                        _view.ShowPuzzle(_model.BaseWord, _model.RequiredLetter);
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
                        _view.ShowStatus(_model.PlayerPoints, _model.GetMaxPoints(), _model.StatusTitles);
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                case "-shuffle":
                    if (_model.Active())
                    {
                        Shuffle();
                    }
                    else
                    {
                        _view.DisplayMessage("A game has not been started. Please start one by calling one of the new game commands.");
                    }
                    break;

                default:
                    if (_model.Active())
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
