using System.Data;
using TwoStepEncryptionDdms.Data;

namespace TwoStepEncryptionDdms.Repositories
{
    public class UserRepository
    {
        private readonly DataIslemleri _data;

        public UserRepository(DataIslemleri data)
        {
            _data = data;
        }

        public DataTable FindActiveByUserName(string userName)
        {
            _data.AddParameter("@uid", userName);
            return _data.QueryTable(
                @"SELECT KID,KULLANICIADI,KSIFRE,ISNULL(KAD,'')+' '+ISNULL(KSOYAD,'') ADSOYAD
                  FROM TBL_USERS WHERE KULLANICIADI=@uid AND KDURUM='1'");
        }

        public string GetPasswordHash(int userId)
        {
            _data.AddParameter("@KID", userId);
            return System.Convert.ToString(_data.QueryScalar("SELECT KSIFRE FROM TBL_USERS WHERE KID=@KID"));
        }

        public void UpdatePasswordHash(int userId, string passwordHash)
        {
            _data.AddParameter("@pwd", passwordHash);
            _data.AddParameter("@KID", userId);
            _data.Execute("UPDATE TBL_USERS SET KSIFRE=@pwd WHERE KID=@KID");
        }

        public DataTable FindByEmailOrUserName(string email, string userName)
        {
            _data.AddParameter("@email", email);
            _data.AddParameter("@userName", userName);
            return _data.QueryTable(
                "Select KEMAIL,KULLANICIADI from TBL_USERS where KEMAIL=@email or KULLANICIADI=@userName");
        }

        public void Insert(string firstName, string lastName, string email, string userName, string passwordHash, string status, string profileCode)
        {
            _data.AddParameter("@KAD", firstName);
            _data.AddParameter("@KSOYAD", lastName);
            _data.AddParameter("@KEMAIL", email);
            _data.AddParameter("@KULLANICIADI", userName);
            _data.AddParameter("@KSIFRE", passwordHash);
            _data.AddParameter("@KDURUM", status);
            _data.AddParameter("@PROFILKODU", profileCode);
            _data.Execute(
                @"insert into TBL_USERS(KAD, KSOYAD, KEMAIL, KULLANICIADI, KSIFRE,KDURUM,PROFILKODU)
                  values(@KAD,@KSOYAD,@KEMAIL,@KULLANICIADI,@KSIFRE,@KDURUM,@PROFILKODU)");
        }

        public DataTable GetAccountInfo(int userId)
        {
            _data.AddParameter("@USERID", userId);
            return _data.QueryTable(
                @"SELECT U.KAD,U.KSOYAD,U.KEMAIL,U.KULLANICIADI,U.KDURUM,P.PROFILADI,
                         CASE WHEN S.LIST='1' THEN 'Listeleme, ' ELSE '' END LISTELEME,
                         CASE WHEN S.EDIT='1' THEN 'Güncelleme, ' ELSE '' END GUNCELLEME,
                         CASE WHEN S.NEW='1' THEN 'Yeni Kayıt Ekleme, ' ELSE '' END YENIEKLEME,
                         CASE WHEN S.[VIEW]='1' THEN 'Görüntüleme, ' ELSE '' END GORUNTULEME,
                         CASE WHEN S.[DELETE]='1' THEN 'Silme, ' ELSE '' END SILME
                  FROM TBL_USERS U
                  INNER JOIN TBL_PROFIL P ON P.PROFILKODU=U.PROFILKODU
                  INNER JOIN TBL_PROFILSEC S ON S.PROFILKODU=P.PROFILKODU
                  WHERE KID=@USERID");
        }
    }
}
