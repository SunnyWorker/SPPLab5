namespace StringFormatterProject.exceptions;

public class DisbalanceBracketsException : SystemException
{
    public DisbalanceBracketsException(int errorPosition) : base("Template has non-balanced bracket in position "+errorPosition)
    {
        
    }
}