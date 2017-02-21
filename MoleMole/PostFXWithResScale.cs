namespace MoleMole
{
    using System;
    using System.IO;
    using UnityEngine;

    [AddComponentMenu("Image Effects/PostFX"), RequireComponent(typeof(Camera)), ExecuteInEditMode, DisallowMultipleComponent]
    public class PostFXWithResScale : PostFXBase
    {
        private Camera __innerCamera;
        private RenderTextureWrapper __rplCameraBuffer;
        private RenderTexture[] _cameraBufferList;
        private static readonly string _INNER_CAMERA_NAME = "InnerCamera";
        public Camera[] CameraList;
        [Tooltip("If not 0, set the scaled height directly ignoring the native res")]
        public int CameraResHeight;
        [Tooltip("The ratio(x100) of camera buffer res to screen")]
        public CAMERA_RES_SCALE CameraResScale = CAMERA_RES_SCALE.RES_100;
        [Tooltip("If not 0, set the scaled width directly ignoring the native res")]
        public int CameraResWidth;
        [HideInInspector]
        public int cullingMask;
        public bool distortionUseDepthOfInnerBuffer;
        private bool needSave;
        [Tooltip("Only do res scale but not other post fx functions")]
        public bool OnlyResScale;

        protected void BackupCameraBuffers()
        {
            if (this.CameraList != null)
            {
                this._cameraBufferList = new RenderTexture[this.CameraList.Length];
                for (int i = 0; i < this._cameraBufferList.Length; i++)
                {
                    this._cameraBufferList[i] = this.CameraList[i].targetTexture;
                }
            }
        }

        public static void CaptureRT(RenderTexture rt)
        {
            Texture2D textured = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            textured.ReadPixels(new Rect(0f, 0f, (float) rt.width, (float) rt.height), 0, 0);
            textured.Apply();
            byte[] bytes = textured.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
            UnityEngine.Object.Destroy(textured);
        }

        protected override void CreateDistortionMap(int w, int h)
        {
            if (this.distortionUseDepthOfInnerBuffer)
            {
                base.distortionMap = GraphicsUtils.GetRenderTexture(w, h, base.DistortionMapDepthBit, base.DistortionMapFormat);
            }
            else
            {
                base.distortionMap = GraphicsUtils.GetRenderTexture(w / 2, h / 2, base.DistortionMapDepthBit, base.DistortionMapFormat);
            }
        }

        protected override void OnDestroy()
        {
            if (this.__rplCameraBuffer != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this.__rplCameraBuffer);
                this.__rplCameraBuffer = null;
            }
            base.OnDestroy();
        }

        protected override void OnDisable()
        {
            base._camera.cullingMask = this.cullingMask;
            this.RestoreCameraBuffers();
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.cullingMask = base._camera.cullingMask;
            this.BackupCameraBuffers();
            this.ReplaceCameraBuffers();
        }

        public void OnPostRender()
        {
            RenderTexture source = (RenderTexture) this._rplCameraBuffer;
            RenderTexture targetTexture = base._camera.targetTexture;
            if (((this.CameraResScale == CAMERA_RES_SCALE.RES_400) && (this.CameraResWidth == 0)) && (this.CameraResHeight == 0))
            {
                RenderTextureWrapper wrapper = GraphicsUtils.GetRenderTexture(source.width, source.height, 0, base.internalBufferFormat);
                if (this.OnlyResScale)
                {
                    Graphics.Blit(source, (RenderTexture) wrapper);
                    base.CheckResources();
                }
                else
                {
                    base.DoPostProcess(source, (RenderTexture) wrapper);
                }
                base.DownSample4X.SetVector(PostFXBase.HASH_TEXEL_SIZE, new Vector2(1f / ((float) source.width), 1f / ((float) source.height)));
                Graphics.Blit((Texture) wrapper, targetTexture, base.DownSample4X, 0);
                GraphicsUtils.ReleaseRenderTexture(wrapper);
            }
            else if (this.OnlyResScale)
            {
                Graphics.Blit((Texture) this._rplCameraBuffer, targetTexture);
            }
            else
            {
                base.DoPostProcess((RenderTexture) this._rplCameraBuffer, targetTexture);
            }
        }

        public void OnPreCull()
        {
            if (Application.isPlaying)
            {
                base._camera.cullingMask = 0;
                base._camera.clearFlags = CameraClearFlags.Nothing;
            }
        }

        public override void OnPreRender()
        {
            if (Application.isPlaying)
            {
                base._camera.cullingMask = this.cullingMask;
                base._camera.clearFlags = CameraClearFlags.Color;
            }
            this._innerCamera.CopyFrom(base._camera);
            this._innerCamera.targetTexture = (RenderTexture) this._rplCameraBuffer;
            this._innerCamera.aspect = base._camera.aspect;
            if (base.WriteDepthTexture)
            {
                this._innerCamera.depthTextureMode = DepthTextureMode.Depth;
            }
            else
            {
                this._innerCamera.depthTextureMode = DepthTextureMode.None;
            }
            this._innerCamera.Render();
            this._innerCamera.targetTexture = null;
        }

        protected override void PrepareDistortion()
        {
            base._distortionCamera.CopyFrom(base._camera);
            base._distortionCamera.backgroundColor = base.DistortionMapClearColor;
            if (base.UseDepthTest)
            {
                if (this.distortionUseDepthOfInnerBuffer)
                {
                    RenderTexture active = RenderTexture.active;
                    RenderTexture.active = (RenderTexture) base.distortionMap;
                    GL.Clear(false, true, base._distortionCamera.backgroundColor);
                    RenderTexture.active = active;
                    base._distortionCamera.SetTargetBuffers(base.distortionMap.content.colorBuffer, this._rplCameraBuffer.content.depthBuffer);
                    base._distortionCamera.clearFlags = CameraClearFlags.Nothing;
                    base._distortionCamera.cullingMask = (int) base.DistortionCullingMask;
                    base._distortionCamera.RenderWithShader(base.DistortionMapNormShader, "Distortion");
                }
                else
                {
                    base._distortionCamera.SetTargetBuffers(base.distortionMap.content.colorBuffer, base.distortionMap.content.depthBuffer);
                    base._distortionCamera.clearFlags = CameraClearFlags.Color;
                    base._distortionCamera.cullingMask = (int) base.DepthCullingMask;
                    base._distortionCamera.RenderWithShader(base.DrawDepthShader, string.Empty);
                    base._distortionCamera.clearFlags = CameraClearFlags.Nothing;
                    base._distortionCamera.cullingMask = (int) base.DistortionCullingMask;
                    base._distortionCamera.RenderWithShader(base.DistortionMapNormShader, "Distortion");
                }
            }
            else
            {
                base._distortionCamera.SetTargetBuffers(base.distortionMap.content.colorBuffer, base.distortionMap.content.depthBuffer);
                base._distortionCamera.clearFlags = CameraClearFlags.Color;
                base._distortionCamera.cullingMask = (int) base.DistortionCullingMask;
                base._distortionCamera.RenderWithShader(base.DistortionMapNormShader, "Distortion");
            }
        }

        protected void ReplaceCameraBuffers()
        {
            if (this.CameraList != null)
            {
                foreach (Camera camera in this.CameraList)
                {
                    camera.targetTexture = (RenderTexture) this._rplCameraBuffer;
                }
            }
        }

        protected void RestoreCameraBuffers()
        {
            if (this.CameraList != null)
            {
                for (int i = 0; i < this.CameraList.Length; i++)
                {
                    if (i < this._cameraBufferList.Length)
                    {
                        this.CameraList[i].targetTexture = (RenderTexture) this._rplCameraBuffer;
                    }
                    else
                    {
                        this.CameraList[i].targetTexture = this._cameraBufferList[i];
                    }
                }
            }
        }

        protected override void Update()
        {
            base.Update();
        }

        private Camera _innerCamera
        {
            get
            {
                if (this.__innerCamera == null)
                {
                    foreach (Camera camera in base.GetComponentsInChildren<Camera>(true))
                    {
                        if (camera.name == _INNER_CAMERA_NAME)
                        {
                            this.__innerCamera = camera;
                            break;
                        }
                    }
                    if (this.__innerCamera == null)
                    {
                        base.UseDistortion = false;
                    }
                }
                return this.__innerCamera;
            }
        }

        private RenderTextureWrapper _rplCameraBuffer
        {
            get
            {
                if ((this.__rplCameraBuffer != null) && ((this.__rplCameraBuffer.width != this.InnerCameraWidth) || (this.__rplCameraBuffer.height != this.InnerCameraHeight)))
                {
                    GraphicsUtils.ReleaseRenderTexture(this.__rplCameraBuffer);
                    this.__rplCameraBuffer = null;
                }
                if (this.__rplCameraBuffer == null)
                {
                    this.__rplCameraBuffer = GraphicsUtils.GetRenderTexture(this.InnerCameraWidth, this.InnerCameraHeight, 0x18, base.internalBufferFormat);
                    if (this.__rplCameraBuffer == null)
                    {
                        base.NotSupported();
                    }
                }
                return this.__rplCameraBuffer;
            }
        }

        public int InnerCameraHeight
        {
            get
            {
                if (this.CameraResHeight != 0)
                {
                    return this.CameraResHeight;
                }
                return ((((base._camera.targetTexture != null) ? base._camera.pixelHeight : Screen.height) * this.CameraResScale) / 100);
            }
        }

        public int InnerCameraWidth
        {
            get
            {
                if (this.CameraResWidth != 0)
                {
                    return this.CameraResWidth;
                }
                return ((((base._camera.targetTexture != null) ? base._camera.pixelWidth : Screen.width) * this.CameraResScale) / 100);
            }
        }

        public enum CAMERA_RES_SCALE
        {
            RES_100 = 100,
            RES_150 = 150,
            RES_200 = 200,
            RES_25 = 0x19,
            RES_400 = 400,
            RES_50 = 50,
            RES_60 = 60,
            RES_70 = 70,
            RES_80 = 80,
            RES_90 = 90
        }
    }
}

