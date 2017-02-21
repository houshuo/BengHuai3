namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ClipFadeInPlugin : BaseEntityFuncPlugin
    {
        private bool _active;
        private float _clipMaxHeight;
        private float _fadeTime;
        private Material[] _materialList;
        private Shader[] _originShaders;
        private float _timer;
        private string EFFECT_SHADER_PATH;
        public const int EXECUTION_ORDER = 0x238e;

        public ClipFadeInPlugin(BaseMonoEntity entity) : base(entity)
        {
            this.EFFECT_SHADER_PATH = "miHoYo/Character/Plane_Clip";
        }

        public override void Core()
        {
            if (this._active)
            {
                this._timer += Time.deltaTime * base._entity.TimeScale;
                this.LerpHeigh(Mathf.Lerp(-this._clipMaxHeight, 0f, this._timer / this._fadeTime));
                if (this._timer > this._fadeTime)
                {
                    this.EndFade();
                }
            }
        }

        private void EndFade()
        {
            this._active = false;
            int index = 0;
            int length = this._materialList.Length;
            while (index < length)
            {
                this._materialList[index].shader = this._originShaders[index];
                index++;
            }
            this._originShaders = null;
        }

        public override void FixedCore()
        {
        }

        public override bool IsActive()
        {
            return this._active;
        }

        private void LerpHeigh(float height)
        {
            foreach (Material material in this._materialList)
            {
                material.SetVector("_ClipPlane", new Vector4(0f, 1f, 0f, height));
            }
        }

        public void StartFade(float clipMaxHeight, float time)
        {
            this._clipMaxHeight = clipMaxHeight;
            this._fadeTime = time;
            this._active = true;
            this._materialList = ((IFadeOff) base._entity).GetAllMaterials();
            this._originShaders = new Shader[this._materialList.Length];
            int index = 0;
            int length = this._materialList.Length;
            while (index < length)
            {
                Material material = this._materialList[index];
                this._originShaders[index] = material.shader;
                material.shader = Shader.Find(this.EFFECT_SHADER_PATH);
                material.SetVector("_ClipPlane", new Vector4(0f, 1f, 0f, -this._clipMaxHeight));
                index++;
            }
        }
    }
}

