using System;
using System.Collections.Generic;

namespace SpellingBee
{
    internal class Game
    {
        /// <summary>
        /// Prints out the words found by the user, in one line
        /// </summary>
        /// <param name="foundWords"></param>
        public void ShowFoundWords(List<string> foundWords)
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
    }
}
