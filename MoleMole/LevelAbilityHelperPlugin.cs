namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class LevelAbilityHelperPlugin : BaseActorPlugin
    {
        public string[] _attachedLevelBuffEffect;
        private LevelActor _levelActor;
        private List<ActorModifier>[] _levelBuffAttachedModifiers;

        public LevelAbilityHelperPlugin(LevelActor levelActor)
        {
            this._levelActor = levelActor;
            this._levelBuffAttachedModifiers = new List<ActorModifier>[2];
            for (int i = 0; i < this._levelBuffAttachedModifiers.Length; i++)
            {
                this._levelBuffAttachedModifiers[i] = new List<ActorModifier>();
            }
            this._attachedLevelBuffEffect = new string[2];
        }

        public void AddLevelBuffModifier(LevelBuffType levelBuffType, ActorModifier modifier)
        {
            this._levelBuffAttachedModifiers[(int) levelBuffType].Add(modifier);
        }

        public void AttachLevelBuffEffect(LevelBuffType levelBuffType, string effectPattern)
        {
            if (Singleton<StageManager>.Instance.GetPerpStage().rainController == null)
            {
                this._attachedLevelBuffEffect[(int) levelBuffType] = effectPattern;
                Singleton<EffectManager>.Instance.CreateUniqueIndexedEffectPattern(effectPattern, levelBuffType.ToString(), Singleton<LevelManager>.Instance.levelEntity);
            }
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtLevelBuffState) && this.OnLevelBuffState((EvtLevelBuffState) evt));
        }

        private bool OnLevelBuffState(EvtLevelBuffState evt)
        {
            if (evt.state == LevelBuffState.Stop)
            {
                List<ActorModifier> list = this._levelBuffAttachedModifiers[(int) evt.levelBuff];
                for (int i = 0; i < list.Count; i++)
                {
                    ActorModifier modifier = list[i];
                    if (modifier.owner != null)
                    {
                        modifier.owner.abilityPlugin.TryRemoveModifier(modifier);
                    }
                }
                list.Clear();
                if (this._attachedLevelBuffEffect[(int) evt.levelBuff] != null)
                {
                    Singleton<EffectManager>.Instance.SetDestroyUniqueIndexedEffectPattern(evt.levelBuff.ToString());
                    this._attachedLevelBuffEffect[(int) evt.levelBuff] = null;
                }
            }
            return true;
        }
    }
}

