namespace MoleMole
{
    using PigeonCoopToolkit.Effects.Trails;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoEffectPluginTrailStatic : MonoEffectPluginTrail
    {
        private StaticTrail _staticTrail;
        private float _timer;
        public AnimationCurve appearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float appearDuration = 1f;
        [Header("Set this to add overlay effect onto the moving trail anchor.")]
        public string EffectOverlayKey;
        public bool generateDefaultDuration = true;
        [Header("Set this for entity overriding the first material of this smooth trail.")]
        public string MaterialOverrideKey;
        public float TimeScale = 1f;
        public AnimationCurve vanishCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float vanishDuration = 1f;

        protected override void Awake()
        {
            base.Awake();
            this._staticTrail = base.TrailRendererTransform.GetComponent<StaticTrail>();
            this._staticTrail.Init(base.FramePosList);
            if (this.generateDefaultDuration)
            {
                this.appearDuration = (base.FramePosList.Length * 0.01666667f) * 1.2f;
                this.vanishDuration = this.appearDuration;
            }
        }

        public void HandleEffectOverride(MonoEffectOverride effectOverride)
        {
            if (!string.IsNullOrEmpty(this.MaterialOverrideKey) && effectOverride.materialOverrides.ContainsKey(this.MaterialOverrideKey))
            {
                if (this._staticTrail.TrailData.TrailMaterials[0].name.EndsWith("(Instance)"))
                {
                    if (Application.isEditor)
                    {
                        UnityEngine.Object.DestroyImmediate(this._staticTrail.TrailData.TrailMaterials[0]);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(this._staticTrail.TrailData.TrailMaterials[0]);
                    }
                }
                this._staticTrail.TrailData.TrailMaterials[0] = new Material(effectOverride.materialOverrides[this.MaterialOverrideKey]);
                Material material1 = this._staticTrail.TrailData.TrailMaterials[0];
                material1.name = material1.name + "(Instance)";
                string tag = effectOverride.materialOverrides[this.MaterialOverrideKey].GetTag("Distortion", false);
                this._staticTrail.TrailData.TrailMaterials[0].SetOverrideTag("Distortion", tag);
                this._staticTrail.ResetAnimation(this.appearDuration, this.appearCurve, this.vanishDuration, this.vanishCurve);
            }
            if (!string.IsNullOrEmpty(this.EffectOverlayKey) && effectOverride.effectOverlays.ContainsKey(this.EffectOverlayKey))
            {
                List<MonoEffect> list;
                Singleton<EffectManager>.Instance.TriggerEntityEffectPatternRaw(effectOverride.effectOverlays[this.EffectOverlayKey], base.AniAnchorTransform.position, base.AniAnchorTransform.forward, base.AniAnchorTransform.localScale, base._effect.owner, out list);
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].transform.SetParent(base.AniAnchorTransform, true);
                }
            }
        }

        public override bool IsToBeRemove()
        {
            return ((base.TrailRendererTransform == null) || !this._staticTrail.IsActive);
        }

        private void SetAniAnchor()
        {
            base.AniAnchorTransform.position = this._staticTrail.GetPointAlongTrail(this._timer / this.appearDuration);
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
            base.Setup();
            base.AniAnchorTransform.localPosition = Vector3.zero;
            base.TrailRendererTransform.localPosition = base.AniAnchorTransform.localPosition;
            this._timer = 0f;
            this._staticTrail.ResetAnimation(this.appearDuration, this.appearCurve, this.vanishDuration, this.vanishCurve);
            this.SetAniAnchor();
        }

        protected override void Update()
        {
            float deltaTime = (Time.deltaTime * base._effect.TimeScale) * this.TimeScale;
            this._timer += deltaTime;
            this._staticTrail.PlayAnimation(deltaTime);
            this.SetAniAnchor();
        }

        public StaticTrail staticTrail
        {
            get
            {
                return this._staticTrail;
            }
        }
    }
}

