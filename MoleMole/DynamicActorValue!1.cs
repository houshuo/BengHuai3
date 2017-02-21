namespace MoleMole
{
    using System;

    public class DynamicActorValue<T> : IDisposable where T: IComparable
    {
        private Action<T, T> _changeDelegate;
        private T _value;

        public DynamicActorValue(T value)
        {
            this._value = value;
        }

        public void Dispose()
        {
            this._changeDelegate = null;
        }

        public void Pub(T newValue)
        {
            T local = this._value;
            this._value = newValue;
            if (this._changeDelegate != null)
            {
                this._changeDelegate(local, newValue);
            }
        }

        public void SubAttach(Action<T, T> changeCallback, ref T curValue)
        {
            this._changeDelegate = (Action<T, T>) Delegate.Combine(this._changeDelegate, changeCallback);
            curValue = this._value;
        }

        public void SubDetach(Action<T, T> changeCallback)
        {
            this._changeDelegate = (Action<T, T>) Delegate.Remove(this._changeDelegate, changeCallback);
        }

        public T Value
        {
            get
            {
                return this._value;
            }
        }
    }
}

