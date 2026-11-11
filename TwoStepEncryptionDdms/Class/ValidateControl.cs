using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;


public class ValidateControl
{
    public static bool ValidatePassword(string pwd, out string errorMsg)
    {
        errorMsg = string.Empty;

        if (string.IsNullOrWhiteSpace(pwd))
        {
            errorMsg = "Şifre bilginiz boş geçilemez!";
            return false;
        }

        if (pwd.Length < 8)
        {
            errorMsg = "Şifreniz en az 8 karakter olmalıdır!";
            return false;
        }

        if (pwd.Length > 128)
        {
            errorMsg = "Şifreniz en fazla 128 karakter olabilir!";
            return false;
        }

        var isNumber = new Regex(@"[0-9]+");
        var isUpper = new Regex(@"[A-Z]+");
        var isLower = new Regex(@"[a-z]+");
        var isOzelKarakter = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

        if (!isLower.IsMatch(pwd))
        {
            errorMsg = "Şifreniz en az bir küçük harf içermelidir!";
            return false;
        }
        else if (!isUpper.IsMatch(pwd))
        {
            errorMsg = "Şifreniz en az bir büyük harf içermelidir!";
            return false;
        }
        else if (!isNumber.IsMatch(pwd))
        {
            errorMsg = "Şifreniz en az bir sayısal ifade içermelidir!";
            return false;
        }
        else if (!isOzelKarakter.IsMatch(pwd))
        {
            errorMsg = "Şifreniz en bir özel karakter içermelidir!";
            return false;
        }
        else
            return true;
    }
}
