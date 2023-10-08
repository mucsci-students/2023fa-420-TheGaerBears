using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SpellingBee.Services;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;

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
        private bool _colorThread = true;
        private string _feedbackMessage = "";

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
        public ReactiveCommand<Unit, Unit> SavePuzzleCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCurrentCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> NewGameFromWordCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowFoundWordsCommand { get; }
        public ReactiveCommand<Unit, Unit> HelpCommand { get; }
        public ReactiveCommand<Unit,Unit> ToggleColorblind { get; }

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
            SavePuzzleCommand = ReactiveCommand.Create(SavePuzzle);
            SaveCurrentCommand = ReactiveCommand.Create(SaveCurrent);
            LoadCommand = ReactiveCommand.Create(Load);
            NewGameFromWordCommand = ReactiveCommand.Create(NewGameFromWord);
            ShowFoundWordsCommand = ReactiveCommand.Create(ShowFoundWords);
            HelpCommand = ReactiveCommand.Create(ShowHelp);
            ToggleColorblind = ReactiveCommand.Create(ToggleColors);
        }

        /// <summary>
        /// Method <c>AppendLetter</c> 
        /// </summary>
        private void AppendLetter(string letter)
        {
            LowerText += letter;
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
        /// Method <c>SwapColors</c> alternates the pair of colors during gameplay.
        /// </summary>
        private async void SwapColors()
        {
            while (true)
            {
               // Use tuple to swap colors.
               (Color1, Color2) = (Color2, Color1);
               await Task.Delay(1500);
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
        private void SavePuzzle()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.SavePuzzle(LowerText);
            FeedbackMessage = _guiController.GetLastMessage();
            LowerText = "";
        }

        /// <summary>
        /// Method <c>SaveCurrent</c> allows the user to click the SaveCurrent button to save state
        /// as long as a game has started.
        /// </summary>
        private void SaveCurrent()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.SaveCurrent(LowerText);
            FeedbackMessage = _guiController.GetLastMessage();
            LowerText = "";
        }

        /// <summary>
        /// Method <c>Load</c> allows the user to click the Load button to load a saved game
        /// or puzzle.
        /// </summary>
        private async void Load()
        {
            if(_colorThread)
            {
                Dispatcher.UIThread.Post(SwapColors, DispatcherPriority.Background);
                _colorThread = false;
            }

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
        private void NewGameFromWord()
        {
            if (_colorThread)
            {
                Dispatcher.UIThread.Post(SwapColors, DispatcherPriority.Background);
                _colorThread = false;
            }

            FeedbackMessage = "";
            _guiController.NewPuzzleBaseWord(LowerText);

            FeedbackMessage = _guiController.GetLastMessage();
            if (!FeedbackMessage.Equals("Not a valid pangram"))
                UpdateState();
            LowerText = "";
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
            if (_colorThread)
            {
                Dispatcher.UIThread.Post(SwapColors, DispatcherPriority.Background);
                _colorThread = false;
            }
            FeedbackMessage = "";
            _guiController.NewPuzzle();
            UpdateState();
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

        public bool LoadVisible
        {
            get { return _loadVisible; }
            set { this.RaiseAndSetIfChanged(ref _loadVisible, value); }
        }

        public bool GuessVisible
        {
            get { return _guessVisible; }
            set { this.RaiseAndSetIfChanged(ref _guessVisible, value); }
        }

        public bool SaveVisible
        {
            get { return _saveVisible; }
            set { this.RaiseAndSetIfChanged(ref _saveVisible, value); }
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
      }
    }
