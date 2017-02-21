namespace MoleMole
{
    using System;

    public static class DelegateUtils
    {
        public static void UpdateField(ref SafeDouble field, double newValue, Action<double, double> updateDelegate)
        {
            double num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeFloat field, float newValue, Action<float, float> updateDelegate)
        {
            float num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeInt16 field, short newValue, Action<short, short> updateDelegate)
        {
            short num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeInt32 field, int newValue, Action<int, int> updateDelegate)
        {
            int num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeInt64 field, long newValue, Action<long, long> updateDelegate)
        {
            long num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeInt8 field, sbyte newValue, Action<sbyte, sbyte> updateDelegate)
        {
            sbyte num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeUInt16 field, ushort newValue, Action<ushort, ushort> updateDelegate)
        {
            ushort num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeUInt32 field, uint newValue, Action<uint, uint> updateDelegate)
        {
            uint num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeUInt64 field, ulong newValue, Action<ulong, ulong> updateDelegate)
        {
            ulong num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField(ref SafeUInt8 field, byte newValue, Action<byte, byte> updateDelegate)
        {
            byte num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue);
            }
        }

        public static void UpdateField<T>(ref T field, T newValue, Action<T, T> updateDelegate)
        {
            T local = field;
            field = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(local, field);
            }
        }

        public static void UpdateField(ref SafeDouble field, double newValue, double delta, Action<double, double, double> updateDelegate)
        {
            double num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeFloat field, float newValue, float delta, Action<float, float, float> updateDelegate)
        {
            float num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeInt16 field, short newValue, short delta, Action<short, short, short> updateDelegate)
        {
            short num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeInt32 field, int newValue, int delta, Action<int, int, int> updateDelegate)
        {
            int num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeInt64 field, long newValue, long delta, Action<long, long, long> updateDelegate)
        {
            long num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeInt8 field, sbyte newValue, sbyte delta, Action<sbyte, sbyte, sbyte> updateDelegate)
        {
            sbyte num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeUInt16 field, ushort newValue, ushort delta, Action<ushort, ushort, ushort> updateDelegate)
        {
            ushort num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeUInt32 field, uint newValue, uint delta, Action<uint, uint, uint> updateDelegate)
        {
            uint num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeUInt64 field, ulong newValue, ulong delta, Action<ulong, ulong, ulong> updateDelegate)
        {
            ulong num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField(ref SafeUInt8 field, byte newValue, byte delta, Action<byte, byte, byte> updateDelegate)
        {
            byte num = field.Value;
            field.Value = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(num, newValue, delta);
            }
        }

        public static void UpdateField<T>(ref T field, T newValue, T delta, Action<T, T, T> updateDelegate)
        {
            T local = field;
            field = newValue;
            if (updateDelegate != null)
            {
                updateDelegate(local, field, delta);
            }
        }
    }
}

