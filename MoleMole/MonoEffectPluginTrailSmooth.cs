namespace MoleMole
{
    using PigeonCoopToolkit.Effects.Trails;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoEffectPluginTrailSmooth : MonoEffectPluginTrail
    {
        private int _controlPointIndex;
        private float _fixedDeltaTime = 0.01666667f;
        private SmoothTrail _smoothTrail;
        private float _timer;
        [Header("Set this to add overlay effect onto the moving trail anchor.")]
        public string EffectOverlayKey;
        [Header("Set this for entity overriding the first material of this smooth trail.")]
        public string MaterialOverrideKey;

        protected override void Awake()
        {
            base.Awake();
            this._smoothTrail = base.TrailRendererTransform.GetComponent<SmoothTrail>();
        }

        private bool GetPointPosition(ref Vector3 position)
        {
            if (this._controlPointIndex < base.FramePosList.Length)
            {
                if (this._controlPointIndex == -1)
                {
                    this._controlPointIndex++;
                    position = base.FramePosList[this._controlPointIndex];
                    return true;
                }
                if (this._timer > this._fixedDeltaTime)
                {
                    this._controlPointIndex++;
                    this._timer -= this._fixedDeltaTime;
                    if (this._controlPointIndex < base.FramePosList.Length)
                    {
                        position = base.FramePosList[this._controlPointIndex];
                        return true;
                    }
                }
            }
            return false;
        }

        public void HandleEffectOverride(MonoEffectOverride effectOverride)
        {
            if (!string.IsNullOrEmpty(this.MaterialOverrideKey) && effectOverride.materialOverrides.ContainsKey(this.MaterialOverrideKey))
            {
                this._smoothTrail.TrailData.TrailMaterials[0] = effectOverride.materialOverrides[this.MaterialOverrideKey];
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
            return ((base.TrailRendererTransform == null) || ((this._smoothTrail.NumSegments() == 0) && (this._timer != 0f)));
        }

        public override void SetDestroy()
        {
            this._controlPointIndex = base.FramePosList.Length;
        }

        public override void Setup()
        {
            base.Setup();
            this._smoothTrail.Emit = true;
            this._timer = 0f;
            this._controlPointIndex = -1;
            base.AniAnchorTransform.localPosition = base.FramePosList[0];
            base.TrailRendererTransform.localPosition = base.AniAnchorTransform.localPosition;
            if (this._smoothTrail != null)
            {
                this._smoothTrail.ClearSystem(true);
            }
        }

        protected override void Update()
        {
            float num = Time.timeScale * base._effect.TimeScale;
            this._timer += Time.deltaTime * num;
            Vector3 zero = Vector3.zero;
            this._smoothTrail.TimeScale = base._effect.TimeScale;
            if (this.GetPointPosition(ref zero))
            {
                base.AniAnchorTransform.localPosition = zero;
                base.TrailRendererTransform.localPosition = base.AniAnchorTransform.localPosition;
            }
        }
    }
}

