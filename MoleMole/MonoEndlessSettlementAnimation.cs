namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEndlessSettlementAnimation : MonoBehaviour
    {
        public void OnAnimationEnd()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessSettlementAnimationEnd, null));
        }
    }
}

