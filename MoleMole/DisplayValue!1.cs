namespace MoleMole
{
    using System;

    public class DisplayValue<T> : IDisposable where T: IComparable
    {
        private T _ceiling;
        private Action<T, T> _changeDelegate;
        private T _floor;
        private T _value;

        public DisplayValue(T floor, T ceiling, T init)
        {
            this._floor = floor;
            this._ceiling = ceiling;
            this._value = init;
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

        public void SubAttach(Action<T, T> changeCallback, ref T curValue, ref T floor, ref T ceiling)
        {
            this._changeDelegate = (Action<T, T>) Delegate.Combine(this._changeDelegate, changeCallback);
            curValue = this._value;
            floor = this._floor;
            ceiling = this._ceiling;
        }

        public void SubDetach(Action<T, T> changeCalback)
        {
            this._changeDelegate = (Action<T, T>) Delegate.Remove(this._changeDelegate, changeCalback);
        }

        public T value
        {
            get
            {
                return this._value;
            }
        }
    }
}

