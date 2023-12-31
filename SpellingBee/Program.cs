﻿
using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using SQLitePCL;
using System.Diagnostics;
using SpellingBee;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text.RegularExpressions;

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
                CliController cliControllerInstance = CliController.GetInstance(model, view);

                //Intro Screen
                cliControllerInstance.BeginScreen();

                //While loop that allows the game to keep going
                string input = string.Empty;
                while(true) 
                {
                    input = view.TabCompleteInput();
                    cliControllerInstance.HandleCommand(input);
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