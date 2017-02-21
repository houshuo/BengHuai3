namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class FakeDOF : PostFXFramework
    {
        private Material _backgroundMat;
        private Renderer _backgroundRenderer;
        private Camera _camera;
        private Vector4[] _dofHexagon = new Vector4[7];
        private Vector4[] _dofVector = new Vector4[7];
        private Material _downSampleMat;
        private RenderTextureWrapper _farBackgroundBuffer1;
        private RenderTextureWrapper _farBackgroundBuffer2;
        private Camera _farCamera;
        private RenderTextureWrapper _farCameraBuffer;
        private int _height;
        private Material _hexBlurMat;
        private static readonly float _maxBlurFactor = 10f;
        private static readonly float _minBlurFactor = 1f;
        private float _origFarPlane;
        private PostFXWithResScale _postFXWithRescale;
        private int _width;
        [Range(0f, 10f)]
        public float backgroundBlurFactor;
        [HideInInspector]
        public Shader BackgroundShader;
        [Range(0f, 5f)]
        public float bokehIntensity;
        public BokehQualityEnum bokehQuality;
        public float bokehRotateSpeed;
        private RenderTextureFormat bufferFormat = RenderTextureFormat.ARGBHalf;
        [HideInInspector]
        public Shader DownSampleShader;
        [HideInInspector]
        public Shader HexBlurShader;
        public float separateDistance = 5f;

        private RenderTexture ApplyDOF(RenderTexture texture)
        {
            RenderTexture texture2;
            RenderTexture texture3;
            int num3;
            int num = this._width;
            int num2 = this._height;
            if (this.backgroundBlurFactor < (_maxBlurFactor / 2f))
            {
                texture2 = (RenderTexture) this._farCameraBuffer;
                texture3 = (RenderTexture) this._farBackgroundBuffer1;
                num3 = 2;
            }
            else
            {
                texture2 = (RenderTexture) this._farBackgroundBuffer1;
                Graphics.Blit((Texture) this._farCameraBuffer, texture2, this._downSampleMat, 0);
                texture3 = (RenderTexture) this._farBackgroundBuffer2;
                num3 = 2;
            }
            if (this.bokehQuality == BokehQualityEnum.Normal)
            {
                RenderTextureWrapper wrapper = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, this.bufferFormat);
                this.ApplyHexBlur(texture2, (RenderTexture) wrapper, this.backgroundBlurFactor, this.bokehIntensity, 1f / ((float) num3), 0);
                this.ApplyHexBlur((RenderTexture) wrapper, texture3, this.backgroundBlurFactor, this.bokehIntensity, 2f / ((float) num3), 0);
                GraphicsUtils.ReleaseRenderTexture(wrapper);
                return texture3;
            }
            if (this.bokehQuality == BokehQualityEnum.High)
            {
                RenderTextureWrapper wrapper2 = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, this.bufferFormat);
                RenderTextureWrapper wrapper3 = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, this.bufferFormat);
                this.ApplyHexBlur(texture2, (RenderTexture) wrapper2, this.backgroundBlurFactor, this.bokehIntensity, 0.5f / ((float) num3), 0);
                this.ApplyHexBlur((RenderTexture) wrapper2, (RenderTexture) wrapper3, this.backgroundBlurFactor, this.bokehIntensity, 1f / ((float) num3), 0);
                this.ApplyHexBlur((RenderTexture) wrapper3, texture3, this.backgroundBlurFactor, this.bokehIntensity, 2f / ((float) num3), 0);
                GraphicsUtils.ReleaseRenderTexture(wrapper2);
                GraphicsUtils.ReleaseRenderTexture(wrapper3);
            }
            return texture3;
        }

        private void ApplyHexBlur(RenderTexture source, RenderTexture destination, float blurFactor, float bokehIntensity, float tapScale, int pass = 0)
        {
            float num = 1f / ((float) source.width);
            float num2 = 1f / ((float) source.height);
            Vector4 vector = new Vector4(blurFactor * num, blurFactor * num2, 0f, bokehIntensity);
            this._hexBlurMat.SetVector("blurScale", vector);
            float num3 = Mathf.Sin(this.backgroundBlurFactor * this.bokehRotateSpeed);
            float num4 = Mathf.Cos(this.backgroundBlurFactor * this.bokehRotateSpeed);
            for (int i = 1; i < 7; i++)
            {
                this._dofVector[i - 1] = (Vector4) (new Vector4((num4 * this._dofHexagon[i].x) - (num3 * this._dofHexagon[i].y), (num3 * this._dofHexagon[i].x) + (num4 * this._dofHexagon[i].y), 0f, 0f) * tapScale);
            }
            for (int j = 0; j < 6; j++)
            {
                this._hexBlurMat.SetVector("dofScatter" + (j + 1), this._dofVector[j]);
            }
            this._hexBlurMat.mainTexture = source;
            destination.DiscardContents();
            Graphics.Blit(source, destination, this._hexBlurMat, pass);
        }

        private void Awake()
        {
            this.Init();
        }

        private void CreateBackgourQuad()
        {
            if (this._backgroundRenderer == null)
            {
                System.Type[] components = new System.Type[] { typeof(MeshRenderer) };
                GameObject obj2 = new GameObject("_DOFBackground", components) {
                    hideFlags = HideFlags.HideAndDontSave
                };
                obj2.transform.SetParentAndReset(base.transform);
                obj2.AddComponent<MeshFilter>().mesh = MeshGenerator.Quad();
                this._backgroundRenderer = obj2.GetComponent<Renderer>();
                this._backgroundRenderer.material = this._backgroundMat;
            }
        }

        private void CreateBuffers()
        {
            int width = this._width / 2;
            int height = this._height / 2;
            if (this._farCameraBuffer == null)
            {
                this._farCameraBuffer = GraphicsUtils.GetRenderTexture(width, height, 0x10, this.bufferFormat);
            }
            if (this._farBackgroundBuffer1 == null)
            {
                this._farBackgroundBuffer1 = GraphicsUtils.GetRenderTexture(width, height, 0, this.bufferFormat);
            }
            if (this._farBackgroundBuffer2 == null)
            {
                this._farBackgroundBuffer2 = GraphicsUtils.GetRenderTexture(width / 2, height / 2, 0, this.bufferFormat);
            }
        }

        private void CreateFarCamera()
        {
            base.CreateCamera(this._camera, ref this._farCamera, "DofFarCamera", HideFlags.HideAndDontSave);
            this._farCamera.farClipPlane = this._origFarPlane;
        }

        private void CreateMaterials()
        {
            this._downSampleMat = base.CheckShaderAndCreateMaterial(this.DownSampleShader, this._downSampleMat);
            this._hexBlurMat = base.CheckShaderAndCreateMaterial(this.HexBlurShader, this._hexBlurMat);
            this._backgroundMat = base.CheckShaderAndCreateMaterial(this.BackgroundShader, this._backgroundMat);
        }

        private void DoProcess()
        {
            if (this.backgroundBlurFactor > _minBlurFactor)
            {
                this.TurnOnDOF();
                this.CreateBuffers();
                Vector3 localPosition = this._backgroundRenderer.transform.localPosition;
                localPosition.z = this.separateDistance - 0.01f;
                this._backgroundRenderer.transform.localPosition = localPosition;
                this.DrawBackgroud();
                RenderTexture tex = this.ApplyDOF((RenderTexture) this._farCameraBuffer);
                Shader.SetGlobalTexture("_miHoYo_Background", tex);
            }
            else
            {
                this.TurnOffDOF();
            }
        }

        private void DrawBackgroud()
        {
            this._farCamera.CopyFrom(this._camera);
            this._farCamera.clearFlags = CameraClearFlags.Color;
            this._farCamera.nearClipPlane = this.separateDistance;
            this._farCamera.farClipPlane = this._origFarPlane;
            this._farCamera.targetTexture = (RenderTexture) this._farCameraBuffer;
            this._farCamera.Render();
        }

        private void GenerateDOFHexagon()
        {
            this._dofHexagon[0] = new Vector4(0f, 0f, 0f, 0f);
            for (int i = 0; i < 6; i++)
            {
                float num2 = Mathf.Sin(1.047198f * i);
                float num3 = Mathf.Cos(1.047198f * i);
                float num4 = 0f;
                float num5 = 1f;
                float x = (num3 * num4) - (num2 * num5);
                float y = (num2 * num4) + (num3 * num5);
                this._dofHexagon[i + 1] = new Vector4(x, y, 0f, 0f);
            }
        }

        private void Init()
        {
            this._camera = base.GetComponent<Camera>();
            this._origFarPlane = this._camera.farClipPlane;
            this._postFXWithRescale = base.GetComponent<PostFXWithResScale>();
            this.GenerateDOFHexagon();
            this.CreateMaterials();
            this.CreateBackgourQuad();
            this.CreateFarCamera();
        }

        private void OnDisable()
        {
            this.TurnOffDOF();
        }

        private void OnPostRender()
        {
            this.ReleaseBuffers();
        }

        private void OnPreRender()
        {
            this._width = (this._postFXWithRescale != null) ? this._postFXWithRescale.InnerCameraWidth : Screen.width;
            this._height = (this._postFXWithRescale != null) ? this._postFXWithRescale.InnerCameraHeight : Screen.height;
            this.DoProcess();
        }

        private void ReleaseBuffers()
        {
            if (this._farCameraBuffer != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._farCameraBuffer);
                this._farCameraBuffer = null;
            }
            if (this._farBackgroundBuffer1 != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._farBackgroundBuffer1);
                this._farBackgroundBuffer1 = null;
            }
            if (this._farBackgroundBuffer2 != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._farBackgroundBuffer2);
                this._farBackgroundBuffer2 = null;
            }
        }

        private void TurnOffDOF()
        {
            this._camera.farClipPlane = this._origFarPlane;
        }

        private void TurnOnDOF()
        {
            this._camera.farClipPlane = this.separateDistance;
        }

        public enum BokehQualityEnum
        {
            Normal,
            High
        }
    }
}

