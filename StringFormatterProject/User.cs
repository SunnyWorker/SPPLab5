namespace StringFormatterProject;

public class User
{
    public string FirstName;
    public string LastName;
    public Dictionary<string, string> Orders { get; }
    //private string LastName2;
    //private static string LastName3;
    
    public User(string firstName, string lastName, Dictionary<string,string> list)
    {
        FirstName = firstName;
        LastName = lastName;
        Orders = list;
    }

    public string GetGreeting()
    {
        return StringFormatter.Shared.Format(
            "{FirstName} {LastName} заказал(а) {Orders[\"integer\"]}", this);
    }
}


