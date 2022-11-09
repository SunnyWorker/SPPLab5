// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Collections.ObjectModel;
using StringFormatterProject;

Console.WriteLine("Hello, World!");
Dictionary<string, string> dictionary = new();
dictionary.Add("integer", "утюг");
Console.WriteLine(dictionary["integer"]);
var user = new User("Петя", "Иванов", dictionary);
var names = new List<String>();
string[] names1 = new string[] { };
Console.WriteLine(names.GetType().GetInterface(nameof(IEnumerable))!=null);
Console.WriteLine(user.GetGreeting());
// Привет, Петя Иванов!