namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEntityAudio : MonoBehaviour
    {
        public string bossBorn;
        public string monsterBorn;
        public string moveEvent;
        public string pickupCoin;
        public string pickupEquipItem;
        public string pickupHPHigh;
        public string pickupHPLow;
        public string ultraReady;
        public string witchTimeEvent;

        public void PostBossBorn()
        {
            this.SavePost(this.bossBorn);
        }

        public void PostMonsterBorn()
        {
            this.SavePost(this.monsterBorn);
        }

        public void PostMove()
        {
            this.SavePost(this.moveEvent);
        }

        public void PostPickupCoin()
        {
            this.SavePost(this.pickupCoin);
        }

        public void PostPickupEquipItem()
        {
            this.SavePost(this.pickupEquipItem);
        }

        public void PostPickupHPHigh()
        {
            this.SavePost(this.pickupHPHigh);
        }

        public void PostPickupHPLow()
        {
            this.SavePost(this.pickupHPLow);
        }

        public void PostUltraReady()
        {
            this.SavePost(this.ultraReady);
        }

        public void PostWitchTime()
        {
            this.SavePost(this.witchTimeEvent);
        }

        private void SavePost(string eventName)
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                Singleton<WwiseAudioManager>.Instance.Post(eventName, base.gameObject, null, null);
            }
        }
    }
}

