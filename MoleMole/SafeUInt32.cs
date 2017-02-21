namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeUInt32 : IComparable, IComparable<SafeUInt32>, IEquatable<SafeUInt32>
    {
        [SerializeField]
        private int _value;
        public SafeUInt32(uint value)
        {
            this._value = (int) (((value & 0xffff0000) ^ (((value & 0xffff) ^ 0xcafe) << 0x10)) + ((value & 0xffff) ^ 0xcafe));
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
        public uint Value
        {
            get
            {
                return (uint) (((this._value & 0xffff0000L) ^ (this._value << 0x10)) + ((this._value & 0xffff) ^ 0xcafe));
            }
            set
            {
                this._value = (int) (((value & 0xffff0000) ^ (((value & 0xffff) ^ 0xcafe) << 0x10)) + ((value & 0xffff) ^ 0xcafe));
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
            if ((obj != null) && (obj is SafeUInt32))
            {
                SafeUInt32 num = (SafeUInt32) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeUInt32))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeUInt32 num2 = (SafeUInt32) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeUInt32 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeUInt32 other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeUInt32(uint v)
        {
            return new SafeUInt32(v);
        }

        public static implicit operator uint(SafeUInt32 v)
        {
            return v.Value;
        }

        public static explicit operator int(SafeUInt32 v)
        {
            return (int) v.Value;
        }

        public static explicit operator float(SafeUInt32 v)
        {
            return (float) v.Value;
        }

        public static explicit operator double(SafeUInt32 v)
        {
            return (double) v.Value;
        }

        public static SafeUInt32 operator ++(SafeUInt32 a)
        {
            return new SafeUInt32(a.Value + 1);
        }

        public static SafeUInt32 operator --(SafeUInt32 a)
        {
            return new SafeUInt32(a.Value - 1);
        }
    }
}

