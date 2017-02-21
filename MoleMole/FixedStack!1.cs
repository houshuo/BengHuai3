namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Runtime.InteropServices;

    public class FixedStack<T>
    {
        protected bool _checkAnyValueChange;
        protected bool[] _occupied;
        protected int _pushTop;
        protected int _realTop;
        protected T[] _stack;
        protected const int DEFAULT_CAPACITY = 6;
        public Action<T, int, T, int> onChanged;

        public FixedStack(int capacity, Action<T, int, T, int> onChanged = null)
        {
            this._stack = new T[capacity];
            this._occupied = new bool[capacity];
            this._pushTop = -1;
            this._realTop = -1;
            this.onChanged = onChanged;
        }

        public static FixedStack<T> CreateDefault(T initValue, Action<T, int, T, int> onChanged = null)
        {
            FixedStack<T> stack = new FixedStack<T>(6, onChanged);
            stack.Push(initValue, true);
            return stack;
        }

        public T Get(int ix)
        {
            return this._stack[ix];
        }

        public int GetRealTopIndex()
        {
            return this._realTop;
        }

        public bool IsOccupied(int ix)
        {
            return this._occupied[ix];
        }

        private void OnChanged(T oldValue, int oldStackIx, T newValue, int newStackIx)
        {
            if (this.onChanged != null)
            {
                this.onChanged(oldValue, oldStackIx, newValue, newStackIx);
            }
        }

        public void Pop(int ix)
        {
            this._occupied[ix] = false;
            this.SeekPushPos(0);
            this.SeekRealTop(false);
        }

        public int Push(T value, bool silent = false)
        {
            this.SeekPushPos(0);
            this._stack[this._pushTop] = value;
            this._occupied[this._pushTop] = true;
            this.SeekRealTop(silent);
            return this._pushTop;
        }

        public void Push(int ix, T value, bool silent = false)
        {
            this._stack[ix] = value;
            this._occupied[ix] = true;
            this.SeekRealTop(silent);
        }

        public int PushAbove(int aboveIx, T value, bool silent = false)
        {
            this.SeekPushPos(aboveIx + 1);
            this._stack[this._pushTop] = value;
            this._occupied[this._pushTop] = true;
            this.SeekRealTop(silent);
            return this._pushTop;
        }

        private void SeekPushPos(int startAt = 0)
        {
            for (int i = startAt; i < this._stack.Length; i++)
            {
                if (!this._occupied[i])
                {
                    this._pushTop = i;
                    return;
                }
            }
        }

        private void SeekRealTop(bool silent = false)
        {
            int index = this._realTop;
            T oldValue = (index < 0) ? default(T) : this._stack[index];
            for (int i = this._stack.Length - 1; i >= 0; i--)
            {
                if (this._occupied[i])
                {
                    this._realTop = i;
                    if (!silent && (this._checkAnyValueChange || (index != this._realTop)))
                    {
                        this.OnChanged(oldValue, index, this._stack[this._realTop], this._realTop);
                    }
                    return;
                }
            }
            if (!silent || this._checkAnyValueChange)
            {
                this.OnChanged(oldValue, index, default(T), -1);
            }
        }

        public void Set(int ix, T value, bool silent = false)
        {
            T oldValue = this._stack[ix];
            this._stack[ix] = value;
            if (!silent && (this._checkAnyValueChange || (ix == this._realTop)))
            {
                this.OnChanged(oldValue, ix, value, ix);
            }
        }

        public void TryPop(int ix)
        {
            if (this._occupied[ix])
            {
                this.Pop(ix);
            }
        }

        [ShowInInspector]
        public virtual T value
        {
            get
            {
                return this._stack[this._realTop];
            }
        }
    }
}

