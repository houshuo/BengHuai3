namespace MoleMole
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class SecurityUtil
    {
        public static void AESDecrypted(ref byte[] bytes, byte[] key, byte[] iv)
        {
            try
            {
                using (Rijndael rijndael = Rijndael.Create())
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, rijndael.CreateDecryptor(key, iv), 1))
                        {
                            stream2.Write(bytes, 0, bytes.Length);
                            stream2.FlushFinalBlock();
                            bytes = stream.ToArray();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void AESEncrypted(ref byte[] bytes, byte[] key, byte[] iv)
        {
            try
            {
                using (Rijndael rijndael = Rijndael.Create())
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, rijndael.CreateEncryptor(key, iv), 1))
                        {
                            stream2.Write(bytes, 0, bytes.Length);
                            stream2.FlushFinalBlock();
                            bytes = stream.ToArray();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static string Base64Decoder(string strToDecode)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            try
            {
                return encoding.GetString(Convert.FromBase64String(strToDecode));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string Base64Encoder(string strToEncode)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            try
            {
                return Convert.ToBase64String(encoding.GetBytes(strToEncode));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string CalculateFileHash(byte[] fileBytes, byte[] hmacKey)
        {
            try
            {
                string str = string.Empty;
                using (HMACSHA1 hmacsha = new HMACSHA1(hmacKey))
                {
                    byte[] buffer = hmacsha.ComputeHash(fileBytes);
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        str = str + buffer[i].ToString("X").PadLeft(2, '0');
                    }
                }
                return str;
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        public static string Md5(string strToEncrypt)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return Md5(encoding.GetBytes(strToEncrypt));
        }

        public static string Md5(byte[] bytes)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string str = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                str = str + Convert.ToString(buffer[i], 0x10).PadLeft(2, '0');
            }
            return str.PadLeft(0x20, '0');
        }

        public static void RSADecrypted(ref byte[] bytes, string rsaPrivate)
        {
            try
            {
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
                {
                    provider.FromXmlString(rsaPrivate);
                    bytes = provider.Decrypt(bytes, false);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void RSAEncrypted(ref byte[] bytes, string rsaPublic)
        {
            try
            {
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
                {
                    provider.FromXmlString(rsaPublic);
                    bytes = provider.Encrypt(bytes, false);
                }
            }
            catch (Exception)
            {
            }
        }

        public static string SHA1(string strToEncrypt)
        {
            byte[] bytes = new UTF8Encoding().GetBytes(strToEncrypt);
            byte[] buffer2 = new SHA1CryptoServiceProvider().ComputeHash(bytes);
            string str = string.Empty;
            for (int i = 0; i < buffer2.Length; i++)
            {
                str = str + Convert.ToString(buffer2[i], 0x10).PadLeft(2, '0');
            }
            return str.PadLeft(0x20, '0');
        }

        public static string SHA256(string strToEncrypt)
        {
            byte[] bytes = new UTF8Encoding().GetBytes(strToEncrypt);
            byte[] buffer2 = new SHA256Managed().ComputeHash(bytes);
            string str = string.Empty;
            for (int i = 0; i < buffer2.Length; i++)
            {
                str = str + Convert.ToString(buffer2[i], 0x10).PadLeft(2, '0');
            }
            return str.PadLeft(0x20, '0');
        }

        public static string SHA256(byte[] bytes)
        {
            byte[] buffer = new SHA256Managed().ComputeHash(bytes);
            string str = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                str = str + Convert.ToString(buffer[i], 0x10).PadLeft(2, '0');
            }
            return str.PadLeft(0x20, '0');
        }
    }
}

