namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FacePartControl
    {
        private IFaceMatInfoProvider _faceMatInfoProvider;
        private Renderer _facePartRenderer;
        private string[] _frameNames;
        private FaceMatInfo _originFaceMatInfo = new FaceMatInfo();

        public string[] GetFrameNames()
        {
            return this._frameNames;
        }

        public int GetMaxIndex()
        {
            return this._faceMatInfoProvider.capacity;
        }

        public void Init(IFaceMatInfoProvider provider, Renderer part)
        {
            this._faceMatInfoProvider = provider;
            this._facePartRenderer = part;
            FaceMatInfo info = new FaceMatInfo {
                texture = part.material.mainTexture as Texture2D,
                tile = part.material.mainTextureScale,
                offset = part.material.mainTextureOffset
            };
            this._originFaceMatInfo = info;
            string[] matInfoNames = provider.GetMatInfoNames();
            this._frameNames = new string[matInfoNames.Length + 1];
            this._frameNames[0] = "origin";
            int index = 0;
            int length = matInfoNames.Length;
            while (index < length)
            {
                this._frameNames[index + 1] = matInfoNames[index];
                index++;
            }
        }

        public void Reset()
        {
            this._facePartRenderer.material.mainTexture = this._originFaceMatInfo.texture;
            this._facePartRenderer.material.mainTextureScale = this._originFaceMatInfo.tile;
            this._facePartRenderer.material.mainTextureOffset = this._originFaceMatInfo.offset;
        }

        public void SetFacePartIndex(int index)
        {
            if ((this._faceMatInfoProvider != null) && (this._facePartRenderer != null))
            {
                FaceMatInfo info = (index != 0) ? this._faceMatInfoProvider.GetFaceMatInfo(index - 1) : this._originFaceMatInfo;
                if (info.valid)
                {
                    this._facePartRenderer.material.mainTexture = info.texture;
                    this._facePartRenderer.material.mainTextureScale = info.tile;
                    this._facePartRenderer.material.mainTextureOffset = info.offset;
                }
            }
        }
    }
}

