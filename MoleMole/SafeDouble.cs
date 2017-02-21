namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeDouble : IComparable, IComparable<SafeDouble>, IEquatable<SafeDouble>
    {
        [SerializeField]
        private long _value;
        public unsafe SafeDouble(double value)
        {
            ulong num = *((ulong*) &value);
            this._value = (long) (((num & 18446744069414584320L) ^ (((num & 0xffffffffL) ^ 0xcafecafeL) << 0x20)) + ((num & 0xffffffffL) ^ 0xcafecafeL));
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
        public double Value
        {
            get
            {
                ulong num = ((ulong) ((this._value & -4294967296L) ^ (this._value << 0x20))) + ((((ulong) this._value) & 0xffffffffL) ^ 0xcafecafeL);
                return *(((double*) &num));
            }
            set
            {
                ulong num = *((ulong*) &value);
                this._value = (long) (((num & 18446744069414584320L) ^ (((num & 0xffffffffL) ^ 0xcafecafeL) << 0x20)) + ((num & 0xffffffffL) ^ 0xcafecafeL));
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
            if ((obj != null) && (obj is SafeDouble))
            {
                SafeDouble num = (SafeDouble) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeDouble))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeDouble num2 = (SafeDouble) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeDouble other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeDouble other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeDouble(double v)
        {
            return new SafeDouble(v);
        }

        public static implicit operator double(SafeDouble v)
        {
            return v.Value;
        }

        public static explicit operator float(SafeDouble v)
        {
            return (float) v.Value;
        }

        public static SafeDouble operator ++(SafeDouble a)
        {
            return new SafeDouble(a.Value + 1.0);
        }

        public static SafeDouble operator --(SafeDouble a)
        {
            return new SafeDouble(a.Value - 1.0);
        }
    }
}

