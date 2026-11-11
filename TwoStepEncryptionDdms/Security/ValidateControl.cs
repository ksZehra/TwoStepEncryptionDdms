using System.Text.RegularExpressions;

namespace TwoStepEncryptionDdms.Security
{
    public class ValidateControl
    {
        private const int MinLength = 8;
        private const int MaxLength = 128;

        private static readonly PasswordRule[] Rules =
        {
            new PasswordRule("[a-z]", "Şifreniz en az bir küçük harf içermelidir!"),
            new PasswordRule("[A-Z]", "Şifreniz en az bir büyük harf içermelidir!"),
            new PasswordRule("[0-9]", "Şifreniz en az bir sayısal ifade içermelidir!"),
            new PasswordRule(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]", "Şifreniz en az bir özel karakter içermelidir!")
        };

        public static bool ValidatePassword(string pwd, out string errorMsg)
        {
            errorMsg = string.Empty;

            if (string.IsNullOrWhiteSpace(pwd))
            {
                errorMsg = "Şifre bilginiz boş geçilemez!";
                return false;
            }

            if (pwd.Length < MinLength)
            {
                errorMsg = "Şifreniz en az " + MinLength + " karakter olmalıdır!";
                return false;
            }

            if (pwd.Length > MaxLength)
            {
                errorMsg = "Şifreniz en fazla " + MaxLength + " karakter olabilir!";
                return false;
            }

            foreach (PasswordRule rule in Rules)
            {
                if (!rule.Pattern.IsMatch(pwd))
                {
                    errorMsg = rule.Message;
                    return false;
                }
            }

            return true;
        }

        private sealed class PasswordRule
        {
            public PasswordRule(string pattern, string message)
            {
                Pattern = new Regex(pattern, RegexOptions.Compiled);
                Message = message;
            }

            public Regex Pattern { get; private set; }
            public string Message { get; private set; }
        }
    }
}
