using System.Collections;
using System.Collections.Concurrent;
using StringFormatterProject;
using StringFormatterProject.exceptions;

namespace Tests;

[TestFixture]
public class Tests
{
    private User user;
    
    [OneTimeSetUp]
    public void Setup()
    {
        Dictionary<string, string> StringDictionary = new();
        Dictionary<int, string> IntDictionary = new();
        Dictionary<byte, string> ByteDictionary = new();
        ConcurrentDictionary<bool, string> BoolDictionary = new();
        List<string> Goods = new() {"утюг", "чайник", "ноутбук АСУС 2 ядра 2 гига 4 гб ОП всего 29.999 рублей мышка в подарок"};
        int[] GoodsCount = new[] {1,9,1000};
        string[] Names = new[] {"Samsung","Tefal","Asus"};
        StringDictionary.Add("качество","супер");
        IntDictionary.Add(0, "плохо");
        IntDictionary.Add(1, "норм");
        IntDictionary.Add(2, "хорошо");
        ByteDictionary.Add(0, "плохо");
        ByteDictionary.Add(1, "норм");
        ByteDictionary.Add(2, "хорошо");
        BoolDictionary.TryAdd(false, "плохо");
        BoolDictionary.TryAdd(true, "хорошо");
        user = new User("Петя", "Иванов", StringDictionary,
            IntDictionary,ByteDictionary,BoolDictionary,Goods,GoodsCount,Names);
    }

    [Test]
    public void StringDictionaryTest()
    {
        Assert.AreEqual("Петя Иванов заказал(а) утюг с качеством супер",
            user.SaySomething("{FirstName} {LastName} заказал(а) {Goods[0]} с качеством {StringDictionary[\"качество\"]}"));
    }
    
    [Test]
    public void IntDictionaryTest()
    {
        Assert.AreEqual("Петя Иванов заказал(а) утюг с качеством норм",
            user.SaySomething("{FirstName} {LastName} заказал(а) {Goods[0]} с качеством {IntDictionary[1]}"));
    }
    
    [Test]
    public void BoolDictionaryTest()
    {
        Assert.AreEqual("Петя Иванов заказал(а) утюг с качеством хорошо",
            user.SaySomething("{FirstName} {LastName} заказал(а) {Goods[0]} с качеством {BoolDictionary[true]}"));
    }
    
    [Test]
    public void ByteDictionaryTest()
    {
        Assert.AreEqual("Петя Иванов заказал(а) утюг с качеством хорошо",
            user.SaySomething("{FirstName} {LastName} заказал(а) {Goods[0]} с качеством {ByteDictionary[2]}"));
    }

    [Test]
    public void ArrayAndListTest()
    {
        Assert.AreEqual("Петя Иванов заказал(а) 9 чайников с качеством норм",
            user.SaySomething("{FirstName} {LastName} заказал(а) {GoodsCount[1]} {Goods[1]}ов с качеством {ByteDictionary[1]}"));
    }

    [Test]
    public void StringArrayTest()
    {
        Assert.AreEqual("Петя Иванов заказал(а) 9 чайников Tefal с качеством норм",
            user.SaySomething("{FirstName} {LastName} заказал(а) {GoodsCount[1]} {Goods[1]}ов {Names[1]} с качеством {ByteDictionary[1]}"));
    }

    [Test]
    public void DisplayingTest()
    {
        Assert.AreEqual("{FirstName} {LastName} заказал(а) утюг с качеством супер",
            user.SaySomething("{{FirstName}} {{LastName}} заказал(а) {Goods[0]} с качеством {StringDictionary[\"качество\"]}"));
    }
    
    [Test]
    public void ErrorTest()
    {
        try
        {
            user.SaySomething("{FirstName}} {{LastName}} заказал(а) {Goods[0]} с качеством {StringDictionary[\"качество\"]}");
            user.SaySomething("{{FirstName} {{LastName}} заказал(а) {Goods[0]} с качеством {StringDictionary[\"качество\"]}");
            Assert.Fail();
        }
        catch (DisbalanceBracketsException e)
        {
            
        }
    }
}