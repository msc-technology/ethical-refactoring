using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Refactor.AnotherRefactoring;

/*
    If we want to take a more object oriented approach to our solution we can create an IValidator
    interface which will be implemented by different validator classes. Its role is simple: validate
    a given code and return a boolean, in the same way the initial method does. This can allow us to
    create different classes for different document types all sharing the same interface.
*/
[TestClass]
public class Part1
{
    [TestMethod] public void ValidIdCard() => Assert.IsTrue(IsValidDocument("AA12345BB"));
    [TestMethod] public void ValidPaperIdCard() => Assert.IsTrue(IsValidDocument("AA1234567"));
    [TestMethod] public void InvalidIdCard() => Assert.IsFalse(IsValidDocument("123456"));
    [TestMethod] public void InvalidPaperIdCard() => Assert.IsFalse(IsValidDocument("AAA123456"));
    [TestMethod] public void ValidPassport() => Assert.IsTrue(IsValidDocument("AA123456789"));
    [TestMethod] public void InvalidPassport() => Assert.IsFalse(IsValidDocument("123456"));
    [TestMethod] public void ValidDriversLicense() => Assert.IsTrue(IsValidDocument("U1123X456K"));
    [TestMethod] public void InvalidDriversLicense() => Assert.IsFalse(IsValidDocument("U1123A456K"));
    [TestMethod] public void ValidDriversLicenseWithOldSchema() => Assert.IsTrue(IsValidDocument("U11234567K"));
    [TestMethod] public void ValidOldDriversLicense() => Assert.IsTrue(IsValidDocument("TO1234567K"));
    [TestMethod] public void InvalidOldDriversLicense() => Assert.IsFalse(IsValidDocument("XX1234567K"));
    [TestMethod] public void InvalidOldDriversLicenseWithNewNumbers() => Assert.IsFalse(IsValidDocument("TO123A567K"));

    interface IValidator
    {
        bool IsValid(string code);
    }

    public class PassportValidator : IValidator
    {
        public bool IsValid(string code)
        {
             return code.Length == 11 && code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).All(Char.IsDigit);
        }
    }

    public static bool IsValidDocument(string code) 
    {
        const string FORBIDDEN = "AOQI";
        string[] provinces = new[] { "TO", "MI", "RM" }; // More should be added

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
            if(code.StartsWith("U1") &&
                code.Skip(2).Take(7).All(Char.IsDigit) &&
                code.Skip(9).Take(1).All(Char.IsLetter))
                return true;
            if(code.Take(2).All(Char.IsLetter) &&
                code.Skip(2).Take(7).All(Char.IsDigit) &&
                code.Skip(9).Take(1).All(Char.IsLetter) &&
                provinces.Any(province => province == $"{code[0]}{code[1]}"))
                return true;
        }
        else if(new PassportValidator().IsValid(code))
            return true;
        /*
        else if(code.Length == 11) // Passport
        {
            if(code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).All(Char.IsDigit))
                return true;
        }*/
        return false;
    }
}
