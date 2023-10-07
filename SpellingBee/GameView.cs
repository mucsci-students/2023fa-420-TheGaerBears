using System;
using System.Collections.Generic;

namespace SpellingBee
{
/*
 * GameView Class - SpellingBee
 * 
 * Purpose:
 * --------
 * The GameView class provides methods to display game-related information to the user.
 * It acts as the visual representation of the game and communicates directly with the user.
 * 
 * Main Features:
 * --------------
 * - CenterText: Displays a given text in the center of the console.
 * - DisplayScore: Shows the player's current score.
 * - ShowStatus: Provides an overview of the player's current status based on their score and achievements.
 * - BeginScreen: Displays the initial welcoming screen of the game.
 * - ShowFoundWords: Lists all the words that the player has found in the current game.
 * - ShowPuzzle: Presents the current puzzle letters and the required letter.
 * - DisplaySaveFilesList: Shows the list of available saved games for loading.
 * - GetFileIdFromUser: Asks the user to select a saved game from the list.
 * - GetInput: Gets input from user.
 * - DisplayMessage: Outputs a generic message to the console.
 * - Help: Lists all the available commands and instructions for the game.
 * - Exit: Displays exiting message for the game.
 * 
 * Dependencies:
 * -------------
 * This class mainly interacts with the CliController and doesn't hold any game logic or data.
 * Instead, it provides visualization based on data and commands given to it.
 * 
 * Usage:
 * ------
 * GameView should be instantiated and used along with GameModel and CliController to enable a complete game flow.
 */

    public class GameView
    {
        // This function center-aligns text on the console
        public void CenterText(string text)
        {
            Console.Write(new string(' ', (Console.WindowWidth - text.Length) / 2));
            Console.WriteLine(text);
        }

        // Display the player's score on the console
        public void DisplayScore(int playerPoints)
        {
            Console.WriteLine($"Your current score is: {playerPoints}");
        }

        // Display the player's status on the console
        public void ShowStatus(int playerPoints, int maxPoints, List<KeyValuePair<string, int>> statusTitles)
        {
            double ratio = (double)playerPoints / maxPoints;
            double percentageAsDecimal = ratio * 100;
            int percentage = (int)Math.Round(percentageAsDecimal);

            string status = "Beginner"; // Default status

            foreach (var title in statusTitles)
            {
                if (percentage >= title.Value)
                {
                    status = title.Key;
                }
            }

            Console.WriteLine($"Your current Points: {playerPoints}");
            Console.WriteLine($"Your status: {status}");
        }

        // Show the beginning screen of the game
        public void BeginScreen()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            CenterText("Spelling Bee Puzzle Game");
            Console.WriteLine();
            CenterText("Enter a Command: (-New Game, -Load, -Help)");
            Console.WriteLine();
            Console.WriteLine();
        }

        public void ShowFoundWords(IEnumerable<string> foundWords)
        {
            Console.WriteLine("Words found so far:");
            foreach (var word in foundWords)
            {
                Console.WriteLine(word);
            }
        }


        // Display the current puzzle letters and the required letter
        public void ShowPuzzle(List<char> baseWord, char requiredLetter)
        {
            Console.Write("Puzzle Letters: ");
            foreach (char letter in baseWord)
            {
                Console.Write(letter + " ");
            }
            Console.WriteLine();

            Console.WriteLine($"The required letter is: {requiredLetter}");
        }

        public void DisplaySaveFilesList(List<string> fileList)
        {
            Console.WriteLine("Which save file id would you like to load?");
            for (int i = 0; i < fileList.Count; i++)
            {
                Console.WriteLine($"{i}: {System.IO.Path.GetFileNameWithoutExtension(fileList[i])}");
            }
        }

        public int GetFileIdFromUser()
        {
            Console.Write("Enter the file ID: ");
            if (int.TryParse(Console.ReadLine(), out int fileId))
            {
                return fileId;
            }
            // Indicates an invalid ID or failed parsing.
            return -1; 
        }

        public string GetInput()
        {
            return Console.ReadLine().ToLower().Trim();
        }


        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void Help()
        {
            Console.WriteLine (
                @"
                -new game:           Starts a new puzzle game.
                -new game from word: Starts a new puzzle game with your word.
                -load:               Load a saved game or puzzle.
                -save current:       Save the current game with progress.
                -save puzzle:        Save the current puzzle.
                -show found words:   Display the words you have found.
                -show puzzle:        Display the puzzle letters.
                -show status:        Display your current game status.
                -shuffle:            Shuffle the puzzle letters.
                -help:               Show this list of commands.
                -exit:               Exit the game.

                Simply type in a word to make a guess.
                Remember, all words must contain the required letter!");
        }


        public void Exit()
        {
            Console.WriteLine("Thank you for playing Spelling Bee. Goodbye!");
        }
    }
}
