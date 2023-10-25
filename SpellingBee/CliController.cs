﻿using Avalonia.Controls.Documents;
using DynamicData.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellingBee
{

    public class CliController
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private string _lastMessage = "";

        /// <summary>
        /// Initializes a new instance of the <c>CliController</c> class with the provided GameModel and GameView.
        /// </summary>
        public CliController(GameModel model, GameView view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        /// <summary>
        /// Starts a new puzzle using random base word.
        /// </summary>
        public void NewPuzzle()
        {
            _model.Reset();
            _model.SelectRandomWordForPuzzle();
            _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
            _model.GenerateValidWords();
        }

        /// <summary>
        /// Starts a new puzzle using a specified base word.
        /// </summary>
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

        /// <summary>
        /// Processes the guessed word by the user.
        /// </summary>
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

        /// <summary>
        /// Retrieves the last message generated by the controller.
        /// </summary>
        public string GetLastMessage()
        {
            return _lastMessage;
        }

        /// <summary>
        /// Retrieves the current base word.
        /// </summary>
        public List<char> GetBaseWord()
        {
            return _model.GetBaseWord();
        }

        /// <summary>
        /// Shuffles the current base word and updates the view.
        /// </summary>
        public void Shuffle()
        {
            _model.ShuffleBaseWord();
            _view.ShowPuzzle(_model.GetBaseWord(), _model.GetRequiredLetter());
        }

        /// <summary>
        /// Saves the current game state prompting the user for a filename.
        /// </summary>
        public void SaveCurrent()
        {
            _view.DisplayMessage("Enter Save File Name:");
            string saveName = _view.GetInput();

            if (_model.SaveCurrentGameState(saveName))
                _view.DisplayMessage("Successfully saved game with progress");
            else
                _view.DisplayMessage("Error: No file name");
        }

        /// <summary>
        /// Saves the current puzzle prompting the user for a filename.
        /// </summary>
        public void SavePuzzle()
        {
            _view.DisplayMessage("Enter Save File Name:");
            string saveName = _view.GetInput();

            if (_model.SaveCurrentGameState(saveName))
                _view.DisplayMessage("Successfully saved game with progress");
            else
                _view.DisplayMessage("Error: No file name");
        }

        /// <summary>
        /// Loads a saved game from a list of available save files.
        /// </summary>
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

        /// <summary>
        /// Displays the initial game screen.
        /// </summary>
        public void BeginScreen()
        {
            _view.BeginScreen();
        }
        public void PrintHintTable(Dictionary<char, int[]> data)
        {
            // Define the range
            int[] wordLengths = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            int arrayLength = wordLengths.Length;

            // Print header
            Console.Write("    ");
            foreach (var length in wordLengths)
            {
                Console.Write($"{length,3} ");
            }
            Console.WriteLine(" tot");

            // Calculate column totals
            int[] columnTotals = new int[arrayLength];
            foreach (var entry in data)
            {
                for (int i = 0; i < arrayLength; i++)
                {
                    columnTotals[i] += entry.Value[i];
                }
            }

            // Print rows
            foreach (var entry in data)
            {
                Console.Write($"{entry.Key}   ");
                int rowTotal = 0;
                for (int i = 0; i < arrayLength; i++)
                {
                    if (entry.Value[i] == 0)
                    {
                        Console.Write("  - ");
                    }
                    else
                    {
                        Console.Write($"{entry.Value[i],3} ");
                        rowTotal += entry.Value[i];
                    }
                }
                Console.WriteLine($"{rowTotal,4}");
            }

            // Print bottom totals
            Console.Write("tot ");
            int grandTotal = 0;
            for (int i = 0; i < arrayLength; i++)
            {
                Console.Write($"{columnTotals[i],3} ");
                grandTotal += columnTotals[i];
            }
            Console.WriteLine($"{grandTotal,4}");

            Console.WriteLine($"Max Points: {_model.GetMaxPoints()}");
            Console.WriteLine($"Total Number of Valid Words: {_model.GetValidWords().Count}");
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
