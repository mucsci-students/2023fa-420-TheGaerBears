using SpellingBee;

namespace TestSpellingBee
{
    /// <summary>
    /// Tests for the CliController class.
    /// </summary>
    public class TestCliController
    {

        private void ClearCliControllerInstance()
        {
            var instanceField = typeof(CliController).GetField("_instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            instanceField.SetValue(null, null);
        }

        ~TestCliController()
        {
            // Clear the Singleton instance after all tests are executed
            ClearCliControllerInstance();
        }

        private CliController cliControllerInstance;

        private void InitializeCliController()
        {
            GameModel model = new GameModel();
            GameView view = new GameView();
            cliControllerInstance = CliController.GetInstance(model, view);
        }

        private void ResetCliControllerInstance()
        {
            var instanceField = typeof(CliController).GetField("_instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            instanceField.SetValue(null, null);
        }
        /// <summary>
        /// Verifies that the <c>Shuffle</c> method changes the order of the letters in <c>baseWord</c>.
        /// </summary>
        [Fact]
        public void ValidateShuffle()
        {
            // Ensure the instance is cleared before the test
            ResetCliControllerInstance();
            // Initialize the CliController instance
            InitializeCliController(); 

            var cliController = CliController.GetExistingInstance();
            var model = cliController.GetModelInstance();
            var counter = 0;
  
            var originalBaseWord = model.GetBaseWord();

            cliController.ShuffleBaseWord();

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
        /// Verifies that the <c>NewPuzzle</c> method changes the <c>baseWord</c>.
        /// </summary>
        [Fact]
        public void NewPuzzleResetsBaseWord()
        {
            // Ensure the instance is cleared before the test
            ResetCliControllerInstance();
            // Initialize the CliController instance
            InitializeCliController();

            var cliController = CliController.GetExistingInstance();

            var model = cliController.GetModelInstance();

            Assert.False(cliController._model.IsNull());
            Assert.False(model.IsNull());
            Assert.NotEmpty(model.GetPangramList());

            cliController.NewPuzzle();
            Assert.NotEqual(0, model.GetRequiredLetter());
            var b1 = cliController.GetBaseWord();
            Assert.NotEmpty(b1);

            Assert.NotEqual(0, model.GetRequiredLetter());
            Assert.False(model.IsNull());

            cliController.NewPuzzle();
            var b2 = cliController.GetBaseWord();

            Assert.NotEmpty(b2);
            Assert.False(model.IsNull());
            Assert.NotEqual(b1, b2);
        }

        /// <summary>
        /// Verifies that the <c>NewPuzzle</c> method resets the <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void NewPuzzleResetsPoints()
        {
            // Ensure the instance is cleared before the test
            ResetCliControllerInstance();
            // Initialize the CliController instance
            InitializeCliController();

            var cliController = CliController.GetExistingInstance();
            var model = cliController.GetModelInstance();
            cliController.NewPuzzle();

            Assert.True(model.GetPlayerPoints() == 0);
        }

        /// <summary>
        /// Verifies that the <c>NewPuzzleFromBaseword</c> method sets the <c>baseWord</c> 
        /// as the inputted word and resets <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void NewPuzzleFromBaseWord()
        {
            // Ensure the instance is cleared before the test
            ResetCliControllerInstance();
            // Initialize the CliController instance
            InitializeCliController();

            var cliController = CliController.GetExistingInstance();

            var model = cliController.GetModelInstance();

            string word = "codable";
            cliController.NewPuzzleBaseWord(word);

            Assert.Equal(0, model.GetPlayerPoints());

            bool testing = false;
            foreach (var chr in model.GetBaseWord())
            {
                testing = false;
                foreach(var let in word.ToList<char>().Distinct())
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
        /// Verifies that <c>Guess</c> method adds a valid guess to <c>foundWords</c> and 
        /// adds points to <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void GuessValid()
        {
            // Ensure the instance is cleared before the test
            ResetCliControllerInstance();
            // Initialize the CliController instance
            InitializeCliController();

            
            var cliController = CliController.GetExistingInstance();
            var model = cliController.GetModelInstance();
            cliController.NewPuzzleBaseWord("codable");

            Assert.Contains("codable", model.GetValidWords());

            cliController.Guess("codable");

            Assert.NotEqual(0, model.GetPlayerPoints());
        }

        /// <summary>
        /// Verifies that the <c>Guess</c> method does not add an invalid word to <c>foundWords</c> nor 
        /// adds points to  <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void GuessInvalid()
        {
            // Ensure the instance is cleared before the test
            ResetCliControllerInstance();
            // Initialize the CliController instance
            InitializeCliController();

            var cliController = CliController.GetExistingInstance();
            var model = cliController.GetModelInstance();

            cliController.NewPuzzleBaseWord("codable");

            Assert.DoesNotContain("false", model.GetValidWords());

            cliController.Guess("false");

            Assert.Equal(0, model.GetPlayerPoints());
        }
    }
}