namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilitySuddenTeleportMixin : BaseAbilityMixin
    {
        public SuddenTeleportMixin config;

        public AbilitySuddenTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (SuddenTeleportMixin) config;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
            if ((actor != null) && (actor.entity != null))
            {
                Vector3 vector = actor.entity.XZPosition + ((Vector3) ((Quaternion.AngleAxis(base.instancedAbility.Evaluate(this.config.Angle), Vector3.up) * actor.entity.transform.forward) * actor.commonConfig.CommonArguments.CollisionRadius));
                vector.y = 0f;
                base.entity.transform.position = vector;
                Vector3 vector2 = actor.entity.XZPosition - base.entity.XZPosition;
                vector2.y = 0f;
                base.entity.transform.forward = vector2;
            }
        }
    }
}

