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
                string distWord = new(str.Distinct().ToArray());
                if (distWord.Length != 7)
                {
                    tester = false;
                    break;
                }
            }
            //Check number of pangrams and make sure every word was a pangram
            Assert.True(pangramList.Count == 40222);
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

            Assert.NotEmpty(word);
            Assert.NotEmpty(model.GetValidWords());

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

            model.SaveCurrentGameState("test-mod-sv");

            String filePath = "../../debug/net6.0/saves/test-mod-sv.json";

            using StreamReader reader = new(filePath);
            string content = reader.ReadToEnd();

            Assert.Contains(oWord, content);
            Assert.Contains(oReqLetter, content);
            Assert.Contains(oPlayerPoints.ToString(), content);
            Assert.Contains(oMaxPoints.ToString(), content);
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


            model.SaveCurrentGameState("a-test-mod-load");

            //Copy old data of puzzle to compare
            List<char> oBaseWord = new(model.GetBaseWord());
            char oReqLetter = model.GetRequiredLetter();
            IEnumerable<string> oFoundWords = new List<string>(model.GetFoundWords());
            int oPlayerPoints = model.GetPlayerPoints();
            int oMaxPoints = model.GetMaxPoints();

            //Verify load
            controller.NewPuzzleBaseWord("companion");

            model = (GameModel)model.LoadGameStateFromFile(0);

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

        /// <summary>
        /// Verifies that the <c>LoadGameStateFromFile</c> function returns null when the provided fileId is out of range.
        /// </summary>
        [Fact]
        public void LoadGameStateFromFileReturnsNullWhenFileIdIsOutOfRange()
        {
            var model = new GameModel();

            // Mock the list of save files to return a known quantity. So I believe the ID's are 0, 1, and 2.... I think
            // and then returning 3 should break it because its out of range I hope.
            List<string> mockFileList = new List<string>
            {
                "mockFilePath1.json",
                "mockFilePath2.json",
                "mockFilePath3.json"
            };

            int fileIdOutOfRange = 100;


            var result = model.LoadGameStateFromFile(fileIdOutOfRange);

            Assert.True(result.IsNull());
        }

        /// <summary>
        /// Verifies that the <c>AssignFrom</c> method correctly assigns properties and fields from a loaded game.
        /// </summary>
        [Fact]
        public void ValidateAssignFromMethod()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            string oWord = "codable";

            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);


            var newGame = new GameModel();

            newGame.AssignFrom(model);

            Assert.Equal(model.GetBaseWord(), newGame.GetBaseWord());
            Assert.Equal(model.GetFoundWords(), newGame.GetFoundWords());
            Assert.Equal(model.GetPlayerPoints(), newGame.GetPlayerPoints());
            Assert.Equal(model.GetRequiredLetter(), newGame.GetRequiredLetter());
            Assert.Equal(model.GetMaxPoints(), newGame.GetMaxPoints());
            Assert.Equal(model.GetValidWords(), newGame.GetValidWords());
        }

        /// <summary>
        /// Verifies that the <c>wonTheGame</c> method correctly identifies when a player has won.
        /// </summary>
        [Fact]
        public void ValidateWonTheGameWhenAllWordsAreFound()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            controller.NewPuzzleBaseWord("codable");
            model.SelectRandomWordForPuzzle();
            var validWords = model.GetValidWords();

            foreach (var word in validWords)
            {
                model.AddFoundWord(word);
            }

            bool hasWon = model.wonTheGame();

            Assert.True(hasWon);
        }

        /// <summary>
        /// Verifies that the <c>wonTheGame</c> method correctly identifies when a player has not won.
        /// </summary>
        [Fact]
        public void ValidateNotWonTheGameWhenSomeWordsAreMissing()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            controller.NewPuzzleBaseWord("codable");

            var validWords = model.GetValidWords();

            // Add only the first half of the valid words as found words
            for (int i = 0; i < validWords.Count / 2; i++)
            {
                model.AddFoundWord(validWords[i]);
            }

            bool hasWon = model.wonTheGame();

            Assert.False(hasWon);
        }


        /// <summary>
        /// Verifies that the <c>GetStatusTitles</c> method correctly retrieves the list of status titles.
        /// </summary>
        [Fact]
        public void ValidateGetStatusTitles()
        {
            var model = new GameModel();
            List<KeyValuePair<string, int>> expectedStatusTitles = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("Beginner", 0),
                new KeyValuePair<string, int>("Good Start", 2),
                new KeyValuePair<string, int>("Moving Up", 5),
                new KeyValuePair<string, int>("Good", 8),
                new KeyValuePair<string, int>("Solid", 15),
                new KeyValuePair<string, int>("Nice", 25),
                new KeyValuePair<string, int>("Great", 40),
                new KeyValuePair<string, int>("Amazing", 50),
                new KeyValuePair<string, int>("Genius", 70),
                new KeyValuePair<string, int>("Queen Bee", 100)
            };

            var returnedStatusTitles = model.GetStatusTitles();

            Assert.Equal(expectedStatusTitles, returnedStatusTitles);
        }

        [Fact]
        public void ValidateGetAllRanks()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            controller.NewPuzzleBaseWord("codable");
            controller.Guess("codable");

            int maxPointsForTest = model.GetMaxPoints();
            Dictionary<string, int> expectedRanks = new Dictionary<string, int>
            {
                { "Beginner", (int)(0.0 * maxPointsForTest) },
                { "Good Start", (int)(0.02 * maxPointsForTest) },
                { "Moving Up", (int)(0.05 * maxPointsForTest) },
                { "Good", (int)(0.08 * maxPointsForTest) },
                { "Solid", (int)(0.15 * maxPointsForTest) },
                { "Nice", (int)(0.25 * maxPointsForTest) },
                { "Great", (int)(0.40 * maxPointsForTest) },
                { "Amazing", (int)(0.50 * maxPointsForTest) },
                { "Genius", (int)(0.70 * maxPointsForTest) },
                { "Queen Bee", (int)(1.00 * maxPointsForTest) }
            };

            var returnedRanks = model.GetAllRanks();

            Assert.Equal(expectedRanks, returnedRanks);
        }

        /// <summary>
        /// Verifies that the <c>PangramCount</c> method correctly counts the number of pangrams
        /// and perfect pangrams from the valid words generated from a given base word.
        /// </summary>
        [Fact]
        public void ValidatePangramCountViaGameSimulation()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view);

            string baseWord = "kamotiq";

            controller.NewPuzzleBaseWord(baseWord);

            int expectedPangramCount = 1;
            int expectedPerfectPangramCount = 1;

            var (pangramCount, perfectPangramCount) = model.PangramCount();

            Assert.Equal(expectedPangramCount, pangramCount);
            Assert.Equal(expectedPerfectPangramCount, perfectPangramCount);
        }

        [Fact]
        public void ValidateGetCurrentScoreReturnsCorrectValue()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new CliController(model, view); 
            
            var baseWord = "soldier";

            controller.NewPuzzleBaseWord(baseWord);

            model.AddFoundWord("soldier");

            int expectedScore = 14; 

            var currentScore = model.GetCurrentScore();

            Assert.Equal(expectedScore, currentScore);
        }
    }
}
