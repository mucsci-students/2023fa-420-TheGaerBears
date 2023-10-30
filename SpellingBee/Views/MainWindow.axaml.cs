using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using DynamicData;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SpellingBee.ViewModels;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

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
            if (e.Key == Key.Enter)
            {
                vm.ExecuteGuess();
            }
            else if (e.Key == Key.Back)
            {
                if (vm.LowerText.Length > 0)
                    vm.LowerText = vm.LowerText.Substring(0, vm.LowerText.Length - 1);
            }
            else if (!vm.Letter1.Equals(""))
            {
                // Equals("") check is to make sure game is started
                // 97 = a; 44 = KEY.A
                // Convert Key entered to ascii - 53 converts from Avalonia enum to ascii
                int keyEntered = (int)e.Key + 53;

                // Check if key entered is in baseword
                if ((int)vm.Letter1.ToCharArray()[0] == keyEntered)
                {
                    vm.LowerText += vm.Letter1;
                }
                else if ((int)vm.Letter2.ToCharArray()[0] == keyEntered)
                {
                    vm.LowerText += vm.Letter2;
                }
                else if ((int)vm.Letter3.ToCharArray()[0] == keyEntered)
                {
                    vm.LowerText += vm.Letter3;
                }
                else if ((int)vm.Letter4.ToCharArray()[0] == keyEntered)
                {
                    vm.LowerText += vm.Letter4;
                }
                else if ((int)vm.Letter5.ToCharArray()[0] == keyEntered)
                {
                    vm.LowerText += vm.Letter5;
                }
                else if ((int)vm.Letter6.ToCharArray()[0] == keyEntered)
                {
                    vm.LowerText += vm.Letter6;
                }
                else if ((int)vm.Letter7.ToCharArray()[0] == keyEntered)
                {
                    vm.LowerText += vm.Letter7;
                }
            }
        }
    }
}