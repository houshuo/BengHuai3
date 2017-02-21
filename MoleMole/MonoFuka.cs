namespace MoleMole
{
    using System;

    public class MonoFuka : BaseMonoAvatar
    {
        protected const string ATTACK_INDEX_NAME = "_AttackIndex";

        public override void Awake()
        {
            base.Awake();
        }

        public void SetPoseLeft()
        {
            base.SetLocomotionBool("_IsLeft", true, false);
        }

        public void SetPoseRight()
        {
            base.SetLocomotionBool("_IsLeft", false, false);
        }

        public void SetStateAir()
        {
            base.SetLocomotionBool("_IsAir", true, false);
        }

        public void SetStateGround()
        {
            base.SetLocomotionBool("_IsAir", false, false);
        }

        public override void SetTrigger(string name)
        {
            if (name == "TriggerAttack")
            {
                base.SetLocomotionInteger("_AttackIndex", 0, false);
            }
            else if (name == "TriggerSkill_2")
            {
                name = "TriggerAttack";
                base.SetLocomotionInteger("_AttackIndex", 2, false);
            }
            base.SetTrigger(name);
        }
    }
}

