namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class DrawShadow : MonoBehaviour
    {
        protected List<int> _oldLayers = new List<int>();
        protected List<Renderer> _renderers = new List<Renderer>();
        private float aspectRatio;
        [Header("Only work in high quality mode")]
        public BlurLevel blurLevel = BlurLevel.Lv4;
        public int CullingLayer = 0x100;
        [HideInInspector]
        public Shader DrawShadowShader;
        public bool HighQuality;
        private static readonly float HighQualityBlurShaderBaseImageSize = 512f;
        private RenderTextureFormat internalBufferFormat;
        private RenderTextureFormat internalBufferFormatHQ;
        protected bool isSupported = true;
        private static readonly float LowQualityBlurShaderBaseImageSize = 64f;
        private Material MultipleGaussPassFilter;
        private Material MultipleGaussPassFilterHQ;
        [HideInInspector]
        public Shader MultipleGaussPassFilterShader;
        [HideInInspector]
        public Shader MultipleGaussPassFilterShaderHQ;
        public int ReplacementLayer = 0x17;
        private RenderTextureWrapper temp_buffer;
        private RenderTextureWrapper[] temp_buffers;
        public bool useBlur = true;

        public void Awake()
        {
            Renderer[] componentsInChildren = base.transform.root.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                int layer = componentsInChildren[i].gameObject.layer;
                if (((((int) 1) << layer) & this.CullingLayer) != 0)
                {
                    this._renderers.Add(componentsInChildren[i]);
                    this._oldLayers.Add(layer);
                }
            }
        }

        private void blur(RenderTexture source, RenderTexture destination, RenderTexture temp, Material material, int level, float offsetScale = 1)
        {
            material.SetVector("_scaler", (Vector4) (new Vector2(((1f / this.aspectRatio) <= 1f) ? (1f / this.aspectRatio) : 1f, 0f) * offsetScale));
            Graphics.Blit(source, temp, material, level);
            material.SetVector("_scaler", (Vector4) (new Vector2(0f, (this.aspectRatio <= 1f) ? this.aspectRatio : 1f) * offsetScale));
            destination.DiscardContents();
            Graphics.Blit(temp, destination, material, level);
        }

        private bool CheckResources()
        {
            this.MultipleGaussPassFilter = this.CheckShaderAndCreateMaterial(this.MultipleGaussPassFilterShader, this.MultipleGaussPassFilter);
            if (this.HighQuality)
            {
                this.MultipleGaussPassFilterHQ = this.CheckShaderAndCreateMaterial(this.MultipleGaussPassFilterShaderHQ, this.MultipleGaussPassFilterHQ);
            }
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

        private void CreateRenderTextures(int w, int h)
        {
            if (!this.HighQuality)
            {
                this.temp_buffer = GraphicsUtils.GetRenderTexture(w, h, 0, this.internalBufferFormat);
            }
            else
            {
                this.temp_buffer = GraphicsUtils.GetRenderTexture(w, h, 0, this.internalBufferFormatHQ);
                this.temp_buffers = new RenderTextureWrapper[this.blurLevel];
                for (int i = 0; i < this.blurLevel; i++)
                {
                    this.temp_buffers[i] = GraphicsUtils.GetRenderTexture(w, h, 0, this.internalBufferFormatHQ);
                }
            }
        }

        private void DoBlur(RenderTexture source, RenderTexture destination)
        {
            if (!this.HighQuality)
            {
                float offsetScale = LowQualityBlurShaderBaseImageSize / ((float) source.width);
                this.blur(source, destination, (RenderTexture) this.temp_buffer, this.MultipleGaussPassFilter, 0, offsetScale);
            }
            else
            {
                float num2 = HighQualityBlurShaderBaseImageSize / ((float) source.width);
                this.blur(source, (RenderTexture) this.temp_buffers[0], (RenderTexture) this.temp_buffer, this.MultipleGaussPassFilterHQ, 0, num2);
                for (int i = 1; i < this.blurLevel; i++)
                {
                    num2 /= 2f;
                    this.blur((RenderTexture) this.temp_buffers[i - 1], (RenderTexture) this.temp_buffers[i], (RenderTexture) this.temp_buffer, this.MultipleGaussPassFilterHQ, i, num2);
                }
                Graphics.Blit((Texture) this.temp_buffers[((int) this.blurLevel) - 1], destination);
            }
        }

        private void NotSupported()
        {
            base.enabled = false;
            this.isSupported = false;
        }

        private void OnEnable()
        {
            this.isSupported = true;
        }

        private void OnPostRender()
        {
            for (int i = 0; i < this._renderers.Count; i++)
            {
                this._renderers[i].gameObject.layer = this._oldLayers[i];
            }
        }

        private void OnPreCull()
        {
            for (int i = 0; i < this._renderers.Count; i++)
            {
                this._renderers[i].gameObject.layer = this.ReplacementLayer;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (destination != null)
            {
                if (!this.CheckResources())
                {
                    Graphics.Blit(source, destination);
                }
                else
                {
                    this.aspectRatio = source.width;
                    this.aspectRatio /= (float) source.height;
                    this.CreateRenderTextures(source.width, source.height);
                    if (this.useBlur)
                    {
                        this.DoBlur(source, destination);
                    }
                    else
                    {
                        Graphics.Blit(source, destination);
                    }
                    this.ReleaseRenderTextures();
                }
            }
        }

        private void ReleaseRenderTextures()
        {
            if (this.temp_buffer != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this.temp_buffer);
                this.temp_buffer = null;
            }
            if (this.HighQuality)
            {
                for (int i = 0; i < this.blurLevel; i++)
                {
                    if (this.temp_buffers[i] != null)
                    {
                        GraphicsUtils.ReleaseRenderTexture(this.temp_buffers[i]);
                        this.temp_buffers[i] = null;
                    }
                }
            }
        }

        private void ReportAutoDisable()
        {
            Debug.LogWarning("The image effect " + this + " has been disabled as it's not supported on the current platform.");
        }

        public enum BlurLevel
        {
            Lv1 = 1,
            Lv2 = 2,
            Lv3 = 3,
            Lv4 = 4
        }
    }
}

