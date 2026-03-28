using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace LibraryManagementSystem.WPF.Helpers
{
    public class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string email = value as string;

            if (string.IsNullOrWhiteSpace(email))
                return new ValidationResult(false, "Email không được để trống");

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return new ValidationResult(false, "Email không đúng định dạng");

            return ValidationResult.ValidResult;
        }
    }
}