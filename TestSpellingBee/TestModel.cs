using SpellingBee;
using static System.Net.Mime.MediaTypeNames;

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
        /// Verifies that the <c>SaveCurrentGame</c> function saves state to a JSON file.
        /// </summary>
        [Fact]
        public void SaveVerify()
        {
            var model = new GameModel();
            //var view = new GameView();
            var controller = new GuiController(model);

            string oWord = "codable";
            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);
            var oBaseWord = controller.GetBaseWord();
            char oReqLetter = model.GetRequiredLetter();
            int oPlayerPoints = model.GetPlayerPoints();
            int oMaxPoints = model.GetMaxPoints();

            model.SaveCurrentGameState("test-mod-sv");

            controller.Load("test-mod-sv");


            Assert.Equal(oBaseWord, controller.GetBaseWord());
            Assert.Equal(oReqLetter, model.GetRequiredLetter());
            Assert.Equal(oPlayerPoints, model.GetCurrentScore());
            Assert.Equal(oMaxPoints, model.GetMaxPoints());
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


            model.SaveCurrentGameState("a-test-mod-load");

            //Copy old data of puzzle to compare
            List<char> oBaseWord = new(model.GetBaseWord());
            char oReqLetter = model.GetRequiredLetter();
            IEnumerable<string> oFoundWords = new List<string>(model.GetFoundWords());
            int oPlayerPoints = model.GetPlayerPoints();
            int oMaxPoints = model.GetMaxPoints();

            //Verify load
            controller.NewPuzzleBaseWord("companion");

            controller.Load("a-test-mod-load");

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
            var controller = new GuiController(model);

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
            var controller = new GuiController(model);

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
            var controller = new GuiController(model);

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
            var controller = new GuiController(model);

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
            var controller = new GuiController(model);

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
            var controller = new GuiController(model);

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
            var controller = new GuiController(model);

            var baseWord = "soldier";

            controller.NewPuzzleBaseWord(baseWord);

            model.AddFoundWord("soldier");

            int expectedScore = 14; 

            var currentScore = model.GetCurrentScore();

            Assert.Equal(expectedScore, currentScore);
        }

        [Fact]
        public void ValidateGetNextRankThresholdReturnsCorrectValue()
        {
            // Set up the game
            var model = new GameModel();

            model.baseWord = new List<char> { 'd', 's', 'o', 'l', 'i', 'e', 'r'};
            model.requiredLetter = 'd';
            model.GenerateValidWords();

            Assert.Equal(2713, model.GetMaxPoints());

            // Test scenario 1: When playerPoints is 0 (Beginner), next threshold should be 2% of maxPoints
            Assert.Equal((int)(0.02 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 2: When playerPoints is 81 (between Good Start and Moving Up), next threshold should be 5% of maxPoints
            model.playerPoints = (int)(0.03 * model.maxPoints);
            Assert.Equal((int)(0.05 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 3: When playerPoints is 189 (7% of maxPoints, between Moving Up and Good), next threshold should be 8% of maxPoints
            model.playerPoints = (int)(0.07 * model.maxPoints);
            Assert.Equal((int)(0.08 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 4: When playerPoints is 271 (10% of maxPoints, between Good and Solid), next threshold should be 15% of maxPoints
            model.playerPoints = (int)(0.10 * model.maxPoints);
            Assert.Equal((int)(0.15 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 5: When playerPoints is 542 (20% of maxPoints, between Solid and Nice), next threshold should be 25% of maxPoints
            model.playerPoints = (int)(0.20 * model.maxPoints); ;
            Assert.Equal((int)(0.25 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 6: When playerPoints is 813 (30% of maxPoints, between Nice and Great), next threshold should be 40% of maxPoints
            model.playerPoints = (int)(0.30 * model.maxPoints);
            Assert.Equal((int)(0.40 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 7: When playerPoints is 1220 (45% of maxPoints, between Great and Amazing), next threshold should be 50% of maxPoints
            model.playerPoints = (int)(0.45 * model.maxPoints);
            Assert.Equal((int)(0.50 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 8: When playerPoints is 1627 (60% of maxPoints, between Amazing and Genius), next threshold should be 70% of maxPoints
            model.playerPoints = (int)(0.60 * model.maxPoints);
            Assert.Equal((int)(0.70 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 9: When playerPoints is 2170 (80% of maxPoints, between Genius and Queen Bee), next threshold should be 100% of maxPoints
            model.playerPoints = (int)(0.80 * model.maxPoints);
            Assert.Equal((int)(1.00 * model.maxPoints), model.GetNextRankThreshold());

            // Test scenario 10: When playerPoints is at maximum, the threshold returned should be the maxPoints itself
            model.playerPoints = model.maxPoints;
            Assert.Equal(model.maxPoints, model.GetNextRankThreshold());
        }

        /// <summary>
        /// Validates that SetBaseWordForPuzzle creates a list of distinct chars out of the string
        /// </summary>
        [Fact]
        public void ValidateSetBaseWord()
        {
            var model = new GameModel();
            string word = "companion";

            model.SetBaseWordForPuzzle(word);

            // Make sure all letters in word are in baseWord
            List<char> bWord = model.GetBaseWord();
            bool testing = false;
            foreach (var let in bWord)
            {
                if (word.Contains(let))
                    testing = true;
                if (!testing)
                    break;
            }
            Assert.True(testing);
        }

        /// <summary>
        /// Validates that ShuffleNewWord reorders the letters of the pangram
        /// </summary>
        [Fact]
        public void ValidateShuffleNewWord()
        {
            var model = new GameModel();

            model.baseWord = new List<char> { 'c', 'o', 'd', 'a', 'b', 'l', 'e' };
            List<char> oBaseWord = new List<char>(model.baseWord);

            model.ShuffleNewWord();
            Assert.NotEqual(oBaseWord, model.baseWord);
        }

        /// <summary>
        /// Validates that ShuffleBaseWord reorders the letters
        /// </summary>
        [Fact]
        public void ValidateShuffleBaseWord()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            controller.NewPuzzleBaseWord("codable");
            var oBaseWord = new List<char> (model.GetBaseWord());

            model.ShuffleBaseWord();

            Assert.NotEqual(oBaseWord, model.baseWord);
            Assert.Equal(model.requiredLetter, model.baseWord[0]);
        }

        /// <summary>
        /// Validates the output of IsValidWord
        /// </summary>
        [Fact]
        public void ValidateIsValidWord()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            controller.NewPuzzleBaseWord("codable");

            Assert.True(model.IsValidWord("codable"));
            Assert.False(model.IsValidWord("nope"));
            Assert.False(model.IsValidWord("cod"));
        }

        /// <summary>
        /// Validates the output of IsWordAlreadyFound
        /// </summary>
        [Fact]
        public void ValidateIsWordAlreadyFound()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            controller.NewPuzzleBaseWord("codable");

            Assert.False(model.IsWordAlreadyFound("codable"));
            Assert.False(model.IsWordAlreadyFound("nope"));

            controller.Guess("codable");

            Assert.True(model.IsWordAlreadyFound("codable"));
            Assert.False(model.IsWordAlreadyFound("nope"));
        }

        /// <summary>
        /// Validates that Reset actually resets the game
        /// </summary>
        [Fact]
        public void ValidateReset()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            controller.NewPuzzleBaseWord("codable");
            controller.Guess("codable");

            model.Reset();

            Assert.Empty(model.GetFoundWords());
            Assert.Empty(model.GetBaseWord());
            Assert.Equal(0, model.requiredLetter);
            Assert.Equal(0, model.GetPlayerPoints());
            Assert.Equal(0, model.GetMaxPoints());
            Assert.Empty(model.GetValidWords());
        }

        /// <summary>
        /// Validate that player points are update for found words
        /// </summary>
        [Fact]
        public void ValidateUpdatePlayerPointsForWordFound()
        {
            var model = new GameModel();

            model.baseWord = new List<char> { 'c', 'o', 'm', 'p', 'a', 'n', 'i' };
            model.requiredLetter = 'c';
            model.GenerateValidWords();

            Assert.Equal(0, model.GetPlayerPoints());

            model.UpdatePlayerPointsForFoundWord("cod");

            Assert.Equal(0, model.GetPlayerPoints());

            model.UpdatePlayerPointsForFoundWord("companion");

            Assert.Equal(16, model.GetPlayerPoints());

            model.UpdatePlayerPointsForFoundWord("coin");

            Assert.Equal(17, model.GetPlayerPoints());
        }

        /// <summary>
        /// Validates the Active function
        /// </summary>
        [Fact]
        public void ValidateActive()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            Assert.False(model.Active());

            controller.NewPuzzle();

            Assert.True(model.Active());
        }

        /// <summary>
        /// Verifies that the <c>SaveCurrentPuzzle</c> function saves state to a JSON file.
        /// </summary>
        [Fact]
        public void ValidateSavePuzzle()
        {
            var model = new GameModel();
            //var view = new GameView();
            var controller = new GuiController(model);

            string oWord = "codable";
            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);
            var oBaseWord = controller.GetBaseWord();
            char oReqLetter = model.GetRequiredLetter();
            int oMaxPoints = model.GetMaxPoints();

            model.SaveCurrentPuzzleState("test-mod-sv-p");

            controller.Load("test-mod-sv-p");


            Assert.Equal(oBaseWord, controller.GetBaseWord());
            Assert.Equal(oReqLetter, model.GetRequiredLetter());
            Assert.Equal(0, model.GetCurrentScore());
            Assert.Equal(oMaxPoints, model.GetMaxPoints());
        }

        /// <summary>
        /// Validate TwoLetterList produces the correct two letter dictionary
        /// </summary>
        [Fact]
        public void ValidateTwoLetterList()
        {
            Dictionary<string, int> twoLettersEx = new Dictionary<string, int> { { "fo", 7}, { "fr", 1}, { "mi", 1}, { "mo", 13 },
             {"om", 1 }, {"ot", 1 }, {"ri", 1 }, {"ro", 9 }, {"ry", 1 }, {"ti", 1 }, {"to", 18 }, {"tr", 5 }, {"ty", 1 }, {"yo", 2 } };

            var model = new GameModel();

            model.baseWord = new List<char> { 'o', 'm', 'r', 't', 'i', 'f', 'y' };
            model.requiredLetter = 'o';
            model.GenerateValidWords();

            Assert.Equal(twoLettersEx, model.TwoLetterList());
        }

        /// <summary>
        /// Validate LettersInWord produces the correct dictionary for letters
        /// </summary>
        [Fact]
        public void ValidateLettersInWord()
        {
            Dictionary<char, int[]> lettersEx = new Dictionary<char, int[]> { { 'f', new int[12] { 4,2,1,1,0,0,0,0,0,0,0,0} },
            { 'm', new int[12] { 5,6,2,1,0,0,0,0,0,0,0,0} },
            { 'o', new int[12] { 2,0,0,0,0,0,0,0,0,0,0,0 } },
            { 'r', new int[12] { 7,3,0,0,1,0,0,0,0,0,0,0} },
            { 't', new int[12] { 16,4,1,2,2,0,0,0,0,0,0,0} },
            { 'y', new int[12] { 1,1,0,0,0,0,0,0,0,0,0,0} }};

            var model = new GameModel();

            model.baseWord = new List<char> { 'o', 'm', 'r', 't', 'i', 'f', 'y' };
            model.requiredLetter = 'o';
            model.GenerateValidWords();

            Assert.Equal(lettersEx, model.LettersInWord());
        }

        /// <summary>
        /// Validates PangramCount returns the correct number of pangrams and perfect pangrams
        /// </summary>
        [Fact]
        public void ValidatePangramCount()
        {
            (int, int) pCountEx = (1, 1);

            var model = new GameModel();

            model.baseWord = new List<char> { 'o', 'm', 'r', 't', 'i', 'f', 'y' };
            model.requiredLetter = 'o';
            model.GenerateValidWords();

            Assert.Equal(pCountEx, model.PangramCount());

            model.Reset();
            model.baseWord = new List<char> { 'n', 'c', 'o', 'm', 'i', 'a', 'p' };
            model.requiredLetter = 'n';
            model.GenerateValidWords();

            pCountEx = (2, 1);

            Assert.Equal(pCountEx, model.PangramCount());
        }

        /// <summary>
        /// Verifies that the <c>PrintHintTable</c> method correctly generates the hint table string
        /// for a known base word and simulated game state.
        /// </summary>
        [Fact]
        public void ValidatePrintHintTable()
        {
            var model = new GameModel();

            model.baseWord = new List<char> { 'r', 'e', 'i', 'l', 's', 'o', 'd' };
            model.requiredLetter = 'r';
            model.GenerateValidWords();

            // There are spaces at the ends of the two letter list on purpose
            string expectedOutput = @"Required letter is first.

reilsod

Words: 443, Points: 2644, Pangrams: 9 (2 Perfect), BINGO

      4   5   6   7   8   9  10  11  tot
d    10  20  18  26  22   6   3   -  105
e     2   8   8   1   -   3   -   -   22
i     4   3   4   1   1   1   -   -   14
l     6   6   9   5   4   -   -   -   30
o     4  11   9   8   4   2   -   -   38
r    24  31  40  38  14  10   2   -  159
s     9  14  17  24   4   5   1   1   75
tot  59  93 105 103  49  27   6   1  443

Two Letter List:

de-21 di-14 do-34 dr-36 
ee-3 ei-2 el-5 er-12 
id-4 il-1 ir-9 
le-7 li-5 lo-18 
od-5 oi-3 ol-3 oo-1 or-23 os-3 
re-88 ri-30 ro-41 
se-23 si-16 sl-5 so-30 sr-1"
            ;
            expectedOutput = expectedOutput.Replace("\r", "");

            string output = model.PrintHintTable();


            Assert.Equal(expectedOutput, output.Trim()); // Trim to ensure no leading/trailing whitespace
        }

        /// <summary>
        /// Validates GetAvailableSaveFiles returns list of save files
        /// </summary>
        [Fact]
        public void ValidateGetAvailableSaveFiles()
        {
            var model = new GameModel();

            var files = model.GetAvailableSaveFiles();

            //Since tests run at different time hard to know count in save file
            //Count will be in range [0, 5]
            Assert.True(files.Count() < 6);

            //Makes sure files are json
            for (int i = 0; i < files.Count; i++)
            {
                Assert.Equal(".json", files[i].Substring(files[i].Length - 5));
            }
        }

        /// <summary>
        /// Validates that calling save without a file name fails
        /// </summary>
        [Fact]
        public void ValidateSavePuzzleFail()
        {
            var model = new GameModel();
            //var view = new GameView();
            var controller = new GuiController(model);

            string oWord = "codable";
            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);

            bool success = model.SaveCurrentPuzzleState("");

            Assert.False(success);
        }

        /// <summary>
        /// Validates that calling save without a file name fails
        /// </summary>
        [Fact]
        public void ValidateSaveGameFail()
        {
            var model = new GameModel();
            //var view = new GameView();
            var controller = new GuiController(model);

            string oWord = "codable";
            controller.NewPuzzleBaseWord(oWord);
            controller.Guess(oWord);

            bool success = model.SaveCurrentGameState("");

            Assert.False(success);
        }

        [Fact]
        public void ValidateSetBaseWordForPuzzleFail()
        {
            var model = new GameModel();
            string word = "zzzzz";

            Model md = model.SetBaseWordForPuzzle(word);

            Assert.IsType<NullModel>(md);
        }

        /// <summary>
        /// Validates AddFoundWord function
        /// </summary>
        [Fact]
        public void ValidateAddFoundWord()
        {
            var model = new GameModel();
            var controller = new GuiController(model);

            var baseWord = "soldier";

            controller.NewPuzzleBaseWord(baseWord);

            model.AddFoundWord("soldier");

            model.AddFoundWord("soldier");

            model.AddFoundWord("soooo");
        }
    }
}
