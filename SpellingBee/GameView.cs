using DynamicData;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellingBee
{

    public class GameView
    {
            private string[] tabCompletable = { "-exit", "-found words", "-help", "-hint", "-load", "-new", "-new game from word", "-puzzle", "-save current", "-save puzzle", "-show found words", "-show puzzle", "-show status", "-shuffle", "-status" };
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
        public void ShowStatus(int playerPoints, int maxPoints, List<KeyValuePair<string, int>> statusTitles, Dictionary<string, int> ranks)
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

            // Show the points needed for each rank
            Console.WriteLine("\n");
            Console.WriteLine("        Rank:                             Points needed:");
            Console.WriteLine("        ================================================");
            foreach (var rank in ranks)
            {
                int pointsPrint = rank.Value;
                string space = String.Concat(Enumerable.Repeat(" ", (40 - rank.Key.Length - (pointsPrint.ToString().Length / 2) + (pointsPrint.ToString().Length % 2 == 0 ? 1 : 0))));
                Console.WriteLine("        " + rank.Key + space + pointsPrint);
            }
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
                -hint:               Display helpful hints for solving the puzzle.
                -help:               Show this list of commands.
                -exit:               Exit the game.

                Simply type in a word to make a guess.
                Remember, all words must contain the required letter!");
        }


        public void Exit()
        {
            Console.WriteLine("Thank you for playing Spelling Bee. Goodbye!");
        }

        public string TabCompleteInput()
        {
            ConsoleKeyInfo input;
            var userInput = string.Empty;
            while (ConsoleKey.Enter != (input = Console.ReadKey()).Key)
            {
                if (input.Key == ConsoleKey.Backspace)
                    userInput = userInput.Any() ? userInput.Remove(userInput.Length - 1, 1) : string.Empty;

                else if (input.Key == ConsoleKey.Tab)
                {
                    List<string> valid = new List<string>();
                    foreach(string str in tabCompletable)
                    {
                        bool temp = true;
                        for(int i = 0; i < userInput.Length && i < str.Length;++i)
                        {
                            if (userInput[i] != str[i])
                            {
                                temp = false;
                            }
                        }
                        if (str.Length < userInput.Length)
                        {
                            temp = false;
                        }

                        if (temp)
                        {
                            valid.Add(str);
                        }
                    }
                    if (valid.Count < 1) ;
                    else if (valid.Count == 1)
                    {
                        userInput = valid[0];
                    }
                    else
                    {
                        Console.WriteLine();
                        foreach (string str in valid)
                        {
                            Console.Write(str + "\t");
                        }
                        Console.WriteLine();
                    }
                }
                else if (input != null)
                    userInput += input.KeyChar;


                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLineCursor);

                Console.Write(userInput);
            }
            Console.WriteLine();
            return userInput;
        }
    }
}