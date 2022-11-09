using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using StringFormatterProject.exceptions;

namespace StringFormatterProject;

public class StringFormatter : IStringFormatter
{
    public static readonly StringFormatter Shared = new StringFormatter();
    private static ConcurrentDictionary<Type, Dictionary<String,Delegate>> cache = new();
    public string Format(string template, object target)
    {
        if (!CheckTemplate(template)) throw new DisbalanceBracketsException();
        AddToCache(target);
        int bracketCounter = 0;
        string resultString = "";
        string helpString = "";
        for (var i = 0; i < template.Length; i++)
        {
            if (template[i] == '{')
            {
                bracketCounter++;
                if(bracketCounter==1) continue;
            }

            else if (template[i] == '}')
            {
                bracketCounter--;
                if (bracketCounter == 0)
                {
                    string accessorName = ConvertFirstLetter(helpString) + "Accessor";
                    if (cache[target.GetType()].ContainsKey(accessorName))
                    {
                        resultString += cache[target.GetType()][accessorName].DynamicInvoke(target);
                    }
                    else throw new MemberNotFoundException(ConvertFirstLetter(helpString));

                    helpString = "";
                }
                continue;
            }

            if (bracketCounter == 1)
            {
                helpString += template[i];
            }
            else
            {
                resultString += template[i];
            }
            
        }

        return resultString;
    }

    private string ConvertFirstLetter(string str)
    {
        return str[0].ToString().ToLower() + str.Remove(0, 1);
    }
    private bool CheckTemplate(string template)
    {
        int correctCounter = 0;
        template = template.Replace("{{", "").Replace("}}", "");
        for (var i = 0; i < template.Length; i++)
        {
            if (template[i] == '{') correctCounter++;
            if (template[i] == '}') correctCounter--;
            if (correctCounter<0)
            {
                return false;
            }
        }
        return correctCounter == 0;
    }

    private void AddToCache(object target)
    {
        if(cache.ContainsKey(target.GetType())) return;
        FieldInfo[] objectFields = target.GetType().GetFields(BindingFlags.NonPublic |
                                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        PropertyInfo[] objectProperties = target.GetType().GetProperties(BindingFlags.NonPublic |
                                                                         BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        cache.TryAdd(target.GetType(), new());
        ParameterExpression typeParam = Expression.Parameter(target.GetType(), 
            ConvertFirstLetter(target.GetType().Name));
        foreach (var fieldInfo in objectFields)
        {
            MemberExpression memberExpression = Expression.PropertyOrField(typeParam,fieldInfo.Name);
            ConstantExpression fieldParam = Expression.Constant(fieldInfo.GetValue(target).ToString(),fieldInfo.FieldType);
            cache[target.GetType()].Add(ConvertFirstLetter(fieldInfo.Name)+"Accessor",
                Expression.Lambda(memberExpression, typeParam).Compile()); 
        }
        foreach (var propertyInfo in objectProperties)
        {
            MemberExpression memberExpression = Expression.PropertyOrField(typeParam,propertyInfo.Name);
            ConstantExpression fieldParam = Expression.Constant(propertyInfo.GetValue(target).ToString(),propertyInfo.PropertyType);
            cache[target.GetType()].Add(ConvertFirstLetter(propertyInfo.Name)+"Accessor",
                Expression.Lambda(memberExpression, typeParam).Compile()); 
        }
    }
}