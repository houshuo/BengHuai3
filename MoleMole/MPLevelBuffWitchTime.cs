namespace MoleMole
{
    using System;

    public class MPLevelBuffWitchTime : LevelBuffWitchTime
    {
        public MPLevelBuffWitchTime(MPLevelActor mpLevelActor) : base(mpLevelActor)
        {
        }

        protected override void ActStartParticleEffect()
        {
            if (this.IsPvP() || (base.levelBuffSide != LevelBuffSide.FromAvatar))
            {
                if (!base._notStartEffect)
                {
                    if (Singleton<MPManager>.Instance.GetIdentity<AvatarIdentity>(base.ownerID).isAuthority)
                    {
                        base.ActBlueOpenEffect();
                    }
                    else
                    {
                        base.ActRedOpenEffect();
                    }
                }
            }
            else
            {
                base.ActStartParticleEffect();
            }
        }

        protected override void ActStopParticleEffect()
        {
            if (this.IsPvP() || (base.levelBuffSide != LevelBuffSide.FromAvatar))
            {
                if (Singleton<MPManager>.Instance.GetIdentity<AvatarIdentity>(base.ownerID).isAuthority)
                {
                    base.ActBlueCloseEffect();
                }
                else
                {
                    base.ActRedCloseEffect();
                }
            }
            else
            {
                base.ActStopParticleEffect();
            }
        }

        protected override void ApplyWitchTimeSlowedBySide()
        {
            if (this.IsPvP() || (base.levelBuffSide != LevelBuffSide.FromAvatar))
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(base.ownerID);
                BaseAbilityActor[] enemyActorsOf = Singleton<LevelManager>.Instance.gameMode.GetEnemyActorsOf<BaseAbilityActor>(actor);
                for (int i = 0; i < enemyActorsOf.Length; i++)
                {
                    base.ApplyWitchTimeEffect(enemyActorsOf[i].runtimeID);
                }
            }
            else
            {
                base.ApplyWitchTimeSlowedBySide();
            }
        }

        public override void ApplyWitchTimeSlowedBySideWithRuntimeID(uint runtimeID)
        {
            if (this.IsPvP() || (base.levelBuffSide != LevelBuffSide.FromAvatar))
            {
                base.ApplyWitchTimeEffect(runtimeID);
            }
            else
            {
                base.ApplyWitchTimeSlowedBySideWithRuntimeID(runtimeID);
            }
        }

        private bool IsPvP()
        {
            return (Singleton<LevelManager>.Instance.gameMode is NetworkedMP_PvPTest_GameMode);
        }

        protected override void PushRenderingDataBySide()
        {
            if (this.IsPvP() || (base.levelBuffSide != LevelBuffSide.FromAvatar))
            {
                if (Singleton<MPManager>.Instance.GetIdentity<AvatarIdentity>(base.ownerID).isAuthority)
                {
                    base.PushBlueRenderingData();
                }
                else
                {
                    base.PushRedRenderingData();
                }
            }
            else
            {
                base.PushRenderingDataBySide();
            }
        }

        public override bool Refresh(float duration, LevelBuffSide side, uint ownerID, bool enteringTimeSlow, bool useMaxDuration, bool notStartEffect)
        {
            if (!this.IsPvP() && (base.levelBuffSide == LevelBuffSide.FromAvatar))
            {
                return base.Refresh(duration, side, ownerID, enteringTimeSlow, useMaxDuration, notStartEffect);
            }
            if ((ownerID != base.ownerID) && Singleton<LevelManager>.Instance.gameMode.IsEnemy(ownerID, base.ownerID))
            {
                this.SwitchSide(enteringTimeSlow, duration, LevelBuffSide.FromAvatar, ownerID, notStartEffect);
                return true;
            }
            base.ExtendDuration(duration, enteringTimeSlow, useMaxDuration);
            return false;
        }

        protected override void RemoveWitchTimeSlowedBySide()
        {
            if (this.IsPvP() || (base.levelBuffSide != LevelBuffSide.FromAvatar))
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(base.ownerID);
                BaseAbilityActor[] enemyActorsOf = Singleton<LevelManager>.Instance.gameMode.GetEnemyActorsOf<BaseAbilityActor>(actor);
                for (int i = 0; i < enemyActorsOf.Length; i++)
                {
                    base.RemoveWitchTimeEffect(enemyActorsOf[i].runtimeID);
                }
            }
            else
            {
                base.RemoveWitchTimeSlowedBySide();
            }
        }
    }
}

