namespace MoleMole
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ReflectionBase : MonoBehaviour
    {
        private Material _hexBlurMat;
        protected bool _insideRendering;
        private bool _oldInvertCulling;
        protected TexResScale _oldTexResScale;
        protected static Camera _reflectionCamera;
        protected Vector3 _reflectionPlanePosition;
        protected RenderTextureWrapper _reflectionRenderTex;
        public RenderTextureFormat bufferFormat = RenderTextureFormat.ARGBHalf;
        public bool FastMode;
        public float hexBlurFactor;
        public Shader hexBlurShader;
        public LayerMask layers = -33;
        [Range(-10f, 10f)]
        public float reflectClipPlaneOffset;
        public ReflectRenderer[] reflectRendererList;
        protected string[] reflectShaderNames = new string[] { "miHoYo/Scene/Metal Wall", "miHoYo/Scene/Metal Floor", "miHoYo/Scene/Metal Floor (With Pow)", "miHoYo/Scene/Water", "miHoYo/Scene/Water Shore" };
        public TexResScale texResScale = TexResScale.RES_33;
        public bool useBlur;

        private void ApplyHexBlur()
        {
            if (this._hexBlurMat == null)
            {
                this._hexBlurMat = new Material(this.hexBlurShader);
            }
            RenderTextureWrapper wrapper = GraphicsUtils.GetRenderTexture(this._reflectionRenderTex.width, this._reflectionRenderTex.height, 0, this.bufferFormat);
            ReflectionTool.ApplyHexBlur(this._hexBlurMat, (RenderTexture) this._reflectionRenderTex, (RenderTexture) wrapper, this.hexBlurFactor, 0f, 0.5f, 1);
            ReflectionTool.ApplyHexBlur(this._hexBlurMat, (RenderTexture) wrapper, (RenderTexture) this._reflectionRenderTex, this.hexBlurFactor, 0f, 0.5f, 1);
            GraphicsUtils.ReleaseRenderTexture(wrapper);
        }

        protected virtual void Awake()
        {
            this._insideRendering = false;
            this._reflectionPlanePosition = base.transform.position;
            if (this.reflectRendererList.Length == 0)
            {
                Renderer component = base.GetComponent<Renderer>();
                if (component != null)
                {
                    this.reflectRendererList = new ReflectRenderer[] { new ReflectRenderer() };
                    this.reflectRendererList[0].renderer = component;
                }
            }
            this.SetFastMode(!GlobalVars.USE_REFLECTION);
            foreach (ReflectRenderer renderer2 in this.reflectRendererList)
            {
                renderer2.Init();
            }
        }

        protected virtual void CreateObjects(Camera srcCam, ref RenderTextureWrapper renderTex, ref Camera destCam)
        {
            if ((renderTex == null) || (this._oldTexResScale != this.texResScale))
            {
                if (renderTex != null)
                {
                    GraphicsUtils.ReleaseRenderTexture(renderTex);
                }
                renderTex = GraphicsUtils.GetRenderTexture((srcCam.pixelWidth * this.texResScale) / 100, (srcCam.pixelHeight * this.texResScale) / 100, 0x10, this.bufferFormat);
                renderTex.content.name = "__RefRenderTexture" + renderTex.content.GetInstanceID();
                renderTex.content.hideFlags = HideFlags.DontSave;
                this._oldTexResScale = this.texResScale;
            }
            if (destCam == null)
            {
                System.Type[] components = new System.Type[] { typeof(Camera) };
                GameObject obj2 = new GameObject("__RefCamera for " + srcCam.GetInstanceID(), components);
                destCam = obj2.GetComponent<Camera>();
                destCam.enabled = false;
                destCam.transform.position = base.transform.position;
                destCam.transform.rotation = base.transform.rotation;
                obj2.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        private void DrawReflectionRenderTexture(Camera cam)
        {
            Vector3 rhs = this._reflectionPlanePosition;
            Vector3 up = base.transform.up;
            this.CreateObjects(cam, ref this._reflectionRenderTex, ref _reflectionCamera);
            _reflectionCamera.CopyFrom(cam);
            float w = -Vector3.Dot(up, rhs);
            Vector4 plane = new Vector4(up.x, up.y, up.z, w);
            Matrix4x4 matrixx = ReflectionTool.CalculateReflectionMatrix(Matrix4x4.zero, plane);
            Vector3 position = cam.transform.position;
            Vector3 vector5 = matrixx.MultiplyPoint(position);
            _reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * matrixx;
            Vector4 clipPlane = ReflectionTool.CameraSpacePlane(_reflectionCamera, rhs, up, 1f, this.reflectClipPlaneOffset);
            Matrix4x4 matrixx2 = ReflectionTool.CalculateObliqueMatrix(cam.projectionMatrix, clipPlane, -1f);
            _reflectionCamera.projectionMatrix = matrixx2;
            _reflectionCamera.cullingMask = -17 & this.layers.value;
            _reflectionCamera.targetTexture = (RenderTexture) this._reflectionRenderTex;
            this._oldInvertCulling = GL.invertCulling;
            GL.invertCulling = true;
            _reflectionCamera.transform.position = vector5;
            Vector3 eulerAngles = cam.transform.eulerAngles;
            _reflectionCamera.transform.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
            _reflectionCamera.Render();
            _reflectionCamera.transform.position = position;
            GL.invertCulling = this._oldInvertCulling;
        }

        protected virtual void LateUpdate()
        {
            if (!this.FastMode && (this.reflectRendererList.Length != 0))
            {
                Camera main = Camera.main;
                if ((main != null) && !this._insideRendering)
                {
                    this._insideRendering = true;
                    this.DrawReflectionRenderTexture(main);
                    if (this.useBlur)
                    {
                        this.ApplyHexBlur();
                    }
                    foreach (ReflectRenderer renderer in this.reflectRendererList)
                    {
                        renderer.SetMaterialBlock((RenderTexture) this._reflectionRenderTex);
                    }
                    this._insideRendering = false;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (this._reflectionRenderTex != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._reflectionRenderTex);
                this._reflectionRenderTex = null;
            }
            if (_reflectionCamera != null)
            {
                UnityEngine.Object.DestroyImmediate(_reflectionCamera.gameObject);
                _reflectionCamera = null;
            }
        }

        public void SetFastMode(bool isFastMode)
        {
            this.FastMode = isFastMode;
            foreach (string str in this.reflectShaderNames)
            {
                Shader shader = Shader.Find(str);
                if (shader != null)
                {
                    if (this.FastMode)
                    {
                        shader.maximumLOD = 100;
                    }
                    else
                    {
                        shader.maximumLOD = 600;
                    }
                }
            }
        }

        protected virtual void Update()
        {
            this._reflectionPlanePosition = base.transform.position;
        }

        [Serializable]
        public class ReflectRenderer
        {
            private MaterialPropertyBlock block;
            public Renderer renderer;

            public void Init()
            {
                this.block = new MaterialPropertyBlock();
            }

            public void SetMaterialBlock(RenderTexture reflectionTex)
            {
                this.block.SetTexture("_ReflectionTex", reflectionTex);
                this.renderer.SetPropertyBlock(this.block);
            }
        }

        public enum TexResScale
        {
            RES_100 = 100,
            RES_25 = 0x19,
            RES_33 = 0x21,
            RES_40 = 40,
            RES_50 = 50,
            RES_75 = 0x4b
        }
    }
}

