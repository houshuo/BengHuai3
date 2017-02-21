namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AtlasMatInfoProvider : BaseScriptableObject, IFaceMatInfoProvider
    {
        private int _refCnt;
        private List<Texture2D> _texturesToUnload = new List<Texture2D>();
        public string basePath;
        public AtlasItem[] items;

        private void CalcTileAndOffset(Vector4 rect, out Vector2 tile, out Vector2 offset)
        {
            tile = new Vector2(rect.z, rect.w);
            offset = new Vector2(rect.x, rect.y);
        }

        private void ClearTextureCache()
        {
            int num = 0;
            int count = this._texturesToUnload.Count;
            while (num < count)
            {
                Resources.UnloadAsset(this._texturesToUnload[num]);
                num++;
            }
            this._texturesToUnload.Clear();
        }

        public FaceMatInfo GetFaceMatInfo(int index)
        {
            if ((index < 0) || (index >= this.items.Length))
            {
                return new FaceMatInfo();
            }
            FaceMatInfo info = new FaceMatInfo();
            Texture2D item = Resources.Load<Texture2D>(string.Format("{0}/{1}", this.basePath, this.items[index].textureName));
            if (!this._texturesToUnload.Contains(item))
            {
                this._texturesToUnload.Add(item);
            }
            info.texture = item;
            this.CalcTileAndOffset(this.items[index].rect, out info.tile, out info.offset);
            return info;
        }

        public string[] GetMatInfoNames()
        {
            string[] strArray = new string[this.items.Length];
            int index = 0;
            int length = this.items.Length;
            while (index < length)
            {
                strArray[index] = this.items[index].name;
                index++;
            }
            return strArray;
        }

        public bool ReleaseReference()
        {
            this._refCnt--;
            if (this._refCnt <= 0)
            {
                this.ClearTextureCache();
                return true;
            }
            return false;
        }

        public void RetainReference()
        {
            this._refCnt++;
        }

        public int capacity
        {
            get
            {
                return this.items.Length;
            }
        }
    }
}

