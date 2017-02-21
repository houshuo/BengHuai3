namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeUInt16 : IComparable, IComparable<SafeUInt16>, IEquatable<SafeUInt16>
    {
        [SerializeField]
        private short _value;
        public SafeUInt16(ushort value)
        {
            this._value = (short) (((value & 0xff00) ^ (((value & 0xff) ^ 0xcf) << 8)) + ((value & 0xff) ^ 0xcf));
        }

        public short EncryptedValue
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
        public ushort Value
        {
            get
            {
                return (ushort) (((this._value & 0xff00) ^ (this._value << 8)) + ((this._value & 0xff) ^ 0xcf));
            }
            set
            {
                this._value = (short) (((value & 0xff00) ^ (((value & 0xff) ^ 0xcf) << 8)) + ((value & 0xff) ^ 0xcf));
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
            if ((obj != null) && (obj is SafeUInt16))
            {
                SafeUInt16 num = (SafeUInt16) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeUInt16))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeUInt16 num2 = (SafeUInt16) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeUInt16 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeUInt16 other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeUInt16(ushort v)
        {
            return new SafeUInt16(v);
        }

        public static implicit operator ushort(SafeUInt16 v)
        {
            return v.Value;
        }

        public static SafeUInt16 operator ++(SafeUInt16 a)
        {
            return new SafeUInt16((ushort) (a.Value + 1));
        }

        public static SafeUInt16 operator --(SafeUInt16 a)
        {
            return new SafeUInt16((ushort) (a.Value - 1));
        }
    }
}

