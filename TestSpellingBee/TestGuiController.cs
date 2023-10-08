using SpellingBee;

namespace TestSpellingBee
{
    /// <summary>
    /// Tests for the GuiController class.
    /// </summary>
    public class TestGuiController
    {   
        /// <summary>
        /// Verifies that the <c>NewPuzzle</c> changes the <c>baseWord</c>
        /// and resets <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void NewPuzzleResetsState()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            controller.NewPuzzle();
            var b1 = model.GetBaseWord();
            controller.NewPuzzle();
            var b2 = model.GetBaseWord();
            Assert.NotEqual(b1, b2);
        }
        /// <summary>
        /// Verifies that the <c>NewPuzzleFromBaseword</c> method sets the <c>baseWord</c> 
        /// as the inputted word and resets <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void NewPuzzleFromBaseWord()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            string word = "codable";
            controller.NewPuzzleBaseWord(word);

            Assert.Equal(0, model.GetPlayerPoints());

            bool testing = false;
            foreach (var chr in model.GetBaseWord())
            {
                testing = false;
                foreach (var let in word.ToList<char>().Distinct())
                {
                    if (let.Equals(chr))
                        testing = true;
                }
                if (testing == false)
                    break;
            }
            Assert.True(testing);
        }

        /// <summary>
        /// Verifies that the <c>ShuffleBaseWord</c> method changes the order of the letters in <c>baseWord</c>.
        /// </summary>
        [Fact]
        public void ValidateShuffle()
        {
            var model = new GameModel();
            var controller = new GuiController(model);
            var counter = 0;

            var originalBaseWord = model.GetBaseWord();

            controller.ShuffleBaseWord();

            var shuffledBaseWord = model.GetBaseWord();

            foreach (var sLetter in shuffledBaseWord)
            {
                foreach (var oLetter in originalBaseWord)
                {
                    if (sLetter.Equals(oLetter))
                    {
                        ++counter;
                    }
                }
            }
            Assert.True(counter != 7);
        }

        /// <summary>
        /// Verifies that <c>Guess</c> method adds a valid guess to <c>foundWords</c> and 
        /// adds points to <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void GuessValid()
        {
            var model = new GameModel();
            var controller = new GuiController(model);
            controller.NewPuzzleBaseWord("codable");

            Assert.Contains("codable", model.GetValidWords());

            controller.Guess("codable");

            Assert.NotEqual(0, model.GetPlayerPoints());
        }

        /// <summary>
        /// Verifies that the <c>Guess</c> method does not add an invalid word to <c>foundWords</c> nor 
        /// adds points to  <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void GuessInvalid()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            controller.NewPuzzleBaseWord("codable");

            Assert.DoesNotContain("false", model.GetValidWords());

            controller.Guess("false");

            Assert.Equal(0, model.GetPlayerPoints());
        }

        /// <summary>
        /// Verifies that the <c>Save</c> functions save state to a JSON file.
        /// </summary>
        [Fact]
        public void SaveVerify()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

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
            var controller = new GuiController(model);

            string oWord = "codable";

            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);


            model.SaveCurrentGameState("testGUI");

            //Copy old data of puzzle to compare
            List<char> oBaseWord = new List<char>(model.GetBaseWord());
            char oReqLetter = model.GetRequiredLetter();
            IEnumerable<string> oFoundWords = new List<string>(model.GetFoundWords());
            int oPlayerPoints = model.GetPlayerPoints();
            int oMaxPoints = model.GetMaxPoints();

            //Verify load
            controller.NewPuzzleBaseWord("companion");

            model = model.LoadGameStateFromFile(1);

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
    }
}
