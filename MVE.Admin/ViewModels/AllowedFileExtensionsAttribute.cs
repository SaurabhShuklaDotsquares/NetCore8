using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace MVE.Admin.ViewModels
{


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowedFileExtensionsAttribute : ValidationAttribute
    {
        private readonly string _fileExtensions;
        private readonly string _errorMessage;

        public AllowedFileExtensionsAttribute(string fileExtensions)
        {
            _fileExtensions = fileExtensions;
            _errorMessage = $"Only files with extensions: {fileExtensions} are allowed";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success; // Allow null values if needed.
            }

            var fileExtensionsArray = _fileExtensions.Split(',').Select(ext => ext.Trim()).ToArray();
            var fileName = value.ToString();
            var fileExtension = Path.GetExtension(fileName);

            if (fileExtensionsArray.Any(ext => string.Equals(fileExtension, ext, StringComparison.OrdinalIgnoreCase)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(_errorMessage);
        }
    }

}
