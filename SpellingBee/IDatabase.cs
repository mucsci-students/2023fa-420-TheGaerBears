using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpellingBee
{
    public interface IDatabase
    {
        public List<string> PangramList();

        public List<string> GenerateValidWords(List<char> baseWord, char requiredLetter);
    }
}