using SpellingBee;

namespace TestSpellingBee
{
    /// <summary>
    /// Tests for the GameModel class.
    /// </summary>
    public class TestModel
    {
        /// <summary>
        /// Verifies that the <c>PangramList</c> function creates a list of pangrams from our dictionary (words with unique letters)
        /// </summary>
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

        /// <summary>
        /// Verifies that the <c>GenerateValidWords</c> function creates a list of words with required letter and contained in base word.
        /// </summary>
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

        /// <summary>
        /// Verifies that the <c>Save</c> function saves state to a JSON file.
        /// </summary>
        [Fact]
        public void SaveVerify()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            string oWord = "codable";
            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);
            char oReqLetter = model.GetRequiredLetter();
            int oPlayerPoints = model.GetPlayerPoints();
            int oMaxPoints = model.GetMaxPoints();

            model.SaveCurrentGameState("test");

            String filePath = "..\\..\\debug\\net6.0\\saves\\test.json";

            using (StreamReader reader = new StreamReader(filePath))
            {
                string content = reader.ReadToEnd();

                Assert.Contains(oWord, content);
                Assert.Contains(oReqLetter, content);
                Assert.Contains(oPlayerPoints.ToString(), content);
                Assert.Contains(oMaxPoints.ToString(), content);
            }
        }

        /// <summary>
        /// Verifies that the <c>Load</c> function loads a saved game from a JSON file.
        /// </summary>
        [Fact]
        public void LoadVerify()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            string oWord = "codable";

            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);
            

            model.SaveCurrentGameState("test");

            //Copy old data of puzzle to compare
            List<char> oBaseWord = new List<char>(model.GetBaseWord());
            char oReqLetter = model.GetRequiredLetter();
            IEnumerable<string> oFoundWords = new List<string>(model.GetFoundWords());
            int oPlayerPoints = model.GetPlayerPoints();
            int oMaxPoints = model.GetMaxPoints();

            //Verify load
            controller.NewPuzzleBaseWord("companion");

            model = model.LoadGameStateFromFile(0);

            oBaseWord.Sort();

            List<char> bWord = model.GetBaseWord();
            bWord.Sort();

            //Checks to make sure word is same
            Assert.Equal(bWord, oBaseWord);

            //Checks to make sure required letter is same
            Assert.Equal(model.GetRequiredLetter(), oReqLetter);
            
            //Checks to make sure points are the same
            Assert.Equal(model.GetFoundWords(), oFoundWords);

            //Checks the player points
            Assert.Equal(model.GetPlayerPoints(), oPlayerPoints);

            //Checks the max points
            Assert.Equal(model.GetMaxPoints(), oMaxPoints);
        }

        /// <summary>
        /// Verifies that <c>PlayerPoints</c> updates when there is a valid word and doesn't update on invalid word.
        /// </summary>
        [Fact]
        public void PlayerPointsUpdate()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            string oWord = "codable";

            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);

            Assert.Equal(14, model.GetPlayerPoints());

            controller.Guess("false");

            Assert.Equal(14, model.GetPlayerPoints());
        }

    }
}
