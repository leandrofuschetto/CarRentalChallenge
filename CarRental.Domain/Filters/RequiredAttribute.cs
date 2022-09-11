using CarRental.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Filters
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredAttribute : ValidationAttribute
    {
        private const string Email = "Email";
        private const string FullName = "FullName";

        public RequiredAttribute(string ErrorMessage = "") : base(ErrorMessage)
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = Convert.ToString(value);
            if (val is null || string.IsNullOrEmpty(val))
            {
                throw new FieldMandatoryException
                    (ErrorMessage, $"{validationContext.MemberName}_IS_REQUIRED");
            }

            return ValidationResult.Success;
        }
    }
}
