namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class AssetList
    {
        private List<AssetItem> _list = new List<AssetItem>();
        private int _max = 10;
        private SizeComparer _sizeComparer = new SizeComparer();

        public AssetList(int max = 10)
        {
            this._max = max;
        }

        public void Clear()
        {
            this._list.Clear();
        }

        public List<AssetItem> GetList()
        {
            return this._list;
        }

        public void Sort()
        {
            this._list.Sort(this._sizeComparer);
        }

        public void TryAdd(uint size, string name)
        {
            if (this._list.Count < this._max)
            {
                this._list.Add(new AssetItem(size, name));
            }
            else
            {
                this.Sort();
                AssetItem item = this._list[this._list.Count - 1];
                if (size > item._size)
                {
                    item._size = size;
                    item._name = name;
                }
            }
        }

        public class AssetItem
        {
            public string _name;
            public uint _size;

            public AssetItem(uint size, string name)
            {
                this._size = size;
                this._name = name;
            }
        }

        public class SizeComparer : IComparer<AssetList.AssetItem>
        {
            public int Compare(AssetList.AssetItem x, AssetList.AssetItem y)
            {
                if (x._size > y._size)
                {
                    return -1;
                }
                if (x._size < y._size)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

