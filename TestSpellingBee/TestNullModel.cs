using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellingBee;

namespace TestSpellingBee
{
    /// <summary>
    /// Tests for the NullModel class.
    /// </summary>
    public class TestNullModel
    {
        /// <summary>
        /// Verifies that the <c>GetCurrentScore</c> method returns -1.
        /// </summary>
        [Fact]
        public void VerifyNullCurrentScore()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());

            Assert.Equal(-1, nullModel.GetCurrentScore());
        }

        /// <summary>
        /// Verifies that the <c>ShuffleBaseWord</c> method does nothing when the model is null.
        /// </summary>
        [Fact]
        public void VerifyNullShuffle() 
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);
            
            Assert.False(controller.GameStarted());
            nullModel.ShuffleBaseWord();
            Assert.Empty(controller.GetBaseWord());
        }

        /// <summary>
        /// Verifies that <c>AddFoundWords</c> method does nothing when
        /// the model is null.
        /// </summary>
        [Fact]
        public void VerifyNullFoundWord()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());
            nullModel.AddFoundWord("codable");
            Assert.Empty(controller.GetFoundWords());
        }

        /// <summary>
        /// Verifies that <c>Active</c> method returns false when the model is null.
        /// </summary>
        [Fact]
        public void VerifyNullActive()
        {
            NullModel nullModel = new();

            Assert.False(nullModel.Active());
        }

        /// <summary>
        /// Verifies that the <c>IsValidWord</c> method returns false when model is null.
        /// </summary>
        [Fact]
        public void VerifyNullIsValidWord()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());
            Assert.False(nullModel.IsValidWord("codable"));
        }
        /// <summary>
        /// Verifies that the <c>SetBaseWordForPuzzle</c> method returns a nullModel
        /// when a game has not started.
        /// </summary>
        [Fact]
        public void VerifyNullModelBaseWord()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());
            Assert.Equal(nullModel, nullModel.SetBaseWordForPuzzle("codable"));
        }

        /// <summary>
        /// Verifies that the <c>IsWordAlreadyFound</c> method returns false
        /// when the model is null.
        /// </summary>
        [Fact]
        public void VerifyNullIsWordAlreadyFound()
        {
            NullModel nullModel = new();
            Assert.False(nullModel.IsWordAlreadyFound("codable"));
        }
        /// <summary>
        /// Verifies that the <c>GetNextRankThreshold</c> method returns -1 when a game has not started.
        /// </summary>
        [Fact]
        public void VerifyNullGetNextRankThreshold()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());

            Assert.Equal(-1, nullModel.GetNextRankThreshold());
        }

        /// <summary>
        /// Verifies that the  <c>SaveCurrentGameState</c> method returns false.
        /// </summary>
        [Fact]
        public void VerifyNullCurrentGameState()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());

            Assert.False(nullModel.SaveCurrentGameState("save"));
        }

        /// <summary>
        /// Verifies that the  <c>SaveCurrentPuzzleState</c> method returns false
        /// when model is null.
        /// </summary>
        [Fact]
        public void VerifyNullPuzzleState()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());

            Assert.False(nullModel.SaveCurrentPuzzleState("save"));
        }

        /// <summary>
        ///  Verifies that <c>GetMaxPoints</c> method returns -1 when model is null.
        /// </summary>
        [Fact]
        public void VerifyNullMaxPoints()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());
            Assert.Equal(-1, nullModel.GetMaxPoints());
        }

        /// <summary>
        /// Verifies that <c>GetValidWords</c> method returns empty when model is null.
        /// </summary>
        [Fact]
        public void VerifyNullValidWords()
        {
            GameModel model = new();
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());
            Assert.Equal(model.GetValidWords(), nullModel.GetValidWords());
        }

        /// <summary>
        /// Verifies that <c>GetAllRanks</c> returns empty when
        /// the model is null. 
        /// </summary>
        [Fact]
        public void VerifyNullAllRanks()
        {
            GameModel model = new();
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());
            Assert.NotEqual(model.GetAllRanks(), nullModel.GetAllRanks());
        }

        /// <summary>
        /// Verifies that <c>wonTheGame</c> method returns false.
        /// </summary>
        [Fact]
        public void VerifyNotWonTheGame()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);
            Assert.False(controller.GameStarted());

            Assert.False(nullModel.wonTheGame());
        }

        /// <summary>
        /// Verifies that <c>GetRequiredLetter</c> returns '-' when model is null.
        /// </summary>
        [Fact]
        public void VerifyNullRequiredLetter()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);
            Assert.False(controller.GameStarted());
            Assert.Equal('-', nullModel.GetRequiredLetter());
        }

        /// <summary>
        /// Verifies that <c>GetPlayerPoints</c> returns -1 when model is null.
        /// </summary>
        [Fact]
        public void VerifyNullPlayerPoints()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);

            Assert.False(controller.GameStarted());
            Assert.Equal(-1, nullModel.GetPlayerPoints());
        }

        /// <summary>
        /// Verifies that <c>GetStatusTitles</c> returns empty when model is null.
        /// </summary>
        [Fact]
        public void VerifyNullStatusTitles()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);
            List<KeyValuePair<string, int>> l = new();
            Assert.False(controller.GameStarted());

            Assert.Equal(l, nullModel.GetStatusTitles());
        }

        /// <summary>
        /// Verifies that <c>GetFoundWords</c> returns empty when model is null.
        /// </summary>
        [Fact]
        public void VerifyNullFoundWords()
        {
            NullModel nullModel = new();
            GuiController controller = new GuiController(nullModel);
            List<string> l = new();
            Assert.False(controller.GameStarted());

            Assert.Equal(l,nullModel.GetFoundWords());
        }
    }
}
