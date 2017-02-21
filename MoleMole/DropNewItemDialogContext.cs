namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class DropNewItemDialogContext : BaseSequenceDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private bool _onlyShow;
        private StorageDataItemBase _storageItem;
        private CanvasTimer _timer;
        public float TimerSpan = 2f;

        public DropNewItemDialogContext(StorageDataItemBase itemData, bool useTimer = true, bool onlyShow = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "DropNewItemDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/NewDropItemGotDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            this._storageItem = itemData;
            this._onlyShow = onlyShow;
            if (useTimer)
            {
                this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(this.TimerSpan, 0f);
                this._timer.timeUpCallback = new Action(this.DialogEnd);
                this._timer.StopRun();
            }
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG/Button").GetComponent<Button>(), new UnityAction(this.DialogEnd));
        }

        public override void Destroy()
        {
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            base.Destroy();
        }

        private void DialogEnd()
        {
            this.Destroy();
        }

        private void PostOpenningAudioEvent()
        {
            if (this._storageItem is StigmataDataItem)
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Item_Tattoo_PTL_Display", null, null, null);
            }
            else if (this._storageItem is AvatarCardDataItem)
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Large", null, null, null);
            }
            else
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Small", null, null, null);
            }
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(null, null);
            base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(false);
            base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(false);
            base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(false);
            if (this._storageItem is WeaponDataItem)
            {
                base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(this._storageItem as WeaponDataItem, false, 0);
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoAnimationinSequence>(), null);
            }
            else if (this._storageItem is StigmataDataItem)
            {
                base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(this._storageItem as StigmataDataItem);
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoAnimationinSequence>(), null);
            }
            else
            {
                string prefabPath = !(this._storageItem is EndlessToolDataItem) ? this._storageItem.GetImagePath() : (this._storageItem as EndlessToolDataItem).GetIconPath();
                base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<MonoAnimationinSequence>(), null);
            }
            base.view.transform.Find("NewAvatarEffect/Green").gameObject.SetActive(false);
            base.view.transform.Find("NewAvatarEffect/Blue").gameObject.SetActive(false);
            base.view.transform.Find("NewAvatarEffect/Purple").gameObject.SetActive(false);
            base.view.transform.Find("NewAvatarEffect/Orange").gameObject.SetActive(false);
            base.view.transform.Find("NewAvatarEffect/" + MiscData.Config.RarityColor[this._storageItem.rarity]).gameObject.SetActive(true);
            if (!this._onlyShow)
            {
                base.view.transform.Find("ItemPanel/Title/DescPanel/Desc").GetComponent<Text>().text = this._storageItem.GetDisplayTitle();
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/Title").GetComponent<MonoAnimationinSequence>(), null);
                Transform trans = base.view.transform.Find("ItemPanel/Stars");
                if ((this._storageItem is AvatarFragmentDataItem) || (this._storageItem is AvatarCardDataItem))
                {
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    trans.gameObject.SetActive(true);
                    for (int i = 0; i < trans.childCount; i++)
                    {
                        Transform child = trans.GetChild(i);
                        child.gameObject.SetActive(i < this._storageItem.rarity);
                        if (i < this._storageItem.rarity)
                        {
                            bool flag = this._storageItem is AvatarCardDataItem;
                            child.Find("1").gameObject.SetActive(!flag);
                            child.Find("2").gameObject.SetActive(flag);
                        }
                    }
                    this._animationManager.AddAllChildrenInTransform(trans);
                }
            }
            this._animationManager.StartPlay(0f, false);
            if (((this._timer != null) && (Singleton<TutorialModule>.Instance != null)) && !Singleton<TutorialModule>.Instance.IsInTutorial)
            {
                this._timer.StartRun(false);
            }
            AvatarCardDataItem item = this._storageItem as AvatarCardDataItem;
            if ((item != null) && !item.IsSplite())
            {
                AvatarUnlockDialogContext dialogContext = new AvatarUnlockDialogContext(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.ID).avatarID, true);
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            this.PostOpenningAudioEvent();
            return false;
        }
    }
}

