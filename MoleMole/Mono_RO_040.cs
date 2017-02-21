namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class Mono_RO_040 : BaseMonoRobot
    {
        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
            if ((InLevelData.AVATAR_LAYER == collision.gameObject.layer) || (InLevelData.MONSTER_LAYER == collision.gameObject.layer))
            {
                BaseMonoEntity component;
                if (InLevelData.AVATAR_LAYER == collision.gameObject.layer)
                {
                    component = collision.gameObject.GetComponent<BaseMonoAvatar>();
                }
                else
                {
                    component = collision.gameObject.GetComponent<BaseMonoMonster>();
                }
                if (component != null)
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtTouch(base.GetRuntimeID(), component.GetRuntimeID()), MPEventDispatchMode.Normal);
                }
            }
        }

        public override void SetDied(KillEffect killEffect)
        {
            base.SetDied(KillEffect.KillImmediately);
        }
    }
}

