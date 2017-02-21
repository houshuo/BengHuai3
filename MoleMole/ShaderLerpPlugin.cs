namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UniRx;

    public class ShaderLerpPlugin : BaseEntityFuncPlugin
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        private List<LerpInstance> _lerpInstances;
        private List<Tuple<E_ShaderData, int>> _newShaderEntries;
        public const int EXECUTION_ORDER = 0x1f41;

        public ShaderLerpPlugin(BaseMonoAnimatorEntity entity) : base(entity)
        {
            this._lerpInstances = new List<LerpInstance>();
            this._newShaderEntries = new List<Tuple<E_ShaderData, int>>();
            this._animatorEntity = entity;
        }

        public override void Core()
        {
            for (int i = this._lerpInstances.Count - 1; i >= 0; i--)
            {
                LerpInstance item = this._lerpInstances[i];
                item.Core();
                if (!item.IsActive())
                {
                    this._lerpInstances.Remove(item);
                }
            }
        }

        public override void FixedCore()
        {
        }

        public override bool IsActive()
        {
            return (this._lerpInstances.Count > 0);
        }

        public int PopFirstNewShaderEntryByShaderDataType(E_ShaderData dataType)
        {
            for (int i = 0; i < this._newShaderEntries.Count; i++)
            {
                Tuple<E_ShaderData, int> tuple = this._newShaderEntries[i];
                if ((tuple != null) && (((E_ShaderData) tuple.Item1) == dataType))
                {
                    this._newShaderEntries[i] = null;
                    return tuple.Item2;
                }
            }
            return -1;
        }

        public void StartLerp(E_ShaderData dataType, List<BaseMonoAnimatorEntity.SpecialStateMaterialData> list, MonoBuffShader_Lerp shaderData, bool dir, int shaderIx)
        {
            LerpInstance item = new LerpInstance(this, this._animatorEntity, dataType, list, shaderData, dir);
            if (shaderIx != -1)
            {
                int num = this._newShaderEntries.SeekAddPosition<Tuple<E_ShaderData, int>>();
                this._newShaderEntries[num] = Tuple.Create<E_ShaderData, int>(dataType, shaderIx);
            }
            this._lerpInstances.Add(item);
            item.StartLerping();
        }
    }
}

