namespace MoleMole
{
    using System;

    public class MonoHimekoKZ : MonoHimeko
    {
        public override void TriggerSkill(int skillNum)
        {
            if (skillNum == 1)
            {
                base.SetLocomotionBool("EvadeBackward", !base.GetActiveControlData().hasSteer, false);
            }
            base.TriggerSkill(skillNum);
        }
    }
}

