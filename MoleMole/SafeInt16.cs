namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeInt16 : IComparable, IComparable<SafeInt16>, IEquatable<SafeInt16>
    {
        [SerializeField]
        private short _value;
        public SafeInt16(short value)
        {
            this._value = (short) (((((ushort) value) & 0xff00) ^ (((((ushort) value) & 0xff) ^ 0xcf) << 8)) + ((((ushort) value) & 0xff) ^ 0xcf));
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
        public short Value
        {
            get
            {
                return (short) (((this._value & 0xff00) ^ (this._value << 8)) + ((this._value & 0xff) ^ 0xcf));
            }
            set
            {
                this._value = (short) (((((ushort) value) & 0xff00) ^ (((((ushort) value) & 0xff) ^ 0xcf) << 8)) + ((((ushort) value) & 0xff) ^ 0xcf));
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
            if ((obj != null) && (obj is SafeInt16))
            {
                SafeInt16 num = (SafeInt16) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeInt16))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeInt16 num2 = (SafeInt16) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeInt16 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeInt16 other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeInt16(short v)
        {
            return new SafeInt16(v);
        }

        public static implicit operator short(SafeInt16 v)
        {
            return v.Value;
        }

        public static SafeInt16 operator ++(SafeInt16 a)
        {
            return new SafeInt16((short) (a.Value + 1));
        }

        public static SafeInt16 operator --(SafeInt16 a)
        {
            return new SafeInt16((short) (a.Value - 1));
        }
    }
}

