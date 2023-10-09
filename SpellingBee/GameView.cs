using System;
using System.Collections.Generic;

namespace SpellingBee
{

    public class GameView
    {
        /// <summary>
        /// Center-aligns and displays the specified text in the console.
        /// </summary>
        public void CenterText(string text)
        {
            Console.Write(new string(' ', (Console.WindowWidth - text.Length) / 2));
            Console.WriteLine(text);
        }

        /// <summary>
        /// Displays the player's score on the console.
        /// </summary>
        public void DisplayScore(int playerPoints)
        {
            Console.WriteLine($"Your current score is: {playerPoints}");
        }

        /// <summary>
        /// Displays the player's status based on points and a list of possible statuses.
        /// </summary>
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

            Console.WriteLine($"Your current points: {playerPoints}");
            Console.WriteLine($"Your status: {status}");
        }

        /// <summary>
        /// Displays the initial game screen with instructions.
        /// </summary>
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

        /// <summary>
        /// Displays a list of words that the player has found so far.
        /// </summary>
        public void ShowFoundWords(IEnumerable<string> foundWords)
        {
            Console.WriteLine("Words found so far:");
            foreach (var word in foundWords)
            {
                Console.WriteLine(word);
            }
        }

        /// <summary>
        /// Displays the puzzle's letters along with the required letter.
        /// </summary>
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

        /// <summary>
        /// Lists available save files for the user to choose from.
        /// </summary>
        public void DisplaySaveFilesList(List<string> fileList)
        {
            Console.WriteLine("Which save file id would you like to load?");
            for (int i = 0; i < fileList.Count; i++)
            {
                Console.WriteLine($"{i}: {System.IO.Path.GetFileNameWithoutExtension(fileList[i])}");
            }
        }

        /// <summary>
        /// Prompts the user for a save file ID and returns the selected ID.
        /// </summary>
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

        /// <summary>
        /// Reads and returns a user input string from the console.
        /// </summary>
        public string GetInput()
        {
            return Console.ReadLine().ToLower().Trim();
        }


        /// <summary>
        /// Displays a specified message on the console.
        /// </summary>
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Displays the game's help information, including available commands.
        /// </summary>
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
