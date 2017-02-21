namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class DropNewItemDialogContextV2 : BaseSequenceDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private List<CanvasTimer> _delayTimerList = new List<CanvasTimer>();
        private List<float> _delayTimerSpanList = new List<float> { 1.2f, 1.4f, 2f };
        private CanvasTimer _firstDelayTimer;
        private CanvasTimer _secondDelayTimer;
        private List<Tuple<StorageDataItemBase, bool>> _storageItemList;
        private CanvasTimer _thirdDelayTimer;
        private const string DROP_AVATAR_CARD_PREFAB_PATH = "UI/Menus/Widget/Storage/DropAvatarCard";
        private const string DROP_ITEM_PREFAB_PATH = "UI/Menus/Widget/Storage/DropItem";

        public DropNewItemDialogContextV2(List<Tuple<StorageDataItemBase, bool>> itemDataList)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "DropNewItemDialogContextV2",
                viewPrefabPath = "UI/Menus/Dialog/NewDropItemGotDialogV2",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            this._storageItemList = itemDataList;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG/Button").GetComponent<Button>(), new UnityAction(this.Destroy));
        }

        public override void Destroy()
        {
            foreach (CanvasTimer timer in this._delayTimerList)
            {
                if (timer != null)
                {
                    timer.Destroy();
                }
            }
            this._animationManager.ClearAnimations();
            base.Destroy();
        }

        private bool OnAnimationCallBack(string msg)
        {
            if (msg == "ShowStigmata")
            {
                IEnumerator enumerator = base.view.transform.Find("ItemPanel").GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        current.GetComponent<MonoDropNewItemShow>().ResetRectMaskForStigmata();
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
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.AnimCallBack) && this.OnAnimationCallBack(ntf.body.ToString()));
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(null, null);
            Transform trans = base.view.transform.Find("ItemPanel");
            trans.DestroyChildren();
            for (int i = 0; i < this._storageItemList.Count; i++)
            {
                <SetupView>c__AnonStoreyE2 ye = new <SetupView>c__AnonStoreyE2 {
                    <>f__this = this
                };
                StorageDataItemBase itemData = this._storageItemList[i].Item1;
                string path = "UI/Menus/Widget/Storage/DropItem";
                if (itemData is AvatarCardDataItem)
                {
                    path = "UI/Menus/Widget/Storage/DropAvatarCard";
                }
                ye.dropItemTrans = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(path, BundleType.RESOURCE_FILE)).transform;
                ye.dropItemTrans.name = (i + 1).ToString();
                ye.dropItemTrans.SetParent(trans, false);
                ye.dropItemTrans.GetComponent<MonoDropNewItemShow>().SetupView(itemData);
                if (!(itemData is AvatarFragmentDataItem))
                {
                    CanvasTimer timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(this._delayTimerSpanList[i], 0f);
                    timer.timeUpCallback = new Action(ye.<>m__10F);
                    timer.StartRun(false);
                }
            }
            return false;
        }

        private void ShowItemStar(Transform itemTrans)
        {
            if ((itemTrans != null) && (itemTrans.Find("Item/Stars") != null))
            {
                Transform trans = itemTrans.Find("Item/Stars");
                if (this._animationManager.IsPlaying)
                {
                    this._animationManager.AddAllChildrenInTransform(trans);
                }
                else
                {
                    this._animationManager.ClearAnimations();
                    this._animationManager.AddAllChildrenInTransform(trans);
                    this._animationManager.StartPlay(0f, true);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SetupView>c__AnonStoreyE2
        {
            internal DropNewItemDialogContextV2 <>f__this;
            internal Transform dropItemTrans;

            internal void <>m__10F()
            {
                this.<>f__this.ShowItemStar(this.dropItemTrans);
            }
        }
    }
}

