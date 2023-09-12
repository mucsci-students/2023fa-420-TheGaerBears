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

        void Shuffle(List<char> letters)
        {
            int n = letters.Count;

            for (int i = n - 1; i > 0; i--)
            {
                // Get a random index up to i inclusive
                int j = rand.Next(i + 1);

                // Swap elements
                char temp = letters[i];
                letters[i] = letters[j];
                letters[j] = temp;
            }
            Console.WriteLine("Shuffled letters:");

            foreach (char letter in letters)
            {
                Console.Write(letter + " ");
            }
            Console.WriteLine("\n");
        }

    }
}
