namespace MoleMole
{
    using System;

    public class AbilityGoodActor : BaseGoodsActor
    {
        public float abilityArgument;
        public string abilityName;

        public override void DoGoodsLogic(uint avatarRuntimeID)
        {
            Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(base._entity.ownerID).abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig(this.abilityName), avatarRuntimeID, this.abilityArgument);
            base.Kill();
        }
    }
}

