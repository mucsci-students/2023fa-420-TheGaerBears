using SpellingBee.SetUpSpellingBee.src.main;
using System;
using SQLitePCL;
using SpellingBee;

internal class Program
{
    static void Main(string[] args)
    {
        // Initialize SQLitePCL
        Batteries.Init();

        Game mainGame = new Game();
        mainGame.BeginScreen();
        //Call help command here
        //mainGame.Help();

        //While loop that allows user to keep guessing
        while (true)
        {
            string input = Console.ReadLine().ToLower().Trim();
            CommandValidation(input, ref mainGame);
        }
    }
    /// <summary>
    /// Parses the commands entered by the player
    /// </summary>
    /// <param name="input"></param> The command typed by the player
    /// <param name="mainGame"></param> The Game object for the puzzle
    static void CommandValidation(string input, ref Game mainGame)
    {
        switch (input)
        {
            case "-help":
                mainGame.Help();
                break;

            case "-exit":
                mainGame.Exit();
                break;

            case "-save current":
                if (mainGame.Active())
                {
                    mainGame.SaveCurrent();
                }
                else
                {
                    Console.WriteLine("A game has not been started. Please start one by calling one of the new game commands.");
                }
                break;

            case "-save puzzle":
                if (mainGame.Active())
                {
                    mainGame.SavePuzzle();
                }
                else
                {
                    Console.WriteLine("A game has not been started. Please start one by calling one of the new game commands.");
                }
                break;

            case "-load puzzle":
            case "-load":
                mainGame.Load(ref mainGame);
                break;

            case "-show puzzle":
                if (mainGame.Active())
                {
                    mainGame.ShowPuzzle();
                }
                else
                {
                    Console.WriteLine("A game has not been started. Please start one by calling one of the new game commands.");
                }
                break;

            case "-show found words":
                if (mainGame.Active())
                {
                    mainGame.ShowFoundWords();
                }
                else
                {
                    Console.WriteLine("A game has not been started. Please start one by calling one of the new game commands.");
                }
                break;

            case "-shuffle":
                if (mainGame.Active())
                {
                    mainGame.Shuffle();
                }
                else
                {
                    Console.WriteLine("A game has not been started. Please start one by calling one of the new game commands.");
                }
                break;

            case "-show status":
                if (mainGame.Active())
                {
                    mainGame.ShowStatus();
                }
                else
                {
                    Console.WriteLine("A game has not been started. Please start one by calling one of the new game commands.");
                }
                break;

            case "-new game":
            case "-new":
                mainGame.NewPuzzle();
                break;

            case "-new game from word":
                Console.WriteLine("Please enter a valid pangram: ");

                string pang = Console.ReadLine().Trim();
                mainGame.NewPuzzleBaseWord(pang);
                break;

            default:
                if (mainGame.Active())
                {
                    mainGame.Guess(input);
                }
                else
                {
                    Console.WriteLine("This is not a valid command. Please use -help to see the list of commands.");
                }
                break;
        }
    }
}