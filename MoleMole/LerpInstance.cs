namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class LerpInstance
    {
        private bool _active;
        private E_ShaderData _dataType;
        private float _duration = 1f;
        private BaseMonoAnimatorEntity _entity;
        private List<BaseMonoAnimatorEntity.SpecialStateMaterialData> _mats;
        private MonoBuffShader_Lerp _shaderData;
        private ShaderLerpPlugin _shaderLerpPlugin;
        private float _time;
        private bool isEnableTransition;

        public LerpInstance(ShaderLerpPlugin plugin, BaseMonoAnimatorEntity entity, E_ShaderData dataType, List<BaseMonoAnimatorEntity.SpecialStateMaterialData> list, MonoBuffShader_Lerp shaderData, bool dir)
        {
            this._shaderLerpPlugin = plugin;
            this._entity = entity;
            this._dataType = dataType;
            this._mats = list;
            this._shaderData = shaderData;
            this.isEnableTransition = dir;
        }

        public void Core()
        {
            if (this._active)
            {
                this._time += Time.deltaTime * this._entity.TimeScale;
                float normalized = Mathf.Clamp01(this._time / this._duration);
                for (int i = 0; i < this._mats.Count; i++)
                {
                    Material targetMat = this._mats[i].material;
                    MaterialColorModifier.Multiplier colorMultiplier = this._mats[i].colorMultiplier;
                    if (this._dataType == E_ShaderData.InverseTimeSpace)
                    {
                        this._shaderData.Lerp<ShaderProperty_Shell>(targetMat, normalized, this.isEnableTransition);
                    }
                    else if (this._dataType == E_ShaderData.Transparent)
                    {
                        this._shaderData.Lerp<ShaderProperty_SpecialState>(targetMat, normalized, this.isEnableTransition);
                    }
                    else if (this._dataType == E_ShaderData.Distortion)
                    {
                        this._shaderData.Lerp<ShaderProperty_Distortion>(targetMat, normalized, this.isEnableTransition);
                    }
                    else if (this._dataType == E_ShaderData.ColorBias)
                    {
                        this._shaderData.Lerp<ShaderProperty_ColorBias>(colorMultiplier, normalized, this.isEnableTransition);
                    }
                    else
                    {
                        this._shaderData.Lerp<ShaderProperty_Rim>(targetMat, normalized, this.isEnableTransition);
                    }
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
            if (!this.isEnableTransition)
            {
                for (int i = 0; i < this._mats.Count; i++)
                {
                    Material material = this._mats[i].material;
                    if (this._shaderData.Keyword == "DISTORTION")
                    {
                        material.SetOverrideTag("Distortion", "None");
                    }
                    else
                    {
                        material.DisableKeyword(this._shaderData.Keyword);
                    }
                }
                if (!string.IsNullOrEmpty(this._shaderData.NewShaderName))
                {
                    int index = this._shaderLerpPlugin.PopFirstNewShaderEntryByShaderDataType(this._dataType);
                    this._entity.PopShaderStackByIndex(index);
                }
            }
        }

        public bool IsActive()
        {
            return this._active;
        }

        public void StartLerping()
        {
            this._duration = !this.isEnableTransition ? this._shaderData.DisableDuration : this._shaderData.EnableDuration;
            this._active = true;
            this._time = 0f;
            if (this._dataType == E_ShaderData.ColorBias)
            {
                for (int i = 0; i < this._mats.Count; i++)
                {
                    Material material = this._mats[i].material;
                    if (this.isEnableTransition)
                    {
                        Color color = !material.HasProperty("_MainColor") ? material.color : material.GetColor("_MainColor");
                        (this._shaderData.FromProperty as ShaderProperty_ColorBias).SetOriginalColor(color);
                        (this._shaderData.ToProperty as ShaderProperty_ColorBias).SetOriginalColor(color);
                    }
                }
            }
        }
    }
}

