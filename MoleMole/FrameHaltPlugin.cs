namespace MoleMole
{
    using System;
    using UnityEngine;

    public sealed class FrameHaltPlugin : BaseEntityFuncPlugin
    {
        private IFrameHaltable _frameHaltEntity;
        private bool _halting;
        private float _haltTime;
        public const int EXECUTION_ORDER = 0x5f;
        private const float FRAME_HALT_SPEED = 0f;

        public FrameHaltPlugin(BaseMonoEntity entity) : base(entity)
        {
            this._frameHaltEntity = (IFrameHaltable) entity;
        }

        public override void Core()
        {
            if (this._halting)
            {
                this._haltTime -= Time.unscaledDeltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                if (this._haltTime < 0f)
                {
                    this._halting = false;
                    this._frameHaltEntity.timeScaleStack.Pop(5);
                }
            }
        }

        public override void FixedCore()
        {
        }

        public void FrameHalt(int frameNum)
        {
            float b = frameNum * 0.01666667f;
            if (this._halting)
            {
                this._haltTime = Mathf.Max(this._haltTime, b);
            }
            else
            {
                this._haltTime = b;
                this._halting = true;
                this._frameHaltEntity.timeScaleStack.Push(5, 0f, false);
            }
        }

        public override bool IsActive()
        {
            return (this._haltTime > 0f);
        }
    }
}

