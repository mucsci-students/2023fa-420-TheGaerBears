using SpellingBee;
using System.Collections.Immutable;

namespace TestSpellingBee
{
    public class TestModel
    {
        [Fact]
        public void ValidatePangramList()
        {
            var model = new GameModel();

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
            Assert.True(pangramList.Count() == 40222);
            Assert.True(tester);
        }
          [Fact]
        public void SaveVerify()
        {
            var model = new GameModel();
            var view = new GameView();
            var controller = new GameController(model, view);

            controller.NewPuzzleBaseWord("codable");
            controller.Guess("codable");

            model.SaveCurrentGameState("test");

            String filePath = "..\\..\\debug\\net6.0\\saves\\test.json";

            using (StreamReader reader = new StreamReader(filePath))
                {
                    string content = reader.ReadToEnd();
                   
                    Assert.Contains("codable", content);
                }

            
        }
    }
}