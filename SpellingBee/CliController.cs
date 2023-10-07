using Avalonia.Controls.Documents;
using DynamicData.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellingBee
{
/*
 * CliController Class - SpellingBee
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

    public class CliController
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private string _lastMessage = "";

        public CliController(GameModel model, GameView view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }



        public void NewPuzzle()
        {
            _model.Reset();
            _model.SelectRandomWordForPuzzle();
            _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
            _model.GenerateValidWords();
        }

        public void NewPuzzleBaseWord(string word)
        {
            if (!_model.SetBaseWordForPuzzle(word))
            {
                _view.DisplayMessage("This is not a valid pangram");
                return;
            }
            _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
            _model.GenerateValidWords();
        }

        public void Guess(string word)
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


        public string GetLastMessage()
        {
            return _lastMessage;
        }

        public void ShuffleBaseWord()
        {
            _model.ShuffleBaseWord();
        }

        public List<char> GetBaseWord()
        {
            return _model.GetBaseWord();
        }

        public void Shuffle()
        {
            _model.ShuffleBaseWord();
            _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
        }

        public void SaveCurrent()
        {
            _view.DisplayMessage("Enter Save File Name:");
            string saveName = _view.GetInput();

            if (_model.SaveCurrentGameState(saveName))
                _view.DisplayMessage("Successfully saved game with progress");
            else
                _view.DisplayMessage("Error: No file name");
        }

        public void SavePuzzle()
        {
            _view.DisplayMessage("Enter Save File Name:");
            string saveName = _view.GetInput();

            if (_model.SaveCurrentGameState(saveName))
                _view.DisplayMessage("Successfully saved game with progress");
            else
                _view.DisplayMessage("Error: No file name");
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
                _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
            }
            else
            {
                _view.DisplayMessage("Invalid file Id");
            }
        }

        public List<string> GetFoundWords()
        {
            return _model.GetFoundWords().ToList();
        }

        public string GetCurrentRank()
        {
            int currentPoints = _model.GetPlayerPoints(); 
            foreach (var rankPair in _model.GetStatusTitles())
            {
                if (currentPoints >= rankPair.Value)
                {
                    return rankPair.Key;
                }
            }
            return "Beginner";
        }


        public string GetNthLetter(int n)
        {
            return _model.GetBaseWord()[n].ToString();
        }

        public void BeginScreen()
        {
            _view.BeginScreen();
        }

        public int GetCurrentScore()
        {
            return _model.GetCurrentScore();
        }

        public void HandleCommand(string input)
        {
            switch (input)
            {
                case "-exit":
                    _view.Exit();
                    _model.Exit();
                    break;

                case "-help":
                    _view.Help();
                    //_lastMessage = _view.Help();

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
                    _view.DisplayMessage("Please enter a valid pangram: ");
                    string pang = _view.GetInput();
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
                        _view.ShowStatus(_model.GetPlayerPoints(), _model.GetMaxPoints(), _model.GetStatusTitles());
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
                    if (_model.Active() && !input[0].Equals('-'))
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
