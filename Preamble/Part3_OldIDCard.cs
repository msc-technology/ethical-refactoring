namespace Refactor.Preamble;

public class Part3
{
    public static bool IsValidDocument(string code) 
    {
        if(code.Length == 9) // ID Card
        {
            if(code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).Take(5).All(Char.IsDigit) && 
                code.Skip(7).All(Char.IsLetter))
                return true;
            if(code.Take(2).All(Char.IsLetter) &&
                code.Skip(2).All(Char.IsDigit))
                return true;

        }
        else if(code.Length == 11) // Passport
        {
            if(code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).All(Char.IsDigit))
                return true;
        }
        return false;
    }
}
