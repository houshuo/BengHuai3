namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class MonoSwitchProp : MonoHitableProp
    {
        private bool _isActive;
        [Header("Alive Transform")]
        public Transform AliveTransform;
        [Header("Killed Transform")]
        public Transform KilledTransform;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
            this._isActive = true;
            this.AliveTransform.gameObject.SetActive(true);
            this.KilledTransform.gameObject.SetActive(false);
            Singleton<PropObjectManager>.Instance.RegisterDestroyOnStageChange(runtimeID);
        }

        public override bool IsActive()
        {
            return this._isActive;
        }

        public override void SetDied(KillEffect killEffect)
        {
            this._isActive = false;
            base.hitbox.enabled = false;
            base.SetCountedDenySelect(true, true);
            this.AliveTransform.gameObject.SetActive(false);
            this.KilledTransform.gameObject.SetActive(true);
            if (base.config.PropArguments.OnKillEffectPattern != null)
            {
                this.FireEffect(base.config.PropArguments.OnKillEffectPattern);
            }
        }
    }
}

