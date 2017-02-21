namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MonoChapterButton : MonoBehaviour
    {
        private ChapterDataItem _chapterData;
        private int _hard_star;
        private int _hard_sum;
        private int _hell_star;
        private int _hell_sum;
        private readonly int _maxStar = 4;
        private int _normal_star;
        private int _normal_sum;

        private void Awake()
        {
            base.transform.Find("Button").GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
        }

        private void InitChapterProgress()
        {
            if (this._chapterData.Unlocked)
            {
                this._normal_star = 0;
                List<LevelDataItem> levelList = this._chapterData.GetLevelList(LevelDiffculty.Normal);
                for (int i = 0; i < levelList.Count; i++)
                {
                    LevelDataItem item = levelList[i];
                    if (item.progress > 0)
                    {
                        this._normal_star++;
                    }
                    for (int m = 0; m < item.challengeList.Count; m++)
                    {
                        if (item.challengeList[m].Finished)
                        {
                            this._normal_star++;
                        }
                    }
                }
                this._normal_sum = levelList.Count * this._maxStar;
                this._hard_star = 0;
                List<LevelDataItem> list2 = this._chapterData.GetLevelList(LevelDiffculty.Hard);
                for (int j = 0; j < list2.Count; j++)
                {
                    LevelDataItem item2 = list2[j];
                    if (item2.progress > 0)
                    {
                        this._hard_star++;
                    }
                    for (int n = 0; n < item2.challengeList.Count; n++)
                    {
                        if (item2.challengeList[n].Finished)
                        {
                            this._hard_star++;
                        }
                    }
                }
                this._hard_sum = list2.Count * this._maxStar;
                this._hell_star = 0;
                List<LevelDataItem> list3 = this._chapterData.GetLevelList(LevelDiffculty.Hell);
                for (int k = 0; k < list3.Count; k++)
                {
                    LevelDataItem item3 = list3[k];
                    if (item3.progress > 0)
                    {
                        this._hell_star++;
                    }
                    for (int num6 = 0; num6 < item3.challengeList.Count; num6++)
                    {
                        if (item3.challengeList[num6].Finished)
                        {
                            this._hell_star++;
                        }
                    }
                }
                this._hell_sum = list3.Count * this._maxStar;
            }
        }

        public void OnClick()
        {
            MonoChapterScroller component = base.transform.parent.parent.GetComponent<MonoChapterScroller>();
            if (component.IsCenter(base.transform))
            {
                if (this._chapterData.Unlocked)
                {
                    Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(this._chapterData), UIType.Page);
                }
            }
            else
            {
                component.ClickToChangeCenter(base.transform);
            }
        }

        private void SetProgress()
        {
            base.transform.Find("Icon/Progres").gameObject.SetActive(this._chapterData.Unlocked);
            if (this._chapterData.Unlocked)
            {
                float num = ((float) this._normal_star) / ((float) this._normal_sum);
                base.transform.Find("Icon/Progres/Normal/Progress").GetComponent<Image>().fillAmount = num;
                Text component = base.transform.Find("Icon/Progres/Normal/Text").GetComponent<Text>();
                int num2 = (this._normal_star != this._normal_sum) ? ((int) (num * 100f)) : 100;
                component.text = string.Format("{0}%", num2);
                float num3 = ((float) this._hard_star) / ((float) this._hard_sum);
                base.transform.Find("Icon/Progres/Hard/Progress").GetComponent<Image>().fillAmount = num3;
                Text text2 = base.transform.Find("Icon/Progres/Hard/Text").GetComponent<Text>();
                num2 = (this._hard_star != this._hard_sum) ? ((int) (num3 * 100f)) : 100;
                text2.text = string.Format("{0}%", num2);
                float num4 = ((float) this._hell_star) / ((float) this._hell_sum);
                base.transform.Find("Icon/Progres/Torment/Progress").GetComponent<Image>().fillAmount = num4;
                Text text3 = base.transform.Find("Icon/Progres/Torment/Text").GetComponent<Text>();
                num2 = (this._hell_star != this._hell_sum) ? ((int) (num4 * 100f)) : 100;
                text3.text = string.Format("{0}%", num2);
            }
        }

        public void SetupView(ChapterDataItem chapterData)
        {
            this._chapterData = chapterData;
            base.transform.Find("Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._chapterData.CoverPic);
            base.transform.Find("Icon/Lock").gameObject.SetActive(!this._chapterData.Unlocked);
            GameObject gameObject = base.transform.Find("Button").gameObject;
            if (gameObject != null)
            {
                MonoButtonWwiseEvent component = gameObject.GetComponent<MonoButtonWwiseEvent>();
                if (component == null)
                {
                    component = gameObject.AddComponent<MonoButtonWwiseEvent>();
                }
                component.eventName = !this._chapterData.Unlocked ? "UI_Gen_Select_Negative" : "UI_Click";
            }
            this.InitChapterProgress();
            this.SetProgress();
        }

        public void UpdateView(bool isSelected)
        {
            this.selected = isSelected;
            base.transform.Find("Icon/Mask").gameObject.SetActive(!isSelected);
            base.transform.Find("Icon/Lock/BG").gameObject.SetActive(isSelected);
            base.transform.Find("Arrow").gameObject.SetActive(isSelected);
            base.transform.Find("Frame").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            base.transform.Find("Frame/Top").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            base.transform.Find("Frame/Bottom").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            base.transform.Find("Frame/Left").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            base.transform.Find("Frame/Right").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            this.SetProgress();
        }

        public bool selected { get; private set; }
    }
}

