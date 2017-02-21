namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using UnityEngine;

    [Serializable]
    public class SharedSafeFloat : SharedFloat
    {
        [SerializeField]
        private SafeFloat mSafeValue = 0f;

        public override object GetValue()
        {
            return this.Value;
        }

        public static implicit operator SharedSafeFloat(float value)
        {
            return new SharedSafeFloat { mSafeValue = value };
        }

        public override void SetValue(object value)
        {
            this.mSafeValue = (float) value;
        }

        public override string ToString()
        {
            return this.mSafeValue.ToString();
        }

        public override float Value
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

