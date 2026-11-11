using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Data;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class Anasayfa : TwoStepEncryptionDdms.Security.BasePage
    {
        DataIslemleri data = new DataIslemleri();
        static DataTable dtDatas = new DataTable();
        readonly DatasetService datasets = new DatasetService();

        protected int DatasetCount { get; private set; }
        protected int EncryptedDatasetCount { get; private set; }
        protected long RecordCount { get; private set; }
        protected bool DashboardAvailable { get; private set; }

        protected bool CanList
        {
            get { return CurrentUser != null && CurrentUser.Has(Permission.List); }
        }

        protected bool CanCreate
        {
            get { return CurrentUser != null && CurrentUser.Has(Permission.New); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //PeformansTesti();

            if (!IsPostBack)
            {
                LoadDashboardSummary();
            }
        }

        // Ana sayfa yalnızca salt-okunur sayımlar gösterir; şifreleme ve kayıt akışına dokunmaz.
        private void LoadDashboardSummary()
        {
            try
            {
                DashboardSummary summary = datasets.GetDashboardSummary();
                DatasetCount = summary.DatasetCount;
                EncryptedDatasetCount = summary.EncryptedDatasetCount;
                RecordCount = summary.RecordCount;
                DashboardAvailable = true;
            }
            catch (Exception)
            {
                DashboardAvailable = false;
            }
        }

        #region Sadece performans testi yapmak amacıyla oluşturulan metottur. Bu nedenle metot yorum satırında bırakıldı.
        /// <summary>
        /// AES algoritmasının farklı modlarda performans testi için kullanılmıştır!!!
        /// </summary>
        //private void PeformansTesti()
        //{
        //    try
        //    {
        //        TimeSpan ts = new TimeSpan();
        //        ts = TimeSpan.FromMilliseconds(1);
        //        System.DateTime dtime = System.DateTime.Now;
        //        //string[] tables = { "SIFRESIZ_TABLO", "ECB_TEK_SIFRELEME", "CBC_TYPE1_TEK_SIFRELEME_NOHMAC", "CFB_TYPE4_TEK_SIFRELEME", "TEK_ADIMLI_DB_SIFRELEME", "ECB_CIFT_SIFRELEME", "CBC_CIFT_SIFRELEME", "CFB_CIFT_SIFRELEME" };
        //        //string[] tables = { "SIFRESIZ_TABLO", "ECB_TEK_SIFRELEME", "CBC_TYPE1_TEK_SIFRELEME_NOHMAC", "CFB_TYPE4_TEK_SIFRELEME" };

        //        data.Open();

        //        for (int i = 0; i < tables.Length; i++)
        //        {
        //            performansiTestEt(Convert.ToString(tables[i]));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        data.Dispose();
        //    }

        //}

        //private void performansiTestEt(string tablo)
        //{
        //    try
        //    {

        //        string qry = "";
        //        string[] indexDizi = null;
        //        string tblAd = tablo;

        //        string dbKey = AppConfig.DbKeyName;
        //        string dbKeyPwd = AppConfig.DbKeyPassword;

        //        if (tablo == "ECB_TEK_SIFRELEME" || tablo == "CBC_TYPE1_TEK_SIFRELEME_NOHMAC" || tablo == "CFB_TYPE4_TEK_SIFRELEME" || tablo == "ECB_CIFT_SIFRELEME" || tablo == "CBC_CIFT_SIFRELEME" || tablo == "CFB_CIFT_SIFRELEME" || tablo == "TEK_ADIMLI_DB_SIFRELEME")
        //        {
        //            qry = @"SELECT KOLONADI FROM TBL_SIFRELIKOLONLAR WHERE TBLAD=@TBLAD";
        //            data.AddParameter("@TBLAD", tblAd);
        //            DataTable dtSifreliKolonlar = data.QueryTable(qry);

        //            string sifreliKolonIndex = "";
        //            foreach (DataRow item in dtSifreliKolonlar.Rows)
        //            {
        //                qry = @"SELECT ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS    
        //                        WHERE TABLE_NAME=@TBLAD AND  COLUMN_NAME=@KOLON";
        //                data.AddParameter("@TBLAD", tblAd);
        //                data.AddParameter("@KOLON", Convert.ToString(item["KOLONADI"]));
        //                string index = Convert.ToString(data.QueryScalar(qry));
        //                sifreliKolonIndex += index + "|";
        //            }
        //            sifreliKolonIndex = sifreliKolonIndex.TrimEnd('|');

        //            indexDizi = sifreliKolonIndex.Split('|');
        //        }

        //        //Performans testi: https://www.cozumpark.com/c-performans-optimizasyonu-bolum-1-donguler/
        //        Stopwatch sw = new Stopwatch();
        //        sw.Start();

        //        string filtre = ""; // Aes256Encrypter.Encrypt(Convert.ToString("ÖĞRETMEN"), "<AES_KEY_32_CHARS>");
        //        if (tablo == "SIFRESIZ_TABLO" || tablo == "ECB_TEK_SIFRELEME" || tablo == "CBC_TYPE1_TEK_SIFRELEME_NOHMAC" || tablo == "CFB_TYPE4_TEK_SIFRELEME")
        //        {
        //            qry = @"SELECT KAYITID,AD,SOYAD,CINSIYET,YAS,MESLEK,DOGUM_YERI
        //                FROM " + tblAd;
        //        }
        //        else
        //        {

        //            qry = @"SELECT KAYITID,
        //                CONVERT(VARCHAR(600),DECRYPTBYASYMKEY(ASYMKEY_ID('<ASYM_KEY_NAME>'),AD,N'<ASYM_KEY_PASSWORD>')) AD,
        //                CONVERT(VARCHAR(600),DECRYPTBYASYMKEY(ASYMKEY_ID('<ASYM_KEY_NAME>'),SOYAD,N'<ASYM_KEY_PASSWORD>')) SOYAD,
        //                CONVERT(VARCHAR(600),DECRYPTBYASYMKEY(ASYMKEY_ID('<ASYM_KEY_NAME>'),CINSIYET,N'<ASYM_KEY_PASSWORD>')) CINSIYET,
        //                CONVERT(VARCHAR(600),DECRYPTBYASYMKEY(ASYMKEY_ID('<ASYM_KEY_NAME>'),YAS,N'<ASYM_KEY_PASSWORD>')) YAS,
        //                CONVERT(VARCHAR(600),DECRYPTBYASYMKEY(ASYMKEY_ID('<ASYM_KEY_NAME>'),MESLEK,N'<ASYM_KEY_PASSWORD>')) MESLEK,
        //                CONVERT(VARCHAR(600),DECRYPTBYASYMKEY(ASYMKEY_ID('<ASYM_KEY_NAME>'),DOGUM_YERI,N'<ASYM_KEY_PASSWORD>')) DOGUM_YERI
        //                FROM " + tblAd; //+ " WHERE CINSIYET=CONVERT(varchar(600),ENCRYPTBYASYMKEY(ASYMKEY_ID('<ASYM_KEY_NAME>'),N'" + filtre + "'))";
        //        }
        //        dtDatas = data.QueryTable(qry);

        //        string htmlText;
        //        htmlText = "<table style='border: 1px solid lightGray; margin-bottom:0px;' id= 'table-id' class='table table-striped table-class'>";
        //        htmlText += " <thead><tr>";

        //        string AD1 = "", AD2 = "", AD3 = "", AD4 = "", AD5 = "", AD6 = "", AD7 = "", AD8 = "", SOYAD1 = "", SOYAD2 = "", SOYAD3 = "", SOYAD4 = "", SOYAD5 = "", SOYAD6 = "", SOYAD7 = "", SOYAD8 = "";
        //        foreach (DataRow item in dtDatas.Rows)
        //        {
        //            if (tablo == "SIFRESIZ_TABLO")
        //            {
        //                AD1 = Convert.ToString(item["AD"]);
        //                SOYAD1 = Convert.ToString(item["SOYAD"]);
        //                string CINSIYET1 = Convert.ToString(item["CINSIYET"]);
        //                string YAS1 = Convert.ToString(item["YAS"]);
        //                string MESLEK1 = Convert.ToString(item["MESLEK"]);
        //                string DOGUM_YERI1 = Convert.ToString(item["DOGUM_YERI"]);
        //            }
        //            else if (tablo == "ECB_TEK_SIFRELEME")
        //            {
        //                AD2 = Aes256Encrypter.Decrypt(Convert.ToString(item["AD"]), "<AES_KEY_32_CHARS>", "ECB");
        //                SOYAD2 = Aes256Encrypter.Decrypt(Convert.ToString(item["SOYAD"]), "<AES_KEY_32_CHARS>", "ECB");
        //                string CINSIYET = Aes256Encrypter.Decrypt(Convert.ToString(item["CINSIYET"]), "<AES_KEY_32_CHARS>", "ECB");
        //                string YAS = Aes256Encrypter.Decrypt(Convert.ToString(item["YAS"]), "<AES_KEY_32_CHARS>", "ECB");
        //                string MESLEK = Aes256Encrypter.Decrypt(Convert.ToString(item["MESLEK"]), "<AES_KEY_32_CHARS>", "ECB");
        //                string DOGUM_YERI = Aes256Encrypter.Decrypt(Convert.ToString(item["DOGUM_YERI"]), "<AES_KEY_32_CHARS>", "ECB");
        //            }
        //            else if (tablo == "CBC_TYPE1_TEK_SIFRELEME_NOHMAC")
        //            {
        //                AD3 = Aes256Encrypter.Decrypt(Convert.ToString(item["AD"]), "<AES_KEY_32_CHARS>", "CBC");
        //                SOYAD3 = Aes256Encrypter.Decrypt(Convert.ToString(item["SOYAD"]), "<AES_KEY_32_CHARS>", "CBC");
        //                string CINSIYET = Aes256Encrypter.Decrypt(Convert.ToString(item["CINSIYET"]), "<AES_KEY_32_CHARS>", "CBC");
        //                string YAS = Aes256Encrypter.Decrypt(Convert.ToString(item["YAS"]), "<AES_KEY_32_CHARS>", "CBC");
        //                string MESLEK = Aes256Encrypter.Decrypt(Convert.ToString(item["MESLEK"]), "<AES_KEY_32_CHARS>", "CBC");
        //                string DOGUM_YERI = Aes256Encrypter.Decrypt(Convert.ToString(item["DOGUM_YERI"]), "<AES_KEY_32_CHARS>", "CBC");
        //            }
        //            else if (tablo == "CFB_TYPE4_TEK_SIFRELEME")
        //            {
        //                AD4 = Aes256Encrypter.Decrypt(Convert.ToString(item["AD"]), "<AES_KEY_32_CHARS>", "CFB");
        //                SOYAD4 = Aes256Encrypter.Decrypt(Convert.ToString(item["SOYAD"]), "<AES_KEY_32_CHARS>", "CFB");
        //                string CINSIYET = Aes256Encrypter.Decrypt(Convert.ToString(item["CINSIYET"]), "<AES_KEY_32_CHARS>", "CFB");
        //                string YAS = Aes256Encrypter.Decrypt(Convert.ToString(item["YAS"]), "<AES_KEY_32_CHARS>", "CFB");
        //                string MESLEK = Aes256Encrypter.Decrypt(Convert.ToString(item["MESLEK"]), "<AES_KEY_32_CHARS>", "CFB");
        //                string DOGUM_YERI = Aes256Encrypter.Decrypt(Convert.ToString(item["DOGUM_YERI"]), "<AES_KEY_32_CHARS>", "CFB");
        //            }
        //            else if (tablo == "ECB_CIFT_SIFRELEME")
        //            {
        //                AD5 = Aes256Encrypter.Decrypt(Convert.ToString(item["AD"]), "<AES_KEY_32_CHARS>", "ECB");
        //                SOYAD5 = Aes256Encrypter.Decrypt(Convert.ToString(item["SOYAD"]), "<AES_KEY_32_CHARS>", "ECB");
        //                string CINSIYET = Aes256Encrypter.Decrypt(Convert.ToString(item["CINSIYET"]), "<AES_KEY_32_CHARS>", "ECB");
        //                string YAS = Aes256Encrypter.Decrypt(Convert.ToString(item["YAS"]), "<AES_KEY_32_CHARS>", "ECB");
        //                string MESLEK = Aes256Encrypter.Decrypt(Convert.ToString(item["MESLEK"]), "<AES_KEY_32_CHARS>", "ECB");
        //                string DOGUM_YERI = Aes256Encrypter.Decrypt(Convert.ToString(item["DOGUM_YERI"]), "<AES_KEY_32_CHARS>", "ECB");
        //            }
        //            else if (tablo == "CBC_CIFT_SIFRELEME")
        //            {
        //                AD6 = Aes256Encrypter.Decrypt(Convert.ToString(item["AD"]), "<AES_KEY_32_CHARS>", "CBC");
        //                SOYAD6 = Aes256Encrypter.Decrypt(Convert.ToString(item["SOYAD"]), "<AES_KEY_32_CHARS>", "CBC");
        //                string CINSIYET = Aes256Encrypter.Decrypt(Convert.ToString(item["CINSIYET"]), "<AES_KEY_32_CHARS>", "CBC");
        //                string YAS = Aes256Encrypter.Decrypt(Convert.ToString(item["YAS"]), "<AES_KEY_32_CHARS>", "CBC");
        //                string MESLEK = Aes256Encrypter.Decrypt(Convert.ToString(item["MESLEK"]), "<AES_KEY_32_CHARS>", "CBC");
        //                string DOGUM_YERI = Aes256Encrypter.Decrypt(Convert.ToString(item["DOGUM_YERI"]), "<AES_KEY_32_CHARS>", "CBC");
        //            }
        //            else if (tablo == "CFB_CIFT_SIFRELEME")
        //            {
        //                AD7 = Aes256Encrypter.Decrypt(Convert.ToString(item["AD"]), "<AES_KEY_32_CHARS>", "CFB");
        //                SOYAD7 = Aes256Encrypter.Decrypt(Convert.ToString(item["SOYAD"]), "<AES_KEY_32_CHARS>", "CFB");
        //                string CINSIYET = Aes256Encrypter.Decrypt(Convert.ToString(item["CINSIYET"]), "<AES_KEY_32_CHARS>", "CFB");
        //                string YAS = Aes256Encrypter.Decrypt(Convert.ToString(item["YAS"]), "<AES_KEY_32_CHARS>", "CFB");
        //                string MESLEK = Aes256Encrypter.Decrypt(Convert.ToString(item["MESLEK"]), "<AES_KEY_32_CHARS>", "CFB");
        //                string DOGUM_YERI = Aes256Encrypter.Decrypt(Convert.ToString(item["DOGUM_YERI"]), "<AES_KEY_32_CHARS>", "CFB");
        //            }
        //            else if (tablo == "TEK_ADIMLI_DB_SIFRELEME")
        //            {
        //                AD8 = Convert.ToString(item["AD"]);
        //                SOYAD8 = Convert.ToString(item["SOYAD"]);

        //            }

        //        }

        //        sw.Stop();

        //        if (tablo == "SIFRESIZ_TABLO")
        //            lbSifresiz.Text = "AD: " + Convert.ToString(sw.Elapsed) + " - " + AD1 + SOYAD1;
        //        if (tablo == "ECB_TEK_SIFRELEME")
        //            lbEcbTek.Text = Convert.ToString(sw.Elapsed) + " - " + AD2 + SOYAD2;
        //        if (tablo == "CBC_TYPE1_TEK_SIFRELEME_NOHMAC")
        //            lbCbcTek.Text = Convert.ToString(sw.Elapsed) + " - " + AD3 + SOYAD3;
        //        if (tablo == "CFB_TYPE4_TEK_SIFRELEME")
        //            lbCfbTek.Text = Convert.ToString(sw.Elapsed) + " - " + AD4 + SOYAD4;
        //        if (tablo == "ECB_CIFT_SIFRELEME")
        //            lbEcbCift.Text = Convert.ToString(sw.Elapsed) + " - " + AD5 + SOYAD5;
        //        if (tablo == "CBC_CIFT_SIFRELEME")
        //            lbCbcCift.Text = Convert.ToString(sw.Elapsed) + " - " + AD6 + SOYAD6;
        //        if (tablo == "CFB_CIFT_SIFRELEME")
        //            lbCfbCift.Text = Convert.ToString(sw.Elapsed) + " - " + AD7 + SOYAD7;
        //        if (tablo == "TEK_ADIMLI_DB_SIFRELEME")
        //            lbTekAdimDb.Text = Convert.ToString(sw.Elapsed) + " - " + AD8 + SOYAD8;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion
    }
}