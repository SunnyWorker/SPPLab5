using System.Collections.Concurrent;

namespace StringFormatterProject;

public class User
{
    private string FirstName;
    public string LastName;
    public Dictionary<string, string> StringDictionary { get; }
    public Dictionary<int, string> IntDictionary { get; }
    public Dictionary<byte, string> ByteDictionary { get; }
    public ConcurrentDictionary<bool, string> BoolDictionary { get; }
    public List<string> Goods { get; }
    public int[] GoodsCount { get; }
    public string[] Names { get; }

    public User(string firstName, string lastName, Dictionary<string, string> stringDictionary, Dictionary<int, string> intDictionary, Dictionary<byte, string> byteDictionary, ConcurrentDictionary<bool, string> boolDictionary, List<string> goods, int[] goodsCount, string[] names)
    {
        FirstName = firstName;
        LastName = lastName;
        StringDictionary = stringDictionary;
        IntDictionary = intDictionary;
        ByteDictionary = byteDictionary;
        BoolDictionary = boolDictionary;
        Goods = goods;
        GoodsCount = goodsCount;
        Names = names;
    }

    public string SaySomething(string template)
    {
        return StringFormatter.Shared.Format(
            template, this);
    }
}


