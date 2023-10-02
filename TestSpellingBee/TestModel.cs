using SpellingBee;
using System.Collections.Immutable;

namespace TestSpellingBee
{
    public class TestModel
    {
        [Fact]
        public void ValidatePangramList()
        {
            var model = new GameModel();

            //Get list of pangrams from database
            var pangramList = model.PangramList();

            //Check to make sure every word is pangram
            bool tester = true;
            foreach (var str in pangramList)
            {
                string distWord = new string(str.Distinct().ToArray());
                if (distWord.Length != 7)
                {
                    tester = false;
                    break;
                }
            }
            //Check number of pangrams and make sure every word was a pangram
            Assert.True(pangramList.Count() == 40222);
            Assert.True(tester);
        }

        [Fact]
        public void ValidateGenerateValidWords()
        {
            //Get random word
            var model = new GameModel();
            model.SelectRandomWordForPuzzle();

            var word = model.GetBaseWord();
            var reqLetter = model.GetRequiredLetter();

            model.GenerateValidWords();

            bool testing = true;
            bool reqTest = false;

            word.Sort();

            //Get list of valid words for what the baseWord generates
            foreach (string vWord in model.GetValidWords())
            {
                //Change valid word to list and sort
                List<char> lets = vWord.Distinct().ToList<char>();
                lets.Sort();

                //Checks to make sure the required letter is in the word
                if (lets.Contains(reqLetter))
                    reqTest = true;

                //Checks to make sure every letter in the valid word is contained in the base word
                foreach (char chr in lets)
                {
                    if (!word.Contains(chr))
                    {
                        testing = false;
                        break;
                    }
                }
                if (!testing)
                    break;
            }

            Assert.True(reqTest);
            Assert.True(testing);
        }
    }
}