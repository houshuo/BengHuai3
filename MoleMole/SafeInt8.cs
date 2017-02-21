namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeInt8 : IComparable, IComparable<SafeInt8>, IEquatable<SafeInt8>
    {
        [SerializeField]
        private sbyte _value;
        public SafeInt8(sbyte value)
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
        public sbyte Value
        {
            get
            {
                return (sbyte) (this._value ^ 0xcf);
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
            if ((obj != null) && (obj is SafeInt8))
            {
                SafeInt8 num = (SafeInt8) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeInt8))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeInt8 num2 = (SafeInt8) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeInt8 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeInt8 other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeInt8(sbyte v)
        {
            return new SafeInt8(v);
        }

        public static implicit operator sbyte(SafeInt8 v)
        {
            return v.Value;
        }

        public static SafeInt8 operator ++(SafeInt8 a)
        {
            return new SafeInt8((sbyte) (a.Value + 1));
        }

        public static SafeInt8 operator --(SafeInt8 a)
        {
            return new SafeInt8((sbyte) (a.Value - 1));
        }
    }
}

