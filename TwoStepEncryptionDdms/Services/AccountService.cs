using System;
using System.Data;
using TwoStepEncryptionDdms.Data;
using TwoStepEncryptionDdms.Repositories;
using TwoStepEncryptionDdms.Security;

namespace TwoStepEncryptionDdms.Services
{
    public class AccountService
    {
        public AuthenticatedUser Authenticate(string userName, string password)
        {
            using (var data = Open())
            {
                var users = new UserRepository(data);
                DataTable table = users.FindActiveByUserName(userName);

                if (table == null || table.Rows.Count == 0)
                {
                    PasswordHasher.BurnTime(password);
                    return null;
                }

                DataRow row = table.Rows[0];
                PasswordVerifyResult result = PasswordHasher.Verify(password, Convert.ToString(row["KSIFRE"]));
                if (result == PasswordVerifyResult.Failed)
                {
                    return null;
                }

                int id = Convert.ToInt32(row["KID"]);
                string name = Convert.ToString(row["KULLANICIADI"]);
                if (string.IsNullOrWhiteSpace(name) || id <= 0)
                {
                    return null;
                }

                if (result == PasswordVerifyResult.SuccessRehashNeeded)
                {
                    users.UpdatePasswordHash(id, PasswordHasher.Hash(password));
                }

                return new AuthenticatedUser(id, name, Convert.ToString(row["ADSOYAD"]).Trim());
            }
        }

        public void Register(string firstName, string lastName, string email, string userName, string password)
        {
            string policyMessage;
            if (!ValidateControl.ValidatePassword(password, out policyMessage))
            {
                throw new UserException(policyMessage);
            }

            using (var data = Open())
            {
                var users = new UserRepository(data);

                foreach (DataRow item in users.FindByEmailOrUserName(email, userName).Rows)
                {
                    if (item["KEMAIL"].ToString() == email)
                    {
                        throw new UserException("İlgili E-posta adresi ile daha önce hesap oluşturulmuş. Lütfen başka bir E-posta adresi deneyiniz!");
                    }
                    else if (item["KULLANICIADI"].ToString() == userName)
                    {
                        throw new UserException("İlgili kullanıcı adı ile daha önce hesap oluşturulmuş. Lütfen başka bir kullanıcı adı deneyiniz!");
                    }
                }

                users.Insert(firstName, lastName, email, userName, PasswordHasher.Hash(password), "1", "nyk");
            }
        }

        public DataTable GetAccountInfo(int userId)
        {
            using (var data = Open())
            {
                return new UserRepository(data).GetAccountInfo(userId);
            }
        }

        public void ChangePassword(int userId, string currentPassword, string newPassword)
        {
            string policyMessage;
            if (!ValidateControl.ValidatePassword(newPassword, out policyMessage))
            {
                throw new UserException(policyMessage);
            }

            using (var data = Open())
            {
                var users = new UserRepository(data);
                string storedHash = users.GetPasswordHash(userId);

                if (PasswordHasher.Verify(currentPassword, storedHash) == PasswordVerifyResult.Failed)
                {
                    throw new UserException("Mevcut şifrenizi yanlış girdiniz!");
                }

                users.UpdatePasswordHash(userId, PasswordHasher.Hash(newPassword));
            }
        }

        private static DataIslemleri Open()
        {
            var data = new DataIslemleri();
            data.Open();
            return data;
        }
    }
}
