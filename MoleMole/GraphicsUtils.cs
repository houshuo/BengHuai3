namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    public static class GraphicsUtils
    {
        private static RenderTextureWrapperPool _renderTextureWrapperPool = new RenderTextureWrapperPool();
        public static bool isDisableRenderTexture = false;

        public static void CreateAndAssignInstancedMaterial(Renderer renderer, Material targetMaterial)
        {
            if (IsInstancedMaterial(renderer.sharedMaterial))
            {
                renderer.sharedMaterial.CopyPropertiesFromMaterial(targetMaterial);
            }
            else
            {
                Material material = new Material(targetMaterial) {
                    name = targetMaterial.name + "#copy#",
                    shaderKeywords = targetMaterial.shaderKeywords
                };
                renderer.sharedMaterial = material;
            }
        }

        public static RenderTextureWrapper GetRenderTexture(RenderTextureWrapper.Param param)
        {
            RenderTextureWrapper item = _renderTextureWrapperPool.GetItem();
            item.Create(param);
            return item;
        }

        public static RenderTextureWrapper GetRenderTexture(int width, int height, int depth)
        {
            RenderTextureWrapper.Param param = new RenderTextureWrapper.Param {
                width = width,
                height = height,
                depth = depth,
                format = RenderTextureFormat.ARGB32,
                readWrite = RenderTextureReadWrite.Default
            };
            return GetRenderTexture(param);
        }

        public static RenderTextureWrapper GetRenderTexture(int width, int height, int depth, RenderTextureFormat format)
        {
            RenderTextureWrapper.Param param = new RenderTextureWrapper.Param {
                width = width,
                height = height,
                depth = depth,
                format = format,
                readWrite = RenderTextureReadWrite.Default
            };
            return GetRenderTexture(param);
        }

        public static RenderTextureWrapper GetRenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite)
        {
            RenderTextureWrapper.Param param = new RenderTextureWrapper.Param {
                width = width,
                height = height,
                depth = depth,
                format = format,
                readWrite = readWrite
            };
            return GetRenderTexture(param);
        }

        public static int GetRenderTextureNumber()
        {
            return _renderTextureWrapperPool.GetUsedCount();
        }

        public static bool IsInstancedMaterial(Material mat)
        {
            return ((mat != null) && mat.name.EndsWith("#copy#"));
        }

        public static void RebindAllRenderTexturesToCamera()
        {
            for (int i = 0; i < _renderTextureWrapperPool.GetUsedCount(); i++)
            {
                if (!_renderTextureWrapperPool.usedList[i].IsCreated())
                {
                    _renderTextureWrapperPool.usedList[i].RebindToCamera();
                }
            }
        }

        public static void ReleaseRenderTexture(RenderTextureWrapper wrapper)
        {
            if (wrapper != null)
            {
                _renderTextureWrapperPool.ReleaseItem(wrapper);
            }
        }

        public static void SetShaderBloomMaxBlendParams()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2)
            {
                Shader.SetGlobalInt("_GlobalBloomMaxBlendSrc", 1);
                Shader.SetGlobalInt("_GlobalBloomMaxBlendDst", 0);
                Shader.SetGlobalInt("_GlobalBloomMaxBlendOp", 0);
            }
            else
            {
                Shader.SetGlobalInt("_GlobalBloomMaxBlendSrc", 1);
                Shader.SetGlobalInt("_GlobalBloomMaxBlendDst", 1);
                Shader.SetGlobalInt("_GlobalBloomMaxBlendOp", 4);
            }
        }

        public static void TryCleanRendererInstancedMaterial(Renderer renderer)
        {
            if (((renderer != null) && (renderer.sharedMaterial != null)) && IsInstancedMaterial(renderer.sharedMaterial))
            {
                UnityEngine.Object.Destroy(renderer.sharedMaterial);
            }
        }

        public static void WarmupAllShaders()
        {
            Shader.WarmupAllShaders();
        }
    }
}

