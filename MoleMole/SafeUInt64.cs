namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeUInt64 : IComparable, IComparable<SafeUInt64>, IEquatable<SafeUInt64>
    {
        [SerializeField]
        private long _value;
        public SafeUInt64(ulong value)
        {
            this._value = (long) (((value & 18446744069414584320L) ^ (((value & 0xffffffffL) ^ 0xcafecafeL) << 0x20)) + ((value & 0xffffffffL) ^ 0xcafecafeL));
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
        public ulong Value
        {
            get
            {
                return (((ulong) ((this._value & -4294967296L) ^ (this._value << 0x20))) + ((((ulong) this._value) & 0xffffffffL) ^ 0xcafecafeL));
            }
            set
            {
                this._value = (long) (((value & 18446744069414584320L) ^ (((value & 0xffffffffL) ^ 0xcafecafeL) << 0x20)) + ((value & 0xffffffffL) ^ 0xcafecafeL));
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
            if ((obj != null) && (obj is SafeUInt64))
            {
                SafeUInt64 num = (SafeUInt64) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeUInt64))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeUInt64 num2 = (SafeUInt64) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeUInt64 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeUInt64 other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeUInt64(ulong v)
        {
            return new SafeUInt64(v);
        }

        public static implicit operator ulong(SafeUInt64 v)
        {
            return v.Value;
        }

        public static SafeUInt64 operator ++(SafeUInt64 a)
        {
            return new SafeUInt64(a.Value + ((ulong) 1L));
        }

        public static SafeUInt64 operator --(SafeUInt64 a)
        {
            return new SafeUInt64(a.Value - ((ulong) 1L));
        }
    }
}

