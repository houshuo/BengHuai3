namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class MonoBarrelProp : MonoHitableProp
    {
        private EntityTimer _destroyTimer;
        private bool _isActive;
        private State _state;
        public bool _toExplode = true;
        [Header("Will be destroyed after this seconds")]
        public float DestroyDelay = 0.5f;

        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
            this._isActive = true;
            this._destroyTimer = new EntityTimer(this.DestroyDelay, this);
            this._destroyTimer.Reset(false);
            this._state = State.Idle;
        }

        public override bool IsActive()
        {
            return this._isActive;
        }

        public override void SetDied(KillEffect killEffect)
        {
            base.hitbox.enabled = false;
            this._isActive = false;
            this._destroyTimer.Reset(true);
            if (((base.config.PropArguments.OnKillEffectPattern != null) && this._toExplode) && base.gameObject.activeSelf)
            {
                this.FireEffect(base.config.PropArguments.OnKillEffectPattern);
            }
        }

        protected override void Update()
        {
            base.UpdatePlugins();
            if (this._state == State.Idle)
            {
                this._destroyTimer.Core(1f);
                if (this._destroyTimer.isTimeUp)
                {
                    if ((base.config.PropArguments.OnDestroyEffectPattern != null) && base.gameObject.activeSelf)
                    {
                        this.FireEffect(base.config.PropArguments.OnDestroyEffectPattern);
                    }
                    if (!string.IsNullOrEmpty(base.config.PropArguments.AnimEventIDForHit) && this._toExplode)
                    {
                        string animEventIDForHit = base.config.PropArguments.AnimEventIDForHit;
                        ConfigPropAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(base.config, animEventIDForHit);
                        event2.AttackPattern.patternMethod(animEventIDForHit, event2.AttackPattern, this, (((int) 1) << InLevelData.MONSTER_HITBOX_LAYER) | (((int) 1) << InLevelData.AVATAR_HITBOX_LAYER));
                    }
                    this._destroyTimer.Reset(true);
                    this._destroyTimer.timespan = 0.1f;
                    this._state = State.WaitingForDestroy;
                }
            }
            else if (this._state == State.WaitingForDestroy)
            {
                this._destroyTimer.Core(1f);
                if (this._destroyTimer.isTimeUp)
                {
                    this._destroyTimer.Reset(false);
                    base._isToBeRemove = true;
                }
            }
        }

        private enum State
        {
            Idle,
            WaitingForDestroy
        }
    }
}

