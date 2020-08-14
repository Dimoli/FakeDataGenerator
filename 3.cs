using System;
using System.Text;
using System.Collections.Generic;

using Bogus;

namespace RandomData
{
  public class Person
  {
    public string Address { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
  }

  class Program
  {
    static void Main(string[] args)
    {
      string language = mapLanguage(args[0]);
      int personsCount = Convert.ToInt32(args[1]);
      double errorCount = getErrorCount(args);

      printPersons(generatePersons(language, personsCount), errorCount, language);

      Console.ReadKey();
    }

    private static string mapLanguage(string language)
    {
      Dictionary<string, string> languageMap = new Dictionary<string, string>
      {
        ["en_US"] = "en_US",
        ["ru_RU"] = "ru",
        ["be_BY"] = "uk",
      };

      return languageMap[language];
    }
    private static double getErrorCount(string[] args)
    {
      try
      {
        return Convert.ToDouble(args[2]);
      }
      catch
      {
        return 0.0;
      }
    }

    private static List<Person> generatePersons(string language, int personsCount)
    {
      Faker<Person> generatorPerson = getGeneratorPerson(language);

      return generatorPerson.Generate(personsCount);
    }
    private static Faker<Person> getGeneratorPerson(string language)
    {
      return new Faker<Person>(language)
        .RuleFor(x => x.Name, f => f.Name.FullName())
        .RuleFor(x => x.Address, f => f.Address.FullAddress())
        .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber());
    }

    private static string createRecord(string cleanRecord, double errorCount, string language)
    {
      StringBuilder record = new StringBuilder(cleanRecord);
      ErrorDelegate[] ErrorFunctions = new ErrorDelegate[3] { insertSymbol, removeSymbol, replaceSymbol };

      addError(ErrorFunctions, record, errorCount, language);

      return record.ToString();
    }
    private delegate System.Text.StringBuilder ErrorDelegate(System.Text.StringBuilder record, string language);

    private static System.Text.StringBuilder insertSymbol(System.Text.StringBuilder record, string language)
    {
      Random rnd = new Random();

      CharArray[] charRange = new CharArray[3] { numberChar, upperAlphabetChar, lowerAlphabetChar };
      char symbol = charRange[rnd.Next(0, 2)](localCharRange(language));

      return record.Insert(rnd.Next(0, record.Length - 1), symbol);
    }
    private delegate char CharArray(int[] charArray);
    private static char numberChar(int[] charArray)
    {
      Random rnd = new Random();

      return (char)rnd.Next(charArray[0], charArray[1]);
    }
    private static char upperAlphabetChar(int[] charArray)
    {
      Random rnd = new Random();

      return (char)rnd.Next(charArray[2], charArray[3]);
    }
    private static char lowerAlphabetChar(int[] charArray)
    {
      Random rnd = new Random();

      return (char)rnd.Next(charArray[4], charArray[5]);
    }
    private static int[] localCharRange(string language)
    {
      Dictionary<string, int[]> localCharRange = new Dictionary<string, int[]>
      {
        ["en_US"] = new int[] { 0x0030, 0x0039, 0x0041, 0x005A, 0x0061, 0x007A },
        ["ru"] = new int[] { 0x0030, 0x0039, 0x0410, 0x042F, 0x0430, 0x044F },
        ["uk"] = new int[] { 0x0030, 0x0039, 0x0410, 0x042F, 0x0430, 0x044F },
      };

      return localCharRange[language];
    }

    private static System.Text.StringBuilder removeSymbol(System.Text.StringBuilder record, string language = "")
    {
      Random rnd = new Random();

      return record.Remove(rnd.Next(0, record.Length - 1), 1);
    }
    private static System.Text.StringBuilder replaceSymbol(System.Text.StringBuilder record, string language = "")
    {
      int rnd = new Random().Next(0, record.Length - 1);
      char replacedSymbol = record[rnd];

      record.Remove(rnd, 1);
      record.Insert(rnd + 1, replacedSymbol);

      return record;
    }

    private static System.Text.StringBuilder addError(ErrorDelegate[] ErrorFunctions, StringBuilder errorRecord, double errorCount, string language)
    {
      return errorCount > 1 ? repeatAddError(ErrorFunctions, errorRecord, errorCount, language) : errorRecord;
    }
    private static System.Text.StringBuilder repeatAddError(ErrorDelegate[] ErrorFunctions, StringBuilder errorRecord, double errorCount, string language)
    {
      Random rnd = new Random();
      errorRecord = ErrorFunctions[rnd.Next(0, 2)](errorRecord, language);

      return addError(ErrorFunctions, errorRecord, errorCount - 1, language);
    }

    private static void printPersons(List<Person> persons, double errorCount, string language)
    {
      foreach (var person in persons)
      {
        Console.WriteLine(createRecord($"{person.Name}; {person.Address}; {person.Phone}", errorCount, language));
      }
    }
  }
}