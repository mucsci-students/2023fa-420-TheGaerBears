using SpellingBee;

namespace TestSpellingBee
{
    /// <summary>
    /// Tests for the CliController class.
    /// </summary>
    public class TestCliController
    {
        /// <summary>
        /// Verifies that the <c>Shuffle</c> method changes the order of the letters in <c>baseWord</c>.
        /// </summary>
        [Fact]
        public void ValidateShuffle()
        {
            var model = new GameModel(); 
            var view = new GameView(); 
            var controller = new CliController(model, view);
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
        /// Verifies that the <c>NewPuzzle</c> method changes the <c>baseWord</c>.
        /// </summary>
        [Fact]
        public void NewPuzzleResetsBaseWord()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            controller.NewPuzzle();
            var b1 = model.GetBaseWord();
            controller.NewPuzzle();
            var b2 = model.GetBaseWord();
            Assert.NotEqual(b1, b2);
        }

        /// <summary>
        /// Verifies that the <c>NewPuzzle</c> method resets the <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void NewPuzzleResetsPoints()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);
            controller.NewPuzzle();

            Assert.True(model.GetPlayerPoints() == 0);
        }

        /// <summary>
        /// Verifies that the <c>NewPuzzleFromBaseword</c> method sets the <c>baseWord</c> 
        /// as the inputted word and resets <c>playerPoints</c>.
        /// </summary>
        [Fact]
        public void NewPuzzleFromBaseWord()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            string word = "codable";
            controller.NewPuzzleBaseWord(word);

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
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);
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
            var view = new GameView();
            var controller = new CliController(model, view);

            controller.NewPuzzleBaseWord("codable");

            Assert.DoesNotContain("false", model.GetValidWords());

            controller.Guess("false");

            Assert.Equal(0, model.GetPlayerPoints());
        }
    }
}