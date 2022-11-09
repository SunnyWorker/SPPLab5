namespace StringFormatterProject;

public class User
{
    public string FirstName;
    public string LastName;
    //private string LastName2;
    //private static string LastName3;
    
    public User(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string GetGreeting()
    {
        return StringFormatter.Shared.Format(
            "Привет, {FirstName} {LastName}!", this);
    }
}


