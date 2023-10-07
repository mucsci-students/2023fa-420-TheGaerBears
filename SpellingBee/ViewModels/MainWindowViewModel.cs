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


        private readonly GuiController _guiController;
        // Unused and can be deleted//
       // private readonly GameModel _model;

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



        public MainWindowViewModel()
        {
            //_guiController = new GuiController(new GameModel(), new GameView());
            _guiController = new GuiController(new GameModel());
            NewPuzzleCommand = ReactiveCommand.Create(StartNewPuzzle);
            AppendLetter1Command = ReactiveCommand.Create(() => AppendLetter(Letter1));
            AppendLetter2Command = ReactiveCommand.Create(() => AppendLetter(Letter2));
            AppendLetter3Command = ReactiveCommand.Create(() => AppendLetter(Letter3));
            AppendLetter4Command = ReactiveCommand.Create(() => AppendLetter(Letter4));
            AppendLetter5Command = ReactiveCommand.Create(() => AppendLetter(Letter5));
            AppendLetter6Command = ReactiveCommand.Create(() => AppendLetter(Letter6));
            AppendLetter7Command = ReactiveCommand.Create(() => AppendLetter(Letter7));
            ShuffleCommand = ReactiveCommand.Create(ShuffleLetters);
            GuessCommand = ReactiveCommand.Create(ExecuteGuess);
            SavePuzzleCommand = ReactiveCommand.Create(SavePuzzle);
            SaveCurrentCommand = ReactiveCommand.Create(SaveCurrent);
            LoadCommand = ReactiveCommand.Create(Load);
            NewGameFromWordCommand = ReactiveCommand.Create(NewGameFromWord);
            ShowFoundWordsCommand = ReactiveCommand.Create(ShowFoundWords);
            HelpCommand = ReactiveCommand.Create(ShowHelp);
            ToggleColorblind = ReactiveCommand.Create(ToggleColor);
        }
        private void ToggleColor()
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
        private async void SwapColors()
        {
            while (true)
            {
                string temp = Color1;
                Color1 = Color2;
                Color2 = temp;
                await Task.Delay(1500);
            }
        }
        private void UpdateLetters()
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

        private void ShuffleLetters()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.ShuffleBaseWord();
            UpdateLetters();
        }

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

        private void AppendLetter(string letter)
        {
            LowerText += letter;
        }
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

        //Unused Method can be deleted
       /* private void SaveCommand()
        {

        }*/

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
                UpdateLetters();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid File. " + ex);
            }
            FeedbackMessage = _guiController.GetLastMessage();

        }

        private async Task<string> OpenFile()
        {
            var filesService = App.Current?.Services?.GetService<IFilesService>();

            if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

            var file = await filesService.OpenFileAsync();
            if (file is null) return null;
            return file.Name;
        }

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
                UpdateLetters();
            LowerText = "";
        }

        private void ShowHelp()
        {
            FeedbackMessage = _guiController.GetHelp();
        }

        private void StartNewPuzzle()
        {
            if (_colorThread)
            {
                Dispatcher.UIThread.Post(SwapColors, DispatcherPriority.Background);
                _colorThread = false;
            }

            FeedbackMessage = "";
            _guiController.NewPuzzle();
            UpdateLetters();
            /*
            Letter1 = _guiController.GetNthLetter(0);
            Letter2 = _guiController.GetNthLetter(1);
            Letter3 = _guiController.GetNthLetter(2);
            Letter4 = _guiController.GetNthLetter(3);
            Letter5 = _guiController.GetNthLetter(4);
            Letter6 = _guiController.GetNthLetter(5);
            Letter7 = _guiController.GetNthLetter(6);
            */
        }
        private string _feedbackMessage = "";

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