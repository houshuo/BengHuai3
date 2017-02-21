namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeUInt8 : IComparable, IComparable<SafeUInt8>, IEquatable<SafeUInt8>
    {
        [SerializeField]
        private sbyte _value;
        public SafeUInt8(byte value)
        {
            this._value = (sbyte) (value ^ 0xcf);
        }

        public sbyte EncryptedValue
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
        public byte Value
        {
            get
            {
                return (byte) (this._value ^ 0xcf);
            }
            set
            {
                this._value = (sbyte) (value ^ 0xcf);
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
            if ((obj != null) && (obj is SafeUInt8))
            {
                SafeUInt8 num = (SafeUInt8) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeUInt8))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeUInt8 num2 = (SafeUInt8) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeUInt8 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeUInt8 other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeUInt8(byte v)
        {
            return new SafeUInt8(v);
        }

        public static implicit operator byte(SafeUInt8 v)
        {
            return v.Value;
        }

        public static SafeUInt8 operator ++(SafeUInt8 a)
        {
            return new SafeUInt8((byte) (a.Value + 1));
        }

        public static SafeUInt8 operator --(SafeUInt8 a)
        {
            return new SafeUInt8((byte) (a.Value - 1));
        }
    }
}

