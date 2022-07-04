using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Refactor.Refactoring;

/*
    Once we have all the tests covering our initial method we can start extracting smaller
    methods which are specific to a given document type. The IsValidDocument still remains,
    but it just delegatest the checks to the other smaller methods which can be implemented 
    one at a time withoug having to change the tests.
*/
[TestClass]
public class Part2
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

    public static bool IsValidDocument(string code) 
    {
        if(IsValidIdCard(code))
            return true;
        if(IsValidDriversLicence(code))
            return true;
        if(IsValidPassport(code))
            return true;
        
        return false;
    }

    private static bool IsValidIdCard(string code) 
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
        return false;
    }

    private static bool IsValidDriversLicence(string code)
    {
        const string FORBIDDEN = "AOQI";
        string[] provinces = new[] { "TO", "MI", "RM"}; // More should be added

        if(code.Length == 10) // Driver's licence
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
        return false;
    }

    private static bool IsValidPassport(string code) 
    {
        if(code.Length == 11) // Passport
        {
            if(code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).All(Char.IsDigit))
                return true;
        }
        return false;
    }
}
