namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class RenderTextureWrapperPool
    {
        private List<RenderTextureWrapper> _availableList = new List<RenderTextureWrapper>();
        private List<RenderTextureWrapper> _usedList = new List<RenderTextureWrapper>();

        public RenderTextureWrapper GetItem()
        {
            RenderTextureWrapper wrapper;
            if (this._availableList.Count > 0)
            {
                wrapper = this._availableList[0];
                this._availableList.RemoveAt(0);
            }
            else
            {
                wrapper = new RenderTextureWrapper();
            }
            this._usedList.Add(wrapper);
            return wrapper;
        }

        public int GetUsedCount()
        {
            return this._usedList.Count;
        }

        public void ReleaseItem(RenderTextureWrapper item)
        {
            if (item != null)
            {
                item.__Release();
                this._usedList.Remove(item);
                this._availableList.Add(item);
            }
        }

        public List<RenderTextureWrapper> usedList
        {
            get
            {
                return this._usedList;
            }
        }
    }
}

