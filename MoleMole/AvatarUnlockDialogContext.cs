namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class AvatarUnlockDialogContext : BaseSequenceDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private AvatarDataItem _avatarData;
        private int _avatarID;
        private bool _enableClipZone;
        private CanvasTimer _timer;
        private const float TIMER_SPAN = 1f;

        public AvatarUnlockDialogContext(int avatarID, bool enableClipZone = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarUnlockDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AvatarUnlockDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            this._avatarID = avatarID;
            this._avatarData = Singleton<AvatarModule>.Instance.GetAvatarByID(this._avatarID);
            this._enableClipZone = enableClipZone;
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(1f, 0f);
            this._timer.timeUpCallback = new Action(this.OnBGClick);
            this._timer.StopRun();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ClipZone").GetComponent<Button>(), new UnityAction(this.OnBGClick));
        }

        public override void Destroy()
        {
            this._timer.Destroy();
            base.Destroy();
        }

        private Color GetFontColorByAttribute(int attr)
        {
            Color white = Color.white;
            if (attr == 1)
            {
                UIUtil.TryParseHexString("#fedf4cFF", out white);
                return white;
            }
            if (attr == 2)
            {
                UIUtil.TryParseHexString("#fc4399FF", out white);
                return white;
            }
            if (attr == 3)
            {
                UIUtil.TryParseHexString("#43c6fcFF", out white);
            }
            return white;
        }

        private void OnBGClick()
        {
            this.Destroy();
        }

        private void OnDialogBGGrowEnd()
        {
            this._animationManager.StartPlay(0f, true);
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(new Action(this.StartTimer), null);
            base.view.transform.Find("Dialog/Content/AnimMoveIn1/Portrait").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._avatarData.IconPath);
            base.view.transform.Find("Dialog/Content/AnimMoveIn2/NameRow/Title").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_UnlockNewAvatar", new object[0]);
            base.view.transform.Find("Dialog/Content/AnimMoveIn2/NameRow/ClassName/FirstName").GetComponent<Text>().text = this._avatarData.ClassFirstName;
            base.view.transform.Find("Dialog/Content/AnimMoveIn2/NameRow/ClassName/LastName").GetComponent<Text>().text = this._avatarData.ClassLastName;
            base.view.transform.Find("Dialog/Content/AnimMoveIn3/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(this._avatarData.star);
            base.view.transform.Find("Dialog/Content/AnimMoveIn3/ShortNameRow/SmallIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._avatarData.AttributeIconPath);
            Text component = base.view.transform.Find("Dialog/Content/AnimMoveIn3/ShortNameRow/ShortName").GetComponent<Text>();
            component.text = this._avatarData.ShortName;
            component.color = this.GetFontColorByAttribute(this._avatarData.Attribute);
            this._animationManager.AddAllChildrenInTransform(base.view.transform.Find("Dialog/Content"));
            base.view.transform.Find("Dialog").GetComponent<MonoDialogHeightGrow>().PlayGrow(new Action(this.OnDialogBGGrowEnd));
            base.view.transform.Find("ClipZone").gameObject.SetActive(this._enableClipZone);
            return false;
        }

        private void StartTimer()
        {
            this._timer.StartRun(false);
        }
    }
}

