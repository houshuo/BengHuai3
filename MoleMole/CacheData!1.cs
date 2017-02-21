namespace MoleMole
{
    using System;

    public class CacheData<T>
    {
        private DateTime _lastUpdateTime;
        private T _value;

        public CacheData()
        {
        }

        public CacheData(T value)
        {
            this._value = value;
            this._lastUpdateTime = TimeUtil.Now;
        }

        public bool CacheValid
        {
            get
            {
                return (this._lastUpdateTime.ToString("MM/dd/yyyy HH") == TimeUtil.Now.ToString("MM/dd/yyyy HH"));
            }
        }

        public T Value
        {
            get
            {
                return (!this.CacheValid ? default(T) : this._value);
            }
            set
            {
                this._lastUpdateTime = TimeUtil.Now;
                this._value = value;
            }
        }
    }
}

