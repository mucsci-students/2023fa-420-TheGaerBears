using Avalonia.Controls.Documents;
using DynamicData.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public class GUIController
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private string _lastMessage = "";

        public GUIController(GameModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }



        public void NewPuzzle()
        {
            _model.Reset();
            _model.SelectRandomWordForPuzzle();
            _model.GenerateValidWords();
        }

        public void NewPuzzleBaseWord(string word)
        {
            if (!_model.SetBaseWordForPuzzle(word))
            {
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
                }
            }
            else
            {
                _lastMessage = $"{word} is not a valid guess.";
            }

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
        }

        public void SaveCurrent()
        {
            string saveName = _view.GetInput();

            _model.SaveCurrentGameState(saveName);
        }

        public void SavePuzzle()
        {
            _view.DisplayMessage("Enter Save File Name:");
            string saveName = _view.GetInput();

            _model.SaveCurrentGameState(saveName);
        }

        public void Load()
        {
            var fileList = _model.GetAvailableSaveFiles();

            if (fileList.Count == 0)
            {
                return;
            }


            //int fileId = _view.GetFileIdFromUser();

            //GameModel loadedGame = _model.LoadGameStateFromFile(fileId);
            /*
            if (loadedGame != null)
            {
                _model.AssignFrom(loadedGame);
            }
            else
            {
                _view.DisplayMessage("Invalid file Id");
            }*/
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

        public string GetHelp()
        {
            return @"
Welcome to the Spelling Bee game!
-new game: Starts a new puzzle game.
-new game from word: Starts a new puzzle game with your word.
-load: Load a saved game or puzzle.
-save current: Save the current game with progress.
-save puzzle: Save the current puzzle.
-show found words: Display the words you have found.
-show puzzle: Display the puzzle letters.
-show status: Display your current game status.
-shuffle: Shuffle the puzzle letters.
-help: Show this list of commands.
-exit: Exit the game.
You can also simply type in a word to make a guess.
Remember, all words must contain the required letter!";
        }


        public string GetNthLetter(int n)
        {
            return _model.GetBaseWord()[n].ToString();
        }

        public int GetCurrentScore()
        {
            return _model.GetCurrentScore();
        }

    }
}
