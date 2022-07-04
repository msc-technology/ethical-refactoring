using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Refactor.AnotherRefactoring;

/*
    A Builder pattern might help us hide part of the building logic, thus preventing us to have
    an ugly Christmas tree of nested statements for building our chain. We can inline our
    validators in a way very similar to those of LINQ or of other chainable APIs (which are
    most of the time just a builder pattern in disguise). The code is more pleasing to read,
    but what is the benefit of having such a complex code structure?
*/
[TestClass]
public class Refactored4
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
        public static readonly BaseValidator Instance = new FailValidator();
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

        public BaseValidator Next { get; set; } = FailValidator.Instance;
    }

    public class IDCardValidator : BaseValidator
    {
        protected override bool Validate(string code) => code.Length == 9 && 
            code.Take(2).All(Char.IsLetter) && 
            code.Skip(2).Take(5).All(Char.IsDigit) && 
            code.Skip(7).All(Char.IsLetter);
    }

    public class PaperIDCardValidator : BaseValidator
    {
        protected override bool Validate(string code) => code.Length == 9 && 
            code.Take(2).All(Char.IsLetter) &&
            code.Skip(2).All(Char.IsDigit);
    }

    public class PassportValidator : BaseValidator
    {
        protected override bool Validate(string code) => code.Length == 11 &&
            code.Take(2).All(Char.IsLetter) &&
            code.Skip(2).All(Char.IsDigit);
    }

    public class UcoDriversLicenceValidator : BaseValidator
    {
        const string FORBIDDEN = "AOQI";
        protected override bool Validate(string code) => code.Length == 10 && 
            code.Length == 10 && 
            code.StartsWith("U1") &&
            code.Skip(2).Take(3).All(Char.IsDigit) &&
            Char.IsLetter(code[5]) && 
            !Enumerable.Any(FORBIDDEN, c => c == code[5]) &&
            code.Skip(6).Take(3).All(Char.IsDigit) &&
            code.Skip(9).Take(1).All(Char.IsLetter);
    }

    public class UcoOldDriversLicenceValidator : BaseValidator
    {
        protected override bool Validate(string code) => code.Length == 10 && 
            code.StartsWith("U1") &&
            code.Skip(2).Take(7).All(Char.IsDigit) &&
            code.Skip(9).Take(1).All(Char.IsLetter);
    }

    public class ProvinceDriversLicenceValidator : BaseValidator
    {
        protected override bool Validate(string code) => code.Length == 10 &&
            code.Take(2).All(Char.IsLetter) &&
            code.Skip(2).Take(7).All(Char.IsDigit) &&
            code.Skip(9).Take(1).All(Char.IsLetter) &&
            IsValidProvince($"{code[0]}{code[1]}");
        
        private bool IsValidProvince(string provinceCode) 
        {
            string[] provinces = new[] { "TO", "MI", "RM"}; // More should be added
            return provinces.Any(province => province == provinceCode);
        }
    }

    public class ValidatorBuilder
    {
        BaseValidator start;
        BaseValidator current;

        public ValidatorBuilder(BaseValidator start) 
        {
            this.start = this.current = start;
        }

        public static ValidatorBuilder From(BaseValidator start) => new ValidatorBuilder(start);

        public ValidatorBuilder FollowedBy(BaseValidator next)
        {
            this.current.Next = next;
            this.current = next;
            return this;
        }

        public BaseValidator Build() => this.start;
    }

    public static bool IsValidDocument(string code)
    {
        var validator = ValidatorBuilder
            .From(new IDCardValidator())
            .FollowedBy(new PaperIDCardValidator())
            .FollowedBy(new PassportValidator())
            .FollowedBy(new UcoOldDriversLicenceValidator())
            .FollowedBy(new UcoDriversLicenceValidator())
            .FollowedBy(new ProvinceDriversLicenceValidator())
            .Build();
        return validator.IsValid(code);
    }
}