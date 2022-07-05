﻿namespace Refactor.Preamble;

public class Part4
{
    public static bool IsValidDocument(string code) 
    {
        const string FORBIDDEN = "AOQI";

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
        else if(code.Length == 10) // Driver's licence
        {
            if(code.StartsWith("U1") &&
                code.Skip(2).Take(3).All(Char.IsDigit) &&
                (Char.IsLetter(code[5]) && 
                !Enumerable.Any(FORBIDDEN, c => c == code[5])) &&
                code.Skip(6).Take(3).All(Char.IsDigit) &&
                code.Skip(9).Take(1).All(Char.IsLetter))
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
