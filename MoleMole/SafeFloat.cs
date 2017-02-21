namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeFloat : IComparable, IComparable<SafeFloat>, IEquatable<SafeFloat>
    {
        [SerializeField]
        private int _value;
        public unsafe SafeFloat(float value)
        {
            uint num = *((uint*) &value);
            this._value = (int) (((num & 0xffff0000) ^ (((num & 0xffff) ^ 0xcafe) << 0x10)) + ((num & 0xffff) ^ 0xcafe));
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
        public float Value
        {
            get
            {
                long num = ((this._value & ((long) 0xffff0000L)) ^ (this._value << 0x10)) + ((this._value & 0xffff) ^ 0xcafe);
                return *(((float*) &num));
            }
            set
            {
                uint num = *((uint*) &value);
                this._value = (int) (((num & 0xffff0000) ^ (((num & 0xffff) ^ 0xcafe) << 0x10)) + ((num & 0xffff) ^ 0xcafe));
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
            if ((obj != null) && (obj is SafeFloat))
            {
                SafeFloat num = (SafeFloat) obj;
                return (this._value == num._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeFloat))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeFloat num2 = (SafeFloat) obj;
            return this.Value.CompareTo(num2.Value);
        }

        public int CompareTo(SafeFloat other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeFloat other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeFloat(float v)
        {
            return new SafeFloat(v);
        }

        public static implicit operator float(SafeFloat v)
        {
            return v.Value;
        }

        public static explicit operator double(SafeFloat v)
        {
            return (double) v.Value;
        }

        public static explicit operator int(SafeFloat v)
        {
            return (int) v.Value;
        }

        public static explicit operator uint(SafeFloat v)
        {
            return (uint) v.Value;
        }

        public static SafeFloat operator ++(SafeFloat a)
        {
            return new SafeFloat(a.Value + 1f);
        }

        public static SafeFloat operator --(SafeFloat a)
        {
            return new SafeFloat(a.Value - 1f);
        }
    }
}

