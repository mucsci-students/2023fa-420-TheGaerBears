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

        Console.WriteLine("Start here!");
        BaseWordAndList baseWord = new BaseWordAndList();
        String pangram = baseWord.IdentifyBaseWord('c');
        List<String> wordList = baseWord.GenerateWordList('c', pangram);
        Console.WriteLine(pangram);
        foreach (string word in wordList)
        {
            Console.WriteLine(word);
        }
    }
}