using SpellingBee;
using System.Collections.Immutable;

namespace TestSpellingBee
{
    public class TestController
    {

        [Fact]
        public void ValidateShuffle()
        {
            // Arrange
            var model = new GameModel(); // Instantiate the actual GameModel
            var view = new GameView();   // Instantiate the actual GameView
            var controller = new GameController(model, view);
            var counter = 0;
            // Save the original base word
            var originalBaseWord = model.GetBaseWord();

            // Act
            controller.Shuffle();

            // Assert
            var shuffledBaseWord = model.GetBaseWord();

          // Verify that the elements in the collections are the same, but their order is different
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

        [Fact]
        public void NewPuzzleResetsPoints()
        {

            var model = new GameModel();
            var view = new GameView();
            var controller = new GameController(model, view);
            controller.NewPuzzle();

            Assert.True(model.GetPlayerPoints() == 0);
        }
        
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
    }
}