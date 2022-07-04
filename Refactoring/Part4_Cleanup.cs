using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Refactor.Refactoring;

/*
    We have all of our methods extracted, we can add an IsValidProvince just for clarity and for
    future expansion, then remove all the if..else branches by just composing the methods with a
    logical OR operand.
*/

[TestClass]
public class Part4
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
        return IsValidIdCard(code) || IsValidDriversLicence(code) || IsValidPassport(code);
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
                IsValidProvince($"{code[0]}{code[1]}");
    }

    private static bool IsValidProvince(string provinceCode)
    {
        string[] provinces = new[] { "TO", "MI", "RM"}; // More should be added
        return provinces.Any(province => province == provinceCode);
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
        return code.Length == 11 && code.Take(2).All(Char.IsLetter) && 
            code.Skip(2).All(Char.IsDigit);
    }
}
