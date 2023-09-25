using System;

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
 * - DisplayMessage: Outputs a generic message to the console.
 * - Help: Lists all the available commands and instructions for the game.
 * 
 * Dependencies:
 * -------------
 * This class mainly interacts with the GameController and doesn't hold any game logic or data.
 * Instead, it provides visualization based on data and commands given to it.
 * 
 * Usage:
 * ------
 * GameView should be instantiated and used along with GameModel and GameController to enable a complete game flow.
 */

    internal class GameView
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
        public void ShowStatus(int playerPoints, int totalPossiblePoints, List<KeyValuePair<string, int>> statusTitles)
        {
            double ratio = (double)playerPoints / totalPossiblePoints;
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

            Console.WriteLine($"Your current points: {playerPoints}");
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

        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void Help()
        {
            Console.WriteLine("Welcome to the Spelling Bee game!");
            Console.WriteLine("-new game: Starts a new puzzle game.");
            Console.WriteLine("-new game from word: Starts a new puzzle game with your word.");
            Console.WriteLine("-load: Load a saved game or puzzle.");
            Console.WriteLine("-save current: Save the current game with progress.");
            Console.WriteLine("-save puzzle: Save the current puzzle.");
            Console.WriteLine("-show found words: Display the words you have found.");
            Console.WriteLine("-show puzzle: Display the puzzle letters.");
            Console.WriteLine("-show status: Display your current game status.");
            Console.WriteLine("-shuffle: Shuffle the puzzle letters.");
            Console.WriteLine("-help: Show this list of commands.");
            Console.WriteLine("-exit: Exit the game.");
            Console.WriteLine("You can also simply type in a word to make a guess.");
            Console.WriteLine("Remember, all words must contain the required letter!");
        }
    }
}
