namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeInt64 : IComparable, IComparable<SafeInt64>, IEquatable<SafeInt64>
    {
        [SerializeField]
        private long _value;
        public SafeInt64(long value)
        {
            this._value = (long) ((((ulong) (value & -4294967296L)) ^ (((value & 0xffffffffL) ^ 0xcafecafeL) << 0x20)) + ((((ulong) value) & 0xffffffffL) ^ 0xcafecafeL));
        }

        public long EncryptedValue
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }
        public long Value
        {
            get
            {
                return (((this._value & -4294967296L) ^ (this._value << 0x20)) + ((long) ((((ulong) this._value) & 0xffffffffL) ^ 0xcafecafeL)));
            }
            set
            {
                this._value = (long) ((((ulong) (value & -4294967296L)) ^ (((value & 0xffffffffL) ^ 0xcafecafeL) << 0x20)) + ((((ulong) value) & 0xffffffffL) ^ 0xcafecafeL));
            }
        }
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is SafeInt64))
            {
                SafeInt64 num = (SafeInt64) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeInt64))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeInt64 num2 = (SafeInt64) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeInt64 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeInt64 other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeInt64(long v)
        {
            return new SafeInt64(v);
        }

        public static implicit operator long(SafeInt64 v)
        {
            return v.Value;
        }

        public static SafeInt64 operator ++(SafeInt64 a)
        {
            return new SafeInt64(a.Value + 1L);
        }

        public static SafeInt64 operator --(SafeInt64 a)
        {
            return new SafeInt64(a.Value - 1L);
        }
    }
}

