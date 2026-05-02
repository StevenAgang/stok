using stok.Repository.ViewModel.UserAccount;
using System.Text.RegularExpressions;

namespace stok.Repository.Configurations.Validation
{
    public class UserAccountValidation
    {
        private static readonly string EmailPattern = @"^[\w.-]+@[\w.-]+\.\w{2,}$";
        private static readonly string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$";

        public static void NotNullEmailAndPassword(UserAccountLoginViewModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BadRequest("Email is required");
            }
            if (!Regex.IsMatch(user.Email, EmailPattern))
            {
                throw new BadRequest("Email is invalid");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new BadRequest("Password is required");
            }
        }
        public static void ValidEmailAndPassword(UserAccountRegistrationViewModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BadRequest("Email is required");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new BadRequest("Password is required");
            }
            if (!Regex.IsMatch(user.Email, EmailPattern))
            {
                throw new BadRequest("Email is invalid");
            }
            if (!Regex.IsMatch(user.Password, PasswordPattern))
            {
                throw new BadRequest("Password must contain at least one uppercase, lowercase, number, and special character, and at least 8 characters long.");
            }
        }

        public static void ValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new BadRequest("Password is required");
            }
            if (!Regex.IsMatch(password, PasswordPattern))
            {
                throw new BadRequest("Password must contain at least one uppercase, lowercase, number, and special character, and at least 8 characters long.");
            }
        }

        public static void ValidInformation(UserAccountRegistrationViewModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BadRequest("Email is required");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new BadRequest("Password is required");
            }
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new BadRequest("Firstname is required");
            }
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                throw new BadRequest("Lastname is required");
            }
        }
    }
}
