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

        Console.WriteLine("Welcome to the Spelling Bee Puzzle Game!");
        Console.WriteLine("Here are some commands to help you out:");
        //Call help command here
        mainGame.Help();

        CreateDatabase db = new CreateDatabase();

        

        //While loop that keeps game running
        while (true)
        {
            string input = Console.ReadLine().Trim();
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
                //Add when done
                break;

            case "-save puzzle":
                //Add when done
                break;

            case "-load puzzle":
                //Add when done
                break;

            case "-show puzzle":
                //Needs to check if puzzle started for many of these commands
                //Will add function to game class to check this
                mainGame.ShowPuzzle();
                break;

            case "-show found words":
                mainGame.ShowFoundWords();
                break;

            case "-shuffle":
                mainGame.Shuffle();
                break;

            case "-show status":
                mainGame.ShowStatus();
                break;

            case "-guess":
                mainGame.Guess();
                break;

            case "-new game":
                mainGame.NewPuzzle();
                break;

            case "-new game from word":
                Console.WriteLine("Please enter a valid pangram: ");

                string pang = Console.ReadLine().Trim();
                mainGame.NewPuzzleBaseWord(pang);
                break;

            default:
                Console.WriteLine("Sorry this is not a valid input. Please refer to '-help' for valid commands.");
                break;
        }
    }
}