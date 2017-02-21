namespace MoleMole
{
    using MoleMole.Config;
    using PigeonCoopToolkit.Effects.Trails;
    using System;
    using UnityEngine;

    public class Mono_BOSS_020 : BaseMonoBoss
    {
        [Header("Show Wing Trail Skill IDs")]
        public string[] ShowWingTrailSkillIDs;
        [Header("Wing Trails")]
        public SmoothTrail[] wingTrails;

        public override void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity)
        {
            if (hitEffect > AttackResult.AnimatorHitEffect.Light)
            {
                this.SetOverrideSteerFaceDirectionFrame(-hitForward);
            }
            base.BeHit(frameHalt, hitEffect, hitEffectAux, killEffect, beHitEffect, aniDamageRatio, hitForward, retreatVelocity);
        }

        protected override void PostInit()
        {
            base.PostInit();
            base.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.onCurrentSkillIDChanged, new Action<string, string>(this.ShowWingTrailBySkillID));
        }

        private void ShowWingTrailBySkillID(string from, string to)
        {
            if (Miscs.ArrayContains<string>(this.ShowWingTrailSkillIDs, to))
            {
                for (int i = 0; i < this.wingTrails.Length; i++)
                {
                    this.wingTrails[i].Emit = true;
                }
            }
            else
            {
                for (int j = 0; j < this.wingTrails.Length; j++)
                {
                    this.wingTrails[j].Emit = false;
                }
            }
        }
    }
}

