namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityPullTargetMixin : BaseAbilityMixin
    {
        private BaseAbilityActor _pullActor;
        protected float _pullVelocity;
        private PullTargetMixin config;

        public AbilityPullTargetMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (PullTargetMixin) config;
            this._pullVelocity = instancedAbility.Evaluate(this.config.PullVelocity);
            this._pullActor = null;
        }

        public override void Core()
        {
            if (this._pullActor != null)
            {
                Vector3 vector = base.entity.transform.position - this._pullActor.entity.transform.position;
                if (vector.magnitude < base.instancedAbility.Evaluate(this.config.StopDistance))
                {
                    this._pullActor.entity.SetAdditiveVelocity(Vector3.zero);
                    this._pullActor.entity.SetHasAdditiveVelocity(false);
                    this._pullActor = null;
                }
                else
                {
                    vector.Normalize();
                    this._pullActor.entity.SetAdditiveVelocity((Vector3) (vector * base.instancedAbility.Evaluate(this.config.PullVelocity)));
                }
            }
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return false;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            BaseMonoEntity attackTarget = base.actor.entity.GetAttackTarget();
            if (attackTarget != null)
            {
                this._pullActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(attackTarget.GetRuntimeID());
            }
            if (this._pullActor != null)
            {
                this._pullActor.entity.SetHasAdditiveVelocity(true);
                Vector3 vector = base.entity.transform.position - attackTarget.transform.position;
                if (vector.magnitude < base.instancedAbility.Evaluate(this.config.PullRadius))
                {
                    vector.Normalize();
                    this._pullActor.entity.SetAdditiveVelocity((Vector3) (vector * base.instancedAbility.Evaluate(this.config.PullVelocity)));
                }
            }
        }

        public override void OnAdded()
        {
        }

        public override void OnRemoved()
        {
        }
    }
}

