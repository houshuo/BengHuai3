namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class MonsterMirrorActor : MonsterActor
    {
        public BaseMonoMonster owner;
        public MonsterActor ownerActor;

        public void InitFromMonsterActor(MonsterActor monsterActor, float hpRatioOfParent)
        {
            this.owner = monsterActor.monster;
            this.ownerActor = monsterActor;
            base.level = this.ownerActor.level;
            base.attack = this.ownerActor.attack;
            base.defense = this.ownerActor.defense;
            base.HP = base.maxHP = hpRatioOfParent * monsterActor.maxHP;
            base.monster.DisableShadow();
            Physics.IgnoreCollision(this.owner.transform.GetComponent<Collider>(), base.monster.transform.GetComponent<Collider>());
            base.monster.transform.position = this.owner.transform.position;
        }

        public override void Kill(uint killerID, string animEventID, KillEffect killEffect)
        {
            Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID, killerID, animEventID), MPEventDispatchMode.Normal);
        }

        public override void PostInit()
        {
            base.PostInit();
            base.abilityPlugin.muteEvents = true;
        }
    }
}

