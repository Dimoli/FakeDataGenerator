using System;
using System.Reflection;
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
      List<RandomData.Person> generatedPersons = generatePersons(language, personsCount);

      printRecords(ref generatedPersons, errorCount, language);

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

    private static void createRecord(ref List<Person> persons, int i, double errorCount, string language)
    {
      ErrorDelegate[] ErrorFunctions = new ErrorDelegate[3] { insertSymbol, removeSymbol, replaceSymbol };

      addError(ErrorFunctions, ref persons, i, errorCount, language);
    }
    private static FieldInfo getRandomField(RandomData.Person cleanRecord)
    {
      Random rnd = new Random();
      string[] fields = { "Address", "Name", "Phone", };
      string randomField = fields[rnd.Next(0, 2)];
      FieldInfo field = typeof(Person).GetField(randomField);

      return field;
    }
    private delegate string ErrorDelegate(string record, string language);

    private static string insertSymbol(string record, string language)
    {
      Random rnd = new Random();
      CharArray[] charRange = new CharArray[3] { numberChar, upperAlphabetChar, lowerAlphabetChar };
      string symbol = charRange[rnd.Next(0, 2)](localCharRange(language));

      return record.Insert(rnd.Next(0, record.Length - 1), symbol);
    }
    private delegate string CharArray(int[] charArray);
    private static string numberChar(int[] charArray)
    {
      Random rnd = new Random();

      return rnd.Next(charArray[0], charArray[1]).ToString();
    }
    private static string upperAlphabetChar(int[] charArray)
    {
      Random rnd = new Random();

      return rnd.Next(charArray[2], charArray[3]).ToString();
    }
    private static string lowerAlphabetChar(int[] charArray)
    {
      Random rnd = new Random();

      return rnd.Next(charArray[4], charArray[5]).ToString();
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

    private static string removeSymbol(string record, string language = "")
    {
      Random rnd = new Random();

      return record.Remove(rnd.Next(0, record.Length - 1), 1);
    }
    private static string replaceSymbol(string record, string language = "")
    {
      int rnd = new Random().Next(0, record.Length - 1);
      char replacedSymbol = record[rnd];

      record.Remove(rnd, 1);
      record.Insert(rnd + 1, replacedSymbol.ToString());

      return record;
    }

    private static void addError(ErrorDelegate[] ErrorFunctions, ref List<Person> persons, int i, double errorCount, string language)
    {
      if (errorCount > 1)
      {
        repeatAddError(ErrorFunctions, ref persons, i, errorCount, language);
      }
      else if (errorCount != 0.0)
        setMultiError(ErrorFunctions, ref persons, i, errorCount, language);
    }
    private static void repeatAddError(ErrorDelegate[] ErrorFunctions, ref List<Person> persons, int i, double errorCount, string language)
    {
      setError(ErrorFunctions, ref persons, i, errorCount, language);
      addError(ErrorFunctions, ref persons, i, errorCount - 1, language);
    }
    private static void setMultiError(ErrorDelegate[] ErrorFunctions, ref List<Person> persons, int i, double errorCount, string language)
    {
      Random rnd = new Random();

      try
      {
        int randomIndex = i + rnd.Next(0, (int)Math.Round(1 / errorCount));
        setError(ErrorFunctions, ref persons, randomIndex, errorCount - 1, language);
      }
      catch
      {
        int randomIndex = i + rnd.Next(0, persons.Count - i - 1);
        setError(ErrorFunctions, ref persons, randomIndex, errorCount - 1, language);
      }
    }
    private static void setError(ErrorDelegate[] ErrorFunctions, ref List<Person> persons, int i, double errorCount, string language)
    {
      Random rnd = new Random();
      Person randomPerson = persons[i];
      FieldInfo randomField = getRandomField(randomPerson);
      string fieldValue = ErrorFunctions[rnd.Next(0, 2)](randomField.GetValue(randomPerson).ToString(), language);
      randomField.SetValue(randomPerson, fieldValue);
    }

    private static void printRecords(ref List<Person> persons, double errorCount, string language)
    {
      for (int i = 0; i < persons.Count - 1; i++)
      {
        createRecord(ref persons, i, errorCount, language);
        Console.WriteLine($"{persons[i].Name}; {persons[i].Address}; {persons[i].Phone}");
      }
    }
  }
}