namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ShaderTransitionPlugin : BaseEntityFuncPlugin
    {
        private bool _active;
        private bool _dir;
        private float _duration;
        private float _from;
        private List<BaseMonoAnimatorEntity.SpecialStateMaterialData> _mats;
        private MonoBuffShader_SpecialTransition _shaderData;
        private float _time;
        private float _to;
        public const int EXECUTION_ORDER = 0x1f40;

        public ShaderTransitionPlugin(BaseMonoEntity entity) : base(entity)
        {
            this._mats = new List<BaseMonoAnimatorEntity.SpecialStateMaterialData>();
            this._dir = true;
            this._to = 1f;
            this._duration = 1f;
        }

        public override void Core()
        {
            if (this._active)
            {
                this._time += Time.deltaTime * base._entity.TimeScale;
                float num = Mathf.Lerp(this._from, this._to, this._time / this._duration);
                for (int i = 0; i < this._mats.Count; i++)
                {
                    this._mats[i].material.SetFloat(this._shaderData.TransitionName, num);
                }
                if (this._time > this._duration)
                {
                    this.EndTransition();
                }
            }
        }

        private void EndTransition()
        {
            this._active = false;
            if (!this._dir)
            {
                for (int i = 0; i < this._mats.Count; i++)
                {
                    this._mats[i].material.DisableKeyword(MonoBuffShader_SpecialTransition.DefaultShaderKeyword);
                }
            }
        }

        public override void FixedCore()
        {
        }

        public override bool IsActive()
        {
            return this._active;
        }

        public void StartTransition(List<BaseMonoAnimatorEntity.SpecialStateMaterialData> list, MonoBuffShader_SpecialTransition shaderData, bool dir)
        {
            this._mats = list;
            this._shaderData = shaderData;
            this._dir = dir;
            this._from = !this._dir ? 1f : 0f;
            this._to = !this._dir ? 0f : 1f;
            this._duration = !this._dir ? this._shaderData.SPExitDuration : this._shaderData.SPEnterDuration;
            this._active = true;
            this._time = 0f;
            for (int i = 0; i < this._mats.Count; i++)
            {
                Material mat = this._mats[i].material;
                mat.EnableKeyword(MonoBuffShader_SpecialTransition.DefaultShaderKeyword);
                this._shaderData.PushValue(ref mat);
                mat.SetFloat(this._shaderData.TransitionName, this._from);
            }
        }
    }
}

