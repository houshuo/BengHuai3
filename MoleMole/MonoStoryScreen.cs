namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoStoryScreen : BaseMonoDynamicObject
    {
        private BaseMonoAvatar _avatar;
        private GameObject _containerLeft;
        private GameObject _containerRight;
        private int _currentLeftModelAvatarID;
        private int _currentRightModelAvatarID;
        private DialogState _dialogState;
        [Header("Story Weights")]
        public Text _dialogueText;
        private bool _isDialogShowing;
        private bool _isToBeRemoved;
        private RenderTextureWrapper _leftRenderTexture;
        public Transform _leftScreen;
        public Transform _leftSoundOnly;
        public Transform _leftSource;
        public Image _leftSpeakingImage;
        public Text _leftSpeakingNameText;
        private RenderTextureWrapper _rightRenderTexture;
        public Transform _rightScreen;
        public Transform _rightSoundOnly;
        public Transform _rightSource;
        public Image _rightSpeakingImage;
        public Text _rightSpeakingNameText;
        private int _rtDepth = 0x18;
        private RenderTextureFormat _rtFormat = RenderTextureFormat.ARGBHalf;
        private float _rtHeight = 300f;
        private float _rtWidth = 300f;
        private StoryScreenState _screenState;
        public Canvas _storyScreenCanvas;
        public RawImage _targetDisplayLeftImage;
        public RawImage _targetDisplayRightImage;
        private TypewriterEffect _typewritterEffect;
        private const int AVATAR_AI = 0x1f;
        private const int AVATAR_KSKSLITA = 0x33;
        private const int AVATAR_SOUND_ONLY = 11;
        private const int AVATAR_TERESAR = 0x15;
        private const int AVATAR_WENDY = 0x29;
        public float backModelCamOffsetZ = 3f;
        private const string CONTAINER_PREFAB_PATH = "UI/Menus/Widget/Storage/StoryScreen3dModel";
        public float downModelCamOffsetY = -10f;
        [Header("Story Camera Control")]
        public float leftModelCamOffsetX = 3f;
        public Action<bool> onOpenAnimationChange;
        public float rightModelCamOffsetX = 3f;
        public float screenScale = 0.01f;
        public float screenXOffset;
        public float screenXRotation;
        public float screenYOffset;
        public float screenYRotation;
        public float screenZOffset = 2f;
        public float screenZRotation;
        private Sprite soundOnlySprite;
        private const string TEXTURE_PATH = "UI/RenderTexture/TestRenderTexture";

        protected void Awake()
        {
            this._isToBeRemoved = false;
        }

        [DebuggerHidden]
        private IEnumerator DieProcessIter()
        {
            return new <DieProcessIter>c__Iterator23 { <>f__this = this };
        }

        public void FinishiDialog()
        {
            this._isDialogShowing = false;
        }

        private string GetAvatarName(int avatarID)
        {
            AvatarDataItem item = Singleton<AvatarModule>.Instance.TryGetAvatarByID(avatarID);
            if (item != null)
            {
                if (item.IsEasterner())
                {
                    return string.Format("{0}\t{1}", item.ClassLastName, item.ClassFirstName);
                }
                return string.Format("{0}\x00b7{1}", item.ClassFirstName, item.ClassLastName);
            }
            if (avatarID == 11)
            {
                return LocalizationGeneralLogic.GetText("UnknownName", new object[0]);
            }
            if (avatarID == 0x15)
            {
                return LocalizationGeneralLogic.GetText("Teresa", new object[0]);
            }
            if (avatarID == 0x1f)
            {
                return LocalizationGeneralLogic.GetText("Ai", new object[0]);
            }
            if (avatarID == 0x29)
            {
                return LocalizationGeneralLogic.GetText("Wendy", new object[0]);
            }
            if (avatarID == 0x33)
            {
                return LocalizationGeneralLogic.GetText("Ksksliya", new object[0]);
            }
            return LocalizationGeneralLogic.GetText("UnknownSpeaker", new object[0]);
        }

        private Vector3 GetAvatarUIModelCameraEuler(int avatarID)
        {
            string key = string.Empty;
            if (avatarID == 0x15)
            {
                key = "Theresa";
            }
            else if (avatarID == 0x29)
            {
                key = "Wendy";
            }
            else
            {
                AvatarDataItem item = Singleton<AvatarModule>.Instance.TryGetAvatarByID(avatarID);
                key = (item != null) ? item.AvatarRegistryKey : string.Empty;
            }
            ConfigPlotAvatarCameraPosInfo plotAvatarCameraPosInfo = MiscData.GetPlotAvatarCameraPosInfo();
            key = !plotAvatarCameraPosInfo.AvatarPlotCameraPosInfos.ContainsKey(key) ? "Default" : key;
            return plotAvatarCameraPosInfo.AvatarPlotCameraPosInfos[key].LookAt.EulerAngle;
        }

        private Vector3 GetAvatarUIModelCameraPos(int avatarID)
        {
            string key = string.Empty;
            if (avatarID == 0x15)
            {
                key = "Theresa";
            }
            else if (avatarID == 0x29)
            {
                key = "Wendy";
            }
            else
            {
                AvatarDataItem item = Singleton<AvatarModule>.Instance.TryGetAvatarByID(avatarID);
                key = (item != null) ? item.AvatarRegistryKey : string.Empty;
            }
            ConfigPlotAvatarCameraPosInfo plotAvatarCameraPosInfo = MiscData.GetPlotAvatarCameraPosInfo();
            key = !plotAvatarCameraPosInfo.AvatarPlotCameraPosInfos.ContainsKey(key) ? "Default" : key;
            return plotAvatarCameraPosInfo.AvatarPlotCameraPosInfos[key].LookAt.Position;
        }

        private string GetAvatarUIModelPrefabPath(int avatarID)
        {
            AvatarDataItem item = Singleton<AvatarModule>.Instance.TryGetAvatarByID(avatarID);
            if (item != null)
            {
                string avatarRegistryKey = item.AvatarRegistryKey;
                return string.Format("Entities/Avatar/{0}/Avatar_{0}_UI", avatarRegistryKey);
            }
            if (avatarID == 0x15)
            {
                string str3 = "Theresa";
                return string.Format("Entities/Avatar/Theresa_Story/Avatar_{0}_Story_UI", str3);
            }
            return string.Empty;
        }

        public override void Init(uint runtimeID, uint ownerID)
        {
            base.Init(runtimeID, ownerID);
        }

        private void InitAvatarModelSetting(int leftAvatarID, int rightAvatarID)
        {
            Vector3 vector = Vector3.Cross(this._avatar.FaceDirection, Vector3.up);
            this._containerLeft = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Storage/StoryScreen3dModel", BundleType.RESOURCE_FILE));
            this._containerRight = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Storage/StoryScreen3dModel", BundleType.RESOURCE_FILE));
            this._leftRenderTexture = GraphicsUtils.GetRenderTexture((int) this._rtWidth, (int) this._rtHeight, this._rtDepth, this._rtFormat);
            this._rightRenderTexture = GraphicsUtils.GetRenderTexture((int) this._rtWidth, (int) this._rtHeight, this._rtDepth, this._rtFormat);
            Camera component = this._containerLeft.transform.Find("StoryCamera").GetComponent<Camera>();
            Camera camera = this._containerRight.transform.Find("StoryCamera").GetComponent<Camera>();
            this._leftRenderTexture.BindToCamera(component);
            this._rightRenderTexture.BindToCamera(camera);
            this.soundOnlySprite = Miscs.GetSpriteByPrefab("SpriteOutput/StoryImgs/SoundOnly");
            this._targetDisplayLeftImage.texture = !this.NeedShowSoundOnly(leftAvatarID) ? ((Texture) this._leftRenderTexture) : ((Texture) this.soundOnlySprite.texture);
            this._targetDisplayRightImage.texture = !this.NeedShowSoundOnly(rightAvatarID) ? ((Texture) this._rightRenderTexture) : ((Texture) this.soundOnlySprite.texture);
            this._containerLeft.transform.position = (Vector3) (((this._avatar.XZPosition - (this._avatar.FaceDirection * this.backModelCamOffsetZ)) - (vector * this.leftModelCamOffsetX)) + (Vector3.up * this.downModelCamOffsetY));
            this._containerRight.transform.position = (Vector3) (((this._avatar.XZPosition - (this._avatar.FaceDirection * this.backModelCamOffsetZ)) + (vector * this.rightModelCamOffsetX)) + (Vector3.up * this.downModelCamOffsetY));
            this._containerLeft.transform.forward = -this._avatar.FaceDirection;
            this._containerRight.transform.forward = -this._avatar.FaceDirection;
            string avatarUIModelPrefabPath = this.GetAvatarUIModelPrefabPath(leftAvatarID);
            string str2 = this.GetAvatarUIModelPrefabPath(rightAvatarID);
            this._currentLeftModelAvatarID = leftAvatarID;
            this._currentRightModelAvatarID = rightAvatarID;
            GameObject obj2 = null;
            GameObject obj3 = null;
            if (!string.IsNullOrEmpty(avatarUIModelPrefabPath))
            {
                obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(avatarUIModelPrefabPath, BundleType.RESOURCE_FILE));
                obj2.transform.position = Vector3.zero;
                obj2.transform.SetParent(this._containerLeft.transform.Find("Model").transform);
                obj2.transform.localPosition = Vector3.zero;
                obj2.transform.forward = -this._avatar.FaceDirection;
                BaseMonoUIAvatar avatar = obj2.GetComponent<BaseMonoUIAvatar>();
                if (avatar != null)
                {
                    avatar.Init(leftAvatarID);
                }
            }
            if (!string.IsNullOrEmpty(str2))
            {
                obj3 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(str2, BundleType.RESOURCE_FILE));
                obj3.transform.position = Vector3.zero;
                obj3.transform.SetParent(this._containerRight.transform.Find("Model").transform);
                obj3.transform.localPosition = Vector3.zero;
                obj3.transform.forward = -this._avatar.FaceDirection;
                BaseMonoUIAvatar avatar2 = obj3.GetComponent<BaseMonoUIAvatar>();
                if (avatar2 != null)
                {
                    avatar2.Init(rightAvatarID);
                }
            }
            Transform transform = this._containerLeft.transform.Find("StoryCamera").transform;
            Transform transform2 = this._containerRight.transform.Find("StoryCamera").transform;
            Vector3 avatarUIModelCameraPos = this.GetAvatarUIModelCameraPos(this._currentLeftModelAvatarID);
            Vector3 vector3 = this.GetAvatarUIModelCameraPos(this._currentRightModelAvatarID);
            Vector3 avatarUIModelCameraEuler = this.GetAvatarUIModelCameraEuler(this._currentLeftModelAvatarID);
            Vector3 vector5 = this.GetAvatarUIModelCameraEuler(this._currentRightModelAvatarID);
            transform.localPosition = avatarUIModelCameraPos;
            transform.localEulerAngles = avatarUIModelCameraEuler;
            transform2.localPosition = vector3;
            transform2.localEulerAngles = vector5;
        }

        private void InitScreenTransfomSetting()
        {
            this.screenZOffset = this._avatar.config.StoryCameraSetting.screenZOffset;
            this.screenYOffset = this._avatar.config.StoryCameraSetting.screenYOffset;
            this.screenXOffset = this._avatar.config.StoryCameraSetting.screenXOffset;
            this.screenScale = this._avatar.config.StoryCameraSetting.screenScale;
            base.transform.forward = this._avatar.transform.forward;
            RectTransform component = base.transform.GetComponent<RectTransform>();
            this.screenXRotation = component.eulerAngles.x;
            this.screenYRotation = component.eulerAngles.y;
            this.screenZRotation = component.eulerAngles.z;
            this._storyScreenCanvas = base.transform.Find("Canvas").GetComponent<Canvas>();
            if (this._storyScreenCanvas != null)
            {
                this._storyScreenCanvas.gameObject.SetActive(false);
            }
            if (this._storyScreenCanvas != null)
            {
                RectTransform transform = this._storyScreenCanvas.GetComponent<RectTransform>();
                transform.SetLocalScaleX(this.screenScale);
                transform.SetLocalScaleY(this.screenScale);
                transform.SetLocalScaleZ(this.screenScale);
            }
            Vector3 vector = Vector3.Cross(this._avatar.FaceDirection, Vector3.up);
            base.transform.position = (Vector3) (((this._avatar.XZPosition + (this._avatar.FaceDirection * this.screenZOffset)) + (Vector3.up * this.screenYOffset)) + (vector * this.screenXOffset));
        }

        private void InitScreenWeights(DialogDataItem leftDataItem, DialogDataItem rightDataItem)
        {
            this._leftSpeakingNameText.text = this.GetAvatarName(leftDataItem.avatarID);
            this._rightSpeakingNameText.text = this.GetAvatarName(rightDataItem.avatarID);
            this._typewritterEffect = this._dialogueText.GetComponent<TypewriterEffect>();
            this._dialogueText.text = string.Empty;
            Color color = MiscData.GetColor("PlotSpeakerOffBgColor");
            this._leftSpeakingImage.color = color;
            this._rightSpeakingImage.color = color;
            if (leftDataItem != null)
            {
                this._leftSoundOnly.gameObject.SetActive(this.NeedShowSoundOnly(leftDataItem.avatarID));
                this.SetSourceByName(leftDataItem.source, SelectScreenSide.Left);
            }
            if (rightDataItem != null)
            {
                this._rightSoundOnly.gameObject.SetActive(this.NeedShowSoundOnly(rightDataItem.avatarID));
                this.SetSourceByName(rightDataItem.source, SelectScreenSide.Right);
            }
            Animation component = base.transform.Find("Canvas/Plot3DDialog/FaceTime/Face_1/Window/3dModel").GetComponent<Animation>();
            if (component != null)
            {
                component.Play("PlotScreenNoise");
            }
            Animation animation2 = base.transform.Find("Canvas/Plot3DDialog/FaceTime/Face_2/Window/3dModel").GetComponent<Animation>();
            if (animation2 != null)
            {
                animation2.Play("PlotScreenNoise1");
            }
        }

        public override bool IsActive()
        {
            return !this._isToBeRemoved;
        }

        public bool IsDialogProcessingOpen()
        {
            Animation component = this._storyScreenCanvas.transform.Find("Plot3DDialog").GetComponent<Animation>();
            if (component == null)
            {
                return false;
            }
            return (component.isPlaying || (this._screenState == StoryScreenState.Loading));
        }

        public bool IsDialogShowing()
        {
            return this._isDialogShowing;
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemoved;
        }

        private bool NeedShowSoundOnly(int avatarID)
        {
            return ((((avatarID == 11) || (avatarID == 0x1f)) || (avatarID == 0x29)) || (avatarID == 0x33));
        }

        public void RefreshAvatar3dModel(int avatarID, SelectScreenSide side)
        {
            Transform modelParent = null;
            bool flag = false;
            if (side == SelectScreenSide.Left)
            {
                modelParent = this._containerLeft.transform.Find("Model");
                if (this._currentLeftModelAvatarID != avatarID)
                {
                    flag = true;
                    this._currentLeftModelAvatarID = avatarID;
                }
            }
            else
            {
                modelParent = this._containerRight.transform.Find("Model");
                if (this._currentRightModelAvatarID != avatarID)
                {
                    flag = true;
                    this._currentRightModelAvatarID = avatarID;
                }
            }
            ((side != SelectScreenSide.Left) ? this._rightScreen.FindChild("Name").GetComponent<Animation>() : this._leftScreen.FindChild("Name").GetComponent<Animation>()).Play("PlotScreenCurrent");
            if (flag)
            {
                int childCount = modelParent.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    UnityEngine.Object.Destroy(modelParent.GetChild(i).gameObject);
                }
                ((side != SelectScreenSide.Left) ? this._rightScreen.GetComponent<Animation>() : this._leftScreen.GetComponent<Animation>()).Play("PlotScreenSwitch");
                base.StartCoroutine(this.SyncLoadModelAndSetPos(avatarID, modelParent));
            }
        }

        public void RefreshCurrentSpeakerWidgets(DialogDataItem dialogDataItem)
        {
            Color color = MiscData.GetColor("PlotSpeakerOnBgColor");
            Color color2 = MiscData.GetColor("PlotSpeakerOffBgColor");
            this._leftSpeakingImage.color = (dialogDataItem.screenSide != SelectScreenSide.Left) ? color2 : color;
            this._rightSpeakingImage.color = (dialogDataItem.screenSide != SelectScreenSide.Right) ? color2 : color;
            Color color3 = MiscData.GetColor("PlotSpeakerFadeScreenColor");
            if (dialogDataItem.screenSide == SelectScreenSide.Left)
            {
                this._leftSpeakingNameText.text = this.GetAvatarName(dialogDataItem.avatarID);
                bool flag = this.NeedShowSoundOnly(dialogDataItem.avatarID);
                this._leftSoundOnly.gameObject.SetActive(flag);
                this._targetDisplayLeftImage.texture = !flag ? ((Texture) this._leftRenderTexture) : ((Texture) this.soundOnlySprite.texture);
                this._targetDisplayLeftImage.color = Color.white;
                this._targetDisplayRightImage.color = color3;
            }
            else if (dialogDataItem.screenSide == SelectScreenSide.Right)
            {
                this._rightSpeakingNameText.text = this.GetAvatarName(dialogDataItem.avatarID);
                bool flag2 = this.NeedShowSoundOnly(dialogDataItem.avatarID);
                this._rightSoundOnly.gameObject.SetActive(flag2);
                this._targetDisplayRightImage.texture = !flag2 ? ((Texture) this._rightRenderTexture) : ((Texture) this.soundOnlySprite.texture);
                this._targetDisplayLeftImage.color = color3;
                this._targetDisplayRightImage.color = Color.white;
            }
            this.SetSourceByName(dialogDataItem.source, dialogDataItem.screenSide);
        }

        public override void SetDied()
        {
            base.SetDied();
            this._isToBeRemoved = true;
            UnityEngine.Object.DestroyImmediate(this._containerLeft);
            UnityEngine.Object.DestroyImmediate(this._containerRight);
            if (this._leftRenderTexture != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._leftRenderTexture);
                this._leftRenderTexture = null;
            }
            if (this._rightRenderTexture != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._rightRenderTexture);
                this._rightRenderTexture = null;
            }
            this._targetDisplayLeftImage = null;
            this._targetDisplayRightImage = null;
            this._containerLeft = null;
            this._containerRight = null;
            base.StopAllCoroutines();
        }

        public void SetDisplayText(string content)
        {
            if ((content != null) && (this._dialogueText != null))
            {
                this._dialogueText.text = content;
                if (this._storyScreenCanvas != null)
                {
                    Animation componentInParent = this._dialogueText.GetComponentInParent<Animation>();
                    if (componentInParent.isPlaying)
                    {
                        componentInParent.Stop();
                    }
                    componentInParent.Play();
                    if (this._typewritterEffect != null)
                    {
                        this._isDialogShowing = true;
                        this._typewritterEffect.RestartRead();
                    }
                }
            }
        }

        private void SetSourceByName(string source, SelectScreenSide side)
        {
            if (side == SelectScreenSide.Left)
            {
                this._leftSource.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(source, new object[0]);
            }
            else if (side == SelectScreenSide.Right)
            {
                this._rightSource.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(source, new object[0]);
            }
        }

        public void SetupView(int plotID)
        {
            this._avatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            PlotDataItem plotDataItem = new PlotDataItem(PlotMetaDataReader.TryGetPlotMetaDataByKey(plotID));
            DialogDataItem leftDataItem = null;
            DialogDataItem rightDataItem = null;
            if (plotDataItem != null)
            {
                leftDataItem = DialogMetaDataReader.GetFirstLeftDialogDataItem(plotDataItem);
                rightDataItem = DialogMetaDataReader.GetFirstRightDialogDataItem(plotDataItem);
            }
            this.InitScreenTransfomSetting();
            this.InitAvatarModelSetting(leftDataItem.avatarID, rightDataItem.avatarID);
            this.InitScreenWeights(leftDataItem, rightDataItem);
            Singleton<MainUIManager>.Instance.ShowPage(new InLevelPlotPageContext(this, plotID, null), UIType.Page);
            this._screenState = StoryScreenState.Loading;
        }

        [DebuggerHidden]
        private IEnumerator ShowProcessIter()
        {
            return new <ShowProcessIter>c__Iterator22 { <>f__this = this };
        }

        public void SkipDialog()
        {
            if (this._typewritterEffect != null)
            {
                this._typewritterEffect.Finish();
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        public void StartDie()
        {
            base.StartCoroutine(this.DieProcessIter());
        }

        public void StartShow()
        {
            Singleton<ApplicationManager>.Instance.StartCoroutine(this.ShowProcessIter());
        }

        [DebuggerHidden]
        private IEnumerator SyncLoadModelAndSetPos(int avatarID, Transform modelParent)
        {
            return new <SyncLoadModelAndSetPos>c__Iterator21 { modelParent = modelParent, avatarID = avatarID, <$>modelParent = modelParent, <$>avatarID = avatarID, <>f__this = this };
        }

        protected override void Update()
        {
            base.Update();
            if (this._screenState == StoryScreenState.Loading)
            {
                if (this._storyScreenCanvas != null)
                {
                    this._storyScreenCanvas.gameObject.SetActive(true);
                    this.StartShow();
                    this._screenState = StoryScreenState.Opening;
                }
            }
            else if (this._screenState == StoryScreenState.Opening)
            {
                if (this._storyScreenCanvas != null)
                {
                    RectTransform transform = this._storyScreenCanvas.GetComponent<RectTransform>();
                    transform.SetLocalScaleX(this.screenScale);
                    transform.SetLocalScaleY(this.screenScale);
                    transform.SetLocalScaleZ(this.screenScale);
                }
                if (this._avatar != null)
                {
                    Vector3 vector = Vector3.Cross(this._avatar.FaceDirection, Vector3.up);
                    base.transform.position = (Vector3) (((this._avatar.XZPosition + (this._avatar.FaceDirection * this.screenZOffset)) + (Vector3.up * this.screenYOffset)) + (vector * this.screenXOffset));
                }
                RectTransform component = base.transform.GetComponent<RectTransform>();
                if (component != null)
                {
                    component.SetLocalEulerAnglesX(this.screenXRotation);
                    component.SetLocalEulerAnglesY(this.screenYRotation);
                    component.SetLocalEulerAnglesZ(this.screenZRotation);
                }
            }
        }

        public DialogState StoryDialogState
        {
            get
            {
                return this._dialogState;
            }
            set
            {
                this._dialogState = value;
            }
        }

        public TypewriterEffect Typewritter
        {
            get
            {
                return this._typewritterEffect;
            }
        }

        [CompilerGenerated]
        private sealed class <DieProcessIter>c__Iterator23 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoStoryScreen <>f__this;
            internal Animation <endAnimation>__0;
            internal AnimationState <endAnimationState>__1;

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
                        if (this.<>f__this._storyScreenCanvas != null)
                        {
                            this.<endAnimation>__0 = this.<>f__this._storyScreenCanvas.transform.Find("Plot3DDialog").GetComponent<Animation>();
                            if (this.<endAnimation>__0 == null)
                            {
                                goto Label_015F;
                            }
                            if (this.<endAnimation>__0.isPlaying)
                            {
                                this.<endAnimation>__0.Stop();
                            }
                            this.<endAnimationState>__1 = this.<endAnimation>__0["PlotDialogAppear"];
                            if (this.<endAnimationState>__1 != null)
                            {
                                this.<endAnimationState>__1.speed = -1f;
                                this.<endAnimationState>__1.time = this.<endAnimationState>__1.clip.length;
                                this.<endAnimation>__0.Play();
                                break;
                            }
                        }
                        goto Label_0166;

                    case 1:
                        break;

                    default:
                        goto Label_0166;
                }
                while (this.<endAnimation>__0.isPlaying)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (this.<>f__this.onOpenAnimationChange != null)
                {
                    this.<>f__this.onOpenAnimationChange(false);
                    Singleton<LevelDesignManager>.Instance.SetPause(false);
                    Singleton<LevelDesignManager>.Instance.SetMuteAvatarVoice(false);
                    this.<>f__this.SetDied();
                }
            Label_015F:
                this.$PC = -1;
            Label_0166:
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
        private sealed class <ShowProcessIter>c__Iterator22 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoStoryScreen <>f__this;
            internal Animation <showAnimation>__0;

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
                        this.<showAnimation>__0 = this.<>f__this._storyScreenCanvas.transform.Find("Plot3DDialog").GetComponent<Animation>();
                        if (this.<showAnimation>__0 != null)
                        {
                            if (this.<showAnimation>__0.isPlaying)
                            {
                                this.<showAnimation>__0.Stop();
                            }
                            this.<showAnimation>__0.Play();
                        }
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00CE;
                }
                while (this.<showAnimation>__0.isPlaying)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (this.<>f__this.onOpenAnimationChange != null)
                {
                    this.<>f__this.onOpenAnimationChange(true);
                }
                this.$PC = -1;
            Label_00CE:
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
        private sealed class <SyncLoadModelAndSetPos>c__Iterator21 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal int <$>avatarID;
            internal Transform <$>modelParent;
            internal MonoStoryScreen <>f__this;
            internal Vector3 <leftAvatarCameraEulerAngle>__7;
            internal Vector3 <leftAvatarCameraPos>__5;
            internal Transform <leftCameraTrans>__3;
            internal Transform <modelTrans>__1;
            internal string <prefabPath>__0;
            internal Vector3 <rightAvatarCameraEulerAngle>__8;
            internal Vector3 <rightAvatarCameraPos>__6;
            internal Transform <rightCameraTrans>__4;
            internal BaseMonoUIAvatar <uiAvatar>__2;
            internal int avatarID;
            internal Transform modelParent;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                this.$PC = -1;
                if ((this.$PC == 0) && (this.modelParent != null))
                {
                    this.<prefabPath>__0 = this.<>f__this.GetAvatarUIModelPrefabPath(this.avatarID);
                    if (!string.IsNullOrEmpty(this.<prefabPath>__0))
                    {
                        this.<modelTrans>__1 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(this.<prefabPath>__0, BundleType.RESOURCE_FILE)).transform;
                        this.<modelTrans>__1.transform.position = Vector3.zero;
                        this.<modelTrans>__1.SetParent(this.modelParent.transform);
                        this.<modelTrans>__1.transform.localPosition = Vector3.zero;
                        this.<modelTrans>__1.transform.forward = -this.<>f__this._avatar.FaceDirection;
                        this.<uiAvatar>__2 = this.<modelTrans>__1.GetComponent<BaseMonoUIAvatar>();
                        if (this.<uiAvatar>__2 != null)
                        {
                            this.<uiAvatar>__2.Init(this.avatarID);
                        }
                        this.<leftCameraTrans>__3 = this.<>f__this._containerLeft.transform.Find("StoryCamera").transform;
                        this.<rightCameraTrans>__4 = this.<>f__this._containerRight.transform.Find("StoryCamera").transform;
                        this.<leftAvatarCameraPos>__5 = this.<>f__this.GetAvatarUIModelCameraPos(this.<>f__this._currentLeftModelAvatarID);
                        this.<rightAvatarCameraPos>__6 = this.<>f__this.GetAvatarUIModelCameraPos(this.<>f__this._currentRightModelAvatarID);
                        this.<leftAvatarCameraEulerAngle>__7 = this.<>f__this.GetAvatarUIModelCameraEuler(this.<>f__this._currentLeftModelAvatarID);
                        this.<rightAvatarCameraEulerAngle>__8 = this.<>f__this.GetAvatarUIModelCameraEuler(this.<>f__this._currentRightModelAvatarID);
                        this.<leftCameraTrans>__3.localPosition = this.<leftAvatarCameraPos>__5;
                        this.<leftCameraTrans>__3.localEulerAngles = this.<leftAvatarCameraEulerAngle>__7;
                        this.<rightCameraTrans>__4.localPosition = this.<rightAvatarCameraPos>__6;
                        this.<rightCameraTrans>__4.localEulerAngles = this.<rightAvatarCameraEulerAngle>__8;
                    }
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

        public enum DialogState
        {
            Default,
            Displaying,
            ChatEnd,
            DialogEnd
        }

        public enum SelectScreenSide
        {
            Left,
            Right,
            None
        }

        public enum StoryScreenState
        {
            Loading,
            Opening,
            Closing
        }
    }
}

