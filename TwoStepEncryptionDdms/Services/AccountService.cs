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
                string storedHash = Convert.ToString(row["KSIFRE"]);
                PasswordVerifyResult result = PasswordHasher.Verify(password, storedHash);
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
                    storedHash = PasswordHasher.Hash(password);
                    users.UpdatePasswordHash(id, storedHash);
                }

                return new AuthenticatedUser(id, name, Convert.ToString(row["ADSOYAD"]).Trim(), PasswordHasher.Stamp(storedHash));
            }
        }

        public void Register(string firstName, string lastName, string email, string userName, string password)
        {
            firstName = (firstName ?? "").Trim();
            lastName = (lastName ?? "").Trim();
            email = (email ?? "").Trim().ToLowerInvariant();
            userName = (userName ?? "").Trim();

            if (email.Length == 0 || userName.Length == 0)
            {
                throw new UserException("E-posta ve kullanıcı adı alanları boş bırakılamaz!");
            }
            if (firstName.Length > 50 || lastName.Length > 50 || email.Length > 250 || userName.Length > 100)
            {
                throw new UserException("Girilen bilgilerden biri izin verilen uzunluğu aşıyor!");
            }

            string policyMessage;
            if (!ValidateControl.ValidatePassword(password, out policyMessage))
            {
                throw new UserException(policyMessage);
            }

            using (var data = Open())
            {
                var users = new UserRepository(data);

                DataTable existing = users.FindByEmailOrUserName(email, userName);
                foreach (DataRow item in existing.Rows)
                {
                    if (string.Equals(Convert.ToString(item["KEMAIL"]), email, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new UserException("İlgili E-posta adresi ile daha önce hesap oluşturulmuş. Lütfen başka bir E-posta adresi deneyiniz!");
                    }
                    if (string.Equals(Convert.ToString(item["KULLANICIADI"]), userName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new UserException("İlgili kullanıcı adı ile daha önce hesap oluşturulmuş. Lütfen başka bir kullanıcı adı deneyiniz!");
                    }
                }
                if (existing.Rows.Count > 0)
                {
                    throw new UserException("Bu E-posta adresi veya kullanıcı adı zaten kullanılıyor. Lütfen farklı bilgiler deneyiniz!");
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

        public string ChangePassword(int userId, string currentPassword, string newPassword)
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

                string newHash = PasswordHasher.Hash(newPassword);
                users.UpdatePasswordHash(userId, newHash);
                return PasswordHasher.Stamp(newHash);
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
