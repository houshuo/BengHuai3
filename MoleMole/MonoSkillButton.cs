namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class MonoSkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
    {
        private BaseMonoAvatar _avatar;
        private AvatarActor _avatarActor;
        private Button _button;
        private bool _buttonClickTrigger;
        private bool _buttonHoldBegin;
        private MonoSkillButtonChargeCount _chargeCount;
        private BaseAvatarInputController _controller;
        private bool _hasPlayedUltraReadySound;
        private float _holdTime;
        private Image _image;
        public bool _isPointerHold;
        private string _maskImagePath;
        private Image _maskImg;
        private bool _muteButtonHighLight;
        private AvatarActor.SKillInfo _skillInfo;
        private float _totalHoldTime;
        private const string BLUE_BG_PATH = "SpriteOutput/AvatarInLevelBtnIcons/BtnSkill";
        private static float HOLD_TIME_THRESHOLD = 0.2f;
        public string KeyButtonCode;
        private const string MATERIAL_COLORIZE_BLUE_PATH = "Material/ImageColorizeSkillIconBlue";
        private const string MATERIAL_COLORIZE_YELLOW_PATH = "Material/ImageColorizeSkillIconYellow";
        public Func<PointerState, bool> onPointerStateChange;
        public string SkillName;
        private const string YELLOW_BG_PATH = "SpriteOutput/AvatarInLevelBtnIcons/BtnUniqueSkill2";

        private void ActButtonBlingEffect()
        {
            if (base.gameObject.activeInHierarchy && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning))
            {
                Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_Button_Bling", base.transform.position, base.transform.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
            }
        }

        private void Awake()
        {
            this._image = base.transform.Find("Image").GetComponent<Image>();
            this._maskImg = base.transform.Find("ImageMask").GetComponent<Image>();
            this._button = base.gameObject.GetComponent<Button>();
        }

        private string GetSkillIconPath()
        {
            this._maskImagePath = this._skillInfo.maskIconPath;
            if (string.IsNullOrEmpty(this._maskImagePath))
            {
                return this._skillInfo.iconPath;
            }
            return this._maskImagePath;
        }

        public void InitSkillButton(BaseAvatarInputController controller)
        {
            this._controller = controller;
            if (this._avatarActor != null)
            {
                this._avatarActor.onSkillChargeChanged = (Action<string, int, int>) Delegate.Remove(this._avatarActor.onSkillChargeChanged, new Action<string, int, int>(this.OnSkillChargeChanged));
                this._avatarActor.onSkillSPNeedChanged = (Action<string, float, float>) Delegate.Remove(this._avatarActor.onSkillSPNeedChanged, new Action<string, float, float>(this.OnSkillSPNeedChanged));
                this._avatarActor.onSPChanged = (Action<float, float, float>) Delegate.Remove(this._avatarActor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
            }
            this._avatar = this._controller.avatar;
            this._avatarActor = (AvatarActor) Singleton<EventManager>.Instance.GetActor(this._avatar.GetRuntimeID());
            this._skillInfo = this._avatarActor.GetSkillInfo(this.SkillName);
            base.gameObject.SetActive(true);
            if (this._skillInfo.MaxChargesCount > 0)
            {
                this._avatarActor.onSkillChargeChanged = (Action<string, int, int>) Delegate.Combine(this._avatarActor.onSkillChargeChanged, new Action<string, int, int>(this.OnSkillChargeChanged));
            }
            this._avatarActor.onSPChanged = (Action<float, float, float>) Delegate.Combine(this._avatarActor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
            this._avatarActor.onSkillSPNeedChanged = (Action<string, float, float>) Delegate.Combine(this._avatarActor.onSkillSPNeedChanged, new Action<string, float, float>(this.OnSkillSPNeedChanged));
            this.RefreshSkillInfo();
        }

        public bool IsPointerHold()
        {
            return this._isPointerHold;
        }

        private bool IsUltraSkill()
        {
            return (this.SkillName == "SKL02");
        }

        private void OnDestroy()
        {
            this._image = null;
            this._maskImg = null;
            this._button = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (this._button.interactable)
            {
                this._isPointerHold = true;
                this._buttonClickTrigger = true;
                if (this.onPointerStateChange != null)
                {
                    this._buttonClickTrigger = this.onPointerStateChange(PointerState.PointerDown);
                }
                if ((this.SkillName == "ATK") && this._avatarActor.IsAttackButtonHoldMode())
                {
                    this._buttonHoldBegin = true;
                    this._buttonClickTrigger = false;
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this._button.interactable)
            {
                if (this._buttonHoldBegin && (this._totalHoldTime < HOLD_TIME_THRESHOLD))
                {
                    this._buttonClickTrigger = true;
                }
                this._buttonHoldBegin = false;
                this._isPointerHold = false;
                this._holdTime = 0f;
                this._totalHoldTime = 0f;
                if (this.onPointerStateChange != null)
                {
                    this.onPointerStateChange(PointerState.PointerUp);
                }
            }
        }

        private void OnSkillChargeChanged(string skillID, int from, int to)
        {
            if (skillID == this.SkillName)
            {
                if (this._chargeCount != null)
                {
                    this._chargeCount.SetupView((int) this._skillInfo.MaxChargesCount, to);
                }
                if ((to > 0) && this._avatarActor.CanUseSkill(this.SkillName))
                {
                    this.SetButtonHighlighted(true);
                    if (from == 0)
                    {
                        this.ActButtonBlingEffect();
                    }
                }
                else
                {
                    this.SetButtonHighlighted(false);
                }
            }
        }

        private void OnSkillSPNeedChanged(string skillID, float from, float to)
        {
            if (skillID == this.SkillName)
            {
                this.RefreshSkillInfo();
            }
        }

        private void OnSPChanged(float field, float newValue, float delta)
        {
            this.SetButtonInteractable(this._avatarActor.CanUseSkill(this.SkillName), false);
        }

        public void RefreshSkillInfo()
        {
            bool interactable = this._avatarActor.CanUseSkill(this.SkillName);
            this._image.sprite = Miscs.GetSpriteByPrefab(this.GetSkillIconPath());
            this._muteButtonHighLight = this._skillInfo.muteHighlighted;
            this._chargeCount = base.transform.Find("ChargesCount").GetComponent<MonoSkillButtonChargeCount>();
            if (this._skillInfo.MaxChargesCount > 0)
            {
                this._chargeCount.gameObject.SetActive(true);
                this._chargeCount.SetupView((int) this._skillInfo.MaxChargesCount, (int) this._skillInfo.chargesCounter);
                this.SetButtonHighlighted((this._skillInfo.chargesCounter > 0) && interactable);
            }
            else
            {
                this._chargeCount.gameObject.SetActive(false);
            }
            this.SetButtonInteractable(interactable, this.SkillName == "SKL02");
            int num = Mathf.FloorToInt(this._avatarActor.GetSkillSPNeed(this.SkillName));
            base.transform.Find("NeedSP").gameObject.SetActive(num > 0);
            if (num > 0)
            {
                base.transform.Find("NeedSP/Num").GetComponent<Text>().text = num.ToString();
            }
            if (this._muteButtonHighLight)
            {
                this.SetButtonHighlighted(interactable);
            }
        }

        private void SetButtonHighlighted(bool highlighted)
        {
            if (!string.IsNullOrEmpty(this._maskImagePath))
            {
                highlighted = false;
            }
            if (this._muteButtonHighLight)
            {
                highlighted = false;
            }
            base.transform.Find("Trigger").gameObject.SetActive(highlighted);
            this.SetImageMaterial();
        }

        private void SetButtonInteractable(bool interactable, bool force = false)
        {
            bool flag = this._button.interactable;
            if ((flag != interactable) || force)
            {
                this._button.interactable = interactable;
                this.SetImageMaterial();
                this.SetButtonHighlighted(interactable);
                this._image.GetComponent<CanvasGroup>().alpha = !interactable ? 0.3f : 1f;
                if (this._chargeCount.gameObject.activeSelf)
                {
                    this._chargeCount.GetComponent<CanvasGroup>().alpha = !interactable ? 0.3f : 1f;
                }
                if (((this.SkillName == "SKL02") && !flag) && this._button.interactable)
                {
                    this.ActButtonBlingEffect();
                }
            }
        }

        private void SetImageMaterial()
        {
        }

        private void TryToPlayUltraSound()
        {
            if (this.IsUltraSkill())
            {
                if (!this._avatarActor.CanUseSkill(this.SkillName))
                {
                    this._hasPlayedUltraReadySound = false;
                }
                else if (!this._hasPlayedUltraReadySound)
                {
                    MonoEntityAudio component = this._avatar.GetComponent<MonoEntityAudio>();
                    if (component != null)
                    {
                        component.PostUltraReady();
                    }
                    this._hasPlayedUltraReadySound = true;
                }
            }
        }

        private void Update()
        {
            this.UpdateForCDMask();
            this.TryToPlayUltraSound();
            if (this._isPointerHold)
            {
                this._totalHoldTime += Time.deltaTime;
                this._holdTime += Time.deltaTime;
                if (((this._holdTime > HOLD_TIME_THRESHOLD) && (this._avatarActor != null)) && (this._avatarActor.IsAttackButtonHoldMode() && (this.SkillName == "ATK")))
                {
                    this._controller.TryHold(this.SkillName);
                    this._holdTime = 0f;
                }
            }
            if ((this._isPointerHold && (this._skillInfo != null)) && this._skillInfo.canHold)
            {
                this.UseSkill();
            }
            else if (this._buttonClickTrigger)
            {
                this._buttonClickTrigger = false;
                this.UseSkill();
            }
        }

        private void UpdateForCDMask()
        {
            if (this._avatarActor.IsSkillInCD(this.SkillName))
            {
                float skillCD = this._avatarActor.GetSkillCD(this.SkillName);
                this.SetButtonInteractable(this._avatarActor.CanUseSkill(this.SkillName), false);
                if (skillCD == 0f)
                {
                    this._maskImg.fillAmount = 1f;
                }
                else
                {
                    this._maskImg.fillAmount = this._avatarActor.GetSkillInfo(this.SkillName).cdTimer / skillCD;
                }
            }
            else
            {
                if (this._maskImg.fillAmount != 0f)
                {
                    this._maskImg.fillAmount = 0f;
                }
                this.SetButtonInteractable(this._avatarActor.CanUseSkill(this.SkillName), false);
            }
        }

        private void UpdateForKeyboradInput()
        {
            if (!string.IsNullOrEmpty(this.KeyButtonCode))
            {
                if (Input.GetButtonDown(this.KeyButtonCode))
                {
                    this.OnPointerDown(null);
                }
                else if (Input.GetButtonUp(this.KeyButtonCode))
                {
                    this.OnPointerUp(null);
                }
            }
        }

        private void UseSkill()
        {
            if (this._avatarActor.CanUseSkill(this.SkillName))
            {
                this._controller.TryUseSkill(this.SkillName);
            }
        }

        public enum PointerState
        {
            PointerUp,
            PointerDown
        }
    }
}

