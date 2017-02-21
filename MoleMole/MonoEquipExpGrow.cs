namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoEquipExpGrow : MonoBehaviour
    {
        private int addTime = 1;
        private int exp;
        private int expBefore;
        public string levelAudioName;
        private int levelBefore;
        private List<float> maxList;
        public MonoMaskSliderGrow slider;

        public void PlayEquipExpSliderGrow()
        {
            this.slider.Play((float) this.expBefore, (float) this.exp, this.maxList, new Action<Transform>(this.ShowLevelUpHint), null);
        }

        public void SetData(int levelBefore, int maxExpBefore, int expBefore, int exp, List<float> maxList)
        {
            this.levelBefore = levelBefore;
            this.expBefore = expBefore;
            this.exp = exp;
            this.maxList = maxList;
            base.transform.Find("LevelLabel").GetComponent<Text>().text = "Lv." + levelBefore.ToString();
            base.transform.Find("Exp/NumText").GetComponent<Text>().text = expBefore.ToString();
            base.transform.Find("Exp/MaxNumText").GetComponent<Text>().text = maxExpBefore.ToString();
            base.transform.Find("Exp/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue((float) expBefore, (float) maxExpBefore, 0f);
        }

        private void ShowLevelUpHint(Transform sliderTrans)
        {
            base.transform.Find("LevelLabel").GetComponent<Text>().text = "Lv." + ((this.levelBefore + this.addTime)).ToString();
            this.addTime++;
            base.transform.Find("LevelUpHint").GetComponent<Animation>().Play();
            if (!string.IsNullOrEmpty(this.levelAudioName))
            {
                Singleton<WwiseAudioManager>.Instance.Post(this.levelAudioName, null, null, null);
            }
        }
    }
}

