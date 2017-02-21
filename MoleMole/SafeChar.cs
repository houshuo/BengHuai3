namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeChar : IComparable, IComparable<SafeChar>, IEquatable<SafeChar>
    {
        [SerializeField]
        private char _value;
        public SafeChar(char value)
        {
            this._value = (char) (((value & 0xff00) ^ (((value & 0xff) ^ 0xcf) << 8)) + ((value & '\x00ff') ^ 0xcf));
        }

        public char EncryptedValue
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
        public char Value
        {
            get
            {
                return (char) (((this._value & 0xff00) ^ (this._value << 8)) + ((this._value & '\x00ff') ^ 0xcf));
            }
            set
            {
                this._value = (char) (((value & 0xff00) ^ (((value & 0xff) ^ 0xcf) << 8)) + ((value & '\x00ff') ^ 0xcf));
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
            if ((obj != null) && (obj is SafeChar))
            {
                SafeChar ch = (SafeChar) obj;
                return (this._value == ch._value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeChar))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeChar ch2 = (SafeChar) obj;
            return this.Value.CompareTo(ch2.Value);
        }

        public int CompareTo(SafeChar other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeChar other)
        {
            return (this._value == other._value);
        }

        public static implicit operator SafeChar(char v)
        {
            return new SafeChar(v);
        }

        public static implicit operator char(SafeChar v)
        {
            return v.Value;
        }

        public static SafeChar operator ++(SafeChar a)
        {
            return new SafeChar((char) (a.Value + '\x0001'));
        }

        public static SafeChar operator --(SafeChar a)
        {
            return new SafeChar((char) (a.Value - '\x0001'));
        }
    }
}

