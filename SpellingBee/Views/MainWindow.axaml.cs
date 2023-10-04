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
                    if (vm.lowerText.Length > 0)
                        vm.lowerText = vm.lowerText.Substring(0, vm.lowerText.Length - 1);
                    break;
                case Key.A:
                    vm.lowerText += "a";
                    break;
                case Key.B:
                    vm.lowerText += "b";
                    break;
                case Key.C:
                    vm.lowerText += "c";
                    break;
                case Key.D:
                    vm.lowerText += "d";
                    break;
                case Key.E:
                    vm.lowerText += "e";
                    break;
                case Key.F:
                    vm.lowerText += "f";
                    break;
                case Key.G:
                    vm.lowerText += "g";
                    break;
                case Key.H:
                    vm.lowerText += "h";
                    break;
                case Key.I:
                    vm.lowerText += "i";
                    break;
                case Key.J:
                    vm.lowerText += "j";
                    break;
                case Key.K:
                    vm.lowerText += "k";
                    break;
                case Key.L:
                    vm.lowerText += "l";
                    break;
                case Key.M:
                    vm.lowerText += "m";
                    break;
                case Key.N:
                    vm.lowerText += "n";
                    break;
                case Key.O:
                    vm.lowerText += "o";
                    break;
                case Key.P:
                    vm.lowerText += "p";
                    break;
                case Key.Q:
                    vm.lowerText += "q";
                    break;
                case Key.R:
                    vm.lowerText += "r";
                    break;
                case Key.S:
                    vm.lowerText += "s";
                    break;
                case Key.T:
                    vm.lowerText += "t";
                    break;
                case Key.U:
                    vm.lowerText += "u";
                    break;
                case Key.V:
                    vm.lowerText += "v";
                    break;
                case Key.W:
                    vm.lowerText += "w";
                    break;
                case Key.X:
                    vm.lowerText += "x";
                    break;
                case Key.Y:
                    vm.lowerText += "y";
                    break;
                case Key.Z:
                    vm.lowerText += "z";
                    break;
            }
        }
    }
}