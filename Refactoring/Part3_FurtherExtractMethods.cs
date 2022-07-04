using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Refactor.Refactoring;

/*
    The extraction of methods might continue by extracting methods which are specific not
    only to the document type, but also to the specific kind of document (e.g. paper id card
    or electronic one). This way our granularity will be even smaller. Again, tests are not
    changing.
*/
[TestClass]
public class Part3
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
        return IsValidElectronicIdCard(code) || IsValidPaperIdCard(code);
    }

    private static bool IsValidElectronicIdCard(string code)
    {
        return code.Length == 9 && code.Take(2).All(Char.IsLetter) && 
            code.Skip(2).Take(5).All(Char.IsDigit) && 
            code.Skip(7).All(Char.IsLetter);
    }

    private static bool IsValidPaperIdCard(string code)
    {
        return code.Length == 9 && code.Take(2).All(Char.IsLetter) &&
            code.Skip(2).All(Char.IsDigit);
    }

    private static bool IsValidDriversLicence(string code)
    {
        return IsValidUCODriversLicence(code) || IsValidOldDriversLicence(code) || IsValidNewDriversLicence(code);
    }

    private static bool IsValidUCODriversLicence(string code)
    {
        return code.Length == 10 && code.StartsWith("U1") &&
            code.Skip(2).Take(7).All(Char.IsDigit) &&
            code.Skip(9).Take(1).All(Char.IsLetter);
    }

    private static bool IsValidOldDriversLicence(string code)
    {
        string[] provinces = new[] { "TO", "MI", "RM"}; // More should be added

        return code.Length == 10 && code.Take(2).All(Char.IsLetter) &&
                code.Skip(2).Take(7).All(Char.IsDigit) &&
                code.Skip(9).Take(1).All(Char.IsLetter) &&
                provinces.Any(province => province == $"{code[0]}{code[1]}");
    }

    private static bool IsValidNewDriversLicence(string code)
    {
        const string FORBIDDEN = "AOQI";
        return code.Length == 10 && code.StartsWith("U1") &&
            code.Skip(2).Take(3).All(Char.IsDigit) &&
            (Char.IsLetter(code[5]) && 
            !Enumerable.Any(FORBIDDEN, c => c == code[5])) &&
            code.Skip(6).Take(3).All(Char.IsDigit) &&
            code.Skip(9).Take(1).All(Char.IsLetter);
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
