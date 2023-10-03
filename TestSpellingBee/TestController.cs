using SpellingBee;
using System.Collections.Immutable;

namespace TestSpellingBee
{
    /// <summary>
    /// Tests for the GameController class.
    /// </summary>
    public class TestController
    {
        /// <summary>
        /// Verifies that Shuffle function changes the order of the letters in baseword
        /// </summary>
        [Fact]
        public void ValidateShuffle()
        {
           
            var model = new GameModel(); 
            var view = new GameView(); 
            var controller = new GameController(model, view);
            var counter = 0;
  
            var originalBaseWord = model.GetBaseWord();

            controller.Shuffle();

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
        /// Verifies that NewPuzzle function changes the baseword
        /// </summary>
        [Fact]
        public void NewPuzzleResetsBaseWord()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new GameController(model, view);

            controller.NewPuzzle();
            var b1 = model.GetBaseWord();
            controller.NewPuzzle();
            var b2 = model.GetBaseWord();
            Assert.NotEqual(b1, b2);
        }

        /// <summary>
        /// Verifies that NewPuzzle resets the player points
        /// </summary>
        [Fact]
        public void NewPuzzleResetsPoints()
        {

            var model = new GameModel();
            var view = new GameView();
            var controller = new GameController(model, view);
            controller.NewPuzzle();

            Assert.True(model.GetPlayerPoints() == 0);
        }

        /// <summary>
        /// Verifies that NewPuzzleFromBaseword sets the baseword as the inputted word and resets points
        /// </summary>
        [Fact]
        public void NewPuzzleFromBaseWord()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new GameController(model, view);

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
        /// Verifies that Guess function adds a valid guess to foundWords and adds points
        /// </summary>
        [Fact]
        public void GuessValid()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new GameController(model, view);
            controller.NewPuzzleBaseWord("codable");

            Assert.Contains("codable", model.GetValidWords());

            controller.Guess("codable");

            Assert.NotEqual(0, model.GetPlayerPoints());
        }

        /// <summary>
        /// Verifies that the Guess function does not add an invalid word to foundWords nor adds points
        /// </summary>
        [Fact]
        public void GuessInvalid()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new GameController(model, view);

            controller.NewPuzzleBaseWord("codable");

            Assert.DoesNotContain("false", model.GetValidWords());

            controller.Guess("false");

            Assert.Equal(0, model.GetPlayerPoints());
        }
      
    }
}