
using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using SQLitePCL;
using System.Diagnostics;
using SpellingBee;
using System.Collections.Generic;

namespace AvaTest
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                //Run GUI
                string[] b = { "" };
                Main2(b);
            }
            else if (args[0].Equals("-cli"))
            {
                //Run CLI version

                //Initialize SQLitePCL
                Batteries.Init();

                GameModel model = new();
                GameView view = new();
                CliController cliController = new(model, view);

                //Intro Screen
                cliController.BeginScreen();

                //While loop that allows the game to keep going
                while (true)
                {
                    var input = Console.ReadLine()!.ToLower().Trim();
                    cliController.HandleCommand(input);
                }
            }
        }


        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main2(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
    }
}