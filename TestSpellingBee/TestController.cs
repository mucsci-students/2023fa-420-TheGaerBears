using SpellingBee;

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

    }
}