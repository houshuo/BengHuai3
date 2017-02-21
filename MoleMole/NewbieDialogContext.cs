namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class NewbieDialogContext : BaseDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private bool _bRewardClicked;
        private Coroutine _delayInputCoroutine;
        private Coroutine _delayShowCoroution;
        private Coroutine _destroyCoroutine;
        private Coroutine _handIconAnimCoroution;
        private bool _hasClicked;
        private bool _hasDestroied;
        private bool _hasSetup;
        private Coroutine _highlightEffectCoroution;
        private float _hightlightEffectAngleSpeed = 300f;
        private bool _isHighlightEffectStarted;
        private NewbieHighlightInfo _newbieHightlightInfo;
        private bool _waitDestroy;
        public bool bShowReward;
        public bool bShowSkip;
        public BubblePosType bubblePosType;
        public float delayInputTime;
        public float delayShowTime;
        public Action destroyButHasClickedCallback;
        public Action destroyButNoClickedCallback;
        public bool destroyByOthers;
        public Action destroyCallback;
        public bool disableHighlightEffect;
        public bool disableHighlightInvoke;
        public bool disableMask;
        private const string expPrefabPath = "SpriteOutput/RewardGotIcons/Exp";
        private const string friendPointPrefabPath = "SpriteOutput/RewardGotIcons/FriendPoint";
        public string guideDesc;
        public int guideID;
        public Sprite guideSprite;
        private const float HAND_ICON_ANIM_MOVE_DISTANCE = 30f;
        private const float HAND_ICON_ANIM_MOVE_DURATION = 0.15f;
        public HandIconPosType handIconPosType;
        private const string hcoinPrefabPath = "SpriteOutput/RewardGotIcons/HCoin";
        public string highlightPath = string.Empty;
        public Transform highlightTrans;
        public bool isMaskClickable;
        public bool playHighlightAnim;
        public Action pointerDownCallback;
        public Func<bool> pointerUpCallback;
        public Action preCallback;
        public BaseContext referredContext;
        public int rewardID;
        private const string scoinPrefabPath = "SpriteOutput/RewardGotIcons/SCoin";
        private const string skillPointPrefabPath = "SpriteOutput/RewardGotIcons/SkillPoint";
        private const string staminaPrefabPath = "SpriteOutput/RewardGotIcons/Stamina";

        public NewbieDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "NewbieDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/Newbie/NewbieDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
        }

        private bool CheckViewValid()
        {
            return !(this.disableMask && (this.highlightTrans == null));
        }

        public override void Destroy()
        {
            if (!this._hasDestroied)
            {
                this._hasDestroied = true;
                base.Destroy();
                this.StopDelayShowCoroution();
                this.StopHandIconAnim();
                this.StopHighlightEffectCoroutine();
                this.StopDelayInputCoroution();
                this.StopDestroyCoroution();
                this.RecoverHighlightView();
                if (this._hasClicked && (this.destroyButHasClickedCallback != null))
                {
                    this.destroyButHasClickedCallback();
                }
                if (!this._hasClicked && (this.destroyButNoClickedCallback != null))
                {
                    this.destroyButNoClickedCallback();
                }
                if (this.destroyCallback != null)
                {
                    this.destroyCallback();
                }
            }
        }

        private List<RewardUIData> GetRewardUIDataList(RewardData rewardData)
        {
            List<RewardUIData> list = new List<RewardUIData>();
            if (rewardData.RewardExp > 0)
            {
                RewardUIData playerExpData = RewardUIData.GetPlayerExpData(rewardData.RewardExp);
                playerExpData.itemID = rewardData.RewardID;
                list.Add(playerExpData);
            }
            if (rewardData.RewardSCoin > 0)
            {
                RewardUIData scoinData = RewardUIData.GetScoinData(rewardData.RewardSCoin);
                scoinData.itemID = rewardData.RewardID;
                list.Add(scoinData);
            }
            if (rewardData.RewardHCoin > 0)
            {
                RewardUIData hcoinData = RewardUIData.GetHcoinData(rewardData.RewardHCoin);
                hcoinData.itemID = rewardData.RewardID;
                list.Add(hcoinData);
            }
            if (rewardData.RewardStamina > 0)
            {
                RewardUIData staminaData = RewardUIData.GetStaminaData(rewardData.RewardStamina);
                staminaData.itemID = rewardData.RewardID;
                list.Add(staminaData);
            }
            if (rewardData.RewardSkillPoint > 0)
            {
                RewardUIData skillPointData = RewardUIData.GetSkillPointData(rewardData.RewardSkillPoint);
                skillPointData.itemID = rewardData.RewardID;
                list.Add(skillPointData);
            }
            if (rewardData.RewardFriendPoint > 0)
            {
                RewardUIData friendPointData = RewardUIData.GetFriendPointData(rewardData.RewardFriendPoint);
                friendPointData.itemID = rewardData.RewardID;
                list.Add(friendPointData);
            }
            if (rewardData.RewardItem1ID > 0)
            {
                RewardUIData item = new RewardUIData(ResourceType.Item, rewardData.RewardItem1Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem1ID, rewardData.RewardItem1Level);
                list.Add(item);
            }
            if (rewardData.RewardItem2ID > 0)
            {
                RewardUIData data8 = new RewardUIData(ResourceType.Item, rewardData.RewardItem2Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem2ID, rewardData.RewardItem2Level);
                list.Add(data8);
            }
            if (rewardData.RewardItem3ID > 0)
            {
                RewardUIData data9 = new RewardUIData(ResourceType.Item, rewardData.RewardItem3Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem3ID, rewardData.RewardItem3Level);
                list.Add(data9);
            }
            if (rewardData.RewardItem4ID > 0)
            {
                RewardUIData data10 = new RewardUIData(ResourceType.Item, rewardData.RewardItem4Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem4ID, rewardData.RewardItem4Level);
                list.Add(data10);
            }
            if (rewardData.RewardItem5ID > 0)
            {
                RewardUIData data11 = new RewardUIData(ResourceType.Item, rewardData.RewardItem5Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem5ID, rewardData.RewardItem5Level);
                list.Add(data11);
            }
            return list;
        }

        private void HideRewardTransSomePart(Transform rewardTrans)
        {
            rewardTrans.Find("BG/UnidentifyText").gameObject.SetActive(false);
            rewardTrans.Find("NewMark").gameObject.SetActive(false);
            rewardTrans.Find("AvatarStar").gameObject.SetActive(false);
            rewardTrans.Find("Star").gameObject.SetActive(false);
            rewardTrans.Find("StigmataType").gameObject.SetActive(false);
            rewardTrans.Find("FragmentIcon").gameObject.SetActive(false);
        }

        private void HighlightView()
        {
            if (this.highlightPath != string.Empty)
            {
                BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
                if (sceneCanvas == null)
                {
                    this.Destroy();
                    return;
                }
                this.highlightTrans = sceneCanvas.transform.FindChild(this.highlightPath);
                if (((this.highlightTrans == null) || (this.highlightTrans.gameObject == null)) || !this.highlightTrans.gameObject.activeInHierarchy)
                {
                    this.Destroy();
                    return;
                }
            }
            if ((((base.view != null) && (base.view.transform != null)) && (((this.highlightTrans != null) && (this.highlightTrans.gameObject != null)) && this.highlightTrans.gameObject.activeInHierarchy)) && ((this._newbieHightlightInfo == null) || (this._newbieHightlightInfo.originTrans != this.highlightTrans)))
            {
                if (!this.disableHighlightEffect && (Singleton<ApplicationManager>.Instance != null))
                {
                    this._highlightEffectCoroution = Singleton<ApplicationManager>.Instance.StartCoroutine(this.PlayHighlightEffect());
                }
                if (!this.bShowReward)
                {
                    this._newbieHightlightInfo = new NewbieHighlightInfo(this, this.highlightTrans, base.view.transform, this.disableHighlightInvoke, new Action<BaseEventData>(this.OnNewbiePanelPreCallback), new Action<BaseEventData>(this.OnHighlightPointerDown), new Action<BaseEventData>(this.OnHighlightPointerUp));
                }
            }
        }

        public override bool NeedRecoverWhenPageStartUp()
        {
            return false;
        }

        private void OnAllBoxAnimationEnd()
        {
            if (Singleton<ApplicationManager>.Instance != null)
            {
                Singleton<ApplicationManager>.Instance.StartCoroutine(this.RewardDelayClick());
            }
        }

        private bool OnGetGuideRewardRsp(GetGuideRewardRsp rsp)
        {
            this._animationManager.StartPlay(0f, true);
            return false;
        }

        private void OnHighlightPointerDown(BaseEventData data = null)
        {
            if (this.pointerDownCallback != null)
            {
                this.pointerDownCallback();
            }
        }

        private void OnHighlightPointerUp(BaseEventData data = null)
        {
            bool flag = false;
            if (this.pointerUpCallback != null)
            {
                flag = this.pointerUpCallback();
            }
            if (!this.destroyByOthers || (this.destroyByOthers && flag))
            {
                this.Destroy();
            }
        }

        private void OnNewbieMaskClick(BaseEventData data = null)
        {
            bool flag = false;
            if (this.pointerUpCallback != null)
            {
                flag = this.pointerUpCallback();
            }
            if (!this.destroyByOthers || (this.destroyByOthers && flag))
            {
                this.Destroy();
            }
        }

        private void OnNewbieMaskPreCallback(BaseEventData data = null)
        {
            this._hasClicked = true;
            if (!this.destroyByOthers)
            {
                this._waitDestroy = true;
            }
            if (this.preCallback != null)
            {
                this.preCallback();
            }
        }

        private void OnNewbiePanelPreCallback(BaseEventData data = null)
        {
            this._hasClicked = true;
            if (!this.destroyByOthers)
            {
                this._waitDestroy = true;
            }
            if (this.preCallback != null)
            {
                this.preCallback();
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0xba) && this.OnGetGuideRewardRsp(pkt.getData<GetGuideRewardRsp>()));
        }

        private void OnRewardClick(BaseEventData data = null)
        {
            if (!this._bRewardClicked)
            {
                Singleton<NetworkManager>.Instance.RequestGuideReward(this.guideID);
                this._bRewardClicked = true;
            }
        }

        private void OnRewardPreCallback(BaseEventData data = null)
        {
            this.OnNewbieMaskPreCallback(data);
        }

        private void OnSkipClick()
        {
            Singleton<TutorialModule>.Instance.TryToSkipTutorial(this.guideID, new Action(this.OnSkipFinish));
        }

        private void OnSkipFinish()
        {
            this.Destroy();
        }

        [DebuggerHidden]
        private IEnumerator PlayHandIconAnim(RectTransform handIconRectTransform)
        {
            return new <PlayHandIconAnim>c__Iterator5B { handIconRectTransform = handIconRectTransform, <$>handIconRectTransform = handIconRectTransform, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator PlayHighlightEffect()
        {
            return new <PlayHighlightEffect>c__Iterator5C { <>f__this = this };
        }

        private void RecoverHighlightView()
        {
            if ((this._newbieHightlightInfo == null) || (this.highlightTrans == null))
            {
                this._newbieHightlightInfo = null;
            }
            else
            {
                this._newbieHightlightInfo.Recover();
                this._newbieHightlightInfo = null;
            }
        }

        [DebuggerHidden]
        private IEnumerator RewardDelayClick()
        {
            return new <RewardDelayClick>c__Iterator5A { <>f__this = this };
        }

        public override void SetActive(bool enabled)
        {
            this.playHighlightAnim = enabled;
            this.SetHighlightEffectActive(enabled);
            base.SetActive(enabled);
            if (!enabled && !this._waitDestroy)
            {
                this.Destroy();
            }
        }

        public void SetAvailable(bool available)
        {
            this.playHighlightAnim = available;
            this.SetHighlightEffectActive(available);
            if (available)
            {
                CanvasGroup component = base.view.transform.GetComponent<CanvasGroup>();
                if (component != null)
                {
                    UnityEngine.Object.Destroy(component);
                }
            }
            else
            {
                CanvasGroup group2 = base.view.transform.GetComponent<CanvasGroup>();
                if (group2 == null)
                {
                    group2 = base.view.AddComponent<CanvasGroup>();
                }
                group2.alpha = 0f;
                group2.blocksRaycasts = false;
            }
        }

        private Transform SetHighlightEffectActive(bool isActive)
        {
            if (((this.highlightTrans == null) || (base.view == null)) || (base.view.transform == null))
            {
                return null;
            }
            Transform transform = base.view.transform.Find("Tutorial_Glow");
            if (((transform != null) && (transform.gameObject != null)) && (!isActive || this._isHighlightEffectStarted))
            {
                transform.gameObject.SetActive(isActive);
            }
            return transform;
        }

        private void SetReferredContext()
        {
            if ((this.highlightTrans != null) && (this.highlightTrans.gameObject != null))
            {
                ContextIdentifier componentInParent = this.highlightTrans.gameObject.GetComponentInParent<ContextIdentifier>();
                if (componentInParent != null)
                {
                    this.referredContext = componentInParent.context;
                }
            }
        }

        private void SetupBubble()
        {
            if ((base.view != null) && (base.view.transform != null))
            {
                Transform transform = base.view.transform.Find("Bubble");
                if (transform != null)
                {
                    if (this.bubblePosType == BubblePosType.None)
                    {
                        transform.gameObject.SetActive(false);
                    }
                    else
                    {
                        transform.gameObject.SetActive(true);
                        IEnumerator enumerator = transform.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                Transform current = (Transform) enumerator.Current;
                                if (current.gameObject.name == this.bubblePosType.ToString())
                                {
                                    current.gameObject.SetActive(true);
                                }
                                else
                                {
                                    current.gameObject.SetActive(false);
                                }
                            }
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                        Transform transform3 = transform.FindChild(this.bubblePosType.ToString() + "/Head/Icon");
                        if (transform3 != null)
                        {
                            float num = UnityEngine.Random.Range((float) 0f, (float) 3f);
                            if (num <= 1f)
                            {
                                transform3.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Newbie/CharaHeadAIMusume01");
                            }
                            else if (num <= 2f)
                            {
                                transform3.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Newbie/CharaHeadAIMusume02");
                            }
                            else
                            {
                                transform3.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Newbie/CharaHeadAIMusume03");
                            }
                            Transform transform4 = transform.FindChild(this.bubblePosType.ToString() + "/Text");
                            if (!string.IsNullOrEmpty(this.guideDesc))
                            {
                                transform4.FindChild("Panel/Text").GetComponent<Text>().text = this.guideDesc;
                            }
                        }
                    }
                }
            }
        }

        private void SetupDisableInput()
        {
            if ((this.delayInputTime < 0.1f) && (this.handIconPosType == HandIconPosType.None))
            {
                this.delayInputTime = 1f;
            }
            if ((this.delayInputTime > 0f) && (Singleton<ApplicationManager>.Instance != null))
            {
                this._delayInputCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.WaitEnableInput());
            }
            else
            {
                base.view.transform.Find("DisableInput").gameObject.SetActive(false);
            }
        }

        private void SetupGuideImage()
        {
            if (this.guideSprite != null)
            {
                Transform transform = base.view.transform.Find("GuideImage");
                transform.gameObject.SetActive(true);
                transform.GetComponent<Image>().sprite = this.guideSprite;
            }
        }

        private void SetupHandIcon()
        {
            if ((base.view != null) && (base.view.transform != null))
            {
                Transform transform = base.view.transform.FindChild("HandIcon");
                if (transform != null)
                {
                    if (this.handIconPosType == HandIconPosType.Downward)
                    {
                        base.view.transform.FindChild("HandIcon/Downward").gameObject.SetActive(true);
                    }
                    else if (this.handIconPosType == HandIconPosType.Tips)
                    {
                        base.view.transform.FindChild("Tips").gameObject.SetActive(true);
                    }
                    else if (this.handIconPosType == HandIconPosType.Arrow)
                    {
                        if (this.highlightTrans != null)
                        {
                            base.view.transform.FindChild("Arrow").gameObject.SetActive(true);
                            base.view.transform.FindChild("Arrow").position = this.highlightTrans.GetComponent<RectTransform>().position;
                        }
                    }
                    else if ((this.handIconPosType == HandIconPosType.None) || (this.highlightTrans == null))
                    {
                        transform.gameObject.SetActive(false);
                    }
                    else
                    {
                        RectTransform component = this.highlightTrans.GetComponent<RectTransform>();
                        Vector3[] fourCornersArray = new Vector3[4];
                        component.GetWorldCorners(fourCornersArray);
                        IEnumerator enumerator = transform.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                Transform current = (Transform) enumerator.Current;
                                if (current.gameObject.name == this.handIconPosType.ToString())
                                {
                                    current.gameObject.SetActive(true);
                                }
                                else
                                {
                                    current.gameObject.SetActive(false);
                                }
                            }
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                        Transform transform4 = base.view.transform.FindChild("HandIcon/" + this.handIconPosType.ToString());
                        if (transform4 != null)
                        {
                            RectTransform handIconRectTransform = transform4.GetComponent<RectTransform>();
                            Vector3[] vectorArray2 = new Vector3[4];
                            handIconRectTransform.GetWorldCorners(vectorArray2);
                            float num = 0f;
                            float num2 = 0f;
                            if (Singleton<LevelManager>.Instance != null)
                            {
                                float num3 = 0.5f;
                                num = Math.Max(Math.Max(Math.Max(Math.Abs((float) (vectorArray2[1].x - vectorArray2[0].x)), Math.Abs((float) (vectorArray2[2].x - vectorArray2[1].x))), Math.Abs((float) (vectorArray2[3].x - vectorArray2[2].x))), Math.Abs((float) (vectorArray2[0].x - vectorArray2[3].x))) + num3;
                                num2 = Math.Max(Math.Max(Math.Max(Math.Abs((float) (vectorArray2[1].y - vectorArray2[0].y)), Math.Abs((float) (vectorArray2[2].y - vectorArray2[1].y))), Math.Abs((float) (vectorArray2[3].y - vectorArray2[2].y))), Math.Abs((float) (vectorArray2[0].y - vectorArray2[3].y))) + num3;
                            }
                            else
                            {
                                num = num2 = 0f;
                            }
                            if (this.handIconPosType == HandIconPosType.Left)
                            {
                                transform4.position = new Vector3(fourCornersArray[0].x - (num / 2f), (fourCornersArray[0].y + fourCornersArray[1].y) / 2f, transform4.position.z);
                            }
                            else if (this.handIconPosType == HandIconPosType.Right)
                            {
                                transform4.position = new Vector3(fourCornersArray[2].x + (num / 2f), (fourCornersArray[0].y + fourCornersArray[1].y) / 2f, transform4.position.z);
                            }
                            else if (this.handIconPosType == HandIconPosType.Up)
                            {
                                transform4.position = new Vector3((fourCornersArray[0].x + fourCornersArray[3].x) / 2f, fourCornersArray[1].y + (num2 / 2f), transform4.position.z);
                            }
                            else if (this.handIconPosType == HandIconPosType.Bottom)
                            {
                                transform4.position = new Vector3((fourCornersArray[0].x + fourCornersArray[3].x) / 2f, fourCornersArray[0].y - (num2 / 2f), transform4.position.z);
                            }
                            this.StopHandIconAnim();
                            if ((this.handIconPosType != HandIconPosType.None) && (Singleton<ApplicationManager>.Instance != null))
                            {
                                this._handIconAnimCoroution = Singleton<ApplicationManager>.Instance.StartCoroutine(this.PlayHandIconAnim(handIconRectTransform));
                            }
                        }
                    }
                }
            }
        }

        private void SetupMask()
        {
            Transform trans = base.view.transform.FindChild("Mask");
            trans.GetComponent<Image>().enabled = !this.disableMask;
            if ((!this.disableMask && (this.highlightTrans == null)) || ((this.isMaskClickable || (this.handIconPosType == HandIconPosType.None)) || (this.handIconPosType == HandIconPosType.Arrow)))
            {
                base.BindViewCallback(trans, EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnNewbieMaskPreCallback));
                base.BindViewCallback(trans, EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnNewbieMaskClick));
            }
        }

        private void SetupReward()
        {
            this._animationManager = new SequenceAnimationManager(new Action(this.OnAllBoxAnimationEnd), null);
            if (this.bShowReward)
            {
                this._bRewardClicked = false;
                base.view.transform.Find("Reward").gameObject.SetActive(true);
                this.highlightTrans = base.view.transform.Find("Reward/Button");
                base.BindViewCallback(this.highlightTrans, EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnRewardPreCallback));
                base.BindViewCallback(this.highlightTrans, EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnRewardClick));
                RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(this.rewardID);
                Transform transform = base.view.transform.Find("Reward/Content/1");
                transform.gameObject.SetActive(false);
                Transform transform2 = base.view.transform.Find("Reward/Content/2");
                transform2.gameObject.SetActive(false);
                Transform transform3 = base.view.transform.Find("Reward/Content/3");
                transform3.gameObject.SetActive(false);
                Transform transform4 = transform.Find("Item");
                Transform transform5 = transform2.Find("Item");
                Transform transform6 = transform3.Find("Item");
                List<Transform> list = new List<Transform> {
                    transform,
                    transform2,
                    transform3
                };
                List<Transform> list2 = new List<Transform> {
                    transform4,
                    transform5,
                    transform6
                };
                List<RewardUIData> rewardUIDataList = this.GetRewardUIDataList(rewardDataByKey);
                for (int i = 0; i < rewardUIDataList.Count; i++)
                {
                    if (i < list.Count)
                    {
                        Transform transform7 = list[i];
                        Transform rewardTrans = list2[i];
                        transform7.gameObject.SetActive(true);
                        this._animationManager.AddAnimation(transform7.GetComponent<MonoAnimationinSequence>(), null);
                        RewardUIData data2 = rewardUIDataList[i];
                        if (data2.rewardType == ResourceType.Item)
                        {
                            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data2.itemID, data2.level);
                            dummyStorageDataItem.number = data2.value;
                            rewardTrans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true, false, false, false);
                        }
                        else
                        {
                            this.HideRewardTransSomePart(rewardTrans);
                            rewardTrans.GetComponent<MonoLevelDropIconButton>().Clear();
                            rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(data2.iconPath);
                            rewardTrans.Find("BG").gameObject.SetActive(true);
                            rewardTrans.Find("BG/Desc").GetComponent<Text>().text = "x" + data2.value.ToString();
                        }
                    }
                }
            }
            else
            {
                base.view.transform.Find("Reward").gameObject.SetActive(false);
            }
        }

        private void SetupSkip()
        {
            if ((base.view != null) && (base.view.transform != null))
            {
                Transform transform = base.view.transform.Find("Skip");
                transform.gameObject.SetActive(this.bShowSkip);
                if (this.bShowSkip)
                {
                    base.BindViewCallback(transform.GetComponent<Button>(), new UnityAction(this.OnSkipClick));
                }
            }
        }

        protected override bool SetupView()
        {
            if (!this._hasSetup)
            {
                this._hasSetup = true;
                this.SetupReward();
                if ((this._newbieHightlightInfo != null) && (this.highlightTrans == null))
                {
                    this.Destroy();
                    return false;
                }
                if (!this.CheckViewValid())
                {
                    this.Destroy();
                    return false;
                }
                if (this.highlightTrans != null)
                {
                    this.playHighlightAnim = true;
                }
                this.SetupDisableInput();
                this.SetupMask();
                this.SetupGuideImage();
                this.SetReferredContext();
                if ((this.delayShowTime > 0f) && (Singleton<ApplicationManager>.Instance != null))
                {
                    this._delayShowCoroution = Singleton<ApplicationManager>.Instance.StartCoroutine(this.WaitSetup());
                }
                else
                {
                    this.SetupBubble();
                    this.SetupHandIcon();
                    this.HighlightView();
                    this.SetupSkip();
                }
            }
            return false;
        }

        private void StopDelayInputCoroution()
        {
            if ((this._delayInputCoroutine != null) && (Singleton<ApplicationManager>.Instance != null))
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._delayInputCoroutine);
                this._delayInputCoroutine = null;
            }
        }

        private void StopDelayShowCoroution()
        {
            if ((this._delayShowCoroution != null) && (Singleton<ApplicationManager>.Instance != null))
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._delayShowCoroution);
                this._delayShowCoroution = null;
            }
        }

        private void StopDestroyCoroution()
        {
            if ((this._destroyCoroutine != null) && (Singleton<ApplicationManager>.Instance != null))
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._destroyCoroutine);
                this._destroyCoroutine = null;
            }
        }

        private void StopHandIconAnim()
        {
            if ((this._handIconAnimCoroution != null) && (Singleton<ApplicationManager>.Instance != null))
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._handIconAnimCoroution);
                this._handIconAnimCoroution = null;
            }
        }

        private void StopHighlightEffectCoroutine()
        {
            if ((this._highlightEffectCoroution != null) && (Singleton<ApplicationManager>.Instance != null))
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._highlightEffectCoroution);
                this._highlightEffectCoroution = null;
            }
        }

        [DebuggerHidden]
        private IEnumerator WaitDestroy()
        {
            return new <WaitDestroy>c__Iterator57 { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator WaitEnableInput()
        {
            return new <WaitEnableInput>c__Iterator59 { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator WaitSetup()
        {
            return new <WaitSetup>c__Iterator58 { <>f__this = this };
        }

        [CompilerGenerated]
        private sealed class <PlayHandIconAnim>c__Iterator5B : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal RectTransform <$>handIconRectTransform;
            internal NewbieDialogContext <>f__this;
            internal float <delta>__5;
            internal bool <isReversed>__1;
            internal float <lerpEnd>__3;
            internal float <lerpStart>__2;
            internal Vector2 <originPos>__4;
            internal float <position_normalized>__7;
            internal float <progress_normalized>__6;
            internal float <timer>__0;
            internal float <x>__10;
            internal float <x>__8;
            internal float <y>__11;
            internal float <y>__9;
            internal RectTransform handIconRectTransform;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<timer>__0 = 0f;
                        this.<isReversed>__1 = false;
                        this.<lerpStart>__2 = 0f;
                        this.<lerpEnd>__3 = 0f;
                        this.<originPos>__4 = new Vector2(this.handIconRectTransform.anchoredPosition.x, this.handIconRectTransform.anchoredPosition.y);
                        break;

                    case 1:
                        goto Label_00A5;

                    case 2:
                        goto Label_0230;

                    case 3:
                        break;
                        this.$PC = -1;
                        goto Label_0348;

                    default:
                        goto Label_0348;
                }
                if (!this.<>f__this.playHighlightAnim)
                {
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_034A;
                }
            Label_00A5:
                this.<timer>__0 += Time.deltaTime;
                this.<delta>__5 = ((this.<>f__this.handIconPosType != NewbieDialogContext.HandIconPosType.Right) && (this.<>f__this.handIconPosType != NewbieDialogContext.HandIconPosType.Up)) ? -30f : 30f;
                if ((this.<>f__this.handIconPosType == NewbieDialogContext.HandIconPosType.Left) || (this.<>f__this.handIconPosType == NewbieDialogContext.HandIconPosType.Right))
                {
                    this.<lerpStart>__2 = !this.<isReversed>__1 ? (this.<originPos>__4.x + this.<delta>__5) : this.<originPos>__4.x;
                    this.<lerpEnd>__3 = !this.<isReversed>__1 ? this.<originPos>__4.x : (this.<originPos>__4.x + this.<delta>__5);
                }
                else
                {
                    this.<lerpStart>__2 = !this.<isReversed>__1 ? (this.<originPos>__4.y + this.<delta>__5) : this.<originPos>__4.y;
                    this.<lerpEnd>__3 = !this.<isReversed>__1 ? this.<originPos>__4.y : (this.<originPos>__4.y + this.<delta>__5);
                }
                this.<progress_normalized>__6 = this.<timer>__0 / 0.15f;
                if (this.<progress_normalized>__6 >= 1f)
                {
                    this.<isReversed>__1 = !this.<isReversed>__1;
                    this.<timer>__0 = 0f;
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_034A;
                }
            Label_0230:
                this.<position_normalized>__7 = (2f * this.<progress_normalized>__6) - (this.<progress_normalized>__6 * this.<progress_normalized>__6);
                if ((this.<>f__this.handIconPosType == NewbieDialogContext.HandIconPosType.Left) || (this.<>f__this.handIconPosType == NewbieDialogContext.HandIconPosType.Right))
                {
                    this.<x>__8 = Mathf.Lerp(this.<lerpStart>__2, this.<lerpEnd>__3, Mathf.Clamp01(this.<position_normalized>__7));
                    this.<y>__9 = this.<originPos>__4.y;
                    this.handIconRectTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.<x>__8, this.<y>__9);
                }
                else
                {
                    this.<x>__10 = this.<originPos>__4.x;
                    this.<y>__11 = Mathf.Lerp(this.<lerpStart>__2, this.<lerpEnd>__3, Mathf.Clamp01(this.<position_normalized>__7));
                    this.handIconRectTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.<x>__10, this.<y>__11);
                }
                this.$current = null;
                this.$PC = 3;
                goto Label_034A;
            Label_0348:
                return false;
            Label_034A:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <PlayHighlightEffect>c__Iterator5C : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Vector3[] <$s_1684>__8;
            internal int <$s_1685>__9;
            internal NewbieDialogContext <>f__this;
            internal Vector3 <center>__7;
            internal Vector3 <corner>__10;
            internal Transform <effectTrans>__0;
            internal RectTransform <highlightRectTransform>__2;
            internal float <highlightWorldHeight>__6;
            internal float <highlightWorldWidth>__5;
            internal Vector3[] <hightlighWorldCorners>__3;
            internal float <sizeOffset>__4;
            internal Vector3 <startPos>__11;
            internal float <timer>__1;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<effectTrans>__0 = this.<>f__this.view.transform.Find("Tutorial_Glow");
                        if ((this.<>f__this.highlightTrans != null) && (this.<effectTrans>__0 != null))
                        {
                            this.<>f__this._isHighlightEffectStarted = true;
                            this.<timer>__1 = 0f;
                            this.<highlightRectTransform>__2 = this.<>f__this.highlightTrans.GetComponent<RectTransform>();
                            this.<hightlighWorldCorners>__3 = new Vector3[4];
                            this.<highlightRectTransform>__2.GetWorldCorners(this.<hightlighWorldCorners>__3);
                            this.<sizeOffset>__4 = 1f;
                            this.<highlightWorldWidth>__5 = Math.Max(Math.Max(Math.Max(Math.Abs((float) (this.<hightlighWorldCorners>__3[1].x - this.<hightlighWorldCorners>__3[0].x)), Math.Abs((float) (this.<hightlighWorldCorners>__3[2].x - this.<hightlighWorldCorners>__3[1].x))), Math.Abs((float) (this.<hightlighWorldCorners>__3[3].x - this.<hightlighWorldCorners>__3[2].x))), Math.Abs((float) (this.<hightlighWorldCorners>__3[0].x - this.<hightlighWorldCorners>__3[3].x))) + this.<sizeOffset>__4;
                            this.<highlightWorldHeight>__6 = Math.Max(Math.Max(Math.Max(Math.Abs((float) (this.<hightlighWorldCorners>__3[1].y - this.<hightlighWorldCorners>__3[0].y)), Math.Abs((float) (this.<hightlighWorldCorners>__3[2].y - this.<hightlighWorldCorners>__3[1].y))), Math.Abs((float) (this.<hightlighWorldCorners>__3[3].y - this.<hightlighWorldCorners>__3[2].y))), Math.Abs((float) (this.<hightlighWorldCorners>__3[0].y - this.<hightlighWorldCorners>__3[3].y))) + this.<sizeOffset>__4;
                            this.<center>__7 = Vector3.zero;
                            this.<$s_1684>__8 = this.<hightlighWorldCorners>__3;
                            this.<$s_1685>__9 = 0;
                            while (this.<$s_1685>__9 < this.<$s_1684>__8.Length)
                            {
                                this.<corner>__10 = this.<$s_1684>__8[this.<$s_1685>__9];
                                this.<center>__7 += this.<corner>__10;
                                this.<$s_1685>__9++;
                            }
                            this.<center>__7 = (Vector3) (this.<center>__7 / 4f);
                            this.<startPos>__11 = this.<center>__7 + new Vector3((Mathf.Cos((this.<timer>__1 * this.<>f__this._hightlightEffectAngleSpeed) * 0.01745329f) * this.<highlightWorldWidth>__5) / 2.5f, (Mathf.Sin((this.<timer>__1 * this.<>f__this._hightlightEffectAngleSpeed) * 0.01745329f) * this.<highlightWorldHeight>__6) / 2.5f, this.<>f__this.highlightTrans.position.z);
                            this.<effectTrans>__0.position = this.<startPos>__11;
                            this.<>f__this.SetHighlightEffectActive(this.<>f__this.playHighlightAnim);
                            break;
                        }
                        goto Label_0439;

                    case 1:
                        break;
                        this.$PC = -1;
                        goto Label_0439;

                    default:
                        goto Label_0439;
                }
                this.<timer>__1 += Time.deltaTime;
                this.<effectTrans>__0.position = this.<center>__7 + new Vector3((Mathf.Cos((this.<timer>__1 * this.<>f__this._hightlightEffectAngleSpeed) * 0.01745329f) * this.<highlightWorldWidth>__5) / 2.5f, (Mathf.Sin((this.<timer>__1 * this.<>f__this._hightlightEffectAngleSpeed) * 0.01745329f) * this.<highlightWorldHeight>__6) / 2.5f, this.<>f__this.highlightTrans.position.z);
                this.<effectTrans>__0.SetLocalPositionZ(0f);
                this.$current = null;
                this.$PC = 1;
                return true;
            Label_0439:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <RewardDelayClick>c__Iterator5A : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal NewbieDialogContext <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(0.8f);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.OnNewbieMaskClick(null);
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitDestroy>c__Iterator57 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal NewbieDialogContext <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this._destroyCoroutine = null;
                        this.<>f__this.Destroy();
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitEnableInput>c__Iterator59 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal NewbieDialogContext <>f__this;
            internal Transform <disableInputTrans>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(this.<>f__this.delayInputTime);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<disableInputTrans>__0 = this.<>f__this.view.transform.Find("DisableInput");
                        this.<disableInputTrans>__0.gameObject.SetActive(false);
                        this.<>f__this._delayInputCoroutine = null;
                        break;

                    default:
                        break;
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitSetup>c__Iterator58 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal NewbieDialogContext <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(this.<>f__this.delayShowTime);
                        this.$PC = 1;
                        return true;

                    case 1:
                        if ((this.<>f__this.view != null) && (this.<>f__this.view.transform != null))
                        {
                            this.<>f__this.SetupBubble();
                            this.<>f__this.SetupHandIcon();
                            this.<>f__this.HighlightView();
                            this.<>f__this.SetupSkip();
                            this.<>f__this._delayShowCoroution = null;
                            break;
                        }
                        this.<>f__this.Destroy();
                        break;

                    default:
                        break;
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public enum BubblePosType
        {
            None,
            LeftUp,
            LeftBottom,
            RightUp,
            RightBottom
        }

        public enum HandIconPosType
        {
            None,
            Up,
            Bottom,
            Left,
            Right,
            Downward,
            Tips,
            Arrow
        }
    }
}

