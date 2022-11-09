using System.Collections;
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
        CheckTemplate(template);
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
                if(bracketCounter==1) continue;
                if (bracketCounter == 0 && !helpString.Equals(""))
                {
                    HandleConcat(ref resultString, ref helpString, target);
                    continue;
                }
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

    private void HandleConcat(ref string resultString, ref string helpString, object target)
    {
        string memberName;
        string localHelpString = helpString;
        List<string> indexes = new();
        if (localHelpString.IndexOf('[') != -1)
        {
            memberName = localHelpString.Substring(0, localHelpString.IndexOf('['));
            int start;
            while ((start = localHelpString.IndexOf('[')) != -1)
            {
                indexes.Add(localHelpString.Substring(start+1, localHelpString.IndexOf(']') - start - 1));
                localHelpString = localHelpString.Remove(start, localHelpString.IndexOf(']') - start+1);
            }
        }
        else memberName = helpString;
        string accessorName = ConvertFirstLetter(memberName) + "Accessor";
        
        if (!cache[target.GetType()].ContainsKey(accessorName)) throw new MemberNotFoundException(ConvertFirstLetter(memberName));
        object invokedObject = cache[target.GetType()][accessorName].DynamicInvoke(target);
        
        if (invokedObject.GetType().IsArray)
        {
            foreach (var index in indexes)
            {
                if(invokedObject.GetType()==typeof(byte[])) invokedObject = ((byte[])invokedObject)[int.Parse(index)];
                else if(invokedObject.GetType()==typeof(short[])) invokedObject = ((short[])invokedObject)[int.Parse(index)];
                else if(invokedObject.GetType()==typeof(int[])) invokedObject = ((int[])invokedObject)[int.Parse(index)];
                else if(invokedObject.GetType()==typeof(long[])) invokedObject = ((long[])invokedObject)[int.Parse(index)];
                else if(invokedObject.GetType()==typeof(float[])) invokedObject = ((float[])invokedObject)[int.Parse(index)];
                else if(invokedObject.GetType()==typeof(double[])) invokedObject = ((double[])invokedObject)[int.Parse(index)];
                else if(invokedObject.GetType()==typeof(bool[])) invokedObject = ((bool[])invokedObject)[int.Parse(index)];
                else if(invokedObject.GetType()==typeof(char[])) invokedObject = ((char[])invokedObject)[int.Parse(index)];
                else invokedObject = ((object[])invokedObject)[int.Parse(index)];
            }
        }
        else if (invokedObject.GetType().GetInterface(nameof(IDictionary)) != null)
        {
            foreach (var index in indexes)
            {
                object indexObject = null;
                if (invokedObject.GetType().GetGenericArguments()[0]==typeof(string))
                {
                    string help = index.Replace("\"", "");
                    indexObject = help;
                }
                else if (invokedObject.GetType().GetGenericArguments()[0]==typeof(char))
                {
                    indexObject = char.Parse(index.Replace("'", ""));
                }
                else if (invokedObject.GetType().GetGenericArguments()[0]==typeof(long))
                {
                    indexObject = long.Parse(index);
                }
                else if (invokedObject.GetType().GetGenericArguments()[0]==typeof(int))
                {
                    indexObject = int.Parse(index);
                }
                else if (invokedObject.GetType().GetGenericArguments()[0]==typeof(short))
                {
                    indexObject = short.Parse(index);
                }
                else if (invokedObject.GetType().GetGenericArguments()[0]==typeof(byte))
                {
                    indexObject = byte.Parse(index);
                }
                else if (invokedObject.GetType().GetGenericArguments()[0]==typeof(bool))
                {
                    indexObject = bool.Parse(index);
                }
                else if (invokedObject.GetType().GetGenericArguments()[0]==typeof(double))
                {
                    indexObject = double.Parse(index);
                }
                else if (invokedObject.GetType().GetGenericArguments()[0]==typeof(float))
                {
                    indexObject = float.Parse(index);
                }

                if (indexObject != null) invokedObject = ((IDictionary)invokedObject)[indexObject];
            }
        }
        else if (invokedObject.GetType().GetInterface(nameof(IEnumerable))!=null)
        {
            foreach (var index in indexes)
            {
                int currentIndex = 0;
                IEnumerable enumerable = (IEnumerable)invokedObject;
                foreach (var innerObject in enumerable)
                {
                    if (currentIndex == int.Parse(index))
                    {
                        invokedObject = innerObject;
                        break;
                    }
                    currentIndex++;
                }
                if(currentIndex>int.Parse(index)) throw new MemberNotFoundException(ConvertFirstLetter(helpString));
            }
        }
        
        resultString += invokedObject;
        helpString = "";
    }

    private string ConvertFirstLetter(string str)
    {
        return str[0].ToString().ToLower() + str.Remove(0, 1);
    }
    private void CheckTemplate(string template)
    {
        int correctCounter = 0;
        template = template.Replace("{{", "").Replace("}}", "");
        for (var i = 0; i < template.Length; i++)
        {
            if (template[i] == '{') correctCounter++;
            else if (template[i] == '}') correctCounter--;
            if(correctCounter>1 || correctCounter<0) throw new DisbalanceBracketsException(i);
        }
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
            ConstantExpression fieldParam = Expression.Constant(fieldInfo.GetValue(target),fieldInfo.FieldType);
            cache[target.GetType()].Add(ConvertFirstLetter(fieldInfo.Name)+"Accessor",
                Expression.Lambda(memberExpression, typeParam).Compile()); 
        }
        foreach (var propertyInfo in objectProperties)
        {
            MemberExpression memberExpression = Expression.PropertyOrField(typeParam,propertyInfo.Name);
            ConstantExpression fieldParam = Expression.Constant(propertyInfo.GetValue(target),propertyInfo.PropertyType);
            cache[target.GetType()].Add(ConvertFirstLetter(propertyInfo.Name)+"Accessor",
                Expression.Lambda(memberExpression, typeParam).Compile()); 
        }
    }
}