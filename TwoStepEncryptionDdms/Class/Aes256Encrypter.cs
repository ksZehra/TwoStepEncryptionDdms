using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.IO;

namespace TwoStepEncryptionDdms.Security
{
    public class Aes256Encrypter
    {
        public static readonly Encoding encoding = Encoding.UTF8;

        static string _encryptType = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ECRYPTTYPE"]);
        static string _mode = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ENCRYPTMODE"]);

        public static string Encrypt(string plainText, string key)
        {
            try
            {
                string isBkEncrypt = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ISBKENCRYPT"]);
                if (isBkEncrypt != "1") { throw new Exception("'ISBKENCRYPT' şifreleme parametresi pasif görünmektedir! Lüften parametreleri kontrol ediniz."); }

                if (_encryptType == "CBC")
                {
                    //https://gist.github.com/doncadavona/fd493b6ced456371da8879c22bb1c263

                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new Exception("Key-Anahtar bilgisi boş!");
                    }

                    if (!string.IsNullOrWhiteSpace(plainText))
                    {
                        RijndaelManaged aes = new RijndaelManaged();
                        aes.KeySize = 256;
                        aes.BlockSize = 128;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.Mode = CipherMode.CBC;
                        aes.Key = encoding.GetBytes(key);

                        aes.GenerateIV(); //Her seferinde rastgele IV değeri oluşturacak
                        //aes.IV = encoding.GetBytes("s7jRoj49C4+oRMR="); //16 byte'lık statik bir iv tanımladık. "GenerateIV" kullanıdığımızda her kaydın iv değeri farklı olduğu için filtrelemede sorun oluşması üzerinde static sabit bir iv belirlenmiştir 

                        ICryptoTransform AESEncrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                        byte[] buffer = encoding.GetBytes(plainText);

                        string encryptedText = Convert.ToBase64String(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));

                        var keyValues = new Dictionary<string, object>
                {
                    { "iv", Convert.ToBase64String(aes.IV) },
                    { "value", encryptedText },
                };

                        JavaScriptSerializer serializer = new JavaScriptSerializer();

                        return Convert.ToBase64String(encoding.GetBytes(serializer.Serialize(keyValues)));
                    }
                    else
                    {
                        return "";
                    }
                }
                else if (_encryptType == "ECB")
                {
                    //https://rextester.com/CSGLMJ94072

                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new Exception("Key-Anahtar bilgisi boş!");
                    }

                    if (!string.IsNullOrWhiteSpace(plainText))
                    {
                        byte[] src = Encoding.UTF8.GetBytes(plainText);
                        byte[] keyByte = Encoding.ASCII.GetBytes(key);
                        RijndaelManaged aes = new RijndaelManaged();
                        aes.Mode = CipherMode.ECB;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.KeySize = 256;
                        aes.BlockSize = 128;
                        //ECB blok modelinde IV değeri kullanılmaz.

                        using (ICryptoTransform encrypt = aes.CreateEncryptor(keyByte, null))
                        {
                            byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
                            encrypt.Dispose();
                            return Convert.ToBase64String(dest);
                        }
                    }
                    else { return ""; }

                }
                else if (_encryptType == "CFB")
                {
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new Exception("Key-Anahtar bilgisi boş!");
                    }

                    if (!string.IsNullOrWhiteSpace(plainText))
                    {
                        byte[] encrypted;

                        //var vIv = encoding.GetBytes("s7jRoj49C4+oRMR=");

                        using (var rijndael = new RijndaelManaged())
                        {
                            rijndael.Mode = CipherMode.CFB;
                            rijndael.Padding = PaddingMode.PKCS7;
                            rijndael.KeySize = 256;
                            rijndael.BlockSize = 128;
                            rijndael.Key = encoding.GetBytes(key);
                            //rijndael.IV = encoding.GetBytes("s7jRoj49C4+oRMR=");                          
                            rijndael.GenerateIV(); //Her seferinde rastgele IV değeri oluşturacak bunu engellemek için yukarıdaki satırdaki gibi sabir bir IV değeri verilebilir ama sağlıklı olan her seferinde rastgele üretilmesidir.

                            using (var msEncrypt = new MemoryStream())
                            {
                                using (ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV))
                                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                                {
                                    var buffer = Encoding.UTF8.GetBytes(plainText);
                                    csEncrypt.Write(buffer, 0, buffer.Length);
                                }

                                encrypted = msEncrypt.ToArray();
                            }

                            var buffer2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(encrypted));

                            using (var ms = new MemoryStream(rijndael.IV.Length + buffer2.Length))
                            {
                                ms.Write(rijndael.IV, 0, rijndael.IV.Length);
                                ms.Write(buffer2, 0, buffer2.Length);

                                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
                            }
                        }
                    }
                    else { return ""; }
                }
                else if (_encryptType == "2")
                {
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new Exception("Key-Anahtar bilgisi boş!");
                    }

                    if (!string.IsNullOrWhiteSpace(plainText))
                    {
                        using (Aes aesAlgorithm = Aes.Create())
                        {
                            if (_mode == "ECB") { aesAlgorithm.Mode = CipherMode.ECB; }
                            if (_mode == "CBC") { aesAlgorithm.Mode = CipherMode.CBC; }
                            if (_mode == "CFB") { aesAlgorithm.Mode = CipherMode.CFB; }

                            aesAlgorithm.Padding = PaddingMode.PKCS7;
                            aesAlgorithm.KeySize = 256;
                            aesAlgorithm.BlockSize = 128;

                            aesAlgorithm.Key = encoding.GetBytes(key);
                            aesAlgorithm.IV = encoding.GetBytes("s7jRoj49C4+oRMR="); //ECB'de iv kullanılmaz

                            ////set the parameters with out keyword
                            //string keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
                            //string vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);

                            // Create encryptor object
                            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

                            byte[] encryptedData;

                            //Encryption will be done in a memory stream through a CryptoStream object
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                                {
                                    using (StreamWriter sw = new StreamWriter(cs))
                                    {
                                        sw.Write(plainText);
                                    }
                                    encryptedData = ms.ToArray();
                                }
                            }

                            return Convert.ToBase64String(encryptedData);
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new Exception("Şİfreleme türü parametresi (ENCRYPTMODE) eksik olduğu için şifreleme yapılamadı!");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error encrypting: " + e.Message);
            }
        }

        public static string Decrypt(string cipherText, string key, string mode = "")
        {
            try
            {
                string isBkEncrypt = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ISBKENCRYPT"]);
                if (isBkEncrypt != "1") { throw new Exception("'ISBKENCRYPT' şifreleme parametresi pasif görünmektedir! Lüften parametreleri kontrol ediniz."); }

                _encryptType = !string.IsNullOrWhiteSpace(mode) ? mode : _encryptType;
                if (_encryptType == "CBC")
                {
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new Exception("Key bilgisi boş!");
                    }

                    if (!string.IsNullOrWhiteSpace(cipherText))
                    {
                        RijndaelManaged aes = new RijndaelManaged();
                        aes.KeySize = 256;
                        aes.BlockSize = 128;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.Mode = CipherMode.CBC;
                        aes.Key = encoding.GetBytes(key);

                        // Base 64 decode
                        byte[] base64Decoded = Convert.FromBase64String(cipherText);
                        string base64DecodedStr = encoding.GetString(base64Decoded);

                        // JSON Decode base64Str
                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        var payload = ser.Deserialize<Dictionary<string, string>>(base64DecodedStr);

                        aes.IV = Convert.FromBase64String(payload["iv"]); //encoding.GetBytes("s7jRoj49C4+oRMR="); sabit iv değeri olması halinde bu komut kullanılır

                        ICryptoTransform AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);
                        byte[] buffer = Convert.FromBase64String(payload["value"]);

                        return encoding.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                    }
                    else
                    {
                        return "";
                    }
                }
                else if (_encryptType == "ECB") //ECB MODE
                {
                    //https://rextester.com/CSGLMJ94072


                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new Exception("Key bilgisi boş!");
                    }

                    if (!string.IsNullOrWhiteSpace(cipherText))
                    {
                        byte[] src = Convert.FromBase64String(cipherText);
                        RijndaelManaged aes = new RijndaelManaged();
                        byte[] keyByte = Encoding.ASCII.GetBytes(key);
                        aes.KeySize = 256;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.Mode = CipherMode.ECB;
                        using (ICryptoTransform decrypt = aes.CreateDecryptor(keyByte, null))
                        {
                            byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                            decrypt.Dispose();
                            return Encoding.UTF8.GetString(dest);
                        }
                    }
                    else { return ""; }
                }
                else if (_encryptType == "CFB")
                {

                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new Exception("Key bilgisi boş!");
                    }

                    if (!string.IsNullOrWhiteSpace(cipherText))
                    {
                        byte[] bytes = Convert.FromBase64String(cipherText);

                        var vIv = bytes;
                        //var vIv = encoding.GetBytes("s7jRoj49C4+oRMR="); sabit iv kullanılması halinde bu komut açılır
                        var vKey = encoding.GetBytes(key); //BytesToLowerHexBytes(encoding.GetBytes(key));

                        Array.Resize(ref vIv, 16);

                        byte[] decrypted;

                        using (var rijndael = new RijndaelManaged())
                        {
                            rijndael.Mode = CipherMode.CFB;
                            rijndael.Padding = PaddingMode.PKCS7;
                            rijndael.KeySize = 256;
                            rijndael.BlockSize = 128;

                            using (var msDecrypt = new MemoryStream())
                            {
                                using (ICryptoTransform decryptor = rijndael.CreateDecryptor(vKey, vIv))
                                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                                {
                                    var buffer = Convert.FromBase64String(Encoding.UTF8.GetString(bytes, 16, bytes.Length - 16));
                                    csDecrypt.Write(buffer, 0, buffer.Length);
                                }

                                decrypted = msDecrypt.ToArray();
                            }
                        }

                        return Encoding.UTF8.GetString(decrypted);
                    }
                    else { return ""; }
                }
                else if (_encryptType == "2")
                {
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new Exception("Key bilgisi boş!");
                    }

                    if (!string.IsNullOrWhiteSpace(cipherText))
                    {
                        using (Aes aesAlgorithm = Aes.Create())
                        {
                            if (_mode == "ECB") { aesAlgorithm.Mode = CipherMode.ECB; }
                            if (_mode == "CBC") { aesAlgorithm.Mode = CipherMode.CBC; }
                            if (_mode == "CFB") { aesAlgorithm.Mode = CipherMode.CFB; }

                            aesAlgorithm.Padding = PaddingMode.PKCS7;
                            aesAlgorithm.KeySize = 256;
                            aesAlgorithm.BlockSize = 128;

                            aesAlgorithm.Key = encoding.GetBytes(key);
                            aesAlgorithm.IV = encoding.GetBytes("s7jRoj49C4+oRMR="); //ECB'de IV kullanılmaz

                            ////set the parameters with out keyword
                            //string keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
                            //string vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);

                            // Create decryptor object
                            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

                            byte[] cipher = Convert.FromBase64String(cipherText);

                            //Decryption will be done in a memory stream through a CryptoStream object
                            using (MemoryStream ms = new MemoryStream(cipher))
                            {
                                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                                {
                                    using (StreamReader sr = new StreamReader(cs))
                                    {
                                        return sr.ReadToEnd();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new Exception("Şfreleme türü parametresi (ENCRYPTMODE) eksik olduğu için şifreleme yapılamadı!");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error decrypting: " + e.Message);
            }
        }
    }
}