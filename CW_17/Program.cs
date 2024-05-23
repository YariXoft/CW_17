using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

class DictionaryApp
{
    private Dictionary<string, List<string>> dictionary;
    private string dictionaryType;
    private string filePath = "dictionary.json";
    private string exportFilePath = "results.json"; // Для експорту

    public DictionaryApp(string dictionaryType)
    {
        this.dictionaryType = dictionaryType;
        this.dictionary = new Dictionary<string, List<string>>();
        LoadDictionary();
    }

    private void LoadDictionary()
    {
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonString) ?? new Dictionary<string, List<string>>();
        }
        else
        {
            SaveDictionary(); // Робимо новий файл, якщо він не існує
        }
    }

    private void SaveDictionary()
    {
        string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
        File.WriteAllText(filePath, jsonString);
    }

    public void AddWord(string word, List<string> translations)
    {
        if (dictionary.ContainsKey(word))
        {
            dictionary[word].AddRange(translations);
            dictionary[word] = dictionary[word].Distinct().ToList();
        }
        else
        {
            dictionary[word] = translations;
        }
        SaveDictionary();
    }

    public void ReplaceWord(string word, List<string> newTranslations)
    {
        if (dictionary.ContainsKey(word))
        {
            dictionary[word] = newTranslations;
            SaveDictionary();
        }
    }

    public void RemoveWord(string word)
    {
        if (dictionary.ContainsKey(word))
        {
            dictionary.Remove(word);
            SaveDictionary();
        }
    }

    public void RemoveTranslation(string word, string translation)
    {
        if (dictionary.ContainsKey(word))
        {
            dictionary[word].Remove(translation);
            if (dictionary[word].Count == 0)
            {
                dictionary.Remove(word);
            }
            SaveDictionary();
        }
    }

    public List<string> SearchWord(string word)
    {
        if (dictionary.ContainsKey(word))
        {
            return dictionary[word];
        }
        return null;
    }

    public void ExportWord(string word)
    {
        if (dictionary.ContainsKey(word))
        {
            var exportData = new Dictionary<string, List<string>> { { word, dictionary[word] } };
            string jsonString = JsonConvert.SerializeObject(exportData, Formatting.Indented);
            File.WriteAllText(exportFilePath, jsonString);
        }
    }

    public void ShowMenu()
    {
        while (true)
        {
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Додати слово");
            Console.WriteLine("2. Замiнити слово");
            Console.WriteLine("3. Видалити слово");
            Console.WriteLine("4. Видалити переклад");
            Console.WriteLine("5. Шукати переклад слова");
            Console.WriteLine("6. Експортувати слово");
            Console.WriteLine("7. Вийти");
            Console.Write("Виберiть опцiю: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введiть слово: ");
                    var word = Console.ReadLine();
                    Console.Write("Введiть переклади (через кому): ");
                    var translations = Console.ReadLine().Split(',').Select(t => t.Trim()).ToList();
                    AddWord(word, translations);
                    break;
                case "2":
                    Console.Write("Введiть слово: ");
                    word = Console.ReadLine();
                    Console.Write("Введiть новi переклади (через кому): ");
                    translations = Console.ReadLine().Split(',').Select(t => t.Trim()).ToList();
                    ReplaceWord(word, translations);
                    break;
                case "3":
                    Console.Write("Введiть слово: ");
                    word = Console.ReadLine();
                    RemoveWord(word);
                    break;
                case "4":
                    Console.Write("Введiть слово: ");
                    word = Console.ReadLine();
                    Console.Write("Введiть переклад для видалення: ");
                    var translation = Console.ReadLine();
                    RemoveTranslation(word, translation);
                    break;
                case "5":
                    Console.Write("Введiть слово: ");
                    word = Console.ReadLine();
                    var result = SearchWord(word);
                    if (result != null)
                    {
                        Console.WriteLine("Переклади: " + string.Join(", ", result));
                    }
                    else
                    {
                        Console.WriteLine("Слово не знайдено.");
                    }
                    break;
                case "6":
                    Console.Write("Введiть слово: ");
                    word = Console.ReadLine();
                    ExportWord(word);
                    Console.WriteLine("Слово експортовано до файлу results.json");
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Неправильний вибiр, спробуйте ще раз.");
                    break;
            }
        }
    }
}

class Program
{
    static void Main()
    {
        Console.Write("Введiть назву словника (наприклад: Англо-Український): ");
        var dictionaryType = Console.ReadLine();

        var app = new DictionaryApp(dictionaryType);
        app.ShowMenu();
    }
}
