using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace _5DAT_console_app
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Génération de données pour le fichier texte...");

            string fileNameInput = "FINKLERS_FIELD_book.txt";
            string fileNameOutput = "FINKLERS_FIELD_result.txt";

            IList<string> wordsList = ExtractWordsFromFiles(fileNameInput);

            IList<char> firstCharacterOfEachWordsList = GetFirstLetterForEachWords(wordsList);

            IDictionary<char, IList<char>> occurencesList = GetListOfCharacters(firstCharacterOfEachWordsList);

            WriteIntoFileFinalResult(occurencesList, fileNameOutput);

            Console.ReadKey();
        }

        private static IList<string> ExtractWordsFromFiles(string fileNameInput)
        {
            string path = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), @"../../../", fileNameInput);

            if (!File.Exists(path))
            {
                throw new Exception($"Impossible de trouver le fichier avec le nom {path}");
            }

            string contents = File.ReadAllText(path);

            IList<string> result = GetWords(contents);

            return result;
        }

        private static IList<string> GetWords(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\b[\w']*\b");

            var words = from m in matches.Cast<Match>()
                        where !string.IsNullOrEmpty(m.Value)
                        select TrimSuffix(m.Value);

            return words.ToList();
        }

        private static string TrimSuffix(string word)
        {
            int apostropheLocation = word.IndexOf('\'');
            if (apostropheLocation != -1)
            {
                word = word.Substring(0, apostropheLocation);
            }

            return word;
        }
        
        private static IList<char> GetFirstLetterForEachWords(IList<string> wordsList)
        {
            return wordsList.Select(x => x[0]).ToList();
        }

        private static IDictionary<char, IList<char>> GetListOfCharacters(IList<char> firstCharacterOfEachWordsList)
        {
            IDictionary<char, IList<char>> result = new Dictionary<char, IList<char>>();

            IList<char> listOfExistingCharacters = new List<char>();

            foreach(char character in firstCharacterOfEachWordsList)
            {
                if (!listOfExistingCharacters.Contains(character))
                {
                    listOfExistingCharacters.Add(character);
                }
            }

            foreach(char characterExisting in listOfExistingCharacters)
            {
                IList<char> existingCharactersIntoTheReferencelist = firstCharacterOfEachWordsList.Where(x => x == characterExisting).ToList();

                result.Add(characterExisting, existingCharactersIntoTheReferencelist);
            }

            return result;
        }

        private static void WriteIntoFileFinalResult(IDictionary<char, IList<char>> occurencesList, string fileNameOutput)
        {
            string path = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), @"../../../", fileNameOutput);

            if (File.Exists(path))
            {
                throw new Exception($"Le fichier avec le chemin {path} existe deja...");
            }

            using (StreamWriter file = new StreamWriter(path))
            {
                foreach (var occurence in occurencesList)
                {
                    string lineToWrite = $"({occurence.Key}, {occurence.Value.Count})";
                    file.WriteLine(lineToWrite);
                    Console.WriteLine($"Occurence : {lineToWrite}");
                }

                Console.WriteLine($"Le fichier {fileNameOutput} a bien été créé avec les résultats du livre");
            }
        }
    }
}
