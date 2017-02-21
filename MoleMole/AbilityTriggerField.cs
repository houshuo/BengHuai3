namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AbilityTriggerField : TriggerFieldActor
    {
        private bool _isFieldFollowOwner;
        private BaseAbilityActor _owner;
        public MonoTriggerField triggerField;

        public override void Core()
        {
            base.Core();
            if ((this._isFieldFollowOwner && (this.triggerField != null)) && (this._owner != null))
            {
                this.triggerField.transform.position = this._owner.entity.transform.position;
            }
        }

        public List<uint> GetInsideRuntimeIDs()
        {
            return base._insideRuntimes;
        }

        public override void Init(BaseMonoEntity entity)
        {
            this.triggerField = (MonoTriggerField) entity;
            base.runtimeID = this.triggerField.GetRuntimeID();
        }

        public override void Kill()
        {
            base.Kill();
            this.triggerField.SetDied();
        }

        public void Setup(BaseAbilityActor owner, float uniformScale, MixinTargetting targetting, bool followOwner = false)
        {
            this._owner = owner;
            this.triggerField.SetCollisionMask(Singleton<EventManager>.Instance.GetAbilityTargettingMask(this._owner.runtimeID, targetting));
            Vector3 vector = (Vector3) (Vector3.one * uniformScale);
            vector.y = 1f;
            this.triggerField.transform.localScale = vector;
            this._isFieldFollowOwner = followOwner;
        }
    }
}

