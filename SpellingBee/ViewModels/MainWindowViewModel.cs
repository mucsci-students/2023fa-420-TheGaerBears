namespace SpellingBee.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _letter1;
        private string _letter2;
        private string _letter3;
        private string _letter4;
        private string _letter5;
        private string _letter6;
        private string _letter7;
        private string _rightText;
        private string _lowerText;

        public string letter1
        {
            get { return _letter1; }
            set { _letter1 = value; }
        }
        public string letter2
        {
            get { return _letter2; }
            set { _letter2 = value; }
        }
        public string letter3
        {
            get { return _letter3; }
            set { _letter3 = value; }
        }
        public string letter4
        {
            get { return _letter4; }
            set { _letter4 = value; }
        }
        public string letter5
        {
            get { return _letter5; }
            set { _letter5 = value; }
        }
        public string letter6
        {
            get { return _letter6; }
            set { _letter6 = value; }
        }
        public string letter7
        {
            get { return _letter7; }
            set { _letter7 = value; }
        }
        public string rightText
        {
            get { return _rightText; }
            set { _rightText = value; }
        }
        public string lowerText
        {
            get { return _lowerText; }
            set { _lowerText = value; }
        }
        public string Greeting => "Welcome to Avalonia!";
    }
}