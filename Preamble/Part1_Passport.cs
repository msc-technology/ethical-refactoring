namespace Refactor.Preamble;

public class Part1
{
    public static bool IsValidDocument(string code) 
    {
        if(code.Length == 11)
            if(code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).All(Char.IsDigit))
                return true;
        return false;
    }
}
