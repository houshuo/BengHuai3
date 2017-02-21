namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MonoActButton : MonoBehaviour
    {
        private ActDataItem _actData;
        private Sprite _activityBgImgSprite;
        private WeekDayActivityDataItem _activityDatta;
        private float _bgAniTimer;
        private Sprite _bgImgSprite;
        private Transform _bgTrans;
        private Outline _descOutline;
        private Text _descText;
        private Transform _descTransform;
        private CanvasGroup _exBGCanvasGroup;
        private GameObject _exBGGameObject;
        private GameObject _hideDotGameObject;
        private GameObject _hideGameObject;
        private GameObject _imageGameObject;
        private Image _imageImage;
        private MonoActivityInfoPanel _infoPanel;
        private bool _needFade;
        private CanvasGroup _postCanvasGroup;
        private Image _postImage;
        private CanvasGroup _preCanvasGroup;
        private Image _preImage;
        private MonoActScroller _scroller;
        private GameObject _selectDotGameObject;
        private GameObject _selectedGameObject;
        private static BGState _state;
        private BGAniStep _step;
        private GameObject _unselectDotGameObject;
        private GameObject _unselectedGameObject;
        private Material _unselectMaterial;
        private const string ACTIVITY_LEVEL_PANEL = "UI/Menus/Widget/Map/LevelPanelActivity";
        private const float BG_ANI_PREPARE_SPAN = 0.1f;
        private const float BG_FADE_IN_SPAN = 0.4f;
        private const string MATERIAL_COLOR_PATH = "Material/ImageMonoColor";
        private const string MATERIAL_COLORIZE_PATH = "Material/ImageColorize";
        private const int SELECT_FONTSIZE = 30;
        private const float SELECT_POSITION_X = -42f;
        private const int UNSELECT_FONTSIZE = 0x18;
        private const float UNSELECT_POSITION_X = -22f;

        public ActDataItem GetActData()
        {
            return this._actData;
        }

        public WeekDayActivityDataItem GetWeekDayActivityData()
        {
            return this._activityDatta;
        }

        private void InitCache()
        {
            this._selectedGameObject = base.transform.Find("Selected").gameObject;
            this._unselectedGameObject = base.transform.Find("Unselected").gameObject;
            this._hideGameObject = base.transform.Find("Hide").gameObject;
            this._imageGameObject = base.transform.Find("Image").gameObject;
            this._imageImage = this._imageGameObject.GetComponent<Image>();
            this._hideDotGameObject = base.transform.Find("TimeLineDots/HideDot").gameObject;
            this._selectDotGameObject = base.transform.Find("TimeLineDots/SelectDot").gameObject;
            this._unselectDotGameObject = base.transform.Find("TimeLineDots/UnselectDot").gameObject;
            this._descTransform = base.transform.Find("Desc");
            this._descText = this._descTransform.GetComponent<Text>();
            this._descOutline = this._descTransform.GetComponent<Outline>();
            this._preImage = this._bgTrans.Find("Pre").GetComponent<Image>();
            this._preCanvasGroup = this._bgTrans.Find("Pre").GetComponent<CanvasGroup>();
            this._postImage = this._bgTrans.Find("Post").GetComponent<Image>();
            this._postCanvasGroup = this._bgTrans.Find("Post").GetComponent<CanvasGroup>();
            this._exBGGameObject = this._bgTrans.Find("ExBG").gameObject;
            this._exBGCanvasGroup = this._bgTrans.Find("ExBG").GetComponent<CanvasGroup>();
            this._unselectMaterial = Miscs.LoadResource<Material>("Material/ImageMonoColor", BundleType.RESOURCE_FILE);
            if (this._actData != null)
            {
                this._bgImgSprite = Miscs.GetSpriteByPrefab(this._actData.BGImgPath);
            }
            if (this._activityDatta != null)
            {
                this._activityBgImgSprite = Miscs.GetSpriteByPrefab(this._activityDatta.GetBgImgPath());
            }
        }

        private void OnClick()
        {
            this._scroller.ClickToChangeCenter(base.transform);
        }

        private void OnDisable()
        {
            this._bgAniTimer = 0f;
            this._step = BGAniStep.None;
            _state = BGState.Idle;
        }

        public void SetupActivityView(WeekDayActivityDataItem activityData, MonoActivityInfoPanel infoPanel, List<LevelDataItem> levels, Transform levelScrollTrans, LevelBtnClickCallBack OnLevelClick, Transform bgTrans, Dictionary<LevelDataItem, Transform> levelTransDict)
        {
            this._activityDatta = activityData;
            this._infoPanel = infoPanel;
            this._bgTrans = bgTrans;
            this.InitCache();
            this._selectedGameObject.SetActive(true);
            this._unselectedGameObject.SetActive(true);
            this._hideGameObject.SetActive(false);
            this._imageGameObject.SetActive(true);
            if (!string.IsNullOrEmpty(activityData.GetSmallImgPath()))
            {
                this._imageImage.sprite = Miscs.GetSpriteByPrefab(activityData.GetSmallImgPath());
            }
            this._descText.text = activityData.GetActitityTitle();
            Transform transform = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(((activityData == null) || (activityData.GetStatus() != ActivityDataItemBase.Status.InProgress)) ? "UI/Menus/Widget/Map/LevelPanelActivity" : activityData.GetLevelPanelPath(), BundleType.RESOURCE_FILE)).transform;
            transform.SetParent(levelScrollTrans.Find("Content"), false);
            transform.GetComponent<MonoLevelPanel>().SetupView(levels, OnLevelClick, levelTransDict, activityData, 0);
            base.transform.GetComponent<MonoItemStatus>().isValid = true;
            transform.GetComponent<MonoItemStatus>().isValid = true;
            this._bgTrans.gameObject.SetActive(true);
            if (this._activityDatta != null)
            {
                this._preImage.sprite = Miscs.GetSpriteByPrefab(this._activityDatta.GetBgImgPath());
                this._postImage.sprite = Miscs.GetSpriteByPrefab(this._activityDatta.GetBgImgPath());
                this._infoPanel.SetupView(this._activityDatta);
            }
        }

        public void SetupActView(ActDataItem actData, List<LevelDataItem> levels, Transform levelScrollTrans, LevelBtnClickCallBack OnLevelClick, Transform bgTrans, Dictionary<LevelDataItem, Transform> levelTransDict, int totalFinishChallengeNum)
        {
            this._actData = actData;
            this._bgTrans = bgTrans;
            this.InitCache();
            this._selectedGameObject.SetActive(true);
            this._unselectedGameObject.SetActive(true);
            this._hideGameObject.SetActive(false);
            this._imageGameObject.SetActive(true);
            if (!string.IsNullOrEmpty(actData.smallImgPath))
            {
                this._imageImage.sprite = Miscs.GetSpriteByPrefab(actData.smallImgPath);
            }
            this._descText.text = actData.actTitle;
            Transform transform = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(actData.levelPanelPath, BundleType.RESOURCE_FILE)).transform;
            transform.SetParent(levelScrollTrans.Find("Content"), false);
            transform.GetComponent<MonoLevelPanel>().SetupView(levels, OnLevelClick, levelTransDict, null, totalFinishChallengeNum);
            base.transform.GetComponent<MonoItemStatus>().isValid = true;
            transform.GetComponent<MonoItemStatus>().isValid = true;
            this._bgTrans.gameObject.SetActive(true);
            if (actData != null)
            {
                this._preImage.sprite = this._bgImgSprite;
                this._preCanvasGroup.alpha = 1f;
                this._postImage.sprite = this._bgImgSprite;
                this._postCanvasGroup.alpha = 1f;
            }
        }

        public void SetupStatus(bool isSelect)
        {
            this._hideGameObject.SetActive(false);
            this._hideDotGameObject.SetActive(false);
            this.selected = isSelect;
            if (!isSelect)
            {
                this._selectedGameObject.SetActive(false);
                this._unselectedGameObject.SetActive(true);
                this._selectDotGameObject.SetActive(false);
                this._unselectDotGameObject.SetActive(true);
                this._imageImage.color = MiscData.GetColor("ActImageUnSelect");
                this._descTransform.SetLocalPositionX(-22f);
                this._descText.fontSize = 0x18;
                this._descText.color = MiscData.GetColor("ActDescGray");
                this._descOutline.enabled = false;
                this._needFade = false;
            }
            else
            {
                this._selectedGameObject.SetActive(true);
                this._unselectedGameObject.SetActive(false);
                this._selectDotGameObject.SetActive(true);
                this._unselectDotGameObject.SetActive(false);
                this._imageImage.material = null;
                this._imageImage.color = MiscData.GetColor("TotalWhite");
                this._descTransform.SetLocalPositionX(-42f);
                this._descText.fontSize = 30;
                this._descText.color = Color.white;
                this._descOutline.enabled = true;
                if (this._step != BGAniStep.FadeIn)
                {
                    this._step = BGAniStep.Prepare;
                    this._bgAniTimer = 0f;
                    this._needFade = true;
                }
            }
            if ((this._actData == null) && (this._activityDatta != null))
            {
                if (this._activityDatta.GetStatus() == ActivityDataItemBase.Status.Unavailable)
                {
                    this._imageImage.material = this._unselectMaterial;
                    this._imageImage.color = MiscData.GetColor("Blue");
                }
                if (isSelect)
                {
                    this._infoPanel.SetupView(this._activityDatta);
                }
            }
        }

        private void Start()
        {
            this._scroller = base.transform.parent.parent.GetComponent<MonoActScroller>();
            base.transform.Find("Btn").GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
        }

        private void Update()
        {
            switch (this._step)
            {
                case BGAniStep.Prepare:
                    this._bgAniTimer += Time.deltaTime;
                    if (this._bgAniTimer >= 0.1f)
                    {
                        if (!this._needFade)
                        {
                            this._step = BGAniStep.None;
                            break;
                        }
                        if (_state != BGState.Fading)
                        {
                            this._bgAniTimer = 0f;
                            this._step = BGAniStep.FadeIn;
                            _state = BGState.Fading;
                            this._postImage.sprite = (this._actData == null) ? this._activityBgImgSprite : this._bgImgSprite;
                        }
                    }
                    break;

                case BGAniStep.FadeIn:
                    this._bgAniTimer += Time.deltaTime;
                    this._preCanvasGroup.alpha = Mathf.Clamp((float) (1f - (this._bgAniTimer / 0.4f)), (float) 0f, (float) 1f);
                    if (this._bgAniTimer >= 0.4f)
                    {
                        this._step = BGAniStep.None;
                        _state = BGState.Idle;
                        this._preImage.sprite = this._postImage.sprite;
                        this._preCanvasGroup.alpha = 1f;
                    }
                    if ((this._activityDatta != null) && (this._activityDatta.GetActivityType() == 3))
                    {
                        this._exBGGameObject.SetActive(true);
                        this._exBGCanvasGroup.alpha = Mathf.Clamp((float) (this._bgAniTimer / 0.4f), (float) 0f, (float) 1f);
                        if (this._bgAniTimer >= 0.4f)
                        {
                            this._exBGCanvasGroup.alpha = 1f;
                        }
                    }
                    else
                    {
                        this._exBGCanvasGroup.alpha = Mathf.Clamp((float) (1f - (this._bgAniTimer / 0.4f)), (float) 0f, (float) 1f);
                        if (this._bgAniTimer >= 0.4f)
                        {
                            this._exBGGameObject.SetActive(false);
                        }
                    }
                    break;
            }
        }

        public bool selected { get; private set; }

        private enum BGAniStep
        {
            None,
            Prepare,
            FadeIn
        }

        private enum BGState
        {
            Idle,
            Fading
        }
    }
}

