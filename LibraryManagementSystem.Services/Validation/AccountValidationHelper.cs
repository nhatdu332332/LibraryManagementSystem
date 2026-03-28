using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Services
{
	internal static class AccountValidationHelper
	{
		private static readonly Regex EmailRegex = new(
			@"^[A-Za-z0-9]+(?:[._-][A-Za-z0-9]+)*@[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*\.com$",
			RegexOptions.Compiled);

		private static readonly Regex PhoneRegex = new(@"^0\d{8,9}$", RegexOptions.Compiled);

		public static string ValidateEmail(string? email, string fieldName = "Email")
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ArgumentException($"{fieldName} khong duoc de trong.");
			}

			var normalizedEmail = email.Trim();
			if (!normalizedEmail.All(character => character <= 127) || !EmailRegex.IsMatch(normalizedEmail))
			{
				throw new ArgumentException(
					$"{fieldName} khong hop le. Bat buoc theo dung dang ***@***.com, khong dau, chi dung chu cai tieng Anh va so.");
			}

			return normalizedEmail;
		}

		public static string? ValidatePhoneNumber(string? phoneNumber, string fieldName = "So dien thoai")
		{
			if (string.IsNullOrWhiteSpace(phoneNumber))
			{
				return null;
			}

			var normalizedPhone = phoneNumber.Trim();
			if (!PhoneRegex.IsMatch(normalizedPhone))
			{
				throw new ArgumentException($"{fieldName} phai bat dau bang so 0 va chi duoc gom 9 den 10 chu so.");
			}

			return normalizedPhone;
		}

		public static string ValidateRequiredText(string? value, string fieldName)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException($"{fieldName} khong duoc de trong.");
			}

			return value.Trim();
		}

		public static bool IsAdministratorRoleName(string? roleName)
		{
			return string.Equals(roleName, "Administrator", StringComparison.OrdinalIgnoreCase);
		}
	}
}
