namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoPlayerExpGrow : MonoBehaviour
    {
        public MonoMaskSliderGrow slider;

        public void PlayPlayerExpSliderGrow()
        {
            int playerLevelBefore = Singleton<LevelScoreManager>.Instance.playerLevelBefore;
            int playerExpBefore = Singleton<LevelScoreManager>.Instance.playerExpBefore;
            int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
            int teamExp = Singleton<PlayerModule>.Instance.playerData.teamExp;
            this.slider.Play((float) playerExpBefore, (float) teamExp, UIUtil.GetPlayerMaxExpList(playerLevelBefore, teamLevel), new Action<Transform>(this.ShowLevelUpHint), new Action<Transform>(this.ShowLevelUpDialog));
        }

        private void ShowLevelUpDialog(Transform sliderTrans)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PlayerLevelUp, null));
        }

        private void ShowLevelUpHint(Transform sliderTrans)
        {
            base.transform.Find("LevelUpHint").GetComponent<Animation>().Play();
        }
    }
}

