namespace StringFormatterProject;

public interface IStringFormatter
{
    string Format(string template, object target);
}