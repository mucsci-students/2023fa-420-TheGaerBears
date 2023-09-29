using Avalonia;
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

                GameModel model = new GameModel();
                GameView view = new GameView();
                GameController gameController = new GameController(model, view);

                //Intro Screen
                gameController.BeginScreen();

                //While loop that allows the game to keep going
                while (true)
                {
                    string input = Console.ReadLine().ToLower().Trim();
                    gameController.HandleCommand(input);
                }
            }
        }


        [STAThread]
        public static void Main2(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}