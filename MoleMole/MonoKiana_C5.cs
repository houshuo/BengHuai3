namespace MoleMole
{
    using System;

    public class MonoKiana_C5 : MonoKiana
    {
        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
        }

        public override void TriggerSkill(int skillNum)
        {
            if (skillNum == 1)
            {
                base.SetLocomotionBool("EvadeBackward", !base.GetActiveControlData().hasSteer, false);
            }
            base.TriggerSkill(skillNum);
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}

