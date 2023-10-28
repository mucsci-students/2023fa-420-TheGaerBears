using Avalonia.Controls;
using Avalonia.Controls.Utils;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SpellingBee.Services;
using SpellingBee.Views;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Castle.Components.DictionaryAdapter.Xml;
using System.Drawing.Printing;

namespace SpellingBee.ViewModels
{
    /// <summary>
    /// Class <c>MainWindowViewModel</c> 
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private string _letter1 = "";
        private string _letter2 = "";
        private string _letter3 = "";
        private string _letter4 = "";
        private string _letter5 = "";
        private string _letter6 = "";
        private string _letter7 = "";
        private string _lowerText = "";
        private int _points = 0;
        private string _rank = "";
        private int _nextRank = 0;
        private bool _loadVisible = false;
        private bool _guessVisible = true;
        private bool _saveVisible = false;
        private string _color1 = "Red";
        private string _color2 = "Green";
        private string _feedbackMessage = "";
        private string _hint = "";
        private int _beginner = 0;
        private int _goodStart = 0;
        private int _movingUp = 0;
        private int _good = 0;
        private int _solid = 0;
        private int _nice = 0;
        private int _great = 0;
        private int _amazing = 0;
        private int _genius = 0;
        private int _queenBee = 0;


        private int _maxPoints = 0;

        private readonly GuiController _guiController;

        public ReactiveCommand<Unit, Unit> NewPuzzleCommand { get; }
        public ReactiveCommand<Unit, Unit> AppendLetter1Command { get; }
        public ReactiveCommand<Unit, Unit> AppendLetter2Command { get; }
        public ReactiveCommand<Unit, Unit> AppendLetter3Command { get; }
        public ReactiveCommand<Unit, Unit> AppendLetter4Command { get; }
        public ReactiveCommand<Unit, Unit> AppendLetter5Command { get; }
        public ReactiveCommand<Unit, Unit> AppendLetter6Command { get; }
        public ReactiveCommand<Unit, Unit> AppendLetter7Command { get; }
        public ReactiveCommand<Unit, Unit> ShuffleCommand { get; }
        public ReactiveCommand<Unit, Unit> GuessCommand { get; }
        public ReactiveCommand<string, Unit> SavePuzzleCommand { get; }
        public ReactiveCommand<string, Unit> SaveCurrentCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<string, Unit> NewGameFromWordCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowFoundWordsCommand { get; }
        public ReactiveCommand<Unit, Unit> HelpCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleColorblind { get; }
        public ReactiveCommand<Unit, Unit> Backspace { get; }
        public ReactiveCommand<Unit, Unit> HintCommand { get; }


        /// <summary>
        /// Instantiates the <c>MainWindowViewModel</c> with <c>GuiController</c> and <c>GameModel</c> 
        /// and sets up the button commands.
        /// </summary>
        public MainWindowViewModel()
        {
            _guiController = new GuiController(new GameModel());

            AppendLetter1Command = ReactiveCommand.Create(() => AppendLetter(Letter1));
            AppendLetter2Command = ReactiveCommand.Create(() => AppendLetter(Letter2));
            AppendLetter3Command = ReactiveCommand.Create(() => AppendLetter(Letter3));
            AppendLetter4Command = ReactiveCommand.Create(() => AppendLetter(Letter4));
            AppendLetter5Command = ReactiveCommand.Create(() => AppendLetter(Letter5));
            AppendLetter6Command = ReactiveCommand.Create(() => AppendLetter(Letter6));
            AppendLetter7Command = ReactiveCommand.Create(() => AppendLetter(Letter7));

            NewPuzzleCommand = ReactiveCommand.Create(StartNewPuzzle);
            ShuffleCommand = ReactiveCommand.Create(ShuffleLetters);
            GuessCommand = ReactiveCommand.Create(ExecuteGuess);
            SavePuzzleCommand = ReactiveCommand.Create<string>(SavePuzzle);
            SaveCurrentCommand = ReactiveCommand.Create<string>(SaveCurrent);
            LoadCommand = ReactiveCommand.Create(Load);
            NewGameFromWordCommand = ReactiveCommand.Create<string>(NewGameFromWord);
            ShowFoundWordsCommand = ReactiveCommand.Create(ShowFoundWords);
            HelpCommand = ReactiveCommand.Create(ShowHelp);
            ToggleColorblind = ReactiveCommand.Create(ToggleColors);
            Backspace = ReactiveCommand.Create(DeleteFromEnd);
            HintCommand = ReactiveCommand.Create(Hint);
        }

        /// <summary>
        /// Method <c>AppendLetter</c> 
        /// </summary>
        private void AppendLetter(string letter)
        {
            LowerText += letter;
        }
        /// <sumary>
        /// Method <c>Backspace</c>
        /// </sumary>
        private void DeleteFromEnd()
        {
            if (LowerText.Length > 0)
            {
                LowerText = LowerText.Substring(0, LowerText.Length - 1);

            }
        }

        /// <summary>
        /// Method <c>ToggleColors</c> chooses the background colors of the letters.
        /// </summary>
        private void ToggleColors()
        {
            if (Color1 == "Green")
            {
                Color1 = "Blue";
                return;
            }
            else if (Color2 == "Green")
            {
                Color2 = "Blue";
                return;
            }
            if (Color1 == "Blue")
            {
                Color1 = "Green";
                return;
            }
            else
            {
                Color2 = "Green";
                return;
            }
        }

        /// <summary>
        /// Method <c>UpdateState</c> displays the generated letters, points, rank, and next rank.
        /// </summary>
        private void UpdateState()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            List<char> updatedLetters = _guiController.GetBaseWord();

            Letter1 = updatedLetters[0].ToString();
            Letter2 = updatedLetters[1].ToString();
            Letter3 = updatedLetters[2].ToString();
            Letter4 = updatedLetters[3].ToString();
            Letter5 = updatedLetters[4].ToString();
            Letter6 = updatedLetters[5].ToString();
            Letter7 = updatedLetters[6].ToString();

            //Updating game in general
            Points = _guiController.GetCurrentScore();
            Rank = _guiController.GetCurrentRank();
            NextRank = _guiController.GetNextRank();
            MaxPoints = _guiController.GetMaxPoints();
            SetThresholds();
        }
        private void SetThresholds()
        {
            /*
             * new KeyValuePair<string, int>("Beginner", 0),
                new KeyValuePair<string, int>("Good Start", 2),
                new KeyValuePair<string, int>("Moving Up", 5),
                new KeyValuePair<string, int>("Good", 8),
                new KeyValuePair<string, int>("Solid", 15),
                new KeyValuePair<string, int>("Nice", 25),
                new KeyValuePair<string, int>("Great", 40),
                new KeyValuePair<string, int>("Amazing", 50),
                new KeyValuePair<string, int>("Genius", 70),
                new KeyValuePair<string, int>("Queen Bee", 100)
             */
            GoodStart = (int) (MaxPoints * .02);
			MovingUp = (int)(MaxPoints * .05);
			Good = (int)(MaxPoints * .08);
			Solid = (int)(MaxPoints * .15);
			Nice = (int)(MaxPoints * .25);
			Great = (int)(MaxPoints * .4);
			Amazing = (int)(MaxPoints * .5);
			Genius = (int)(MaxPoints * .7);
			QueenBee = MaxPoints;
		}
		/// <summary>
		/// Method <c>ShuffleLetters</c> shuffles the displayed letters.
		/// </summary>
		private void ShuffleLetters()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.ShuffleBaseWord();
            UpdateState();

        }

        /// <summary>
        /// Method <c>ExecuteGuess</c> allows the word entered by the user to be checked against
        /// <c>validWords</c> when the Guess button is clicked or the Enter key is pressed.
        /// </summary>
        public void ExecuteGuess()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.Guess(LowerText);
            FeedbackMessage = _guiController.GetLastMessage();
            if (FeedbackMessage.Equals("Word found!"))
            {
                Points = _guiController.GetCurrentScore();
                Rank = _guiController.GetCurrentRank();
                NextRank = _guiController.GetNextRank();

            }
            LowerText = "";
        }

        /// <summary>
        /// Method <c>SavePuzzle</c> allows the user to click the SavePuzzle button to save state
        /// as long as a game has started.
        /// </summary>
        private void SavePuzzle(string word)
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.SavePuzzle(word);
            FeedbackMessage = _guiController.GetLastMessage();
            LowerText = "";
        }

        /// <summary>
        /// Method <c>SaveCurrent</c> allows the user to click the SaveCurrent button to save state
        /// as long as a game has started.
        /// </summary>
        private void SaveCurrent(string word)
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.SaveCurrent(word);
            FeedbackMessage = _guiController.GetLastMessage();
            LowerText = "";
        }

        /// <summary>
        /// Method <c>Load</c> allows the user to click the Load button to load a saved game
        /// or puzzle.
        /// </summary>
        private async void Load()
        {
            FeedbackMessage = "";
            var fileName = await OpenFile();
            if (fileName == null) return;
            try
            {
                _guiController.Load(fileName);
                UpdateState();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid File. " + ex);
            }
            FeedbackMessage = _guiController.GetLastMessage();

        }

        /// <summary>
        /// Method <c>OpenFile</c> allows loading a specific save file as long as
        /// it is not null.
        /// <return>The filename to open.</return>
        /// </summary>
        private async Task<string?> OpenFile()
        {
            var filesService = (App.Current?.Services?.GetService<IFilesService>()) ?? throw new NullReferenceException("Missing File Service instance.");
            var file = await filesService.OpenFileAsync();
            if (file is null) return null;
            return file.Name;
        }

        /// <summary>
        /// Method <c>ShowFoundWords</c> displays the words found by the user so far.
        /// </summary>
        private void ShowFoundWords()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            var foundWords = _guiController.GetFoundWords();
            FeedbackMessage = foundWords.Count > 0 ? string.Join("\n", foundWords) : "No words found!";
        }

        /// <summary>
        /// Method <c>NewGameFromWord</c> allows the user to start a new game from the word
        /// typed into the textbox as long as it is a valid pangram.
        /// </summary>
        private void NewGameFromWord(String word)
        {
            FeedbackMessage = "";
            if (word != null)
            {
                _guiController.NewPuzzleBaseWord(word);
                FeedbackMessage = _guiController.GetLastMessage();
            }
            else
                FeedbackMessage = "Not a valid pangram";

            if (!FeedbackMessage.Equals("Not a valid pangram"))
            {
                UpdateState();
            }

        }

        /// <summary>
        /// Method <c>ShowHelp</c> displays the description of commands.
        /// </summary>
        private void ShowHelp()
        {
            FeedbackMessage = _guiController.GetHelp();
        }

        /// <summary>
        /// Method <c>StartNewPuzzle</c> resets the puzzle and state of the game.
        /// </summary>
        private void StartNewPuzzle()
        {
            FeedbackMessage = "";
            _guiController.NewPuzzle();
            UpdateState();
        }

        private void Hint()
        {
            _guiController.Hint();
            HintString = _guiController.GetLastMessage();
        }


        public string FeedbackMessage
        {
            get { return _feedbackMessage; }
            set { this.RaiseAndSetIfChanged(ref _feedbackMessage, value); }
        }

        public string Letter1
        {
            get { return _letter1; }
            set { this.RaiseAndSetIfChanged(ref _letter1, value); }
        }

        public string Letter2
        {
            get { return _letter2; }
            set { this.RaiseAndSetIfChanged(ref _letter2, value); }
        }

        public string Letter3
        {
            get { return _letter3; }
            set { this.RaiseAndSetIfChanged(ref _letter3, value); }
        }

        public string Letter4
        {
            get { return _letter4; }
            set { this.RaiseAndSetIfChanged(ref _letter4, value); }
        }

        public string Letter5
        {
            get { return _letter5; }
            set { this.RaiseAndSetIfChanged(ref _letter5, value); }
        }

        public string Letter6
        {
            get { return _letter6; }
            set { this.RaiseAndSetIfChanged(ref _letter6, value); }
        }

        public string Letter7
        {
            get { return _letter7; }
            set { this.RaiseAndSetIfChanged(ref _letter7, value); }
        }

        public string LowerText
        {
            get { return _lowerText; }
            set { this.RaiseAndSetIfChanged(ref _lowerText, value); }
        }

        public int Points
        {
            get { return _points; }
            set { this.RaiseAndSetIfChanged(ref _points, value); }
        }

        public string Rank
        {
            get { return _rank; }
            set { this.RaiseAndSetIfChanged(ref _rank, value); }
        }

        public int NextRank
        {
            get { return _nextRank; }
            set { this.RaiseAndSetIfChanged(ref _nextRank, value); }
        }

        public string Color1
        {
            get { return _color1; }
            set { this.RaiseAndSetIfChanged(ref _color1, value); }
        }

        public string Color2
        {
            get { return _color2; }
            set { this.RaiseAndSetIfChanged(ref _color2, value); }
        }

        public string HintString
        {
            get { return _hint; }
            set { this.RaiseAndSetIfChanged(ref _hint, value); }
        }

        public int MaxPoints
        {
            get { return _maxPoints; }
            set { this.RaiseAndSetIfChanged(ref _maxPoints, value); }
        }
        public int Beginner
        {
            get { return _beginner; }
            set { this.RaiseAndSetIfChanged(ref _beginner, value); }

        }
        public int MovingUp
        {
            get { return _movingUp; }
            set { this.RaiseAndSetIfChanged(ref _movingUp, value); }
        }
		public int GoodStart
		{
			get { return _goodStart; }
			set { this.RaiseAndSetIfChanged(ref _goodStart, value); }
		}
		public int Good
        {
            get { return _good; }
            set { this.RaiseAndSetIfChanged(ref _good, value); }
        }
        public int Solid
        {
            get { return _solid; }
            set { this.RaiseAndSetIfChanged(ref _solid, value); }
        }
        public int Nice
        {
            get { return _nice; }
            set { this.RaiseAndSetIfChanged(ref _nice, value); }
        }
        public int Great
        {
            get { return _great; }
            set { this.RaiseAndSetIfChanged(ref _great, value); }
        }
        public int Amazing
        {
            get { return _amazing; }
            set { this.RaiseAndSetIfChanged(ref _amazing, value); }
        }
        public int Genius
        {
            get { return _genius; }
            set { this.RaiseAndSetIfChanged(ref _genius, value); }
        }
        public int QueenBee
        {
            get { return _queenBee; }
            set { this.RaiseAndSetIfChanged(ref _queenBee, value); }
        }
    }
}
