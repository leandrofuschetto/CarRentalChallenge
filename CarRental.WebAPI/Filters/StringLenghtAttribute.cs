using CarRental.WebAPI.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace CarRental.WebAPI.Filters
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringLenghtAttribute : ValidationAttribute
    {
        private int maxLenght;
        private int minLenght;
        private string errorMessage;
        public StringLenghtAttribute(int MaxLenght, int MinLenght, string ErrorMessage) 
            : base(ErrorMessage)
        {
            maxLenght = MaxLenght;
            minLenght = MinLenght;
            errorMessage = ErrorMessage;
        }

        protected override ValidationResult IsValid
            (object value, ValidationContext validationContext)
        {
            var val = (string)value;
            if (val.Count() > maxLenght)
            {
                throw new MaxLenghtException(
                    errorMessage, 
                    $"{validationContext.MemberName.ToUpper()}_MAXLENGHT_ERROR");
            }

            if (val.Count() < minLenght)
            {
                throw new MaxLenghtException(
                        errorMessage,
                        $"{validationContext.MemberName.ToUpper()}_MINLENGHT_ERROR");
            }

            return ValidationResult.Success;
        }
    }
}
