namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BaseMonoDarkAvatar : BaseMonoMonster
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map4;

        protected BaseMonoDarkAvatar()
        {
        }

        public override void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity)
        {
            if ((hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow) || (hitEffect == AttackResult.AnimatorHitEffect.ThrowDown))
            {
                hitEffect = AttackResult.AnimatorHitEffect.KnockDown;
                retreatVelocity *= 0.1f;
            }
            base.BeHit(frameHalt, hitEffect, hitEffectAux, killEffect, beHitEffect, aniDamageRatio, hitForward, retreatVelocity);
        }

        [AnimationCallback]
        protected void ClearSkillTriggers()
        {
        }

        private string ConvertTrigger(string name)
        {
            string key = name;
            if (key != null)
            {
                int num;
                if (<>f__switch$map4 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
                    dictionary.Add("HitTrigger", 0);
                    dictionary.Add("ThrowBlowTrigger", 1);
                    dictionary.Add("ATKTrigger", 2);
                    dictionary.Add("ThrowDownTrigger", 3);
                    dictionary.Add("ThrowTrigger", 3);
                    <>f__switch$map4 = dictionary;
                }
                if (<>f__switch$map4.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            name = "TriggerHit";
                            return name;

                        case 1:
                            name = "TriggerKnockDown";
                            return name;

                        case 2:
                            name = "TriggerAttack";
                            return name;

                        case 3:
                            name = null;
                            return name;
                    }
                }
            }
            return name;
        }

        protected override void PostInit()
        {
            object obj2;
            object obj3;
            base.PostInit();
            base.config.DynamicArguments.TryGetValue("MuteAttachWeapon", out obj2);
            if ((obj2 == null) || !((bool) obj2))
            {
                int weaponID = (int) base.config.DynamicArguments["WeaponID"];
                string avatarType = (string) base.config.DynamicArguments["AvatarType"];
                WeaponData.WeaponModelAndEffectAttach(weaponID, avatarType, this);
            }
            base.config.DynamicArguments.TryGetValue("MuteDarkAvatarShader", out obj3);
            if ((obj3 == null) || !((bool) obj3))
            {
                ConfigBaseRenderingData renderingDataConfig = RenderingData.GetRenderingDataConfig<ConfigBaseRenderingData>("Basic_DarkAvatar");
                for (int i = 0; i < base._matListForSpecailState.Count; i++)
                {
                    RenderingData.ApplyRenderingData(renderingDataConfig, base._matListForSpecailState[i].material);
                }
            }
            this.TriggerAppear();
        }

        public override void ResetTrigger(string name)
        {
            name = this.ConvertTrigger(name);
            if (name != null)
            {
                base.ResetTrigger(name);
            }
        }

        [AnimationCallback]
        protected void RunOnLeftFoot()
        {
        }

        [AnimationCallback]
        protected void RunOnRightFoot()
        {
        }

        public override void SetEliteShader()
        {
        }

        [AnimationCallback]
        public void SetLevelComboTimerState(int state)
        {
        }

        public override void SetTrigger(string name)
        {
            name = this.ConvertTrigger(name);
            if (name != null)
            {
                base.SetTrigger(name);
            }
        }

        [AnimationCallback]
        private void TimeSlowTrigger(float time)
        {
        }

        public virtual void TriggerAppear()
        {
            this.SetTrigger("TriggerSwitchIn");
        }

        [AnimationCallback]
        private void TriggerCameraPullFar(float time)
        {
        }

        [AnimationCallback]
        private void TriggerCameraPullFurther(float time)
        {
        }

        [AnimationCallback]
        private void TriggerCameraPushNear(float time)
        {
        }

        [AnimationCallback]
        private void TriggerTintCamera(float duration)
        {
        }
    }
}

