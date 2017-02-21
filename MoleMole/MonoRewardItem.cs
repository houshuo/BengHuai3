namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoRewardItem : MonoBehaviour
    {
        private RewardUIData _rewardData;

        public void SetupView(RewardUIData rewardData)
        {
            this._rewardData = rewardData;
            if (rewardData == null)
            {
                base.gameObject.SetActive(false);
            }
            else
            {
                base.transform.Find("Number").GetComponent<Text>().text = this._rewardData.value.ToString();
                base.transform.Find("Icon").GetComponent<Image>().sprite = this._rewardData.GetIconSprite();
            }
        }
    }
}

