namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class MonoLevelView : MonoBehaviour
    {
        private int _challengeNumNeed;
        private bool _enterTimesEnough;
        private int _levelNeed;
        private bool _playAni;
        private int _playerLevel;
        private Image _prefixLineImage;
        private float _timer;
        private float _timerSpan;
        private int _totalFinishChallengeNum;
        private LevelBtnClickCallBack btnCallBack;
        private const float ENTER_TIME_LACK_LEVEL_ALPHA = 0.8f;
        private LevelDataItem levelData;
        private const string MATERIAL_GREY_PATH = "Material/LevelGrey";

        protected void BindViewCallback(Transform trans, EventTriggerType eventType, Action<BaseEventData> callback)
        {
            <BindViewCallback>c__AnonStorey10D storeyd = new <BindViewCallback>c__AnonStorey10D {
                callback = callback
            };
            MonoEventTrigger component = trans.gameObject.GetComponent<MonoEventTrigger>();
            if (component == null)
            {
                component = trans.gameObject.AddComponent<MonoEventTrigger>();
            }
            component.ClearTriggers();
            EventTrigger.Entry entry = new EventTrigger.Entry {
                eventID = eventType
            };
            entry.callback.AddListener(new UnityAction<BaseEventData>(storeyd.<>m__1C4));
            component.AddTrigger(entry);
        }

        private void EnterTimesLack()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_LevelEnterTimeLack", new object[0]), 2f), UIType.Any);
        }

        private bool HasCg()
        {
            return (Singleton<CGModule>.Instance.GetCgDataItemList().Find(x => x.levelID == this.levelData.levelId) != null);
        }

        private bool HasPlot()
        {
            return (PlotMetaDataReader.GetItemList().Find(x => x.levelID == this.levelData.levelId) != null);
        }

        private void OnBtnClick()
        {
            if (this.levelData.status != 1)
            {
                if (!this._enterTimesEnough)
                {
                    this.EnterTimesLack();
                }
                else if (this._levelNeed > this._playerLevel)
                {
                    object[] replaceParams = new object[] { this._levelNeed };
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ActivityLock", replaceParams), 2f), UIType.Any);
                }
                else if (this.btnCallBack != null)
                {
                    this.btnCallBack(this.levelData);
                }
            }
        }

        public void PlayNewUnlockAnimation(float timerSpan = 0.5f)
        {
            if (this._prefixLineImage != null)
            {
                this._timer = 0f;
                this._timerSpan = timerSpan;
                this._playAni = true;
            }
            else
            {
                base.transform.GetComponent<Animation>().Play();
            }
        }

        public void SetBGColor(string colorCode)
        {
            base.transform.Find("Active/BG").GetComponent<Image>().color = UIUtil.SetupColor(colorCode);
        }

        public void SetupView(LevelDataItem levelData, bool enterTimesEnough, LevelBtnClickCallBack callBack = null, int totalFinishChallengeNum = 0)
        {
            this.levelData = levelData;
            this.btnCallBack = callBack;
            this._enterTimesEnough = enterTimesEnough;
            this._levelNeed = levelData.UnlockPlayerLevel;
            this._playerLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
            this._challengeNumNeed = levelData.UnlockChanllengeNum;
            this._totalFinishChallengeNum = totalFinishChallengeNum;
            base.transform.Find("LevelLack").gameObject.SetActive(false);
            base.transform.Find("ChallengeLack").gameObject.SetActive(false);
            base.transform.Find("BattleType").gameObject.SetActive(!levelData.IsNormalBattleType);
            if (!levelData.IsNormalBattleType)
            {
                base.transform.Find("BattleType/Type").GetComponent<Image>().sprite = levelData.GetBattleTypeSprite();
            }
            if (levelData.SectionId > 1)
            {
                string name = "LevelLinesVia/" + levelData.SectionId.ToString();
                this._prefixLineImage = base.transform.parent.parent.Find(name).GetComponent<Image>();
                float num = 0f;
                if ((levelData.status != 1) && !Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Contains(levelData.levelId))
                {
                    num = 1f;
                }
                this._prefixLineImage.fillAmount = num;
            }
            if (!this._enterTimesEnough)
            {
                Material material = Miscs.LoadResource<Material>("Material/LevelGrey", BundleType.RESOURCE_FILE);
                base.transform.Find("Active/Pic").GetComponent<Image>().material = material;
                base.transform.Find("Active/Pic").GetComponent<CanvasGroup>().alpha = 0.8f;
            }
            else
            {
                base.transform.Find("Active/Pic").GetComponent<Image>().material = null;
                base.transform.Find("Active/Pic").GetComponent<CanvasGroup>().alpha = 1f;
            }
            base.transform.Find("Story").gameObject.SetActive(false);
            bool flag = this.HasPlot() || this.HasCg();
            switch (levelData.LevelType)
            {
                case 1:
                    if (levelData.status != 1)
                    {
                        base.transform.Find("Active").gameObject.SetActive(true);
                        base.transform.Find("Unactive").gameObject.SetActive(false);
                        base.transform.Find("Active/ActivityLevelName").gameObject.SetActive(false);
                        base.transform.Find("Active/ChapterName").gameObject.SetActive(true);
                        base.transform.Find("Active/ChapterName").GetComponent<Text>().text = levelData.StageName;
                        for (int i = 0; i < levelData.challengeList.Count; i++)
                        {
                            Transform child = base.transform.Find("Active/Badges").GetChild(i);
                            if (child != null)
                            {
                                bool finished = levelData.challengeList[i].Finished;
                                child.Find("Achieve").gameObject.SetActive(finished);
                                child.Find("Unachieve").gameObject.SetActive(!finished);
                            }
                        }
                        base.transform.Find("Active/Caution").gameObject.SetActive(false);
                        base.transform.Find("Active/Pic").GetComponent<Image>().sprite = levelData.GetBriefPicSprite();
                        base.transform.Find("Story").gameObject.SetActive(flag);
                        break;
                    }
                    base.transform.Find("Active").gameObject.SetActive(false);
                    base.transform.Find("Unactive").gameObject.SetActive(true);
                    base.transform.Find("Unactive/ChapterName").GetComponent<Text>().text = levelData.StageName;
                    if (levelData.SectionId != 1)
                    {
                    }
                    break;

                case 2:
                case 3:
                case 5:
                {
                    base.transform.Find("Active").gameObject.SetActive(true);
                    base.transform.Find("Unactive").gameObject.SetActive(false);
                    base.transform.Find("Active/ChapterName").gameObject.SetActive(false);
                    base.transform.Find("Active/ActivityLevelName").gameObject.SetActive(true);
                    base.transform.Find("Active/ActivityLevelName").GetComponent<Text>().text = levelData.StageName;
                    for (int j = 0; j < levelData.challengeList.Count; j++)
                    {
                        Transform transform2 = base.transform.Find("Active/Badges").GetChild(j);
                        if (transform2 != null)
                        {
                            bool flag3 = levelData.challengeList[j].Finished;
                            transform2.Find("Achieve").gameObject.SetActive(flag3);
                            transform2.Find("Unachieve").gameObject.SetActive(!flag3);
                        }
                    }
                    base.transform.Find("Active/Caution").gameObject.SetActive(false);
                    base.transform.Find("BattleType/Type").GetComponent<Image>().sprite = levelData.GetBattleTypeSprite();
                    base.transform.Find("Active/Pic").GetComponent<Image>().sprite = levelData.GetBriefPicSprite();
                    bool flag4 = levelData.LevelType == 5;
                    base.transform.Find("Active/BG").GetComponent<Image>().color = !flag4 ? Color.white : MiscData.GetColor("Blue");
                    base.transform.Find("Outter").GetComponent<Image>().color = !flag4 ? MiscData.GetColor("Blue") : MiscData.GetColor("Purple");
                    break;
                }
            }
            string str2 = null;
            if (levelData.status != 1)
            {
                if (this._levelNeed > this._playerLevel)
                {
                    base.transform.Find("LevelLack").gameObject.SetActive(true);
                    base.transform.Find("LevelLack/LevelNeed").GetComponent<Text>().text = "Lv." + this._levelNeed;
                    Material material2 = Miscs.LoadResource<Material>("Material/LevelGrey", BundleType.RESOURCE_FILE);
                    base.transform.Find("Active/Pic").GetComponent<Image>().material = material2;
                    str2 = "UI_Gen_Select_Negative";
                }
                else if (this._challengeNumNeed > this._totalFinishChallengeNum)
                {
                    base.transform.Find("ChallengeLack").gameObject.SetActive(true);
                    base.transform.Find("ChallengeLack/Num").GetComponent<Text>().text = "x" + this._challengeNumNeed;
                }
            }
            else
            {
                str2 = "UI_Gen_Select_Negative";
            }
            if (!string.IsNullOrEmpty(str2))
            {
                MonoButtonWwiseEvent component = base.transform.Find("Btn").GetComponent<MonoButtonWwiseEvent>();
                if (component == null)
                {
                    component = base.transform.Find("Btn").gameObject.AddComponent<MonoButtonWwiseEvent>();
                }
                if (component != null)
                {
                    component.eventName = str2;
                }
            }
            base.transform.Find("BattleType/Type").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(levelData.BattleTypePath);
            base.transform.Find("Btn").GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnBtnClick));
        }

        private void Update()
        {
            if (this._playAni)
            {
                this._timer += Time.deltaTime;
                this._prefixLineImage.fillAmount = Mathf.Clamp((float) (this._timer / this._timerSpan), (float) 0f, (float) 1f);
                if (this._timer > this._timerSpan)
                {
                    this._playAni = false;
                    base.transform.GetComponent<Animation>().Play();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <BindViewCallback>c__AnonStorey10D
        {
            internal Action<BaseEventData> callback;

            internal void <>m__1C4(BaseEventData evtData)
            {
                this.callback(evtData);
            }
        }
    }
}

