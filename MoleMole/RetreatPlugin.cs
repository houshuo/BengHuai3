namespace MoleMole
{
    using System;
    using UnityEngine;

    public sealed class RetreatPlugin : BaseEntityFuncPlugin
    {
        private float _decelerateEndNormalizedTime;
        private float _decelerateInitVelocity;
        private string _namedRetreatName;
        private NamedStateRetreatState _namedRetreatState;
        private Vector3 _retreatDir;
        private IRetreatable _retreatEntity;
        private RetreatMode _retreatMode;
        private float _scaleVelocityScale;
        private int _scaleVelocityStackIx;
        private float _velocity;
        private bool active;
        private const float END_THRESHOLD = 0.1f;
        public const int EXECUTION_ORDER = 100;
        private const float LERP_RATIO = 28f;
        private const float RETREAT_DECELERATE = 7f;
        private const float RETREAT_WALL_KEEP_DISTANCE_TO_WALL = 0.3f;
        private const float RETREAT_WALL_OFFSET_DISTANCE = 0.8f;

        public RetreatPlugin(BaseMonoEntity entity) : base(entity)
        {
            this._retreatEntity = (IRetreatable) entity;
        }

        public void BlowDecelerateRetreat(Vector3 retreatDir, float velocity, string namedState, float velocityRatio, float endNormalizedTime)
        {
            if (this.active)
            {
                this.CancelActiveRetreat();
            }
            this._retreatMode = RetreatMode.BlowDecelerated;
            this._retreatDir = retreatDir;
            this._decelerateInitVelocity = this._velocity = velocity * velocityRatio;
            this._decelerateEndNormalizedTime = endNormalizedTime;
            this._namedRetreatName = namedState;
            this._namedRetreatState = NamedStateRetreatState.WaitingForState;
            this.active = true;
        }

        private void BlowDecelerateRetreatFixedCore()
        {
            if (this._namedRetreatState == NamedStateRetreatState.WaitingForState)
            {
                if (this._retreatEntity.GetCurrentNamedState() == this._namedRetreatName)
                {
                    this._namedRetreatState = NamedStateRetreatState.InState;
                }
            }
            else if (this._namedRetreatState == NamedStateRetreatState.InState)
            {
                this._velocity = Mathf.Lerp(this._decelerateInitVelocity, 0f, Mathf.Clamp01(this._retreatEntity.GetCurrentNormalizedTime() / this._decelerateEndNormalizedTime));
                this._retreatEntity.SetNeedOverrideVelocity(true);
                this._retreatEntity.SetOverrideVelocity((Vector3) ((this._velocity * this._retreatDir) * base._entity.TimeScale));
                if ((this._retreatEntity.GetCurrentNamedState() != this._namedRetreatName) || ((this._velocity >= -0.1f) && (this._velocity <= 0.1f)))
                {
                    this._retreatEntity.SetNeedOverrideVelocity(false);
                    this.active = false;
                }
            }
        }

        public void BlowVelocityScaledRetreat(Vector3 retreatDir, float velocity, string namedState, float velocityScale)
        {
            if (this.active)
            {
                this.CancelActiveRetreat();
            }
            this._retreatMode = RetreatMode.BlowVelocityScaled;
            this._retreatDir = retreatDir;
            this._velocity = velocity;
            this._namedRetreatName = namedState;
            this._scaleVelocityScale = velocityScale;
            this._namedRetreatState = NamedStateRetreatState.WaitingForState;
            this.active = true;
        }

        private void BlowVelocityScaledRetreatCore()
        {
            if (this._namedRetreatState == NamedStateRetreatState.WaitingForState)
            {
                if (this._retreatEntity.GetCurrentNamedState() == this._namedRetreatName)
                {
                    this._namedRetreatState = NamedStateRetreatState.InState;
                    this._scaleVelocityStackIx = this._retreatEntity.PushProperty("Animator_RigidBodyVelocityRatio", this._scaleVelocityScale);
                }
            }
            else if ((this._namedRetreatState == NamedStateRetreatState.InState) && (this._retreatEntity.GetCurrentNamedState() != this._namedRetreatName))
            {
                this._retreatEntity.PopProperty("Animator_RigidBodyVelocityRatio", this._scaleVelocityStackIx);
                this.active = false;
            }
        }

        public void CancelActiveRetreat()
        {
            if (this._retreatMode == RetreatMode.Retreat)
            {
                this._retreatEntity.SetNeedOverrideVelocity(false);
            }
            else if (this._retreatMode == RetreatMode.BlowVelocityScaled)
            {
                if (this._namedRetreatState == NamedStateRetreatState.InState)
                {
                    this._retreatEntity.PopProperty("Animator_RigidBodyVelocityRatio", this._scaleVelocityStackIx);
                }
            }
            else if (this._retreatMode == RetreatMode.BlowDecelerated)
            {
                this._retreatEntity.SetNeedOverrideVelocity(false);
            }
            this.active = false;
        }

        public override void Core()
        {
            if (this.active && (this._retreatMode == RetreatMode.BlowVelocityScaled))
            {
                this.BlowVelocityScaledRetreatCore();
            }
        }

        public override void FixedCore()
        {
            if (this.active)
            {
                if (this._retreatMode == RetreatMode.Retreat)
                {
                    this.StandRetreatFixedCore();
                }
                else if (this._retreatMode == RetreatMode.BlowDecelerated)
                {
                    this.BlowDecelerateRetreatFixedCore();
                }
            }
        }

        public override bool IsActive()
        {
            return this.active;
        }

        public void StandRetreat(Vector3 retreatDir, float velocity)
        {
            retreatDir.y = 0f;
            if (this.IsActive())
            {
                if (this._retreatMode == RetreatMode.Retreat)
                {
                    Vector3 vector = (Vector3) (this._retreatDir * this._velocity);
                    Vector3 vector2 = (Vector3) (retreatDir * velocity);
                    Vector3 vector3 = vector + vector2;
                    this._retreatDir.y = 0f;
                    this._retreatDir = vector3.normalized;
                    this._velocity = vector3.magnitude;
                }
                else
                {
                    this.CancelActiveRetreat();
                    this._retreatDir = retreatDir.normalized;
                    this._velocity = velocity;
                }
            }
            else
            {
                this._retreatDir = retreatDir.normalized;
                this._velocity = velocity;
            }
            this._retreatMode = RetreatMode.Retreat;
            this.active = true;
        }

        private void StandRetreatFixedCore()
        {
            this._retreatEntity.SetNeedOverrideVelocity(true);
            this._retreatEntity.SetOverrideVelocity((Vector3) ((this._retreatDir * this._velocity) * base._entity.TimeScale));
            this._velocity = Mathf.Lerp(this._velocity, 0f, (28f * Time.fixedDeltaTime) * base._entity.TimeScale);
            if ((this._velocity >= -0.1f) && (this._velocity <= 0.1f))
            {
                this._retreatEntity.SetNeedOverrideVelocity(false);
                this.active = false;
            }
        }

        private enum NamedStateRetreatState
        {
            WaitingForState,
            InState
        }

        private enum RetreatMode
        {
            Retreat,
            BlowVelocityScaled,
            BlowDecelerated
        }
    }
}

