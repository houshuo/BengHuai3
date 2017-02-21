namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class MainMenuCamera : MonoBehaviour
    {
        private Material _backgroundMat;
        private float _bufferSizeScale = 1f;
        private Camera _camera;
        private Material _compositionMat;
        private Material _downSample4xMat;
        private Material _downSampleCompMat;
        private Material _downSampleMat;
        private RenderTextureWrapper _farBackgroundBuffer1;
        private RenderTextureWrapper _farBackgroundBuffer2;
        private RenderTextureWrapper _farCameraBuffer;
        private Material _gaussMat;
        private int _height;
        private Material _hexBlurMat;
        private float _origFarPlane;
        private float _startBlurFactor = 1f;
        private int _width;
        public AnimationCurve BackgrondBlurCurve;
        [Range(0f, 10f)]
        public float BackgroundBlurFactor;
        [Header("Background")]
        public GameObject BackgroundQuad;
        [Range(0f, 5f)]
        public float BokehIntensity = 1.7f;
        private BokehQualityEnum BokehQuality;
        public float BokehRotateSpeed;
        private RenderTextureFormat bufferFormat = RenderTextureFormat.ARGBHalf;
        [HideInInspector]
        public Shader CompositionShader;
        private Vector4[] dofHexagon = new Vector4[7];
        private Vector4[] dofVector = new Vector4[7];
        [HideInInspector]
        public Shader DownSample4xShader;
        [HideInInspector]
        public Shader DownSampleCompShader;
        [HideInInspector]
        public Shader DownSampleShader;
        public Camera FarCamera;
        [HideInInspector]
        public Shader GaussShader;
        [HideInInspector]
        public Shader HexBlurShader;
        private bool isSupported = true;
        private static float maxBlurFactor = 10f;
        public float NearFarSeparateDistance = 5f;
        public int ReferencedBufferHeight = 0x360;
        [HideInInspector]
        public Shader ReflectedShader;
        private bool useDOF;

        private RenderTexture ApplyDOFtoBackground()
        {
            RenderTexture texture;
            RenderTexture texture2;
            int num3;
            this.CreateBuffers();
            this.FarCamera.Render();
            int num = (int) (this._width * this._bufferSizeScale);
            int num2 = (int) (this._height * this._bufferSizeScale);
            if (this.BackgroundBlurFactor < (maxBlurFactor / 2f))
            {
                texture = (RenderTexture) this._farCameraBuffer;
                texture2 = (RenderTexture) this._farBackgroundBuffer1;
                num3 = 2;
            }
            else
            {
                texture = (RenderTexture) this._farBackgroundBuffer1;
                Graphics.Blit((Texture) this._farCameraBuffer, texture, this._downSampleMat, 0);
                texture2 = (RenderTexture) this._farBackgroundBuffer2;
                num3 = 2;
            }
            if (this.BokehQuality == BokehQualityEnum.Normal)
            {
                RenderTextureWrapper wrapper = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, this.bufferFormat);
                this.ApplyHexBlur((RenderTexture) this._farCameraBuffer, (RenderTexture) wrapper, this.BackgroundBlurFactor, this.BokehIntensity, 1f / ((float) num3), 0);
                this.ApplyHexBlur((RenderTexture) wrapper, texture2, this.BackgroundBlurFactor, this.BokehIntensity, 2f / ((float) num3), 0);
                GraphicsUtils.ReleaseRenderTexture(wrapper);
                return texture2;
            }
            if (this.BokehQuality == BokehQualityEnum.High)
            {
                RenderTextureWrapper wrapper2 = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, this.bufferFormat);
                RenderTextureWrapper wrapper3 = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, this.bufferFormat);
                this.ApplyHexBlur((RenderTexture) this._farCameraBuffer, (RenderTexture) wrapper2, this.BackgroundBlurFactor, this.BokehIntensity, 0.5f / ((float) num3), 0);
                this.ApplyHexBlur((RenderTexture) wrapper2, (RenderTexture) wrapper3, this.BackgroundBlurFactor, this.BokehIntensity, 1f / ((float) num3), 0);
                this.ApplyHexBlur((RenderTexture) wrapper3, texture2, this.BackgroundBlurFactor, this.BokehIntensity, 2f / ((float) num3), 0);
                GraphicsUtils.ReleaseRenderTexture(wrapper2);
                GraphicsUtils.ReleaseRenderTexture(wrapper3);
            }
            return texture2;
        }

        private void ApplyHexBlur(RenderTexture source, RenderTexture destination, float blurFactor, float bokehIntensity, float tapScale, int pass = 0)
        {
            float num = 1f / ((float) source.width);
            float num2 = 1f / ((float) source.height);
            Vector4 vector = new Vector4(blurFactor * num, blurFactor * num2, 0f, bokehIntensity);
            this._hexBlurMat.SetVector("blurScale", vector);
            float num3 = Mathf.Sin(this.BackgroundBlurFactor * this.BokehRotateSpeed);
            float num4 = Mathf.Cos(this.BackgroundBlurFactor * this.BokehRotateSpeed);
            for (int i = 1; i < 7; i++)
            {
                this.dofVector[i - 1] = (Vector4) (new Vector4((num4 * this.dofHexagon[i].x) - (num3 * this.dofHexagon[i].y), (num3 * this.dofHexagon[i].x) + (num4 * this.dofHexagon[i].y), 0f, 0f) * tapScale);
            }
            for (int j = 0; j < 6; j++)
            {
                this._hexBlurMat.SetVector("dofScatter" + (j + 1), this.dofVector[j]);
            }
            this._hexBlurMat.mainTexture = source;
            destination.DiscardContents();
            Graphics.Blit(source, destination, this._hexBlurMat, pass);
        }

        private void Awake()
        {
            this.Init();
        }

        private bool CheckResources()
        {
            this._compositionMat = this.CheckShaderAndCreateMaterial(this.CompositionShader, this._compositionMat);
            this._gaussMat = this.CheckShaderAndCreateMaterial(this.GaussShader, this._gaussMat);
            this._downSampleMat = this.CheckShaderAndCreateMaterial(this.DownSampleShader, this._downSampleMat);
            this._downSample4xMat = this.CheckShaderAndCreateMaterial(this.DownSample4xShader, this._downSample4xMat);
            this._downSampleCompMat = this.CheckShaderAndCreateMaterial(this.DownSampleCompShader, this._downSampleCompMat);
            this._hexBlurMat = this.CheckShaderAndCreateMaterial(this.HexBlurShader, this._hexBlurMat);
            if (!this.isSupported)
            {
                this.ReportAutoDisable();
            }
            return this.isSupported;
        }

        private Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
        {
            if (s == null)
            {
                base.enabled = false;
                return null;
            }
            if ((s.isSupported && (m2Create != null)) && (m2Create.shader == s))
            {
                return m2Create;
            }
            if (!s.isSupported)
            {
                this.NotSupported();
                return null;
            }
            m2Create = new Material(s);
            m2Create.hideFlags = HideFlags.DontSave;
            if (m2Create != null)
            {
                return m2Create;
            }
            return null;
        }

        private void CreateBuffers()
        {
            int width = ((int) (this._width * this._bufferSizeScale)) / 2;
            int height = ((int) (this._height * this._bufferSizeScale)) / 2;
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
            this._compositionMat.SetTexture("_FarTex", (Texture) this._farCameraBuffer);
            this.FarCamera.targetTexture = (RenderTexture) this._farCameraBuffer;
        }

        private void DrawBackground()
        {
            if (this.useDOF)
            {
                RenderTexture tex = this.ApplyDOFtoBackground();
                Shader.SetGlobalTexture("_miHoYo_Background", tex);
            }
        }

        private void GenerateDOFHexagon()
        {
            this.dofHexagon[0] = new Vector4(0f, 0f, 0f, 0f);
            for (int i = 0; i < 6; i++)
            {
                float num2 = Mathf.Sin(1.047198f * i);
                float num3 = Mathf.Cos(1.047198f * i);
                float num4 = 0f;
                float num5 = 1f;
                float x = (num3 * num4) - (num2 * num5);
                float y = (num2 * num4) + (num3 * num5);
                this.dofHexagon[i + 1] = new Vector4(x, y, 0f, 0f);
            }
        }

        private void Init()
        {
            this.GenerateDOFHexagon();
            this._camera = base.GetComponent<Camera>();
            this._origFarPlane = this._camera.farClipPlane;
            this._backgroundMat = this.BackgroundQuad.GetComponent<Renderer>().material;
            this.UpdateTargetSize();
            this.CheckResources();
            this.SetMaterials();
            this.SetCameras();
        }

        private void NotSupported()
        {
            base.enabled = false;
            this.isSupported = false;
        }

        private void OnDestroy()
        {
            this.ReleaseBuffers();
        }

        private void OnEnable()
        {
            this.SetDOF();
            this.DrawBackground();
        }

        [AnimationCallback]
        public void OnGameEntryBeforeEnterSpaceshipAnimOver()
        {
            (Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry).OnCameraBeforeEnterSpaceshipAnimOver();
        }

        [AnimationCallback]
        public void OnGameEntryEnterSpaceshipAnimEvent(int phase)
        {
            (Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry).OnCameraEnterSpaceshipAnimEvent(phase);
        }

        private void OnPostRender()
        {
            this.ReleaseBuffers();
        }

        private void OnPreRender()
        {
            this.DrawBackground();
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

        private void ReportAutoDisable()
        {
            UnityEngine.Debug.LogWarning("Camera effect of " + this + " has been disabled as it's not supported on the current platform.");
        }

        private void SetCameraParamsWithMainCamera(Camera cam)
        {
            cam.fieldOfView = this._camera.fieldOfView;
            cam.nearClipPlane = this._camera.nearClipPlane;
            cam.farClipPlane = this._camera.farClipPlane;
        }

        private void SetCameras()
        {
            this.SetCameraParamsWithMainCamera(this.FarCamera);
            this.FarCamera.enabled = false;
        }

        private void SetDOF()
        {
            this.BackgroundBlurFactor = this.BackgrondBlurCurve.Evaluate(base.transform.position.z);
            if (!this.useDOF && (this.BackgroundBlurFactor > this._startBlurFactor))
            {
                this.useDOF = true;
                this.TurnOnDOF();
            }
            else if (this.useDOF && (this.BackgroundBlurFactor < this._startBlurFactor))
            {
                this.useDOF = false;
                this.TurnOffDOF();
            }
        }

        private void SetGaussParam(float s, float scale, float width)
        {
            GaussParamGenerator generator = new GaussParamGenerator(s, width);
            float[] numArray = new float[8];
            for (int i = 0; i < generator.Weights.Length; i++)
            {
                numArray[i] = generator.Weights[i];
            }
            this._gaussMat.SetVector("_weights", new Vector4(numArray[0], numArray[1], numArray[2], numArray[3]));
            this._gaussMat.SetVector("_weights2", new Vector4(numArray[4], numArray[5], numArray[6], numArray[7]));
            for (int j = 0; j < generator.Offsets.Length; j++)
            {
                this._gaussMat.SetVector("_offset_4_" + (j + 1), (Vector4) (generator.Offsets[j] * scale));
            }
        }

        private void SetMaterials()
        {
            if (this._downSample4xMat != null)
            {
                this._downSample4xMat.SetVector("twoTexelSize", new Vector2(2f / ((float) this._width), 2f / ((float) this._height)));
            }
        }

        private void TurnOffDOF()
        {
            this.useDOF = false;
            this._camera.farClipPlane = this._origFarPlane;
            this.BackgroundQuad.SetActive(false);
        }

        private void TurnOnDOF()
        {
            this.FarCamera.nearClipPlane = this.NearFarSeparateDistance;
            base.StartCoroutine(this.TurnOnDOFDelaySet());
        }

        [DebuggerHidden]
        private IEnumerator TurnOnDOFDelaySet()
        {
            return new <TurnOnDOFDelaySet>c__Iterator3F { <>f__this = this };
        }

        private void Update()
        {
            Shader.SetGlobalVector("_miHoYo_CameraRight", base.transform.right);
            this.UpdateTargetSize();
            this.SetDOF();
        }

        private void UpdateTargetSize()
        {
            if ((this._width != this._camera.pixelHeight) || (this._height != this._camera.pixelHeight))
            {
                this._width = this._camera.pixelWidth;
                this._height = this._camera.pixelHeight;
                this._backgroundMat.SetVector("_TexelOffset", new Vector4(1f / ((float) this._width), -1f / ((float) this._height)));
                if (this._height < this.ReferencedBufferHeight)
                {
                    this._bufferSizeScale = ((float) this.ReferencedBufferHeight) / ((float) this._height);
                }
                else
                {
                    this._bufferSizeScale = 1f;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <TurnOnDOFDelaySet>c__Iterator3F : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MainMenuCamera <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.useDOF = true;
                        this.<>f__this._camera.farClipPlane = this.<>f__this.NearFarSeparateDistance;
                        this.<>f__this.BackgroundQuad.SetActive(true);
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public enum BlurTypeEnum
        {
            Gauss,
            Hexagon
        }

        public enum BokehQualityEnum
        {
            Normal,
            High
        }

        public enum SizeDivEnum
        {
            Div_1 = 1,
            Div_2 = 2,
            Div_3 = 3,
            Div_4 = 4
        }
    }
}

