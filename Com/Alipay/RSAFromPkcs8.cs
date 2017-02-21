namespace Com.Alipay
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public sealed class RSAFromPkcs8
    {
        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            int index = 0;
            foreach (byte num2 in a)
            {
                if (num2 != b[index])
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        {
            byte[] sourceArray = Convert.FromBase64String(pemFileConent);
            if (sourceArray.Length < 0x261)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            int sourceIndex = 11;
            byte[] destinationArray = new byte[0x80];
            Array.Copy(sourceArray, sourceIndex, destinationArray, 0, 0x80);
            sourceIndex += 0x80;
            sourceIndex += 2;
            byte[] buffer3 = new byte[3];
            Array.Copy(sourceArray, sourceIndex, buffer3, 0, 3);
            sourceIndex += 3;
            sourceIndex += 4;
            byte[] buffer4 = new byte[0x80];
            Array.Copy(sourceArray, sourceIndex, buffer4, 0, 0x80);
            sourceIndex += 0x80;
            sourceIndex += (sourceArray[sourceIndex + 1] != 0x40) ? 3 : 2;
            byte[] buffer5 = new byte[0x40];
            Array.Copy(sourceArray, sourceIndex, buffer5, 0, 0x40);
            sourceIndex += 0x40;
            sourceIndex += (sourceArray[sourceIndex + 1] != 0x40) ? 3 : 2;
            byte[] buffer6 = new byte[0x40];
            Array.Copy(sourceArray, sourceIndex, buffer6, 0, 0x40);
            sourceIndex += 0x40;
            sourceIndex += (sourceArray[sourceIndex + 1] != 0x40) ? 3 : 2;
            byte[] buffer7 = new byte[0x40];
            Array.Copy(sourceArray, sourceIndex, buffer7, 0, 0x40);
            sourceIndex += 0x40;
            sourceIndex += (sourceArray[sourceIndex + 1] != 0x40) ? 3 : 2;
            byte[] buffer8 = new byte[0x40];
            Array.Copy(sourceArray, sourceIndex, buffer8, 0, 0x40);
            sourceIndex += 0x40;
            sourceIndex += (sourceArray[sourceIndex + 1] != 0x40) ? 3 : 2;
            byte[] buffer9 = new byte[0x40];
            Array.Copy(sourceArray, sourceIndex, buffer9, 0, 0x40);
            return new RSAParameters { Modulus = destinationArray, Exponent = buffer3, D = buffer4, P = buffer5, Q = buffer6, DP = buffer7, DQ = buffer8, InverseQ = buffer9 };
        }

        private static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {
            byte[] sourceArray = Convert.FromBase64String(pemFileConent);
            if (sourceArray.Length < 0xa2)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] destinationArray = new byte[0x80];
            byte[] buffer3 = new byte[3];
            Array.Copy(sourceArray, 0x1d, destinationArray, 0, 0x80);
            Array.Copy(sourceArray, 0x9f, buffer3, 0, 3);
            return new RSAParameters { Modulus = destinationArray, Exponent = buffer3 };
        }

        private static RSACryptoServiceProvider DecodePemPrivateKey(string pemstr)
        {
            byte[] buffer = Convert.FromBase64String(pemstr);
            if (buffer != null)
            {
                return DecodePrivateKeyInfo(buffer);
            }
            return null;
        }

        private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            RSACryptoServiceProvider provider2;
            byte[] b = new byte[] { 0x30, 13, 6, 9, 0x2a, 0x86, 0x48, 0x86, 0xf7, 13, 1, 1, 1, 5, 0, 0 };
            byte[] buffer2 = new byte[15];
            MemoryStream input = new MemoryStream(pkcs8);
            int length = (int) input.Length;
            BinaryReader reader = new BinaryReader(input);
            try
            {
                switch (reader.ReadUInt16())
                {
                    case 0x8130:
                        reader.ReadByte();
                        break;

                    case 0x8230:
                        reader.ReadInt16();
                        break;

                    default:
                        return null;
                }
                if (reader.ReadByte() != 2)
                {
                    return null;
                }
                if (reader.ReadUInt16() != 1)
                {
                    return null;
                }
                if (!CompareBytearrays(reader.ReadBytes(15), b))
                {
                    return null;
                }
                if (reader.ReadByte() != 4)
                {
                    return null;
                }
                switch (reader.ReadByte())
                {
                    case 0x81:
                        reader.ReadByte();
                        break;

                    case 130:
                        reader.ReadUInt16();
                        break;
                }
                provider2 = DecodeRSAPrivateKey(reader.ReadBytes(length - ((int) input.Position)));
            }
            catch (Exception)
            {
                provider2 = null;
            }
            finally
            {
                reader.Close();
            }
            return provider2;
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            RSACryptoServiceProvider provider2;
            MemoryStream input = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(input);
            int count = 0;
            try
            {
                switch (binr.ReadUInt16())
                {
                    case 0x8130:
                        binr.ReadByte();
                        break;

                    case 0x8230:
                        binr.ReadInt16();
                        break;

                    default:
                        return null;
                }
                if (binr.ReadUInt16() != 0x102)
                {
                    return null;
                }
                if (binr.ReadByte() != 0)
                {
                    return null;
                }
                count = GetIntegerSize(binr);
                byte[] buffer = binr.ReadBytes(count);
                count = GetIntegerSize(binr);
                byte[] buffer2 = binr.ReadBytes(count);
                count = GetIntegerSize(binr);
                byte[] buffer3 = binr.ReadBytes(count);
                count = GetIntegerSize(binr);
                byte[] buffer4 = binr.ReadBytes(count);
                count = GetIntegerSize(binr);
                byte[] buffer5 = binr.ReadBytes(count);
                count = GetIntegerSize(binr);
                byte[] buffer6 = binr.ReadBytes(count);
                count = GetIntegerSize(binr);
                byte[] buffer7 = binr.ReadBytes(count);
                count = GetIntegerSize(binr);
                byte[] buffer8 = binr.ReadBytes(count);
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                RSAParameters parameters = new RSAParameters {
                    Modulus = buffer,
                    Exponent = buffer2,
                    D = buffer3,
                    P = buffer4,
                    Q = buffer5,
                    DP = buffer6,
                    DQ = buffer7,
                    InverseQ = buffer8
                };
                provider.ImportParameters(parameters);
                provider2 = provider;
            }
            catch (Exception)
            {
                provider2 = null;
            }
            finally
            {
                binr.Close();
            }
            return provider2;
        }

        private static byte[] decrypt(byte[] data, string privateKey, string input_charset)
        {
            return DecodePemPrivateKey(privateKey).Decrypt(data, false);
        }

        public static string decryptData(string resData, string privateKey, string input_charset)
        {
            byte[] buffer = Convert.FromBase64String(resData);
            List<byte> list = new List<byte>();
            for (int i = 0; i < (buffer.Length / 0x80); i++)
            {
                byte[] data = new byte[0x80];
                for (int j = 0; j < 0x80; j++)
                {
                    data[j] = buffer[j + (0x80 * i)];
                }
                list.AddRange(decrypt(data, privateKey, input_charset));
            }
            byte[] bytes = list.ToArray();
            char[] chars = new char[Encoding.GetEncoding(input_charset).GetCharCount(bytes, 0, bytes.Length)];
            Encoding.GetEncoding(input_charset).GetChars(bytes, 0, bytes.Length, chars, 0);
            return new string(chars);
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte num = 0;
            byte num2 = 0;
            byte num3 = 0;
            int num4 = 0;
            if (binr.ReadByte() != 2)
            {
                return 0;
            }
            num = binr.ReadByte();
            switch (num)
            {
                case 0x81:
                    num4 = binr.ReadByte();
                    break;

                case 130:
                {
                    num3 = binr.ReadByte();
                    num2 = binr.ReadByte();
                    byte[] buffer1 = new byte[4];
                    buffer1[0] = num2;
                    buffer1[1] = num3;
                    byte[] buffer = buffer1;
                    num4 = BitConverter.ToInt32(buffer, 0);
                    break;
                }
                default:
                    num4 = num;
                    break;
            }
            while (binr.ReadByte() == 0)
            {
                num4--;
            }
            binr.get_BaseStream().Seek(-1L, SeekOrigin.Current);
            return num4;
        }

        public static string sign(string content, string privateKey, string input_charset)
        {
            byte[] bytes = Encoding.GetEncoding(input_charset).GetBytes(content);
            RSACryptoServiceProvider provider = DecodePemPrivateKey(privateKey);
            SHA1 sha = new SHA1CryptoServiceProvider();
            return Convert.ToBase64String(provider.SignData(bytes, sha));
        }

        public static bool verify(string content, string signedString, string publicKey, string input_charset)
        {
            byte[] bytes = Encoding.GetEncoding(input_charset).GetBytes(content);
            byte[] buffer2 = Convert.FromBase64String(signedString);
            RSAParameters parameters = ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.ImportParameters(parameters);
            SHA1 sha = new SHA1CryptoServiceProvider();
            return provider.VerifyData(bytes, sha, buffer2);
        }
    }
}

