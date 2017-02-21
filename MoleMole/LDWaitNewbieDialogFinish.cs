namespace MoleMole
{
    using System;
    using UnityEngine;

    public class LDWaitNewbieDialogFinish : BaseLDEvent
    {
        private Transform newbie = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().transform.Find("Dialogs/NewbieDialog(Clone)");

        public override void Core()
        {
            if (Singleton<MainUIManager>.Instance.GetInLevelUICanvas().transform.Find("Dialogs/NewbieDialog(Clone)") == null)
            {
                base.Done();
            }
        }

        private void PlayBossBornSound()
        {
            MonoEntityAudio component = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetComponent<MonoEntityAudio>();
            if (component != null)
            {
                component.PostBossBorn();
            }
        }
    }
}

