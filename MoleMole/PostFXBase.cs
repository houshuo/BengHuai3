namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [AddComponentMenu("Image Effects/PostFX"), RequireComponent(typeof(Camera)), ExecuteInEditMode]
    public class PostFXBase : MonoBehaviour
    {
        private Camera __distortionCamera;
        private Shader __multipleGaussPassShader;
        protected Camera _camera;
        private static readonly string _DISTORTION_CAMERA_NAME = "DistortionCamera";
        private static bool _hashInited;
        protected RenderTextureWrapper alpha_buffer;
        private Camera alphaCamera;
        protected float aspectRatio;
        [Range(0f, 1f), SerializeField]
        protected float avatarShadowAdjust = 0.5f;
        protected string basedOnTempTex = string.Empty;
        protected RenderTextureWrapper[] blur_bufferA = new RenderTextureWrapper[6];
        protected RenderTextureWrapper[] blur_bufferB = new RenderTextureWrapper[6];
        protected Vector4 blurCoeff = new Vector4(0.3f, 0.3f, 0.26f, 0.15f);
        protected Material BrightPassEx;
        [HideInInspector]
        public Shader BrightPassExShader;
        protected RenderTextureWrapper[] compose_buffer = new RenderTextureWrapper[6];
        [Range(0.3f, 10f)]
        public float constrast = 2f;
        protected Texture2D converted2DLut;
        [NonSerialized]
        public const float defaultGameConstrast = 2f;
        public const float defaultUIConstrast = 2.1f;
        public LayerMask DepthCullingMask;
        [Header("Radial Blur")]
        public bool DisableRadialBlur;
        protected Material DistortionApplyMat;
        [HideInInspector]
        public Shader DistortionApplyShader;
        public LayerMask DistortionCullingMask;
        protected RenderTextureWrapper distortionMap;
        [HideInInspector]
        public Color DistortionMapClearColor = new Color(0.498f, 0.498f, 0f, 0f);
        [HideInInspector]
        public int DistortionMapDepthBit = 0x10;
        [HideInInspector]
        public RenderTextureFormat DistortionMapFormat;
        [HideInInspector]
        public Shader DistortionMapNormShader;
        [HideInInspector]
        public int DistortionMapSizeDown = 2;
        protected Material DownSample;
        protected Material DownSample4X;
        protected RenderTextureWrapper downsample4X_buffer;
        [HideInInspector]
        public Shader DownSample4XShader;
        protected int downsampleLevel;
        [HideInInspector]
        public Shader DownSampleShader;
        [HideInInspector]
        public Shader DrawAlphaShader;
        [HideInInspector]
        public Shader DrawDepthShader;
        [Range(0f, 5000f)]
        public float Exposure = 13f;
        public bool FastMode;
        public bool FXAA;
        public bool FXAAForceHQ;
        public float fxAlphaIntensity = 0.5f;
        protected RenderTextureWrapper gauss_buffer;
        protected Material GaussCompositionEx;
        [HideInInspector]
        public Shader GaussCompositionExShader;
        protected Vector4 glareCoeff = new Vector4(0f, 0f, 0f, 0f);
        protected Material GlareCompositionEx;
        [HideInInspector]
        public Shader GlareCompositionExShader;
        [Range(0f, 1f)]
        public float glareIntensity = 0.75f;
        [Range(0f, 5f)]
        public float glareScaler = 1.06f;
        [Range(0f, 5f)]
        public float glareThreshold = 0.65f;
        protected static int HASH_ALPHA_TEX;
        protected static int HASH_COEFF;
        protected static int HASH_CONSTRAST;
        protected static int HASH_DIM;
        protected static int HASH_DISTORTION_TEX;
        protected static int HASH_EXPOSURE;
        protected static int HASH_FX_ALPHA_INTENSITY;
        protected static int HASH_LUM_SCALER;
        protected static int HASH_LUM_TRESHOLD;
        protected static int HASH_LUT_TEX;
        protected static int HASH_MAIN_TEX;
        protected static int HASH_MAIN_TEX_0;
        protected static int HASH_MAIN_TEX_1;
        protected static int HASH_MAIN_TEX_2;
        protected static int HASH_MAIN_TEX_3;
        protected static int HASH_OFFSET;
        protected static int HASH_RADIAL_BLUR_PARAM;
        protected static int HASH_RADIAL_BLUR_TEX;
        protected static int HASH_SCALE_RG;
        protected static int HASH_SCALER_LOWER_CASE;
        protected static int HASH_SCALER_UPPER_CASE;
        protected static int HASH_SEPIA_COLOR;
        protected static int HASH_TEXEL_SIZE;
        protected static int HASH_THRESHOLD;
        public bool HDRBuffer = true;
        protected RenderTextureFormat internalBufferFormat = RenderTextureFormat.ARGBHalf;
        public InternalBufferSizeEnum internalBufferSize = InternalBufferSizeEnum.SIZE_128;
        protected bool isSupported = true;
        private InternalBufferSizeEnum lastInternalBufferSize;
        protected Material MultipleGaussPassFilter;
        [HideInInspector]
        public Shader MultipleGaussPassFilterShader_128;
        [HideInInspector]
        public Shader MultipleGaussPassFilterShader_256;
        [HideInInspector]
        public Shader MultipleGaussPassFilterShader_512;
        protected Dictionary<InternalBufferSizeEnum, Shader> MultipleGaussPassFilterShaderMap;
        [HideInInspector]
        public bool originalEnabled;
        protected RenderTextureWrapper radial_blur_buffer;
        protected RenderTextureWrapper radial_blur_buffer_temp;
        private static readonly string RADIAL_BLUR_SHDER_KEYWORD = "RADIAL_BLUR";
        public Vector2 RadialBlurCenter = new Vector2(0.5f, 0.5f);
        public SizeDownScaleEnum RadialBlurDownScale = SizeDownScaleEnum.Div_4;
        protected Material RadialBlurMat;
        [Range(0f, 2f)]
        public float RadialBlurScatterScale = 1f;
        [HideInInspector]
        public Shader RadialBlurShader;
        [Range(0f, 10f)]
        public float RadialBlurStrenth;
        public Color SepiaColor = new Color(0.5f, 0.5f, 0.5f, 0f);
        public Texture2D sourceLut3D;
        protected float stepx;
        protected float stepy;
        protected bool supportDX11;
        protected bool supportHDRTextures = true;
        [Header("Color Grading")]
        public bool UseColorGrading = true;
        public bool UseDepthTest = true;
        [Header("Distortion")]
        public bool UseDistortion = true;
        public bool WriteAlpha;
        [Tooltip("If camera writes the depth texture"), Header("Utility")]
        public bool WriteDepthTexture;

        protected void blur(RenderTexture source, RenderTexture destination, RenderTexture temp, int level)
        {
            this.MultipleGaussPassFilter.SetVector(HASH_SCALER_LOWER_CASE, new Vector2(((1f / this.aspectRatio) <= 1f) ? (1f / this.aspectRatio) : 1f, 0f));
            Graphics.Blit(source, temp, this.MultipleGaussPassFilter, level);
            this.MultipleGaussPassFilter.SetVector(HASH_SCALER_LOWER_CASE, new Vector2(0f, (this.aspectRatio <= 1f) ? this.aspectRatio : 1f));
            destination.DiscardContents();
            Graphics.Blit(temp, destination, this.MultipleGaussPassFilter, level);
        }

        protected bool CheckResources()
        {
            this.CheckSupport(false);
            this.DistortionApplyMat = this.CheckShaderAndCreateMaterial(this.DistortionApplyShader, this.DistortionApplyMat);
            this.DownSample4X = this.CheckShaderAndCreateMaterial(this.DownSample4XShader, this.DownSample4X);
            this.DownSample = this.CheckShaderAndCreateMaterial(this.DownSampleShader, this.DownSample);
            this.BrightPassEx = this.CheckShaderAndCreateMaterial(this.BrightPassExShader, this.BrightPassEx);
            this.GaussCompositionEx = this.CheckShaderAndCreateMaterial(this.GaussCompositionExShader, this.GaussCompositionEx);
            this.GlareCompositionEx = this.CheckShaderAndCreateMaterial(this.GlareCompositionExShader, this.GlareCompositionEx);
            this.MultipleGaussPassFilter = this.CheckShaderAndCreateMaterial(this._multipleGaussPassShader, this.MultipleGaussPassFilter);
            if (this.UseRadialBlur)
            {
                this.RadialBlurMat = this.CheckShaderAndCreateMaterial(this.RadialBlurShader, this.RadialBlurMat);
            }
            this.CheckShaderExists(this.DistortionMapNormShader);
            this.CheckShaderExists(this.DrawDepthShader);
            this.CheckShaderExists(this.DrawAlphaShader);
            if (!this.isSupported)
            {
                this.ReportAutoDisable();
            }
            return this.isSupported;
        }

        private bool CheckShader(Shader s)
        {
            if (!s.isSupported)
            {
                this.NotSupported();
                return false;
            }
            return false;
        }

        protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
        {
            if (s == null)
            {
                Debug.LogError("Missing shader in " + this);
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
                Debug.LogError(string.Concat(new object[] { "The shader ", s, " on effect ", this, " is not supported on this platform!" }));
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

        private void CheckShaderExists(Shader s)
        {
            if (s == null)
            {
                base.enabled = false;
            }
        }

        private void CheckShaderExists(out Shader s, string name)
        {
            s = Shader.Find(name);
            if (s == null)
            {
                base.enabled = false;
            }
        }

        private bool CheckSupport(bool needDepth)
        {
            this.isSupported = true;
            this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(this.internalBufferFormat);
            this.supportDX11 = (SystemInfo.graphicsShaderLevel >= 50) && SystemInfo.supportsComputeShaders;
            if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
            {
                this.NotSupported();
                return false;
            }
            if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
            {
                this.NotSupported();
                return false;
            }
            if (needDepth)
            {
                Camera component = base.GetComponent<Camera>();
                component.depthTextureMode |= DepthTextureMode.Depth;
            }
            return true;
        }

        private bool CheckSupport(bool needDepth, bool needHdr)
        {
            if (!this.CheckSupport(needDepth))
            {
                return false;
            }
            if (needHdr && !this.supportHDRTextures)
            {
                this.NotSupported();
                return false;
            }
            return true;
        }

        protected void ComposeBlur(RenderTexture destination)
        {
            this.GaussCompositionEx.SetTexture(HASH_MAIN_TEX_0, (Texture) this.blur_bufferA[0]);
            this.GaussCompositionEx.SetTexture(HASH_MAIN_TEX_1, (Texture) this.blur_bufferA[1]);
            this.GaussCompositionEx.SetTexture(HASH_MAIN_TEX_2, (Texture) this.blur_bufferA[2]);
            this.GaussCompositionEx.SetTexture(HASH_MAIN_TEX_3, (Texture) this.blur_bufferA[3]);
            this.GaussCompositionEx.SetVector(HASH_COEFF, this.blurCoeff);
            this.compose_buffer[0].content.DiscardContents();
            Graphics.Blit(null, destination, this.GaussCompositionEx, 0);
        }

        protected void ComposeEffect(RenderTexture source, RenderTexture destination, bool useRadialBlur_ = false)
        {
            this.glareCoeff.y = this.glareIntensity;
            this.glareCoeff.x = 1f - this.glareIntensity;
            this.GlareCompositionEx.SetVector(HASH_COEFF, this.glareCoeff);
            this.GlareCompositionEx.SetFloat(HASH_EXPOSURE, this.Exposure);
            this.GlareCompositionEx.SetFloat(HASH_CONSTRAST, this.constrast);
            this.GlareCompositionEx.SetFloat(HASH_LUM_TRESHOLD, this.glareThreshold);
            this.GlareCompositionEx.SetFloat(HASH_LUM_SCALER, this.glareScaler);
            this.GlareCompositionEx.SetTexture(HASH_MAIN_TEX, source);
            this.GlareCompositionEx.SetTexture(HASH_MAIN_TEX_1, (Texture) this.gauss_buffer);
            if (this.UseDistortion)
            {
                this.GlareCompositionEx.SetTexture(HASH_DISTORTION_TEX, (Texture) this.distortionMap);
            }
            else
            {
                this.GlareCompositionEx.SetTexture(HASH_DISTORTION_TEX, null);
            }
            if (useRadialBlur_)
            {
                this.GlareCompositionEx.SetTexture(HASH_RADIAL_BLUR_TEX, (Texture) this.radial_blur_buffer);
                Vector4 vector = new Vector4(this.RadialBlurCenter.x, this.RadialBlurCenter.y, this.RadialBlurStrenth, this.RadialBlurScatterScale);
                this.GlareCompositionEx.SetVector(HASH_RADIAL_BLUR_PARAM, vector);
                this.GlareCompositionEx.EnableKeyword(RADIAL_BLUR_SHDER_KEYWORD);
            }
            else
            {
                this.GlareCompositionEx.SetTexture(HASH_RADIAL_BLUR_TEX, null);
                this.GlareCompositionEx.SetVector(HASH_RADIAL_BLUR_PARAM, Vector4.zero);
                this.GlareCompositionEx.DisableKeyword(RADIAL_BLUR_SHDER_KEYWORD);
            }
            if (this.FXAA)
            {
                if (this.FXAAForceHQ)
                {
                    Graphics.Blit(null, destination, this.GlareCompositionEx, 2);
                }
                else if (this.SepiaColor.a != 0f)
                {
                    this.GlareCompositionEx.SetVector(HASH_SEPIA_COLOR, (Vector4) this.SepiaColor);
                    Graphics.Blit(null, destination, this.GlareCompositionEx, 7);
                }
                else
                {
                    Graphics.Blit(null, destination, this.GlareCompositionEx, 1);
                }
            }
            else if (this.SepiaColor.a != 0f)
            {
                this.GlareCompositionEx.SetVector(HASH_SEPIA_COLOR, (Vector4) this.SepiaColor);
                Graphics.Blit(null, destination, this.GlareCompositionEx, 6);
            }
            else if (this.UseColorGrading)
            {
                float width = this.converted2DLut.width;
                float num2 = Mathf.Sqrt(width);
                this.converted2DLut.wrapMode = TextureWrapMode.Clamp;
                this.GlareCompositionEx.SetFloat(HASH_SCALE_RG, (num2 - 1f) / width);
                this.GlareCompositionEx.SetFloat(HASH_DIM, num2);
                this.GlareCompositionEx.SetFloat(HASH_OFFSET, 1f / (2f * width));
                this.GlareCompositionEx.SetTexture(HASH_LUT_TEX, this.converted2DLut);
                Graphics.Blit(null, destination, this.GlareCompositionEx, 3);
            }
            else if (this.WriteAlpha)
            {
                this.GlareCompositionEx.SetFloat(HASH_FX_ALPHA_INTENSITY, this.fxAlphaIntensity);
                this.GlareCompositionEx.SetTexture(HASH_ALPHA_TEX, (Texture) this.alpha_buffer);
                Graphics.Blit(null, destination, this.GlareCompositionEx, 4);
            }
            else
            {
                Graphics.Blit(null, destination, this.GlareCompositionEx, 0);
            }
        }

        private void Convert(Texture2D temp2DTex, string path)
        {
            if (temp2DTex != null)
            {
                int height = temp2DTex.width * temp2DTex.height;
                height = temp2DTex.height;
                if (!this.ValidDimensions(temp2DTex))
                {
                    Debug.LogWarning("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");
                    this.basedOnTempTex = string.Empty;
                }
                else
                {
                    Color[] pixels = temp2DTex.GetPixels();
                    Color[] colors = new Color[((height * height) * height) * height];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            for (int k = 0; k < height; k++)
                            {
                                for (int m = 0; m < height; m++)
                                {
                                    float f = (i + ((j * height) * 1f)) / ((float) height);
                                    int num7 = Mathf.FloorToInt(f);
                                    int num8 = Mathf.Min((int) (num7 + 1), (int) (height - 1));
                                    float t = f - num7;
                                    int num10 = k + ((((height - m) - 1) * height) * height);
                                    Color a = pixels[num10 + (num7 * height)];
                                    Color b = pixels[num10 + (num8 * height)];
                                    colors[((k + (i * height)) + ((m * height) * height)) + (((j * height) * height) * height)] = Color.Lerp(a, b, t);
                                }
                            }
                        }
                    }
                    if (this.converted2DLut != null)
                    {
                        UnityEngine.Object.DestroyImmediate(this.converted2DLut);
                    }
                    this.converted2DLut = new Texture2D(height * height, height * height, TextureFormat.ARGB32, false);
                    this.converted2DLut.SetPixels(colors);
                    this.converted2DLut.Apply();
                    this.basedOnTempTex = path;
                }
            }
            else
            {
                Debug.LogError("Couldn't color correct with 2D LUT texture. Image Effect will be disabled.");
            }
        }

        protected void CreateCamera(Camera srcCam, ref Camera destCam)
        {
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

        protected virtual void CreateDistortionMap(int w, int h)
        {
            this.distortionMap = GraphicsUtils.GetRenderTexture(w / 2, h / 2, this.DistortionMapDepthBit, this.DistortionMapFormat);
        }

        private Material CreateMaterial(Shader s, Material m2Create)
        {
            if (s != null)
            {
                if (((m2Create != null) && (m2Create.shader == s)) && s.isSupported)
                {
                    return m2Create;
                }
                if (!s.isSupported)
                {
                    return null;
                }
                m2Create = new Material(s);
                m2Create.hideFlags = HideFlags.DontSave;
                if (m2Create != null)
                {
                    return m2Create;
                }
            }
            return null;
        }

        protected void CreateRenderTextures(int w, int h)
        {
            if (this.WriteAlpha)
            {
                this.alpha_buffer = GraphicsUtils.GetRenderTexture(w, h, 0, RenderTextureFormat.R8);
            }
            this.downsample4X_buffer = GraphicsUtils.GetRenderTexture(w / 4, h / 4, 0x10, this.internalBufferFormat);
            if (this.UseDistortion)
            {
                this.CreateDistortionMap(w, h);
            }
            if (this.UseRadialBlur)
            {
                int radialBlurDownScale = (int) this.RadialBlurDownScale;
                this.radial_blur_buffer = GraphicsUtils.GetRenderTexture(w / radialBlurDownScale, h / radialBlurDownScale, 0, this.internalBufferFormat);
                this.radial_blur_buffer_temp = GraphicsUtils.GetRenderTexture(w / radialBlurDownScale, h / radialBlurDownScale, 0, this.internalBufferFormat);
            }
            int internalBufferSize = (int) this.internalBufferSize;
            int width = (int) this.internalBufferSize;
            this.gauss_buffer = GraphicsUtils.GetRenderTexture(width, width, 0, RenderTextureFormat.ARGBHalf);
            this.gauss_buffer.content.filterMode = FilterMode.Bilinear;
            for (int i = 0; i < 6; i++)
            {
                this.compose_buffer[i] = GraphicsUtils.GetRenderTexture(internalBufferSize, width, 0, this.internalBufferFormat);
                this.blur_bufferA[i] = GraphicsUtils.GetRenderTexture(internalBufferSize, width, 0, this.internalBufferFormat);
                this.blur_bufferB[i] = GraphicsUtils.GetRenderTexture(internalBufferSize, width, 0, this.internalBufferFormat);
                internalBufferSize /= 2;
                width /= 2;
            }
        }

        public void DoPostProcess(RenderTexture source, RenderTexture destination)
        {
            if (!this.CheckResources())
            {
                Graphics.Blit(source, destination);
            }
            else
            {
                if ((this.converted2DLut == null) && this.UseColorGrading)
                {
                    if (this.sourceLut3D == null)
                    {
                        this.SetIdentityLut();
                    }
                    else
                    {
                        this.Convert(this.sourceLut3D, string.Empty);
                    }
                }
                if (this.FastMode)
                {
                    if (this.SepiaColor.a != 0f)
                    {
                        this.GlareCompositionEx.SetVector(HASH_SEPIA_COLOR, (Vector4) this.SepiaColor);
                        this.GlareCompositionEx.SetTexture(HASH_MAIN_TEX, source);
                        Graphics.Blit(null, destination, this.GlareCompositionEx, 5);
                    }
                    else
                    {
                        Graphics.Blit(source, destination);
                    }
                }
                else
                {
                    this.aspectRatio = source.width;
                    this.aspectRatio /= (float) source.height;
                    if (this.HDRBuffer)
                    {
                        this.internalBufferFormat = RenderTextureFormat.ARGBHalf;
                    }
                    else
                    {
                        this.internalBufferFormat = RenderTextureFormat.ARGB32;
                    }
                    this.CreateRenderTextures(source.width, source.height);
                    if (this.WriteAlpha)
                    {
                        this.PrepareAlpha();
                    }
                    if (this.UseDistortion)
                    {
                        this.PrepareDistortion();
                    }
                    this.DownSample4X.SetVector(HASH_TEXEL_SIZE, new Vector2(1f / ((float) source.width), 1f / ((float) source.height)));
                    Graphics.Blit(source, (RenderTexture) this.downsample4X_buffer, this.DownSample4X, 0);
                    this.DownSample4X.SetVector(HASH_TEXEL_SIZE, new Vector2(0.5f / ((float) this.downsample4X_buffer.content.width), 0.5f / ((float) this.downsample4X_buffer.content.height)));
                    Graphics.Blit((Texture) this.downsample4X_buffer, (RenderTexture) this.blur_bufferA[0], this.DownSample4X, 0);
                    this.extractHL((RenderTexture) this.blur_bufferA[0], (RenderTexture) this.compose_buffer[0]);
                    this.blur((RenderTexture) this.compose_buffer[0], (RenderTexture) this.blur_bufferA[0], (RenderTexture) this.blur_bufferB[0], 0);
                    for (int i = 1; i < 4; i++)
                    {
                        Graphics.Blit((Texture) this.compose_buffer[i - 1], (RenderTexture) this.compose_buffer[i], this.DownSample, 0);
                        this.blur((RenderTexture) this.compose_buffer[i], (RenderTexture) this.blur_bufferA[i], (RenderTexture) this.blur_bufferB[i], i);
                    }
                    this.ComposeBlur((RenderTexture) this.gauss_buffer);
                    if (this.UseRadialBlur)
                    {
                        this.ComposeEffect(source, (RenderTexture) this.radial_blur_buffer_temp, false);
                        this.RadialBlur((RenderTexture) this.radial_blur_buffer_temp, (RenderTexture) this.radial_blur_buffer);
                    }
                    this.ComposeEffect(source, destination, this.UseRadialBlur);
                    this.ReleaseRenderTextures();
                }
            }
        }

        private bool Dx11Support()
        {
            return this.supportDX11;
        }

        protected void extractHL(RenderTexture source, RenderTexture destination)
        {
            this.BrightPassEx.SetFloat(HASH_THRESHOLD, this.glareThreshold);
            this.BrightPassEx.SetFloat(HASH_SCALER_UPPER_CASE, this.glareScaler);
            destination.DiscardContents();
            Graphics.Blit(source, destination, this.BrightPassEx, 0);
        }

        private static void InitUniformHashes()
        {
            if (!_hashInited)
            {
                HASH_SEPIA_COLOR = Shader.PropertyToID("_SepiaColor");
                HASH_MAIN_TEX = Shader.PropertyToID("_MainTex");
                HASH_TEXEL_SIZE = Shader.PropertyToID("_texelSize");
                HASH_THRESHOLD = Shader.PropertyToID("_Threshhold");
                HASH_SCALER_UPPER_CASE = Shader.PropertyToID("_Scaler");
                HASH_SCALER_LOWER_CASE = Shader.PropertyToID("_scaler");
                HASH_MAIN_TEX_0 = Shader.PropertyToID("_MainTex0");
                HASH_MAIN_TEX_1 = Shader.PropertyToID("_MainTex1");
                HASH_MAIN_TEX_2 = Shader.PropertyToID("_MainTex2");
                HASH_MAIN_TEX_3 = Shader.PropertyToID("_MainTex3");
                HASH_COEFF = Shader.PropertyToID("coeff");
                HASH_EXPOSURE = Shader.PropertyToID("exposure");
                HASH_CONSTRAST = Shader.PropertyToID("constrast");
                HASH_LUM_TRESHOLD = Shader.PropertyToID("lumThreshold");
                HASH_LUM_SCALER = Shader.PropertyToID("lumScaler");
                HASH_DISTORTION_TEX = Shader.PropertyToID("_DistortionTex");
                HASH_RADIAL_BLUR_TEX = Shader.PropertyToID("_RadialBlurTex");
                HASH_RADIAL_BLUR_PARAM = Shader.PropertyToID("_RadialBlurParam");
                HASH_SCALE_RG = Shader.PropertyToID("_ScaleRG");
                HASH_DIM = Shader.PropertyToID("_Dim");
                HASH_OFFSET = Shader.PropertyToID("_Offset");
                HASH_LUT_TEX = Shader.PropertyToID("_LutTex");
                HASH_FX_ALPHA_INTENSITY = Shader.PropertyToID("_fxAlphaIntensity");
                HASH_ALPHA_TEX = Shader.PropertyToID("_AlphaTex");
                _hashInited = true;
            }
        }

        protected static void MyDestroy(UnityEngine.Object obj)
        {
            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        protected void NotSupported()
        {
            base.enabled = false;
            this.isSupported = false;
        }

        protected virtual void OnDestroy()
        {
            this.ReleaseMaterials();
            if (this.converted2DLut != null)
            {
                UnityEngine.Object.DestroyImmediate(this.converted2DLut);
            }
            this.converted2DLut = null;
        }

        protected virtual void OnDisable()
        {
            if (this.alphaCamera != null)
            {
                UnityEngine.Object.DestroyImmediate(this.alphaCamera.gameObject);
                this.alphaCamera = null;
            }
        }

        protected virtual void OnEnable()
        {
            InitUniformHashes();
            this.isSupported = true;
            this._camera = base.GetComponent<Camera>();
            this.PrepareMultipleGaussShaders();
        }

        public virtual void OnPreRender()
        {
            if (this.WriteDepthTexture)
            {
                this._camera.depthTextureMode = DepthTextureMode.Depth;
            }
            else
            {
                this._camera.depthTextureMode = DepthTextureMode.None;
            }
        }

        protected void PrepareAlpha()
        {
            this.CreateCamera(this._camera, ref this.alphaCamera);
            this.alphaCamera.CopyFrom(this._camera);
            this.alphaCamera.targetTexture = (RenderTexture) this.alpha_buffer;
            this.alphaCamera.SetReplacementShader(this.DrawAlphaShader, "OutlineType");
            this.alphaCamera.Render();
        }

        protected virtual void PrepareDistortion()
        {
            if (this._distortionCamera != null)
            {
                this._distortionCamera.CopyFrom(this._camera);
                this._distortionCamera.backgroundColor = this.DistortionMapClearColor;
                this._distortionCamera.SetTargetBuffers(this.distortionMap.content.colorBuffer, this.distortionMap.content.depthBuffer);
                if (this.UseDepthTest)
                {
                    this._distortionCamera.clearFlags = CameraClearFlags.Color;
                    this._distortionCamera.cullingMask = (int) this.DepthCullingMask;
                    this._distortionCamera.RenderWithShader(this.DrawDepthShader, string.Empty);
                    this._distortionCamera.clearFlags = CameraClearFlags.Nothing;
                    this._distortionCamera.cullingMask = (int) this.DistortionCullingMask;
                    this._distortionCamera.RenderWithShader(this.DistortionMapNormShader, "Distortion");
                }
                else
                {
                    this._distortionCamera.clearFlags = CameraClearFlags.Color;
                    this._distortionCamera.cullingMask = (int) this.DistortionCullingMask;
                    this._distortionCamera.RenderWithShader(this.DistortionMapNormShader, "Distortion");
                }
            }
        }

        public void PrepareMultipleGaussShaders()
        {
            this.MultipleGaussPassFilterShaderMap = new Dictionary<InternalBufferSizeEnum, Shader>();
            this.MultipleGaussPassFilterShaderMap[InternalBufferSizeEnum.SIZE_128] = this.MultipleGaussPassFilterShader_128;
            this.MultipleGaussPassFilterShaderMap[InternalBufferSizeEnum.SIZE_256] = this.MultipleGaussPassFilterShader_256;
            this.MultipleGaussPassFilterShaderMap[InternalBufferSizeEnum.SIZE_512] = this.MultipleGaussPassFilterShader_512;
        }

        protected void RadialBlur(RenderTexture source, RenderTexture destination)
        {
            Vector4 vector = new Vector4(this.RadialBlurCenter.x, this.RadialBlurCenter.y, this.RadialBlurStrenth, this.RadialBlurScatterScale);
            this.RadialBlurMat.SetVector(HASH_RADIAL_BLUR_PARAM, vector);
            this.RadialBlurMat.SetTexture(HASH_MAIN_TEX, source);
            Graphics.Blit(source, destination, this.RadialBlurMat, 0);
        }

        protected void ReleaseMaterials()
        {
            MyDestroy(this.DistortionApplyMat);
            MyDestroy(this.DownSample4X);
            MyDestroy(this.DownSample);
            MyDestroy(this.BrightPassEx);
            MyDestroy(this.GaussCompositionEx);
            MyDestroy(this.GlareCompositionEx);
            MyDestroy(this.MultipleGaussPassFilter);
            if (this.UseRadialBlur)
            {
                MyDestroy(this.RadialBlurMat);
            }
        }

        protected void ReleaseRenderTextures()
        {
            if (this.WriteAlpha)
            {
                GraphicsUtils.ReleaseRenderTexture(this.alpha_buffer);
            }
            GraphicsUtils.ReleaseRenderTexture(this.downsample4X_buffer);
            GraphicsUtils.ReleaseRenderTexture(this.gauss_buffer);
            if (this.UseDistortion)
            {
                GraphicsUtils.ReleaseRenderTexture(this.distortionMap);
            }
            if (this.UseRadialBlur)
            {
                GraphicsUtils.ReleaseRenderTexture(this.radial_blur_buffer);
                GraphicsUtils.ReleaseRenderTexture(this.radial_blur_buffer_temp);
            }
            for (int i = 0; i < 6; i++)
            {
                GraphicsUtils.ReleaseRenderTexture(this.compose_buffer[i]);
                GraphicsUtils.ReleaseRenderTexture(this.blur_bufferA[i]);
                GraphicsUtils.ReleaseRenderTexture(this.blur_bufferB[i]);
            }
        }

        private void ReportAutoDisable()
        {
            Debug.LogWarning("The image effect " + this + " has been disabled as it's not supported on the current platform.");
        }

        private void SetIdentityLut()
        {
            int num = 0x10;
            Color[] colors = new Color[((num * num) * num) * num];
            float num2 = 1f / ((1f * num) - 1f);
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    for (int k = 0; k < num; k++)
                    {
                        for (int m = 0; m < num; m++)
                        {
                            colors[((k + (i * num)) + ((m * num) * num)) + (((j * num) * num) * num)] = new Color(k * num2, m * num2, ((float) ((j * num) + i)) / ((num * num) - 1f), 1f);
                        }
                    }
                }
            }
            if (this.converted2DLut != null)
            {
                UnityEngine.Object.DestroyImmediate(this.converted2DLut);
            }
            this.converted2DLut = new Texture2D(num * num, num * num, TextureFormat.ARGB32, false);
            this.converted2DLut.SetPixels(colors);
            this.converted2DLut.Apply();
            this.basedOnTempTex = string.Empty;
        }

        protected virtual void Update()
        {
            if (this.internalBufferSize == InternalBufferSizeEnum.SIZE_128)
            {
                this.blurCoeff = new Vector4(0.3f, 0.3f, 0.26f, 0.15f);
            }
            else if (this.internalBufferSize == InternalBufferSizeEnum.SIZE_256)
            {
                this.blurCoeff = new Vector4(0.24f, 0.24f, 0.28f, 0.225f);
            }
            else
            {
                this.blurCoeff = new Vector4(0.18f, 0.18f, 0.3f, 0.3f);
            }
        }

        private bool ValidDimensions(Texture2D tex2d)
        {
            if (tex2d == null)
            {
                return false;
            }
            int height = tex2d.height;
            if (height != Mathf.FloorToInt(Mathf.Sqrt((float) tex2d.width)))
            {
                return false;
            }
            if (height != 0x10)
            {
                return false;
            }
            return true;
        }

        protected Camera _distortionCamera
        {
            get
            {
                if (this.__distortionCamera == null)
                {
                    foreach (Camera camera in base.GetComponentsInChildren<Camera>(true))
                    {
                        if (camera.name == _DISTORTION_CAMERA_NAME)
                        {
                            this.__distortionCamera = camera;
                            break;
                        }
                    }
                    if (this.__distortionCamera == null)
                    {
                        this.UseDistortion = false;
                    }
                }
                return this.__distortionCamera;
            }
        }

        private Shader _multipleGaussPassShader
        {
            get
            {
                if (this.lastInternalBufferSize != this.internalBufferSize)
                {
                    this.lastInternalBufferSize = this.internalBufferSize;
                    if ((this.MultipleGaussPassFilterShaderMap != null) && this.MultipleGaussPassFilterShaderMap.ContainsKey(this.internalBufferSize))
                    {
                        this.__multipleGaussPassShader = this.MultipleGaussPassFilterShaderMap[this.internalBufferSize];
                    }
                }
                return this.__multipleGaussPassShader;
            }
        }

        public float AvatarShadowAdjust
        {
            get
            {
                return (!this.FastMode ? 0f : this.avatarShadowAdjust);
            }
        }

        public bool SupportHDR
        {
            get
            {
                return this.supportHDRTextures;
            }
        }

        public bool UseRadialBlur
        {
            get
            {
                return (!this.DisableRadialBlur && (this.RadialBlurStrenth > float.Epsilon));
            }
        }

        public enum InternalBufferSizeEnum
        {
            NULL = 0,
            SIZE_128 = 0x80,
            SIZE_256 = 0x100,
            SIZE_512 = 0x200
        }

        public enum SizeDownScaleEnum
        {
            Div_1 = 1,
            Div_2 = 2,
            Div_3 = 3,
            Div_4 = 4,
            Div_5 = 5,
            Div_6 = 6,
            Div_7 = 7,
            Div_8 = 8
        }
    }
}

