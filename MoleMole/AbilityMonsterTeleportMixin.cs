namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterTeleportMixin : BaseAbilityMixin
    {
        private float _baselineDistance;
        private bool _isTeleporting;
        private BaseMonoMonster _monster;
        private RaycastHit _teleportHit;
        private float _teleportInterval;
        private Vector3 _teleportOffset;
        private EntityTimer _teleportTimer;
        private MonsterTeleportMixin config;

        public AbilityMonsterTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterTeleportMixin) config;
            this._baselineDistance = instancedAbility.Evaluate(this.config.BaselineDistance);
            this._monster = (BaseMonoMonster) base.entity;
            this._isTeleporting = false;
            this._teleportInterval = this.config.TeleportInverval;
            this._teleportTimer = new EntityTimer(this._teleportInterval);
            this._teleportTimer.Reset(false);
        }

        public override void Core()
        {
            base.Core();
            if (this._isTeleporting && this._teleportTimer.isActive)
            {
                this._teleportTimer.Core(1f);
                if (this._teleportTimer.isTimeUp)
                {
                    this._teleportTimer.Reset(false);
                    this.EndTeleport(this._teleportOffset);
                }
            }
        }

        private void EndTeleport(Vector3 offset)
        {
            this._isTeleporting = false;
            Transform transform = this._monster.transform;
            transform.position += offset;
            base.FireMixinEffect(this.config.TeleportToEffect, this._monster, false);
            this._teleportOffset = Vector3.zero;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            float abilityArgument = 1f;
            if (evt.abilityArgument != null)
            {
                abilityArgument = (float) evt.abilityArgument;
            }
            float signedDistance = abilityArgument * this._baselineDistance;
            if (this._monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.IdleOrMovement))
            {
                this.Teleport(signedDistance);
            }
        }

        private void Teleport(float signedDistance)
        {
            float num = Mathf.Sign(signedDistance);
            float maxDistance = Mathf.Abs(signedDistance);
            Vector3 xZPosition = this._monster.XZPosition;
            xZPosition.y = 1.1f;
            Vector3 direction = (this._monster.AttackTarget == null) ? this._monster.FaceDirection : (this._monster.AttackTarget.XZPosition - this._monster.XZPosition);
            if (!this.config.towardsTarget)
            {
                direction = -direction;
            }
            direction = (Vector3) (direction * num);
            if (Physics.Raycast(xZPosition, direction, out this._teleportHit, maxDistance, ((((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER)) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)))
            {
                maxDistance = this._teleportHit.distance;
            }
            float createCollisionRadius = this._monster.config.CommonArguments.CreateCollisionRadius;
            base.FireMixinEffect(this.config.TeleportFromEffect, this._monster, false);
            this._isTeleporting = true;
            this._teleportTimer.Reset(true);
            float num4 = maxDistance - createCollisionRadius;
            this._teleportOffset = (Vector3) (num4 * direction.normalized);
        }
    }
}

