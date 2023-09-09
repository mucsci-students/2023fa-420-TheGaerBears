using SpellingBee.SetUpSpellingBee.src.main;
using System;
using SQLitePCL;
internal class Program
{
    static void Main(string[] args)
    {
        // Initialize SQLitePCL
        Batteries.Init();

        Console.WriteLine("Start here!");

        // Create an instance of the DatabaseLoader
        var loader = new CreateDatabase();

        // Load data from JSON files into corresponding tables
       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\4-letter-words.json", "four_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\5-letter-words.json", "five_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\6-letter-words.json", "six_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\7-letter-words.json", "seven_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\8-letter-words.json", "eight_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\9-letter-words.json", "nine_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\10-letter-words.json", "ten_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\11-letter-words.json", "eleven_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\12-letter-words.json", "twelve_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\13-letter-words.json", "thirteen_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\14-letter-words.json", "fourteen_letter_words");

       loader.LoadDataFromJsonFile("C:\\Users\\skyfa\\source\\repos\\mucsci-students\\2023fa-420-TheGaerBears\\SpellingBee\\SetUpSpellingBee\\src\\15-letter-words.json", "fifteen_letter_words");
    }
}