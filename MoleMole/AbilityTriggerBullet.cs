namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AbilityTriggerBullet : BaseActor
    {
        private BaseAbilityActor _owner;
        public MonoTriggerBullet triggerBullet;

        public override void Init(BaseMonoEntity entity)
        {
            this.triggerBullet = (MonoTriggerBullet) entity;
            base.runtimeID = this.triggerBullet.GetRuntimeID();
        }

        public void Kill()
        {
            this.triggerBullet.SetDied();
            Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID), MPEventDispatchMode.Normal);
            this.triggerBullet.enabled = false;
        }

        public void Setup(BaseAbilityActor owner, float speed, MixinTargetting targetting, bool ignoreTimeScale, float aliveDuration = -1)
        {
            this._owner = owner;
            this.triggerBullet.SetCollisionMask(Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(this._owner.runtimeID, targetting));
            this.triggerBullet.speed = speed;
            this.triggerBullet.speedAdd = Vector3.zero;
            this.triggerBullet.AliveDuration = aliveDuration;
            this.triggerBullet.IgnoreTimeScale = ignoreTimeScale;
        }
    }
}

