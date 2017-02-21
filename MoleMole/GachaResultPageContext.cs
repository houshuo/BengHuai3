namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class GachaResultPageContext : BasePageContext
    {
        private SequenceAnimationManager _animationManager;
        private int _cost;
        private GachaDisplayInfo _displayInfo;
        private List<MonoLevelDropIconButton> _dropList = new List<MonoLevelDropIconButton>();
        private List<StorageDataItemBase> _itemList;
        private HashSet<int> _rareItemList;
        private GachaType _type;
        private const string DROP_ITEM_ANIMATION_NAME = "DropItemScale10";

        public GachaResultPageContext(GachaDisplayInfo displayInfo, GachaType type, List<StorageDataItemBase> itemList, List<GachaItem> gachaItemList, int cost)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GachaResultPageContext",
                viewPrefabPath = "UI/Menus/Page/Gacha/GachaResultPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            this._type = type;
            this._itemList = itemList;
            this._rareItemList = new HashSet<int>();
            foreach (GachaItem item in gachaItemList)
            {
                if (item.get_is_rare_dropSpecified() && item.get_is_rare_drop())
                {
                    this._rareItemList.Add((int) item.get_item_id());
                }
            }
            this._cost = cost;
            this._displayInfo = displayInfo;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BottomPanel/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOkBtnClick));
        }

        private int GachaGotItemComparor(StorageDataItemBase left, StorageDataItemBase right)
        {
            ItemType itemTypePriority = this.GetItemTypePriority(left);
            ItemType type2 = this.GetItemTypePriority(right);
            if (itemTypePriority != type2)
            {
                return (int) (itemTypePriority - type2);
            }
            ItemType type3 = itemTypePriority;
            if (type3 != ItemType.AvatarCard)
            {
                if (type3 == ItemType.AvatarFragment)
                {
                    int num3 = AvatarMetaDataReader.GetAvatarMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((left as AvatarFragmentDataItem).ID).avatarID).unlockStar;
                    int num4 = AvatarMetaDataReader.GetAvatarMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((right as AvatarFragmentDataItem).ID).avatarID).unlockStar;
                    if (num3 == num4)
                    {
                        return (left.ID - right.ID);
                    }
                    return (num4 - num3);
                }
                if (left.rarity == right.rarity)
                {
                    return (left.ID - right.ID);
                }
                return (right.rarity - left.rarity);
            }
            int unlockStar = AvatarMetaDataReader.GetAvatarMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((left as AvatarCardDataItem).ID).avatarID).unlockStar;
            int num2 = AvatarMetaDataReader.GetAvatarMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((right as AvatarCardDataItem).ID).avatarID).unlockStar;
            if (unlockStar == num2)
            {
                return (left.ID - right.ID);
            }
            return (num2 - unlockStar);
        }

        private ItemType GetItemTypePriority(StorageDataItemBase itemData)
        {
            if (itemData is AvatarCardDataItem)
            {
                return ItemType.AvatarCard;
            }
            if (itemData is AvatarFragmentDataItem)
            {
                return ItemType.AvatarFragment;
            }
            if (itemData is WeaponDataItem)
            {
                return ItemType.Weapon;
            }
            if (itemData is StigmataDataItem)
            {
                return ItemType.Stigmata;
            }
            return ItemType.Material;
        }

        private void InitDropItems()
        {
            this._dropList.Clear();
            IEnumerator enumerator = base.view.transform.Find("Drops/ScrollView/Content").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MonoLevelDropIconButton component = ((Transform) enumerator.Current).GetComponent<MonoLevelDropIconButton>();
                    component.StopRareEffect();
                    if ((component != null) && (this._rareItemList.Contains(component.GetDropItemID()) || component.IsAvatarCard()))
                    {
                        this._dropList.Add(component);
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
        }

        private void MergeAllMaterialAndSort()
        {
            List<StorageDataItemBase> list = new List<StorageDataItemBase>();
            foreach (StorageDataItemBase base2 in this._itemList)
            {
                if (base2 is MaterialDataItem)
                {
                    bool flag = false;
                    foreach (StorageDataItemBase base3 in list)
                    {
                        if (base3.ID == base2.ID)
                        {
                            base3.number += base2.number;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        list.Add(base2);
                    }
                    continue;
                }
                list.Add(base2);
            }
            this._itemList = list;
            this._itemList.Sort(new Comparison<StorageDataItemBase>(this.GachaGotItemComparor));
        }

        private void OnAllAnimationEnd()
        {
            try
            {
                float delay = 0.1f;
                if (this._dropList.Count > 0)
                {
                    delay += 1f + (0.2f * (this._dropList.Count - 1));
                }
                Singleton<ApplicationManager>.Instance.StartCoroutine(this.OnToFragmentOver(delay));
            }
            catch
            {
                base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
            }
            IEnumerator enumerator = base.view.transform.Find("Drops/ScrollView/Content").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.SetLocalScaleX(1f);
                    current.SetLocalScaleY(1f);
                    current.GetComponent<CanvasGroup>().alpha = 1f;
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
            this.PlayVFXSequence();
            Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
        }

        private void OnDropItemBtnClick(StorageDataItemBase itemData)
        {
            UIUtil.ShowItemDetail(itemData, true, true);
        }

        private bool OnGetGachaDisplayRsp(GetGachaDisplayRsp rsp)
        {
            this.UpdateView();
            return false;
        }

        private void OnOkBtnClick()
        {
            Singleton<MainUIManager>.Instance.BackPage();
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x3f) && this.OnGetGachaDisplayRsp(pkt.getData<GetGachaDisplayRsp>()));
        }

        private void OnScrollChange(Transform trans, int index)
        {
            StorageDataItemBase itemData = this._itemList[index];
            trans.GetComponent<MonoLevelDropIconButton>().SetupView(itemData, new DropItemButtonClickCallBack(this.OnDropItemBtnClick), true, true, false, false);
            trans.GetComponent<MonoAnimationinSequence>().animationName = "DropItemScale10";
        }

        [DebuggerHidden]
        private IEnumerator OnToFragmentOver(float delay)
        {
            return new <OnToFragmentOver>c__Iterator61 { delay = delay, <$>delay = delay, <>f__this = this };
        }

        private void PlayVFXSequence()
        {
            float delay = 0.4f;
            for (int i = 0; i < this._dropList.Count; i++)
            {
                MonoLevelDropIconButton button = this._dropList[i];
                if ((button != null) && (button.gameObject != null))
                {
                    button.PlayVFX(delay, this._rareItemList.Contains(button.GetDropItemID()));
                    delay += 0.2f;
                }
            }
        }

        protected override bool SetupView()
        {
            Singleton<MainUIManager>.Instance.LockUI(true, 3f);
            base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
            this.MergeAllMaterialAndSort();
            this.UpdateView();
            return false;
        }

        private void UpdateView()
        {
            base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
            base.view.transform.Find("Drops/ScrollView").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnScrollChange), this._itemList.Count, new Vector2(0f, 1f));
            this._animationManager = new SequenceAnimationManager(new Action(this.OnAllAnimationEnd), null);
            this._animationManager.AddAllChildrenInTransform(base.view.transform.Find("Drops/ScrollView/Content"));
            this.InitDropItems();
            switch (this._type)
            {
                case 1:
                {
                    base.view.transform.Find("BottomPanel/Cost/Cost/Layout/Name").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("10119", new object[0]);
                    base.view.transform.Find("BottomPanel/Cost/Cost/Layout/Num").GetComponent<Text>().text = this._cost.ToString();
                    base.view.transform.Find("BottomPanel/Cost/Left/Layout/Name").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("10119", new object[0]);
                    base.view.transform.Find("BottomPanel/Cost/Left/Layout/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.friendsPoint.ToString();
                    FriendsPointGachaData friendPointGachaData = this._displayInfo.friendPointGachaData;
                    if (!string.IsNullOrEmpty(friendPointGachaData.get_common_data().get_title_image()))
                    {
                        UIUtil.TrySetupEventSprite(base.view.transform.Find("BottomPanel/Title").GetComponent<Image>(), friendPointGachaData.get_common_data().get_title_image());
                        break;
                    }
                    base.view.transform.Find("BottomPanel/Title").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.GachaTypeTitleFigures[this._type]);
                    break;
                }
                case 2:
                case 3:
                {
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) this._displayInfo.hcoinGachaData.get_ticket_material_id(), 1);
                    if (this._type == 3)
                    {
                        dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) this._displayInfo.specialGachaData.get_ticket_material_id(), 1);
                    }
                    base.view.transform.Find("BottomPanel/Cost/Cost/Layout/Name").GetComponent<Text>().text = dummyStorageDataItem.GetDisplayTitle();
                    base.view.transform.Find("BottomPanel/Cost/Cost/Layout/Num").GetComponent<Text>().text = this._cost.ToString();
                    base.view.transform.Find("BottomPanel/Cost/Left/Layout/Name").GetComponent<Text>().text = dummyStorageDataItem.GetDisplayTitle();
                    int number = 0;
                    StorageDataItemBase base3 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(dummyStorageDataItem.ID);
                    if (base3 != null)
                    {
                        number = base3.number;
                    }
                    base.view.transform.Find("BottomPanel/Cost/Left/Layout/Num").GetComponent<Text>().text = number.ToString();
                    HcoinGachaData data = (this._type != 2) ? this._displayInfo.specialGachaData : this._displayInfo.hcoinGachaData;
                    if (string.IsNullOrEmpty(data.get_common_data().get_title_image()))
                    {
                        base.view.transform.Find("BottomPanel/Title").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.GachaTypeTitleFigures[this._type]);
                    }
                    else
                    {
                        UIUtil.TrySetupEventSprite(base.view.transform.Find("BottomPanel/Title").GetComponent<Image>(), data.get_common_data().get_title_image());
                    }
                    break;
                }
            }
            base.view.transform.Find("Drops/ScrollView").GetComponent<RectMask>().SetGraphicDirty();
            this._animationManager.StartPlay(0.1f, true);
        }

        [CompilerGenerated]
        private sealed class <OnToFragmentOver>c__Iterator61 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>delay;
            internal GachaResultPageContext <>f__this;
            internal float delay;

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
                        this.$current = new WaitForSeconds(this.delay);
                        this.$PC = 1;
                        return true;

                    case 1:
                        if (this.<>f__this.view != null)
                        {
                            this.<>f__this.view.transform.Find("BlockPanel").gameObject.SetActive(false);
                        }
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
    }
}

