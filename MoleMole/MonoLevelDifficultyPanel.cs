namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoLevelDifficultyPanel : MonoBehaviour
    {
        private ChapterDataItem _chapterData;
        private LevelDiffculty _currentDifficulty = LevelDiffculty.Normal;
        private List<LevelDiffculty> _difficultyList;
        private bool _popUpActive;
        public GameObject levelDifficultyGO;

        private void FireNotify(bool isBtnClick)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLevelDifficulty, this._currentDifficulty));
        }

        public void Init(LevelDiffculty difficulty, ChapterDataItem chapterData)
        {
            this._popUpActive = false;
            base.transform.Find("PopUp").gameObject.SetActive(this._popUpActive);
            base.transform.Find("Btn").gameObject.SetActive(this._popUpActive);
            this._currentDifficulty = difficulty;
            this._chapterData = chapterData;
            this._difficultyList = new List<LevelDiffculty>();
            this.InitPopUp();
            this.SetupView();
            this.FireNotify(true);
        }

        private void InitPopUp()
        {
            this._difficultyList = this._chapterData.GetLevelDifficultyListInChapter();
            Transform parent = base.transform.Find("PopUp");
            for (int i = 1; i < this._difficultyList.Count; i++)
            {
                Transform child;
                if (parent.childCount >= i)
                {
                    child = parent.GetChild(i - 1);
                }
                else
                {
                    GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(this.levelDifficultyGO);
                    obj2.name = "difficulty_" + i;
                    child = obj2.transform;
                    child.SetParent(parent, false);
                }
                child.GetComponent<MonoLevelDifficultyButton>().SetupClickCallBack(new Action<LevelDiffculty>(this.OnDifficultyBtnClick));
            }
            if (parent.childCount > (this._difficultyList.Count - 1))
            {
                for (int j = parent.childCount - 1; j >= this._difficultyList.Count; j--)
                {
                    UnityEngine.Object.Destroy(parent.GetChild(j));
                }
            }
        }

        private int LevelDifficultySort(LevelDiffculty lobj, LevelDiffculty robj)
        {
            if (lobj == robj)
            {
                return 0;
            }
            if (lobj == this._currentDifficulty)
            {
                return -1;
            }
            if (robj == this._currentDifficulty)
            {
                return 1;
            }
            return lobj.CompareTo(robj);
        }

        public void OnBGClick()
        {
            this._popUpActive = false;
            base.transform.Find("PopUp").gameObject.SetActive(this._popUpActive);
            base.transform.Find("Btn").gameObject.SetActive(this._popUpActive);
        }

        public void OnCurrentDifficultyClick()
        {
            if (this._difficultyList.Count < 2)
            {
                base.transform.Find("PopUp").gameObject.SetActive(false);
                base.transform.Find("Btn").gameObject.SetActive(false);
            }
            else
            {
                this._popUpActive = !this._popUpActive;
                bool flag = UnlockUIDataReaderExtend.UnLockByMission(3) && UnlockUIDataReaderExtend.UnlockByTutorial(3);
                bool flag2 = UnlockUIDataReaderExtend.UnLockByMission(4) && UnlockUIDataReaderExtend.UnlockByTutorial(4);
                if (this._difficultyList.Count > 2)
                {
                    bool flag3 = this._popUpActive && (flag || flag2);
                    base.transform.Find("PopUp").gameObject.SetActive(flag3);
                    base.transform.Find("Btn").gameObject.SetActive(flag3);
                    bool flag4 = flag3 && flag;
                    bool flag5 = flag3 && flag2;
                    if (((LevelDiffculty) this._difficultyList[0]) == LevelDiffculty.Normal)
                    {
                        base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(flag4);
                        base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(flag5);
                    }
                    else if (((LevelDiffculty) this._difficultyList[0]) == LevelDiffculty.Hard)
                    {
                        base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(this._popUpActive);
                        base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(flag5);
                    }
                    else
                    {
                        base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(this._popUpActive);
                        base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(flag4);
                    }
                }
                else
                {
                    bool flag6 = this._popUpActive && flag;
                    base.transform.Find("PopUp").gameObject.SetActive(flag6);
                    base.transform.Find("Btn").gameObject.SetActive(flag6);
                    if (((LevelDiffculty) this._difficultyList[0]) == LevelDiffculty.Normal)
                    {
                        base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(flag6);
                        base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(false);
                    }
                    else
                    {
                        base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(this._popUpActive);
                        base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(false);
                    }
                }
            }
        }

        public void OnDifficultyBtnClick(LevelDiffculty difficulty)
        {
            this._currentDifficulty = difficulty;
            this.SetupView();
            this.FireNotify(true);
            this.OnBGClick();
        }

        private void SetupDifficultyView(Transform trans, LevelDiffculty difficulty)
        {
            Color difficultyColor = Miscs.GetDifficultyColor(difficulty);
            string difficultyDesc = Miscs.GetDifficultyDesc(difficulty);
            string difficultyMark = UIUtil.GetDifficultyMark(difficulty);
            trans.Find("Color").GetComponent<Image>().color = difficultyColor;
            trans.Find("Desc").GetComponent<Text>().text = difficultyDesc;
            trans.Find("Icon/Image").GetComponent<Image>().color = difficultyColor;
            trans.Find("Icon/Text").GetComponent<Text>().text = difficultyMark;
        }

        private void SetupView()
        {
            this._difficultyList.Sort(new Comparison<LevelDiffculty>(this.LevelDifficultySort));
            this.SetupDifficultyView(base.transform.Find("Current"), this._currentDifficulty);
            bool flag = UnlockUIDataReaderExtend.UnLockByMission(3) && UnlockUIDataReaderExtend.UnlockByTutorial(3);
            bool flag2 = UnlockUIDataReaderExtend.UnLockByMission(4) && UnlockUIDataReaderExtend.UnlockByTutorial(4);
            Transform transform = base.transform.Find("Current/Arrow");
            if (this._difficultyList.Count > 2)
            {
                transform.gameObject.SetActive(flag || flag2);
            }
            else if (this._difficultyList.Count > 1)
            {
                transform.gameObject.SetActive(flag);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
            if (this._difficultyList.Count > 1)
            {
                Transform transform2 = base.transform.Find("PopUp");
                for (int i = 1; i < this._difficultyList.Count; i++)
                {
                    LevelDiffculty difficulty = this._difficultyList[i];
                    Transform child = transform2.GetChild(i - 1);
                    this.SetupDifficultyView(child, difficulty);
                    child.GetComponent<MonoLevelDifficultyButton>().SetupDifficulty(difficulty);
                }
            }
        }
    }
}

