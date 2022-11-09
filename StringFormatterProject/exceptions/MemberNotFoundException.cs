namespace StringFormatterProject.exceptions;

public class MemberNotFoundException : SystemException
{
    public MemberNotFoundException(string memberName) : base("Can't find member with this name in specified class: "+memberName)
    {
        
    }
}