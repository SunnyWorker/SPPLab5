// See https://aka.ms/new-console-template for more information

using StringFormatterProject;

Console.WriteLine("Hello, World!");
var user = new User("Петя", "Иванов");
Console.WriteLine(user.GetGreeting());
// Привет, Петя Иванов!