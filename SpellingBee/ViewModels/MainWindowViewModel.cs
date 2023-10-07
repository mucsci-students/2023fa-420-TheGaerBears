using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SpellingBee.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Enumeration;
using Avalonia.Controls;

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

        private readonly GUIController _guiController;
        private readonly GameModel _model;

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




        public MainWindowViewModel()
        {
            //_guiController = new GuiController(new GameModel(), new GameView());
            _guiController = new GUIController(new GameModel());
            NewPuzzleCommand = ReactiveCommand.Create(StartNewPuzzle);
            AppendLetter1Command = ReactiveCommand.Create(() => AppendLetter(letter1));
            AppendLetter2Command = ReactiveCommand.Create(() => AppendLetter(letter2));
            AppendLetter3Command = ReactiveCommand.Create(() => AppendLetter(letter3));
            AppendLetter4Command = ReactiveCommand.Create(() => AppendLetter(letter4));
            AppendLetter5Command = ReactiveCommand.Create(() => AppendLetter(letter5));
            AppendLetter6Command = ReactiveCommand.Create(() => AppendLetter(letter6));
            AppendLetter7Command = ReactiveCommand.Create(() => AppendLetter(letter7));
            ShuffleCommand = ReactiveCommand.Create(ShuffleLetters);
            GuessCommand = ReactiveCommand.Create(ExecuteGuess);
            SavePuzzleCommand = ReactiveCommand.Create(SavePuzzle);
            SaveCurrentCommand = ReactiveCommand.Create(SaveCurrent);
            LoadCommand = ReactiveCommand.Create(Load); 
            NewGameFromWordCommand = ReactiveCommand.Create(NewGameFromWord);
            ShowFoundWordsCommand = ReactiveCommand.Create(ShowFoundWords);
            HelpCommand = ReactiveCommand.Create(ShowHelp);


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

            letter1 = updatedLetters[0].ToString();
            letter2 = updatedLetters[1].ToString();
            letter3 = updatedLetters[2].ToString();
            letter4 = updatedLetters[3].ToString();
            letter5 = updatedLetters[4].ToString();
            letter6 = updatedLetters[5].ToString();
            letter7 = updatedLetters[6].ToString();

            //Updating game in general
            points = _guiController.GetCurrentScore();
            rank = _guiController.GetCurrentRank();
            nextRank = _guiController.GetNextRank();
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
            _guiController.Guess(lowerText);
            FeedbackMessage = _guiController.GetLastMessage();
            if (FeedbackMessage.Equals("Word found!"))
            {
                points = _guiController.GetCurrentScore();
                rank = _guiController.GetCurrentRank();
                nextRank = _guiController.GetNextRank();
            }
            lowerText = "";

        }

        private void AppendLetter(string letter)
        {
            lowerText += letter;
        }
        private void SavePuzzle()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.SavePuzzle(lowerText);
            FeedbackMessage = _guiController.GetLastMessage();
            lowerText = "";
        }

        private void SaveCurrent()
        {
            FeedbackMessage = "";
            if (!_guiController.GameStarted())
            {
                FeedbackMessage = "Game Not Started";
                return;
            }
            _guiController.SaveCurrent(lowerText);
            FeedbackMessage = _guiController.GetLastMessage();
            lowerText = "";
        }

        private void SaveCommand()
        {

        }

        private async void Load()
        {
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
                Console.WriteLine("Invalid File");
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
            FeedbackMessage = "";
            _guiController.NewPuzzleBaseWord(lowerText);

            FeedbackMessage = _guiController.GetLastMessage();
            if (!FeedbackMessage.Equals("Not a valid pangram"))
                UpdateLetters();
            lowerText = "";
        }

        private void ShowHelp()
        {
            FeedbackMessage = _guiController.GetHelp();
        }

        private void StartNewPuzzle()
        {
            FeedbackMessage = "";
            _guiController.NewPuzzle();
            UpdateLetters();
            /*
            letter1 = _guiController.GetNthLetter(0);
            letter2 = _guiController.GetNthLetter(1);
            letter3 = _guiController.GetNthLetter(2);
            letter4 = _guiController.GetNthLetter(3);
            letter5 = _guiController.GetNthLetter(4);
            letter6 = _guiController.GetNthLetter(5);
            letter7 = _guiController.GetNthLetter(6);
            */
        }
        private string _feedbackMessage = "";

        public string FeedbackMessage
        {
            get { return _feedbackMessage; }
            set { this.RaiseAndSetIfChanged(ref _feedbackMessage, value); }
        }

        public string letter1
        {
            get { return _letter1; }
            set { this.RaiseAndSetIfChanged(ref _letter1, value); }
        }
        public string letter2
        {
            get { return _letter2; }
            set { this.RaiseAndSetIfChanged(ref _letter2, value); }
        }
        public string letter3
        {
            get { return _letter3; }
            set { this.RaiseAndSetIfChanged(ref _letter3, value); }
        }
        public string letter4
        {
            get { return _letter4; }
            set { this.RaiseAndSetIfChanged(ref _letter4, value); }
        }
        public string letter5
        {
            get { return _letter5; }
            set { this.RaiseAndSetIfChanged(ref _letter5, value); }
        }
        public string letter6
        {
            get { return _letter6; }
            set { this.RaiseAndSetIfChanged(ref _letter6, value); }
        }
        public string letter7
        {
            get { return _letter7; }
            set { this.RaiseAndSetIfChanged(ref _letter7, value); }
        }
        public string lowerText
        {
            get { return _lowerText; }
            set { this.RaiseAndSetIfChanged(ref _lowerText, value); }
        }
        public int points
        {
            get { return _points; }
            set { this.RaiseAndSetIfChanged(ref _points, value); }
        }
        public string rank
        {
            get { return _rank; }
            set { this.RaiseAndSetIfChanged(ref _rank, value); }
        }
        public int nextRank 
        { 
            get { return _nextRank; } 
            set { this.RaiseAndSetIfChanged(ref _nextRank, value); }
        }
        public bool loadVisible
        {
            get { return _loadVisible; }
            set { this.RaiseAndSetIfChanged(ref _loadVisible, value); }
        }
        public bool guessVisible
        {
            get { return _guessVisible; }
            set { this.RaiseAndSetIfChanged(ref _guessVisible, value); }
        }
        public bool saveVisible
        {
            get { return _saveVisible; }
            set { this.RaiseAndSetIfChanged(ref _saveVisible, value); }
        }
    }
}