namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public class MiHoYoGameDataV2
    {
        private byte[] _dataBytes;
        private int _dataLength;
        private byte[] _indexBytes;
        private int _indexLength;
        private static MiHoYoGameDataV2 _instance;
        private List<string> _keyIndex;
        private List<int> _lenIndex;
        private List<int> _posIndex;
        public const string PLAYERPREFS_KEY = "MGD_V2";
        private const byte XOR_CODE_DISK = 0x68;
        private const byte XOR_CODE_MEMORY = 0xa9;

        private MiHoYoGameDataV2(bool independent)
        {
            if (!independent)
            {
                if (!PlayerPrefs.HasKey("MGD_V2"))
                {
                    this.ResetData();
                }
                else
                {
                    this.Deserialize();
                }
            }
            else
            {
                this.ResetStructure();
            }
        }

        private void CheckThreadSafe()
        {
            PlayerPrefs.HasKey("TestThread");
        }

        public static MiHoYoGameDataV2 CreateIndepedentOne()
        {
            return new MiHoYoGameDataV2(true);
        }

        private void Deserialize()
        {
            this.CheckThreadSafe();
            string fullDataStr = PlayerPrefs.GetString("MGD_V2");
            this.DeserializeFromString(fullDataStr);
        }

        public void DeserializeFromString(string fullDataStr)
        {
            this.CheckThreadSafe();
            string s = fullDataStr.Substring(0x20, fullDataStr.Length - 0x20);
            string str2 = fullDataStr.Substring(0, 0x20);
            if (SecurityUtil.Md5(s + this.GetMD5MagicCode()) != str2)
            {
                PlayerPrefs.DeleteKey("MGD_V2");
                this.ResetData();
            }
            else
            {
                byte[] byteArray = Convert.FromBase64String(s);
                this.XORByteArrayDisk(byteArray);
                int startIndex = 0;
                this._indexLength = BitConverter.ToInt32(byteArray, startIndex);
                startIndex += Marshal.SizeOf(this._indexLength);
                this._indexBytes = new byte[this._indexLength];
                Array.Copy(byteArray, startIndex, this._indexBytes, 0, this._indexLength);
                startIndex += this._indexLength;
                this._dataLength = BitConverter.ToInt32(byteArray, startIndex);
                startIndex += Marshal.SizeOf(this._dataLength);
                this._dataBytes = new byte[this._dataLength];
                Array.Copy(byteArray, startIndex, this._dataBytes, 0, this._dataLength);
                startIndex += this._dataLength;
                this.DeserializeIndex();
            }
        }

        private void DeserializeIndex()
        {
            this.CheckThreadSafe();
            UTF8Encoding encoding = new UTF8Encoding();
            int startIndex = 0;
            int structure = BitConverter.ToInt32(this._indexBytes, startIndex);
            startIndex += Marshal.SizeOf(structure);
            this._keyIndex = new List<string>();
            this._posIndex = new List<int>();
            this._lenIndex = new List<int>();
            for (int i = 0; i < structure; i++)
            {
                int num4 = BitConverter.ToInt32(this._indexBytes, startIndex);
                startIndex += Marshal.SizeOf(num4);
                byte[] destinationArray = new byte[num4];
                Array.Copy(this._indexBytes, startIndex, destinationArray, 0, num4);
                startIndex += destinationArray.Length;
                this._keyIndex.Add(encoding.GetString(destinationArray));
                this._posIndex.Add(BitConverter.ToInt32(this._indexBytes, startIndex));
                startIndex += Marshal.SizeOf(this._posIndex[i]);
                this._lenIndex.Add(BitConverter.ToInt32(this._indexBytes, startIndex));
                startIndex += Marshal.SizeOf(this._lenIndex[i]);
            }
        }

        public float GetFloat(string key)
        {
            this.CheckThreadSafe();
            int num = -1;
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                if (this._keyIndex[i] == key)
                {
                    num = i;
                    break;
                }
            }
            if (num >= 0)
            {
                byte[] destinationArray = new byte[Marshal.SizeOf(typeof(float))];
                Array.Copy(this._dataBytes, this._posIndex[num], destinationArray, 0, this._lenIndex[num]);
                this.XORByteArrayMemory(destinationArray);
                return BitConverter.ToSingle(destinationArray, 0);
            }
            return 0f;
        }

        public static MiHoYoGameDataV2 GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MiHoYoGameDataV2(false);
            }
            if (_instance == null)
            {
                throw new Exception("MGD Singleton Null Error");
            }
            return _instance;
        }

        public int GetInt(string key)
        {
            this.CheckThreadSafe();
            int num = -1;
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                if (this._keyIndex[i] == key)
                {
                    num = i;
                    break;
                }
            }
            if (num >= 0)
            {
                byte[] destinationArray = new byte[Marshal.SizeOf(typeof(int))];
                Array.Copy(this._dataBytes, this._posIndex[num], destinationArray, 0, this._lenIndex[num]);
                this.XORByteArrayMemory(destinationArray);
                return BitConverter.ToInt32(destinationArray, 0);
            }
            return 0;
        }

        private string GetMD5MagicCode()
        {
            return "p]e!IO6SoR~d-BcBb^dTkVcx2015";
        }

        public string GetString(string key)
        {
            this.CheckThreadSafe();
            UTF8Encoding encoding = new UTF8Encoding();
            int num = -1;
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                if (this._keyIndex[i] == key)
                {
                    num = i;
                    break;
                }
            }
            if (num >= 0)
            {
                byte[] destinationArray = new byte[this._lenIndex[num]];
                Array.Copy(this._dataBytes, this._posIndex[num], destinationArray, 0, this._lenIndex[num]);
                this.XORByteArrayMemory(destinationArray);
                return encoding.GetString(destinationArray);
            }
            return string.Empty;
        }

        public bool HasKey(string key)
        {
            this.CheckThreadSafe();
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                if (this._keyIndex[i] == key)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveKey(string key)
        {
            this.CheckThreadSafe();
            int index = -1;
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                if (this._keyIndex[i] == key)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                int length = this._posIndex[index];
                int num4 = this._lenIndex[index];
                this._keyIndex.RemoveAt(index);
                for (int j = index + 1; j < this._posIndex.Count; j++)
                {
                    List<int> list;
                    int num7;
                    num7 = list[num7];
                    (list = this._posIndex)[num7 = j] = num7 - num4;
                }
                this._posIndex.RemoveAt(index);
                this._lenIndex.RemoveAt(index);
                int num6 = this._dataLength - num4;
                byte[] destinationArray = new byte[num6];
                Array.Copy(this._dataBytes, 0, destinationArray, 0, length);
                Array.Copy(this._dataBytes, length + num4, destinationArray, length, (this._dataLength - length) - num4);
                this._dataBytes = destinationArray;
                this._dataLength = num6;
            }
        }

        public void ResetData()
        {
            this.CheckThreadSafe();
            this.ResetStructure();
            this.Serialize();
            this.Deserialize();
        }

        private void ResetStructure()
        {
            this._keyIndex = new List<string>();
            this._posIndex = new List<int>();
            this._lenIndex = new List<int>();
            this._dataLength = 0;
            this._dataBytes = new byte[0];
        }

        public void Serialize()
        {
            this.CheckThreadSafe();
            PlayerPrefs.SetString("MGD_V2", this.SerializeToString());
            PlayerPrefs.Save();
        }

        private void SerializeIndex()
        {
            this.CheckThreadSafe();
            UTF8Encoding encoding = new UTF8Encoding();
            List<byte[]> list = new List<byte[]>();
            int num = 0;
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                list.Add(encoding.GetBytes(this._keyIndex[i]));
                num += list[i].Length;
            }
            this._indexLength = ((((Marshal.SizeOf(typeof(int)) * this._keyIndex.Count) + (Marshal.SizeOf(typeof(int)) * this._posIndex.Count)) + (Marshal.SizeOf(typeof(int)) * this._lenIndex.Count)) + num) + Marshal.SizeOf(typeof(int));
            this._indexBytes = new byte[this._indexLength];
            int index = 0;
            BitConverter.GetBytes(this._keyIndex.Count).CopyTo(this._indexBytes, index);
            index += Marshal.SizeOf(this._keyIndex.Count);
            for (int j = 0; j < this._keyIndex.Count; j++)
            {
                BitConverter.GetBytes(list[j].Length).CopyTo(this._indexBytes, index);
                index += Marshal.SizeOf(list[j].Length);
                Array.Copy(list[j], 0, this._indexBytes, index, list[j].Length);
                index += list[j].Length;
                BitConverter.GetBytes(this._posIndex[j]).CopyTo(this._indexBytes, index);
                index += Marshal.SizeOf(this._posIndex[j]);
                BitConverter.GetBytes(this._lenIndex[j]).CopyTo(this._indexBytes, index);
                index += Marshal.SizeOf(this._lenIndex[j]);
            }
        }

        public string SerializeToString()
        {
            this.CheckThreadSafe();
            this.SerializeIndex();
            int num = ((Marshal.SizeOf(this._indexLength) + this._indexBytes.Length) + Marshal.SizeOf(this._dataLength)) + this._dataBytes.Length;
            byte[] array = new byte[num];
            int index = 0;
            BitConverter.GetBytes(this._indexLength).CopyTo(array, index);
            index += Marshal.SizeOf(this._indexLength);
            Array.Copy(this._indexBytes, 0, array, index, this._indexLength);
            index += this._indexBytes.Length;
            BitConverter.GetBytes(this._dataLength).CopyTo(array, index);
            index += Marshal.SizeOf(this._dataLength);
            Array.Copy(this._dataBytes, 0, array, index, this._dataLength);
            index += this._dataBytes.Length;
            this.XORByteArrayDisk(array);
            string str = Convert.ToBase64String(array);
            return (SecurityUtil.Md5(str + this.GetMD5MagicCode()) + str);
        }

        public void SetFloat(string key, float value)
        {
            byte[] bytes;
            this.CheckThreadSafe();
            int num = -1;
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                if (this._keyIndex[i] == key)
                {
                    num = i;
                    break;
                }
            }
            if (num >= 0)
            {
                bytes = BitConverter.GetBytes(value);
                this.XORByteArrayMemory(bytes);
                bytes.CopyTo(this._dataBytes, this._posIndex[num]);
            }
            else
            {
                this._keyIndex.Add(key);
                this._posIndex.Add(this._dataLength);
                this._lenIndex.Add(Marshal.SizeOf(typeof(float)));
                int num3 = this._dataLength + Marshal.SizeOf(typeof(float));
                byte[] destinationArray = new byte[num3];
                Array.Copy(this._dataBytes, 0, destinationArray, 0, this._dataLength);
                bytes = BitConverter.GetBytes(value);
                this.XORByteArrayMemory(bytes);
                bytes.CopyTo(destinationArray, this._dataLength);
                this._dataLength = num3;
                this._dataBytes = destinationArray;
            }
        }

        public void SetInt(string key, int value)
        {
            byte[] bytes;
            this.CheckThreadSafe();
            int num = -1;
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                if (this._keyIndex[i] == key)
                {
                    num = i;
                    break;
                }
            }
            if (num >= 0)
            {
                bytes = BitConverter.GetBytes(value);
                this.XORByteArrayMemory(bytes);
                bytes.CopyTo(this._dataBytes, this._posIndex[num]);
            }
            else
            {
                this._keyIndex.Add(key);
                this._posIndex.Add(this._dataLength);
                this._lenIndex.Add(Marshal.SizeOf(typeof(int)));
                int num3 = this._dataLength + Marshal.SizeOf(typeof(int));
                byte[] destinationArray = new byte[num3];
                Array.Copy(this._dataBytes, 0, destinationArray, 0, this._dataLength);
                bytes = BitConverter.GetBytes(value);
                this.XORByteArrayMemory(bytes);
                bytes.CopyTo(destinationArray, this._dataLength);
                this._dataLength = num3;
                this._dataBytes = destinationArray;
            }
        }

        public void SetString(string key, string value)
        {
            byte[] bytes;
            int length;
            int num4;
            byte[] buffer2;
            this.CheckThreadSafe();
            UTF8Encoding encoding = new UTF8Encoding();
            int num = -1;
            for (int i = 0; i < this._keyIndex.Count; i++)
            {
                if (this._keyIndex[i] == key)
                {
                    num = i;
                    break;
                }
            }
            if (num >= 0)
            {
                bytes = encoding.GetBytes(value);
                this.XORByteArrayMemory(bytes);
                length = bytes.Length;
                int num5 = length - this._lenIndex[num];
                for (int j = num + 1; j < this._posIndex.Count; j++)
                {
                    List<int> list;
                    int num7;
                    num7 = list[num7];
                    (list = this._posIndex)[num7 = j] = num7 + num5;
                }
                num4 = this._dataLength + num5;
                buffer2 = new byte[num4];
                Array.Copy(this._dataBytes, 0, buffer2, 0, this._posIndex[num]);
                Array.Copy(bytes, 0, buffer2, this._posIndex[num], length);
                Array.Copy(this._dataBytes, (int) (this._posIndex[num] + this._lenIndex[num]), buffer2, (int) (this._posIndex[num] + length), (int) ((this._dataLength - this._posIndex[num]) - this._lenIndex[num]));
                this._lenIndex[num] = length;
                this._dataLength = num4;
                this._dataBytes = buffer2;
            }
            else
            {
                this._keyIndex.Add(key);
                this._posIndex.Add(this._dataLength);
                bytes = encoding.GetBytes(value);
                this.XORByteArrayMemory(bytes);
                length = bytes.Length;
                this._lenIndex.Add(length);
                num4 = this._dataLength + length;
                buffer2 = new byte[num4];
                Array.Copy(this._dataBytes, 0, buffer2, 0, this._dataLength);
                Array.Copy(bytes, 0, buffer2, this._dataLength, length);
                this._dataLength = num4;
                this._dataBytes = buffer2;
            }
        }

        private void XORByteArrayDisk(byte[] byteArray)
        {
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = (byte) (byteArray[i] ^ 0x68);
            }
        }

        private void XORByteArrayMemory(byte[] byteArray)
        {
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = (byte) (byteArray[i] ^ 0xa9);
            }
        }
    }
}

