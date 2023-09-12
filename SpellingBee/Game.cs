using System;
using System.Collections.Generic;

namespace SpellingBee
{
    internal class Game
    {
        private List<char> baseWord;
        private char requiredLetter;
        private Random rand;
        private List<string> validWords;
        private List<string> foundWords;
        private List<KeyValuePair<string, int>> statusTitles;
        private int playerPoints;
        private int totalPossiblePoints;

        public Game()
        {
            baseWord = new List<char>();
            requiredLetter = 'a';
            rand = new Random();
            validWords = new List<string>();
            foundWords = new List<string>();
            statusTitles = new List<KeyValuePair<string, int>>()
                {
                    new KeyValuePair<string, int>("Beginner", 0),
                    new KeyValuePair<string, int>("Good Start", 2),
                    new KeyValuePair<string, int>("Moving Up", 5),
                    new KeyValuePair<string, int>("Good", 8),
                    new KeyValuePair<string, int>("Solid", 15),
                    new KeyValuePair<string, int>("Nice", 25),
                    new KeyValuePair<string, int>("Great", 40),
                    new KeyValuePair<string, int>("Amazing", 50),
                    new KeyValuePair<string, int>("Genius", 70),
                    new KeyValuePair<string, int>("Queen Bee", 100)
                };
            playerPoints = 0;
            totalPossiblePoints = 0;
        }

        /// <summary>
        /// Prints out the words found by the user, in one line
        /// </summary>
        /// <param name="foundWords"></param>
        public void ShowFoundWords()
        {
            Console.WriteLine("The words you have found are: ");
            foreach (string word in foundWords)
            {
                if (foundWords.IndexOf(word) < foundWords.Count - 1)
                    Console.Write(word + ", ");

                else
                    Console.WriteLine(word);
            }
        }
        /// <summary>
        /// Exits the console app
        /// </summary>
        public void Exit()
        {
            System.Environment.Exit(0);
        }

        public void Shuffle()
        {
            int n = baseWord.Count;

            for (int i = n - 1; i > 0; i--)
            {
                // Get a random index up to i inclusive
                int j = rand.Next(i + 1);

                // Swap elements
                char temp = baseWord[i];
                baseWord[i] = baseWord[j];
                baseWord[j] = temp;
            }
            Console.WriteLine("Shuffled letters:");

            foreach (char letter in baseWord)
            {
                Console.Write(letter + " ");
            }
            Console.WriteLine("\n");
        }
        public void ShowStatus()
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
        void ShowPuzzle()
        {
            Console.Write("Puzzle Letters: ");
            foreach (char letter in letters)
            {
                Console.Write(letter + " ");
            }
            Console.WriteLine();

            Console.WriteLine($"The required letter is: {requiredLetter}");
        }

        public void PuzzleRank()
        {
            int uniqueLetterCount = foundWords.Last().Distinct().Count(); // Count of unique letters in the word
            int wordLength = foundWords.Last().Length;
            int points = 0;

            if (wordLength == 4)
            {
                points = 1;
            }
            else if (wordLength == 5 || wordLength == 6)
            {
                points = wordLength;
            }
            else if (wordLength > 6)
            {
                points = wordLength + (uniqueLetterCount == 7 ? 7 : 0);
            }

            playerPoints += points; // Add the points to the player's total score.
        }

    }
}
