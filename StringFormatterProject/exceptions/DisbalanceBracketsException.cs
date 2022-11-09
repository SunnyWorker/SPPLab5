namespace StringFormatterProject.exceptions;

public class DisbalanceBracketsException : SystemException
{
    public DisbalanceBracketsException() : base("Template has non-balanced brackets positions")
    {
        
    }
}