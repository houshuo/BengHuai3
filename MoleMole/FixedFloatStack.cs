namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class FixedFloatStack : FixedStack<float>
    {
        private float _ceiling;
        private float _floor;
        private Action _updateValue;
        private float _value;
        private const int DEFAULT_FLOAT_STACK_CAPACITY = 12;

        public FixedFloatStack(float initial, StackMethod valueType, float floor, float ceiling, Action<float, int, float, int> onChanged = null) : base(12, onChanged)
        {
            switch (valueType)
            {
                case StackMethod.Top:
                    base._checkAnyValueChange = false;
                    this._updateValue = new Action(this.UpdateTop);
                    break;

                case StackMethod.Sum:
                    base._checkAnyValueChange = true;
                    this._updateValue = new Action(this.UpdateSummed);
                    break;

                case StackMethod.Multiplied:
                    base._checkAnyValueChange = true;
                    this._updateValue = new Action(this.UpdateMultiplied);
                    break;

                case StackMethod.OneMinusMultiplied:
                    base._checkAnyValueChange = true;
                    initial = 1f - initial;
                    this._updateValue = new Action(this.UpdateOneMinusMultiplied);
                    break;
            }
            this._floor = floor;
            this._ceiling = ceiling;
            base.onChanged = (Action<float, int, float, int>) Delegate.Combine(base.onChanged, new Action<float, int, float, int>(this.SelfOnChanged));
            base.Push(initial, true);
        }

        public static FixedFloatStack CreateDefault(float initValue, StackMethod stackMethod, float floor, float ceiling, Action<float, int, float, int> onChanged = null)
        {
            return new FixedFloatStack(initValue, stackMethod, floor, ceiling, onChanged);
        }

        private void SelfOnChanged(float oldValue, int oldIx, float newValue, int newIx)
        {
            this._updateValue();
            this._value = Mathf.Clamp(this._value, this._floor, this._ceiling);
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

        public override float value
        {
            get
            {
                return this._value;
            }
        }

        public enum StackMethod
        {
            Top,
            Sum,
            Multiplied,
            OneMinusMultiplied
        }
    }
}

