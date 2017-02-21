namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginOverrideHandler : BaseMonoEffectPlugin
    {
        private static MaterialPropertyBlock _emptyBlock = new MaterialPropertyBlock();
        [HideInInspector]
        public string[] effectOverlays = Miscs.EMPTY_STRINGS;
        [HideInInspector]
        public ParticleSystemStartColorOverride[] particleSystemStartColorOverrides = ParticleSystemStartColorOverride.EMPTY;
        [HideInInspector]
        public RendererColorOverride[] rendererColorOverrides = RendererColorOverride.EMPTY;
        [HideInInspector]
        public RendererFloatOverride[] rendererFloatOverrides = RendererFloatOverride.EMPTY;
        [HideInInspector]
        public RendererMaterialOverride[] rendererOverrides = RendererMaterialOverride.EMPTY;

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < this.rendererOverrides.Length; i++)
            {
                RendererMaterialOverride @override = this.rendererOverrides[i];
                @override.originalMaterial = @override.renderer.sharedMaterial;
            }
            for (int j = 0; j < this.rendererColorOverrides.Length; j++)
            {
                RendererColorOverride override2 = this.rendererColorOverrides[j];
                override2.originalColor = override2.renderer.sharedMaterial.GetColor(override2.colorPropertyKey);
            }
            for (int k = 0; k < this.rendererFloatOverrides.Length; k++)
            {
                RendererFloatOverride override3 = this.rendererFloatOverrides[k];
                override3.originalValue = override3.renderer.sharedMaterial.GetFloat(override3.floatPropertyKey);
            }
            for (int m = 0; m < this.particleSystemStartColorOverrides.Length; m++)
            {
                ParticleSystemStartColorOverride override4 = this.particleSystemStartColorOverrides[m];
                override4.originalStartColor = override4.particleSystem.startColor;
            }
        }

        public void HandleEffectOverride(MonoEffectOverride effectOverride)
        {
            for (int i = 0; i < this.rendererOverrides.Length; i++)
            {
                Material material;
                RendererMaterialOverride @override = this.rendererOverrides[i];
                if (effectOverride.materialOverrides.TryGetValue(@override.materialOverrideKey, out material))
                {
                    if (@override.renderer is ParticleSystemRenderer)
                    {
                        GraphicsUtils.CreateAndAssignInstancedMaterial(@override.renderer, material);
                    }
                    else
                    {
                        @override.renderer.sharedMaterial = material;
                    }
                }
                else if (@override.renderer is ParticleSystemRenderer)
                {
                    if (GraphicsUtils.IsInstancedMaterial(@override.renderer.sharedMaterial))
                    {
                        GraphicsUtils.CreateAndAssignInstancedMaterial(@override.renderer, @override.originalMaterial);
                    }
                    else
                    {
                        @override.renderer.sharedMaterial = @override.originalMaterial;
                    }
                }
                else
                {
                    @override.renderer.sharedMaterial = @override.originalMaterial;
                }
            }
            for (int j = 0; j < this.effectOverlays.Length; j++)
            {
                string str;
                if (effectOverride.effectOverlays.TryGetValue(this.effectOverlays[j], out str))
                {
                    Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(str, base.transform.position, base.transform.forward, base.transform.localScale, base._effect.owner);
                }
            }
            MaterialPropertyBlock dest = new MaterialPropertyBlock();
            for (int k = 0; k < this.rendererColorOverrides.Length; k++)
            {
                Color color;
                RendererColorOverride override2 = this.rendererColorOverrides[k];
                if (effectOverride.colorOverrides.TryGetValue(override2.colorOverrideKey, out color))
                {
                    if (override2.renderer is ParticleSystemRenderer)
                    {
                        if (!GraphicsUtils.IsInstancedMaterial(override2.renderer.sharedMaterial))
                        {
                            GraphicsUtils.CreateAndAssignInstancedMaterial(override2.renderer, override2.renderer.sharedMaterial);
                        }
                        override2.renderer.sharedMaterial.SetColor(override2.colorPropertyKey, color);
                    }
                    else
                    {
                        dest.Clear();
                        override2.renderer.GetPropertyBlock(dest);
                        dest.SetColor(override2.colorPropertyKey, effectOverride.colorOverrides[override2.colorOverrideKey]);
                        override2.renderer.SetPropertyBlock(dest);
                    }
                }
            }
            for (int m = 0; m < this.rendererFloatOverrides.Length; m++)
            {
                float num5;
                RendererFloatOverride override3 = this.rendererFloatOverrides[m];
                if (effectOverride.floatOverrides.TryGetValue(override3.floatOverrideKey, out num5))
                {
                    if (override3.renderer is ParticleSystemRenderer)
                    {
                        if (!GraphicsUtils.IsInstancedMaterial(override3.renderer.sharedMaterial))
                        {
                            GraphicsUtils.CreateAndAssignInstancedMaterial(override3.renderer, override3.renderer.sharedMaterial);
                        }
                        override3.renderer.sharedMaterial.SetFloat(override3.floatPropertyKey, num5);
                    }
                    else
                    {
                        dest.Clear();
                        override3.renderer.GetPropertyBlock(dest);
                        dest.SetFloat(override3.floatPropertyKey, effectOverride.floatOverrides[override3.floatOverrideKey]);
                        override3.renderer.SetPropertyBlock(dest);
                    }
                }
            }
            for (int n = 0; n < this.particleSystemStartColorOverrides.Length; n++)
            {
                ParticleSystemStartColorOverride override4 = this.particleSystemStartColorOverrides[n];
                if (override4.type == ParticleSystemStartColorOverride.StartColorType.ConstColor1)
                {
                    Color color2;
                    if (effectOverride.colorOverrides.TryGetValue(override4.colorOverrideKey1, out color2))
                    {
                        override4.particleSystem.startColor = color2;
                    }
                }
                else
                {
                    Color color3;
                    Color color4;
                    if (((override4.type == ParticleSystemStartColorOverride.StartColorType.RandomBetweenColor12) && effectOverride.colorOverrides.TryGetValue(override4.colorOverrideKey1, out color3)) && effectOverride.colorOverrides.TryGetValue(override4.colorOverrideKey2, out color4))
                    {
                        override4.particleSystem.startColor = Color.Lerp(color3, color4, UnityEngine.Random.value);
                    }
                }
            }
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < this.rendererOverrides.Length; i++)
            {
                RendererMaterialOverride @override = this.rendererOverrides[i];
                GraphicsUtils.TryCleanRendererInstancedMaterial(@override.renderer);
            }
            for (int j = 0; j < this.rendererColorOverrides.Length; j++)
            {
                RendererColorOverride override2 = this.rendererColorOverrides[j];
                if (override2 != null)
                {
                    GraphicsUtils.TryCleanRendererInstancedMaterial(override2.renderer);
                }
            }
            for (int k = 0; k < this.rendererFloatOverrides.Length; k++)
            {
                RendererFloatOverride override3 = this.rendererFloatOverrides[k];
                if (override3 != null)
                {
                    GraphicsUtils.TryCleanRendererInstancedMaterial(override3.renderer);
                }
            }
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
            base.Setup();
            _emptyBlock.Clear();
            for (int i = 0; i < this.rendererColorOverrides.Length; i++)
            {
                RendererColorOverride @override = this.rendererColorOverrides[i];
                if (@override != null)
                {
                    if (@override.renderer is ParticleSystemRenderer)
                    {
                        if (GraphicsUtils.IsInstancedMaterial(@override.renderer.sharedMaterial))
                        {
                            @override.renderer.sharedMaterial.SetColor(@override.colorPropertyKey, @override.originalColor);
                        }
                    }
                    else
                    {
                        @override.renderer.SetPropertyBlock(_emptyBlock);
                    }
                }
            }
            for (int j = 0; j < this.rendererFloatOverrides.Length; j++)
            {
                RendererFloatOverride override2 = this.rendererFloatOverrides[j];
                if (override2 != null)
                {
                    if (override2.renderer is ParticleSystemRenderer)
                    {
                        if (GraphicsUtils.IsInstancedMaterial(override2.renderer.sharedMaterial))
                        {
                            override2.renderer.sharedMaterial.SetFloat(override2.floatPropertyKey, override2.originalValue);
                        }
                    }
                    else
                    {
                        override2.renderer.SetPropertyBlock(_emptyBlock);
                    }
                }
            }
            for (int k = 0; k < this.particleSystemStartColorOverrides.Length; k++)
            {
                ParticleSystemStartColorOverride override3 = this.particleSystemStartColorOverrides[k];
                override3.particleSystem.startColor = override3.originalStartColor;
            }
        }

        [Serializable]
        public class ParticleSystemStartColorOverride
        {
            public string colorOverrideKey1;
            public string colorOverrideKey2;
            public static MonoEffectPluginOverrideHandler.ParticleSystemStartColorOverride[] EMPTY = new MonoEffectPluginOverrideHandler.ParticleSystemStartColorOverride[0];
            [NonSerialized]
            public Color originalStartColor;
            public ParticleSystem particleSystem;
            public StartColorType type;

            public enum StartColorType
            {
                ConstColor1,
                RandomBetweenColor12
            }
        }

        [Serializable]
        public class RendererColorOverride
        {
            public string colorOverrideKey;
            public string colorPropertyKey = "_TintColor";
            public static MonoEffectPluginOverrideHandler.RendererColorOverride[] EMPTY = new MonoEffectPluginOverrideHandler.RendererColorOverride[0];
            [NonSerialized]
            public Color originalColor;
            public Renderer renderer;
        }

        [Serializable]
        public class RendererFloatOverride
        {
            public static MonoEffectPluginOverrideHandler.RendererFloatOverride[] EMPTY = new MonoEffectPluginOverrideHandler.RendererFloatOverride[0];
            public string floatOverrideKey;
            public string floatPropertyKey;
            [NonSerialized]
            public float originalValue;
            public Renderer renderer;
        }

        [Serializable]
        public class RendererMaterialOverride
        {
            public static MonoEffectPluginOverrideHandler.RendererMaterialOverride[] EMPTY = new MonoEffectPluginOverrideHandler.RendererMaterialOverride[0];
            public string materialOverrideKey;
            [NonSerialized]
            public Material originalMaterial;
            public Renderer renderer;
        }
    }
}

