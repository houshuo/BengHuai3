namespace MoleMole
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class SimpleLRU<T> where T: class
    {
        private T[] _arr;
        public int count;

        public SimpleLRU(int count)
        {
            this._arr = new T[count];
            this.count = count;
        }

        public void Clear()
        {
            for (int i = 0; i < this.count; i++)
            {
                this._arr[i] = null;
            }
        }

        public void MarkClear(int ix)
        {
            this._arr[ix] = null;
        }

        public void Rebuild()
        {
            T[] localArray = new T[this.count];
            int num = 0;
            for (int i = 0; i < this.count; i++)
            {
                if (this._arr[i] != null)
                {
                    localArray[num++] = this._arr[i];
                }
            }
            this._arr = localArray;
        }

        public void Touch(T entry, out T outdated)
        {
            int index = -1;
            for (int i = 0; i < this.count; i++)
            {
                if (this._arr[i] == null)
                {
                    index = i;
                    break;
                }
                if (this._arr[i] == entry)
                {
                    outdated = null;
                    for (int j = i; j >= 1; j--)
                    {
                        this._arr[j] = this._arr[j - 1];
                    }
                    this._arr[0] = entry;
                    return;
                }
            }
            if (index != -1)
            {
                this._arr[index] = entry;
                outdated = null;
            }
            else
            {
                outdated = this._arr[this.count - 1];
                for (int k = this.count - 1; k >= 1; k--)
                {
                    this._arr[k] = this._arr[k - 1];
                }
                this._arr[0] = entry;
            }
        }

        public T this[int ix]
        {
            get
            {
                return this._arr[ix];
            }
        }
    }
}

