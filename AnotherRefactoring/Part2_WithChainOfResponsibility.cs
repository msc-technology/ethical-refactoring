using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Refactor.AnotherRefactoring;

/*
    The Chain of Responsibility pattern can help us define a sequence of validators which can 
    subsequently be chained together. If one validator fails it can delegate the result to the
    next one in the chain. A FailValidator can be used as a terminal validator which will always
    return false and properly terminate the sequence.
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

    public interface IValidator
    {
        bool IsValid(string code);
    }

    public class FailValidator : BaseValidator
    {
        public static readonly IValidator Instance = new FailValidator();
        private FailValidator() {}
        protected override bool Validate(string code) => false;
    }

    public abstract class BaseValidator: IValidator 
    {
        public bool IsValid(string code)
        {
            var result = Validate(code);
            if(!result && Next != null)
                result = Next.IsValid(code);
            return result;
        }

        protected abstract bool Validate(string code);

        public IValidator Next { get; init; } = FailValidator.Instance;
    }

    public class IDCardValidator : BaseValidator
    {
        protected override bool Validate(string code) 
        {
           return code.Length == 9 && 
                (code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).Take(5).All(Char.IsDigit) && 
                code.Skip(7).All(Char.IsLetter));
        }
    }

    public static bool IsValidDocument(string code) 
    {
        const string FORBIDDEN = "AOQI";
        string[] provinces = new[] { "TO", "MI", "RM"}; // More should be added

        var validator = new IDCardValidator();
        var result = validator.IsValid(code);
        
        if(result)
            return result;

        if(code.Length == 9) // ID Card
        {
/*            if(code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).Take(5).All(Char.IsDigit) && 
                code.Skip(7).All(Char.IsLetter))
                return true;
*/                
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
        else if(code.Length == 11) // Passport
        {
            if(code.Take(2).All(Char.IsLetter) && 
                code.Skip(2).All(Char.IsDigit))
                return true;
        }
        return false;
    }
}
