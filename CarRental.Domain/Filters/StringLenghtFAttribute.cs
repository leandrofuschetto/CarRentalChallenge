using CarRental.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Filters
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringLenghtFAttribute : ValidationAttribute
    {
        private int maxLenght;
        private string errorMessage;
        public StringLenghtFAttribute(int MaxLenght, string ErrorMessage) : base(ErrorMessage)
        {
            maxLenght = MaxLenght;
            errorMessage = ErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (string)value;
            if (val.Count() > maxLenght)
            {
                throw new MaxLenghtException
                    (errorMessage, $"{validationContext.MemberName}_MAXLENGHT_ERROR");
            }

            return ValidationResult.Success;
        }
    }
}
