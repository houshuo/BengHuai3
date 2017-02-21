namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("{Value}")]
    public struct SafeBool : IComparable, IComparable<SafeBool>, IEquatable<SafeBool>
    {
        [SerializeField]
        private sbyte _value;
        public SafeBool(bool value)
        {
            this._value = (sbyte) ((!value ? 0 : 1) ^ 0xcf);
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
        public bool Value
        {
            get
            {
                return (((sbyte) (this._value ^ 0xcf)) != 0);
            }
            set
            {
                this._value = (sbyte) ((!value ? 0 : ((this._value + 1) | 1)) ^ 0xcf);
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
            if ((obj != null) && (obj is SafeBool))
            {
                SafeBool @bool = (SafeBool) obj;
                return (this.Value == @bool.Value);
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SafeBool))
            {
                throw new ArgumentException("Invalid type!");
            }
            SafeBool @bool = (SafeBool) obj;
            return this.Value.CompareTo(@bool.Value);
        }

        public int CompareTo(SafeBool other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(SafeBool other)
        {
            return (this.Value == other.Value);
        }

        public static implicit operator SafeBool(bool v)
        {
            return new SafeBool(v);
        }

        public static implicit operator bool(SafeBool v)
        {
            return v.Value;
        }
    }
}

