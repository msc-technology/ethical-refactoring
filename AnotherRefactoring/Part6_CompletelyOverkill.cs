using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Refactor.AnotherRefactoring;

/*
    We can go even further with our refactoring, and make our classes generic, thus having a
    way to validate any given model of any given kind. Would this be extensible? Certainly
    yes, allowing for more complex validations, which can go beyond the document code.
    In this case we are adding a Composite pattern for creating composite validation on top of
    the chain we had for the document. This can allow us to check for required fields in addition
    to the validity of a document.
    Is this worth it, however? Again there is not a single answer, as it will depend on the use 
    you will be required to have. If you are writing a generic validation library this might probably
    be a good start (even though there are several parts missing), but for checking just a document code
    it might be completely overkill and unmaintainable by junior members in your team.
    As for many things in computer science this solution might be aesthetically pleasing, but is it ethical?
    Does it allow for anybody to understand and maintain our code? Will somebody be able to keep it tidy and
    follow our initial design, or will it crumble under the weight of changes done by less experienced members
    of our team? This is probably among the hardest questions you will be faced to answer in your career.
*/
[TestClass]
public class Refactored6
{
#region Tests
    [TestMethod] public void NoFirstName() => Assert.IsFalse(IsValidRegistrationForm(new RegistrationForm("", "Last Name", "Document")));
    [TestMethod] public void NoLastName() => Assert.IsFalse(IsValidRegistrationForm(new RegistrationForm("First Name", "", "Document")));
    [TestMethod] public void NoDocument() => Assert.IsFalse(IsValidRegistrationForm(new RegistrationForm("First Name", "Last Name", "")));
    [TestMethod] public void InvalidDocument() => Assert.IsFalse(IsValidRegistrationForm(new RegistrationForm("First Name", "Last Name", "XXXX")));
    [TestMethod] public void Valid() => Assert.IsTrue(IsValidRegistrationForm(new RegistrationForm("First Name", "Last Name", "U1123X456X")));
   
    public record RegistrationForm(string firstName, string lastName, string documentId);
#endregion

#region Validators
    public interface IValidator<T>
    {
        bool IsValid(T model);
    }

    public class FailValidator<T> : BaseValidator<T>
    {
        public static readonly BaseValidator<T> Instance = new FailValidator<T>();
        private FailValidator() {}
        protected override bool Validate(T model) => false;
    }

    public abstract class BaseValidator<T>: IValidator<T>
    {
        public bool IsValid(T model)
        {
            var result = Validate(model);
            if(!result && Next != null)
                result = Next.IsValid(model);
            return result;
        }

        protected abstract bool Validate(T model);

        public BaseValidator<T>? Next { get; set; } = FailValidator<T>.Instance;
    }

        public class LoggingValidator<T> : BaseValidator<T>
    {
        BaseValidator<T> source;
        public LoggingValidator(BaseValidator<T> source) => this.source = source;
        protected override bool Validate(T model)
        {
            var result = source.IsValid(model);
            Console.WriteLine($"Validation of {model} with {source.GetType().Name}: {result}");
            return result;
        }
    }

    public class FieldValidator<T, T1> : BaseValidator<T>
    {
        private Func<T, T1> fieldExtractor;
        private IValidator<T1> validator;

        public FieldValidator(Func<T, T1> fieldExtractor, IValidator<T1> validator)
        {
            this.fieldExtractor = fieldExtractor;
            this.validator = validator;
        }

        protected override bool Validate(T model) => validator.IsValid(fieldExtractor(model));
    }

    public class RequiredValidator : BaseValidator<string>
    {
        protected override bool Validate(string model) => !String.IsNullOrWhiteSpace(model);
    }
    
    public class CompositeValidator<T> : IValidator<T>
    {
        IValidator<T>[] validators;

        public CompositeValidator(params IValidator<T>[] validators)
        {
            this.validators = validators;
        }

        public bool IsValid(T model)
        {
            var result = true;
            foreach(var validator in validators)
                result &= validator.IsValid(model);
            return result;
        }
    }

#endregion

#region Document Validators
    public class IDCardValidator : BaseValidator<string>
    {
        protected override bool Validate(string model) => model.Length == 9 && 
            model.Take(2).All(Char.IsLetter) && 
            model.Skip(2).Take(5).All(Char.IsDigit) && 
            model.Skip(7).All(Char.IsLetter);
    }

    public class PaperIDCardValidator : BaseValidator<string>
    {
        protected override bool Validate(string model) => model.Length == 9 && 
            model.Take(2).All(Char.IsLetter) &&
            model.Skip(2).All(Char.IsDigit);
    }

    public class PassportValidator : BaseValidator<string>
    {
        protected override bool Validate(string model) => model.Length == 11 &&
            model.Take(2).All(Char.IsLetter) &&
            model.Skip(2).All(Char.IsDigit);
    }

    public class UcoDriversLicenceValidator : BaseValidator<string>
    {
        const string FORBIDDEN = "AOQI";
        protected override bool Validate(string model) => model.Length == 10 && 
            model.Length == 10 && 
            model.StartsWith("U1") &&
            model.Skip(2).Take(3).All(Char.IsDigit) &&
            Char.IsLetter(model[5]) && 
            !Enumerable.Any(FORBIDDEN, c => c == model[5]) &&
            model.Skip(6).Take(3).All(Char.IsDigit) &&
            model.Skip(9).Take(1).All(Char.IsLetter);
    }

    public class UcoOldDriversLicenceValidator : BaseValidator<string>
    {
        protected override bool Validate(string model) => model.Length == 10 && 
            model.StartsWith("U1") &&
            model.Skip(2).Take(7).All(Char.IsDigit) &&
            model.Skip(9).Take(1).All(Char.IsLetter);
    }

    public class ProvinceDriversLicenceValidator : BaseValidator<string>
    {
        protected override bool Validate(string model) => model.Length == 10 &&
            model.Take(2).All(Char.IsLetter) &&
            model.Skip(2).Take(7).All(Char.IsDigit) &&
            model.Skip(9).Take(1).All(Char.IsLetter) &&
            IsValidProvince($"{model[0]}{model[1]}");
        
        private bool IsValidProvince(string provinceCode) 
        {
            string[] provinces = new[] { "TO", "MI", "RM"}; // More should be added
            return provinces.Any(province => province == provinceCode);
        }
    }
#endregion

    public class ValidatorBuilder<T>
    {
        BaseValidator<T> start;
        BaseValidator<T> current;

        public ValidatorBuilder(BaseValidator<T> start) 
        {
            this.start = this.current = start;
        }

        public static ValidatorBuilder<T> From(BaseValidator<T> start) => new ValidatorBuilder<T>(start);

        public ValidatorBuilder<T> FollowedBy(BaseValidator<T> next)
        {
            this.current.Next = next;
            this.current = next;
            return this;
        }

        public BaseValidator<T> Build() => this.start;
    }

    public static FieldValidator<T, string> FieldRequired<T>(Func<T, string> extractor) 
    {
        return new FieldValidator<T, string>(extractor, new RequiredValidator());
    }

    public static bool IsValidRegistrationForm(RegistrationForm form)
    {
        var firstNameValidator = FieldRequired<RegistrationForm>(m => m.firstName);
        var lastNameValidator = FieldRequired<RegistrationForm>(m => m.lastName);
        var documentIdValidator = FieldRequired<RegistrationForm>(m => m.documentId);

        var documentValidator = ValidatorBuilder<string>
            .From(new IDCardValidator())
            .FollowedBy(new PaperIDCardValidator())
            .FollowedBy(new PassportValidator())
            .FollowedBy(new UcoOldDriversLicenceValidator())
            .FollowedBy(new UcoDriversLicenceValidator())
            .FollowedBy(new ProvinceDriversLicenceValidator())
            .Build();

        var formDocumentValidator = new FieldValidator<RegistrationForm, string>(form => form.documentId, documentValidator);

        var formValidator = new CompositeValidator<RegistrationForm>(firstNameValidator, lastNameValidator, documentIdValidator, formDocumentValidator);

        return formValidator.IsValid(form);
    }
}