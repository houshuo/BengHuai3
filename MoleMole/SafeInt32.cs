namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeInt32 : IComparable, IComparable<SafeInt32>, IEquatable<SafeInt32>
    {
        [SerializeField]
        private int _value;
        public SafeInt32(int value)
        {
            this._value = ((value & -65536) ^ (((value & 0xffff) ^ 0xcafe) << 0x10)) + ((value & 0xffff) ^ 0xcafe);
        }

        public int EncryptedValue
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
        public int Value
        {
            get
            {
                return (((this._value & ((int) 0xffff0000L)) ^ (this._value << 0x10)) + ((this._value & 0xffff) ^ 0xcafe));
            }
            set
            {
                this._value = ((value & -65536) ^ (((value & 0xffff) ^ 0xcafe) << 0x10)) + ((value & 0xffff) ^ 0xcafe);
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
            if ((obj != null) && (obj is SafeInt32))
            {
                SafeInt32 num = (SafeInt32) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeInt32))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeInt32 num2 = (SafeInt32) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeInt32 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeInt32 other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeInt32(int v)
        {
            return new SafeInt32(v);
        }

        public static implicit operator int(SafeInt32 v)
        {
            return v.Value;
        }

        public static explicit operator uint(SafeInt32 v)
        {
            return (uint) v.Value;
        }

        public static explicit operator float(SafeInt32 v)
        {
            return (float) v.Value;
        }

        public static explicit operator double(SafeInt32 v)
        {
            return (double) v.Value;
        }

        public static SafeInt32 operator ++(SafeInt32 a)
        {
            return new SafeInt32(a.Value + 1);
        }

        public static SafeInt32 operator --(SafeInt32 a)
        {
            return new SafeInt32(a.Value - 1);
        }
    }
}

