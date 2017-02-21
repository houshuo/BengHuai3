namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class RenderTextureWrapper
    {
        private static readonly Dictionary<RenderTextureFormat, RenderTextureFormat[]> _alternativeFormatDict;
        private List<Camera> _cameraList = new List<Camera>();
        private Param _param;
        private RenderTexture _renderTexture;
        public Action onRebindToCameraCallBack;

        static RenderTextureWrapper()
        {
            Dictionary<RenderTextureFormat, RenderTextureFormat[]> dictionary = new Dictionary<RenderTextureFormat, RenderTextureFormat[]>();
            dictionary.Add(RenderTextureFormat.ARGBHalf, new RenderTextureFormat[1]);
            dictionary.Add(RenderTextureFormat.R8, new RenderTextureFormat[1]);
            _alternativeFormatDict = dictionary;
        }

        public void __Release()
        {
            if (this._renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(this._renderTexture);
                this._renderTexture = null;
            }
            for (int i = 0; i < this._cameraList.Count; i++)
            {
                if (this._cameraList[i] != null)
                {
                    this._cameraList[i].targetTexture = null;
                }
            }
            this._cameraList.Clear();
        }

        public bool BindToCamera(Camera camera)
        {
            if ((camera == null) || !this.IsValid())
            {
                return false;
            }
            camera.targetTexture = this._renderTexture;
            if (!this._cameraList.Contains(camera))
            {
                this._cameraList.Add(camera);
            }
            return true;
        }

        public void Create(Param param)
        {
            this._param = param;
            if (this._renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(this._renderTexture);
                this._renderTexture = null;
            }
            if (!GraphicsUtils.isDisableRenderTexture)
            {
                if (!SystemInfo.SupportsRenderTextureFormat(param.format))
                {
                    if (param.format == RenderTextureFormat.ARGBHalf)
                    {
                        param.format = RenderTextureFormat.ARGB32;
                    }
                    if (_alternativeFormatDict.ContainsKey(param.format))
                    {
                        RenderTextureFormat[] formatArray = _alternativeFormatDict[param.format];
                        for (int i = 0; i < formatArray.Length; i++)
                        {
                            if (SystemInfo.SupportsRenderTextureFormat(formatArray[i]))
                            {
                                param.format = formatArray[i];
                            }
                        }
                    }
                }
                this._renderTexture = RenderTexture.GetTemporary(param.width, param.height, param.depth, param.format, param.readWrite);
                if (this._renderTexture == null)
                {
                }
            }
        }

        public bool IsCreated()
        {
            return this._renderTexture.IsCreated();
        }

        public bool IsValid()
        {
            return (this._renderTexture != null);
        }

        public static implicit operator RenderTexture(RenderTextureWrapper wrapper)
        {
            return wrapper.content;
        }

        public void RebindToCamera()
        {
            for (int i = 0; i < this._cameraList.Count; i++)
            {
                if (this._cameraList[i] != null)
                {
                    this._cameraList[i].targetTexture = this._renderTexture;
                }
            }
            if (this.onRebindToCameraCallBack != null)
            {
                this.onRebindToCameraCallBack();
            }
        }

        public bool UnbindFromCamera(Camera camera)
        {
            if ((camera == null) || (camera.targetTexture != this._renderTexture))
            {
                return false;
            }
            camera.targetTexture = null;
            this._cameraList.Remove(camera);
            return true;
        }

        public RenderTexture content
        {
            get
            {
                return this._renderTexture;
            }
        }

        public int height
        {
            get
            {
                return this._renderTexture.height;
            }
        }

        public Param param
        {
            get
            {
                return this._param;
            }
        }

        public int width
        {
            get
            {
                return this._renderTexture.width;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Param
        {
            public int width;
            public int height;
            public int depth;
            public RenderTextureFormat format;
            public RenderTextureReadWrite readWrite;
        }
    }
}

