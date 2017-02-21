namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using UnityEngine;

    [Serializable]
    public class SharedSafeInt : SharedInt
    {
        [SerializeField]
        private SafeInt32 mSafeValue = 0;

        public override object GetValue()
        {
            return this.Value;
        }

        public static implicit operator SharedSafeInt(int value)
        {
            return new SharedSafeInt { mSafeValue = value };
        }

        public override void SetValue(object value)
        {
            this.mSafeValue = (int) value;
        }

        public override string ToString()
        {
            return this.mSafeValue.ToString();
        }

        public override int Value
        {
            get
            {
                return this.mSafeValue.Value;
            }
            set
            {
                bool flag = this.Value == value;
                this.mSafeValue = value;
                if (flag)
                {
                    base.ValueChanged();
                }
            }
        }
    }
}

