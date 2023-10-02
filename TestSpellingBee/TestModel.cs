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
    }
}