namespace MoleMole
{
    using System;

    public static class SafeTypeUtil
    {
        public static bool DecryptBool(byte value)
        {
            return ((value ^ 0xcf) != 0);
        }

        public static unsafe double DecryptDouble(ulong value)
        {
            ushort* numPtr = (ushort*) &value;
            ushort* numPtr1 = numPtr + 2;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            ushort* numPtr2 = numPtr + 3;
            numPtr2[0] = (ushort) (numPtr2[0] ^ numPtr[1]);
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr3 = numPtr + 1;
            numPtr3[0] = (ushort) (numPtr3[0] ^ 0xcafe);
            return *(((double*) &value));
        }

        public static unsafe float DecryptFloat(uint value)
        {
            ushort* numPtr = (ushort*) &value;
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            return *(((float*) &value));
        }

        public static unsafe short DecryptInt16(ushort value)
        {
            byte* numPtr = (byte*) &value;
            byte* numPtr1 = numPtr + 1;
            numPtr1[0] = (byte) (numPtr1[0] ^ numPtr[0]);
            numPtr[0] = (byte) (numPtr[0] ^ 0xcf);
            return *(((short*) &value));
        }

        public static unsafe int DecryptInt32(uint value)
        {
            ushort* numPtr = (ushort*) &value;
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            return *(((int*) &value));
        }

        public static unsafe long DecryptInt64(ulong value)
        {
            ushort* numPtr = (ushort*) &value;
            ushort* numPtr1 = numPtr + 2;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            ushort* numPtr2 = numPtr + 3;
            numPtr2[0] = (ushort) (numPtr2[0] ^ numPtr[1]);
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr3 = numPtr + 1;
            numPtr3[0] = (ushort) (numPtr3[0] ^ 0xcafe);
            return *(((long*) &value));
        }

        public static sbyte DecryptInt8(byte value)
        {
            return (sbyte) (value ^ 0xcf);
        }

        public static unsafe ushort DecryptUInt16(ushort value)
        {
            byte* numPtr = (byte*) &value;
            byte* numPtr1 = numPtr + 1;
            numPtr1[0] = (byte) (numPtr1[0] ^ numPtr[0]);
            numPtr[0] = (byte) (numPtr[0] ^ 0xcf);
            return value;
        }

        public static unsafe uint DecryptUInt32(uint value)
        {
            ushort* numPtr = (ushort*) &value;
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            return value;
        }

        public static unsafe ulong DecryptUInt64(ulong value)
        {
            ushort* numPtr = (ushort*) &value;
            ushort* numPtr1 = numPtr + 2;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            ushort* numPtr2 = numPtr + 3;
            numPtr2[0] = (ushort) (numPtr2[0] ^ numPtr[1]);
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr3 = numPtr + 1;
            numPtr3[0] = (ushort) (numPtr3[0] ^ 0xcafe);
            return value;
        }

        public static byte DecryptUInt8(byte value)
        {
            return (byte) (value ^ 0xcf);
        }

        public static byte EncryptBool(bool value)
        {
            return (byte) ((!value ? 0 : 1) ^ 0xcf);
        }

        public static unsafe ulong EncryptDouble(double value)
        {
            ushort* numPtr = (ushort*) &value;
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ 0xcafe);
            ushort* numPtr2 = numPtr + 2;
            numPtr2[0] = (ushort) (numPtr2[0] ^ numPtr[0]);
            ushort* numPtr3 = numPtr + 3;
            numPtr3[0] = (ushort) (numPtr3[0] ^ numPtr[1]);
            return *(((ulong*) &value));
        }

        public static unsafe uint EncryptFloat(float value)
        {
            ushort* numPtr = (ushort*) &value;
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            return *(((uint*) &value));
        }

        public static unsafe ushort EncryptInt16(short value)
        {
            byte* numPtr = (byte*) &value;
            numPtr[0] = (byte) (numPtr[0] ^ 0xcf);
            byte* numPtr1 = numPtr + 1;
            numPtr1[0] = (byte) (numPtr1[0] ^ numPtr[0]);
            return *(((ushort*) &value));
        }

        public static unsafe uint EncryptInt32(int value)
        {
            ushort* numPtr = (ushort*) &value;
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            return *(((uint*) &value));
        }

        public static unsafe ulong EncryptInt64(long value)
        {
            ushort* numPtr = (ushort*) &value;
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ 0xcafe);
            ushort* numPtr2 = numPtr + 2;
            numPtr2[0] = (ushort) (numPtr2[0] ^ numPtr[0]);
            ushort* numPtr3 = numPtr + 3;
            numPtr3[0] = (ushort) (numPtr3[0] ^ numPtr[1]);
            return *(((ulong*) &value));
        }

        public static byte EncryptInt8(sbyte value)
        {
            return (byte) (value ^ 0xcf);
        }

        public static unsafe ushort EncryptUInt16(ushort value)
        {
            byte* numPtr = (byte*) &value;
            numPtr[0] = (byte) (numPtr[0] ^ 0xcf);
            byte* numPtr1 = numPtr + 1;
            numPtr1[0] = (byte) (numPtr1[0] ^ numPtr[0]);
            return value;
        }

        public static unsafe uint EncryptUInt32(uint value)
        {
            ushort* numPtr = (ushort*) &value;
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ numPtr[0]);
            return value;
        }

        public static unsafe ulong EncryptUInt64(ulong value)
        {
            ushort* numPtr = (ushort*) &value;
            numPtr[0] = (ushort) (numPtr[0] ^ 0xcafe);
            ushort* numPtr1 = numPtr + 1;
            numPtr1[0] = (ushort) (numPtr1[0] ^ 0xcafe);
            ushort* numPtr2 = numPtr + 2;
            numPtr2[0] = (ushort) (numPtr2[0] ^ numPtr[0]);
            ushort* numPtr3 = numPtr + 3;
            numPtr3[0] = (ushort) (numPtr3[0] ^ numPtr[1]);
            return value;
        }

        public static byte EncryptUInt8(byte value)
        {
            return (byte) (value ^ 0xcf);
        }
    }
}

