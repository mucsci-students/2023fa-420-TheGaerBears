using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using DynamicData;
using SpellingBee.ViewModels;
using System.Diagnostics;

namespace SpellingBee.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        void KeyInput_KeyDown(object? sender, KeyEventArgs e)
        {
            var vm = (MainWindowViewModel)this.DataContext;
            switch (e.Key)
            {
                case Key.Enter:
                    vm.ExecuteGuess();
                    break;
                case Key.Back:
                    if (vm.LowerText.Length > 0)
                        vm.LowerText = vm.LowerText.Substring(0, vm.LowerText.Length - 1);
                    break;
                case Key.A:
                    vm.LowerText += "a";
                    break;
                case Key.B:
                    vm.LowerText += "b";
                    break;
                case Key.C:
                    vm.LowerText += "c";
                    break;
                case Key.D:
                    vm.LowerText += "d";
                    break;
                case Key.E:
                    vm.LowerText += "e";
                    break;
                case Key.F:
                    vm.LowerText += "f";
                    break;
                case Key.G:
                    vm.LowerText += "g";
                    break;
                case Key.H:
                    vm.LowerText += "h";
                    break;
                case Key.I:
                    vm.LowerText += "i";
                    break;
                case Key.J:
                    vm.LowerText += "j";
                    break;
                case Key.K:
                    vm.LowerText += "k";
                    break;
                case Key.L:
                    vm.LowerText += "l";
                    break;
                case Key.M:
                    vm.LowerText += "m";
                    break;
                case Key.N:
                    vm.LowerText += "n";
                    break;
                case Key.O:
                    vm.LowerText += "o";
                    break;
                case Key.P:
                    vm.LowerText += "p";
                    break;
                case Key.Q:
                    vm.LowerText += "q";
                    break;
                case Key.R:
                    vm.LowerText += "r";
                    break;
                case Key.S:
                    vm.LowerText += "s";
                    break;
                case Key.T:
                    vm.LowerText += "t";
                    break;
                case Key.U:
                    vm.LowerText += "u";
                    break;
                case Key.V:
                    vm.LowerText += "v";
                    break;
                case Key.W:
                    vm.LowerText += "w";
                    break;
                case Key.X:
                    vm.LowerText += "x";
                    break;
                case Key.Y:
                    vm.LowerText += "y";
                    break;
                case Key.Z:
                    vm.LowerText += "z";
                    break;
            }
        }
    }
}