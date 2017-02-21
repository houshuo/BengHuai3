namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class FixedSafeFloatStack : FixedStack<SafeFloat>
    {
        private float _ceiling;
        private float _floor;
        private Action _updateValue;
        private SafeFloat _value;
        private const int DEFAULT_FLOAT_STACK_CAPACITY = 12;

        public FixedSafeFloatStack(float initial, FixedFloatStack.StackMethod valueType, float floor, float ceiling, Action<SafeFloat, int, SafeFloat, int> onChanged = null) : base(12, onChanged)
        {
            this._value = 0f;
            switch (valueType)
            {
                case FixedFloatStack.StackMethod.Top:
                    base._checkAnyValueChange = false;
                    this._updateValue = new Action(this.UpdateTop);
                    break;

                case FixedFloatStack.StackMethod.Sum:
                    base._checkAnyValueChange = true;
                    this._updateValue = new Action(this.UpdateSummed);
                    break;

                case FixedFloatStack.StackMethod.Multiplied:
                    base._checkAnyValueChange = true;
                    this._updateValue = new Action(this.UpdateMultiplied);
                    break;

                case FixedFloatStack.StackMethod.OneMinusMultiplied:
                    base._checkAnyValueChange = true;
                    initial = 1f - initial;
                    this._updateValue = new Action(this.UpdateOneMinusMultiplied);
                    break;
            }
            this._floor = floor;
            this._ceiling = ceiling;
            base.onChanged = (Action<SafeFloat, int, SafeFloat, int>) Delegate.Combine(base.onChanged, new Action<SafeFloat, int, SafeFloat, int>(this.SelfOnChanged));
            base.Push(initial, true);
        }

        public static FixedSafeFloatStack CreateDefault(float initValue, FixedFloatStack.StackMethod stackMethod, float floor, float ceiling, Action<SafeFloat, int, SafeFloat, int> onChanged = null)
        {
            return new FixedSafeFloatStack(initValue, stackMethod, floor, ceiling, onChanged);
        }

        private void SelfOnChanged(SafeFloat oldValue, int oldIx, SafeFloat newValue, int newIx)
        {
            this._updateValue();
            this._value = Mathf.Clamp((float) this._value, this._floor, this._ceiling);
        }

        private void UpdateMultiplied()
        {
            this._value = 1f;
            for (int i = 0; i < base._stack.Length; i++)
            {
                if (base._occupied[i])
                {
                    this._value *= base._stack[i];
                }
            }
        }

        private void UpdateOneMinusMultiplied()
        {
            this._value = 1f;
            for (int i = 0; i < base._stack.Length; i++)
            {
                if (base._occupied[i])
                {
                    this._value *= 1f - base._stack[i];
                }
            }
        }

        private void UpdateSummed()
        {
            this._value = 0f;
            for (int i = 0; i < base._stack.Length; i++)
            {
                if (base._occupied[i])
                {
                    this._value += base._stack[i];
                }
            }
        }

        private void UpdateTop()
        {
            this._value = base.value;
        }

        public override SafeFloat value
        {
            get
            {
                return this._value;
            }
        }
    }
}

