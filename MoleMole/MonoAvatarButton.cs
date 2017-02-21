namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoAvatarButton : MonoBehaviour
    {
        private BaseMonoAvatar _avatar;
        private AvatarActor _avatarActor;
        private Button _button;
        private MonoEffect _buttonQTEEffect;
        private MonoEffect _buttonSwitchInEffect;
        private Image _CDMaskImg;
        private Animation _frameLightAnimation;
        private Image _frameLightImg;
        private Image _hpBarImg;
        private Image _iconImage;
        private Image _spBarImg;
        public uint avatarRuntimeID;
        public bool canChange = true;
        private const float HP_START_RATIO = 0.56f;
        public int index;
        private bool onlyForShow;
        private const float SP_START_RATIO = 0.314f;

        private void ActButtonBlingEffect()
        {
            if (base.gameObject.activeInHierarchy && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning))
            {
                Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_Button_Bling", base.transform.TransformPoint((Vector3) ((RectTransform) base.transform).rect.center), base.transform.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
            }
        }

        private bool CheckCanShowButtonAllowSwitchEffect()
        {
            bool flag = false;
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
            if (((!Singleton<AvatarManager>.Instance.IsLocalAvatar(this._avatar.GetRuntimeID()) && (actor != null)) && (actor.IsOnStage() && !Singleton<LevelManager>.Instance.IsPaused())) && actor.AllowOtherSwitchIn)
            {
                flag = true;
            }
            return flag;
        }

        private bool CheckCanShowButtonQTEEffect()
        {
            return (!Singleton<LevelManager>.Instance.IsPaused() && this._avatarActor.IsInQTE);
        }

        public void Init(uint runtimeID)
        {
            this._button = base.GetComponent<Button>();
            this._frameLightImg = base.transform.Find("FrameLightBG").GetComponent<Image>();
            this._frameLightAnimation = base.transform.Find("FrameLightBG").GetComponent<Animation>();
            this._hpBarImg = base.transform.Find("HPBar/Inner").GetComponent<Image>();
            this._spBarImg = base.transform.Find("SPBar/Inner").GetComponent<Image>();
            this._iconImage = base.transform.Find("Icon").GetComponent<Image>();
            this._CDMaskImg = base.transform.Find("CDMask").GetComponent<Image>();
            this._buttonQTEEffect = base.transform.Find("ButtonEffect/Button_QTE_Effect").GetComponent<MonoEffect>();
            this._buttonSwitchInEffect = base.transform.Find("ButtonEffect/Button_Switch_In_Effect").GetComponent<MonoEffect>();
            this.onlyForShow = false;
            this.SetupAvatar(runtimeID);
        }

        public void InitForReviveButton(BaseMonoAvatar avatar)
        {
            this._button = base.GetComponent<Button>();
            this._hpBarImg = base.transform.Find("HPBar/Inner").GetComponent<Image>();
            this._spBarImg = base.transform.Find("SPBar/Inner").GetComponent<Image>();
            this._iconImage = base.transform.Find("Icon").GetComponent<Image>();
            this._CDMaskImg = base.transform.Find("CDMask").GetComponent<Image>();
            this.avatarRuntimeID = avatar.GetRuntimeID();
            this._avatar = avatar;
            this._avatarActor = (AvatarActor) Singleton<EventManager>.Instance.GetActor(this.avatarRuntimeID);
            GameObject obj2 = Miscs.LoadResource<GameObject>(this._avatarActor.avatarIconPath, BundleType.RESOURCE_FILE);
            this._iconImage.sprite = obj2.GetComponent<SpriteRenderer>().sprite;
            Transform transform = base.transform.Find("Attr");
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.Find(this._avatarActor.avatarDataItem.Attribute.ToString()).gameObject.SetActive(true);
            this.onlyForShow = true;
            this._button.interactable = true;
            base.transform.Find("CDMask").gameObject.SetActive(true);
        }

        public void OnClick()
        {
            if (this.canChange)
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                if (this._avatar.IsAlive())
                {
                    if ((!this._avatar.IsControlMuted() && localAvatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AllowTriggerInput)) && (localAvatar.GetRuntimeID() != this.avatarRuntimeID))
                    {
                        Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(localAvatar.GetRuntimeID(), this.avatarRuntimeID, false);
                    }
                }
                else if (Singleton<LevelScoreManager>.Instance.LevelType != 4)
                {
                    Singleton<LevelManager>.Instance.SetPause(true);
                    if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
                    {
                        Singleton<MainUIManager>.Instance.ShowDialog(new InLevelReviveDialogContext(this.avatarRuntimeID, localAvatar.transform.position, false), UIType.Any);
                    }
                    else if ((Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi) || (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.MultiRemote))
                    {
                        Singleton<MainUIManager>.Instance.ShowDialog(new InLevelReviveDialogContext(this.avatarRuntimeID, this._avatar.transform.position, false), UIType.Any);
                    }
                }
            }
        }

        public void OnClickForRevive()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AvatarSelectForRevive, this._avatar.GetRuntimeID()));
        }

        private void OnDestroy()
        {
            if (this._buttonQTEEffect != null)
            {
                this._buttonQTEEffect.gameObject.SetActive(false);
            }
            if (this._buttonSwitchInEffect != null)
            {
                this._buttonSwitchInEffect.gameObject.SetActive(false);
            }
            if (this._hpBarImg != null)
            {
                this._hpBarImg.sprite = null;
                this._hpBarImg = null;
            }
            if (this._spBarImg != null)
            {
                this._spBarImg.sprite = null;
                this._spBarImg = null;
            }
            if (this._frameLightImg != null)
            {
                this._frameLightImg.sprite = null;
                this._frameLightImg = null;
            }
            if (this._button != null)
            {
                SpriteState state = new SpriteState();
                state.set_disabledSprite(null);
                state.set_highlightedSprite(null);
                state.set_pressedSprite(null);
                this._button.spriteState = state;
                this._button.transition = Selectable.Transition.None;
                this._button = null;
            }
            if (this._iconImage != null)
            {
                this._iconImage.sprite = null;
                this._iconImage = null;
            }
            if (this._CDMaskImg != null)
            {
                this._CDMaskImg.sprite = null;
                this._CDMaskImg = null;
            }
        }

        private void OnHPChanged(float from, float to, float delta)
        {
            float ratio = to / this._avatarActor.maxHP;
            this.SetHPBarByRatio(ratio);
        }

        private void OnMaxHPChanged(float from, float to)
        {
            float ratio = (float) (this._avatarActor.HP / this._avatarActor.maxHP);
            this.SetHPBarByRatio(ratio);
        }

        private void OnMaxSPChanged(float from, float to)
        {
            float a = (float) (this._avatarActor.SP / this._avatarActor.maxSP);
            if ((from != to) && Mathf.Approximately(a, 1f))
            {
                this.ActButtonBlingEffect();
            }
            this.SetSPBarByRatio(a);
        }

        public void OnSetActive(bool active)
        {
            if (active)
            {
                this._avatarActor.onHPChanged = (Action<float, float, float>) Delegate.Combine(this._avatarActor.onHPChanged, new Action<float, float, float>(this.OnHPChanged));
                this._avatarActor.onSPChanged = (Action<float, float, float>) Delegate.Combine(this._avatarActor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
                this._avatarActor.onMaxHPChanged = (Action<float, float>) Delegate.Combine(this._avatarActor.onMaxHPChanged, new Action<float, float>(this.OnMaxHPChanged));
                this._avatarActor.onMaxSPChanged = (Action<float, float>) Delegate.Combine(this._avatarActor.onMaxSPChanged, new Action<float, float>(this.OnMaxSPChanged));
                this.RefreshBarView();
            }
            else
            {
                this._avatarActor.onHPChanged = (Action<float, float, float>) Delegate.Remove(this._avatarActor.onHPChanged, new Action<float, float, float>(this.OnHPChanged));
                this._avatarActor.onSPChanged = (Action<float, float, float>) Delegate.Remove(this._avatarActor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
                this._avatarActor.onMaxHPChanged = (Action<float, float>) Delegate.Remove(this._avatarActor.onMaxHPChanged, new Action<float, float>(this.OnMaxHPChanged));
                this._avatarActor.onMaxSPChanged = (Action<float, float>) Delegate.Remove(this._avatarActor.onMaxSPChanged, new Action<float, float>(this.OnMaxSPChanged));
            }
        }

        private void OnSPChanged(float from, float to, float delta)
        {
            float a = to / this._avatarActor.maxSP;
            if ((from != to) && Mathf.Approximately(a, 1f))
            {
                this.ActButtonBlingEffect();
            }
            this.SetSPBarByRatio(a);
        }

        private void RefreshBarView()
        {
            float ratio = (float) (this._avatarActor.HP / this._avatarActor.maxHP);
            this.SetHPBarByRatio(ratio);
            float num2 = (float) (this._avatarActor.SP / this._avatarActor.maxSP);
            this.SetSPBarByRatio(num2);
        }

        private void SetHPBarByRatio(float ratio)
        {
            this._hpBarImg.fillAmount = (ratio * 0.44f) + 0.56f;
        }

        public void SetIndex(int index)
        {
            this.index = index;
            UnityEngine.Object.Destroy(base.GetComponent<MonoKeyButton>());
        }

        private void SetSPBarByRatio(float ratio)
        {
            this._spBarImg.fillAmount = (ratio * 0.186f) + 0.314f;
        }

        public void SetupAvatar(uint runtimeId)
        {
            this.avatarRuntimeID = runtimeId;
            this._avatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(this.avatarRuntimeID);
            this._avatarActor = (AvatarActor) Singleton<EventManager>.Instance.GetActor(this.avatarRuntimeID);
            this._avatarActor.onHPChanged = (Action<float, float, float>) Delegate.Combine(this._avatarActor.onHPChanged, new Action<float, float, float>(this.OnHPChanged));
            this._avatarActor.onSPChanged = (Action<float, float, float>) Delegate.Combine(this._avatarActor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
            this._avatarActor.onMaxHPChanged = (Action<float, float>) Delegate.Combine(this._avatarActor.onMaxHPChanged, new Action<float, float>(this.OnMaxHPChanged));
            this._avatarActor.onMaxSPChanged = (Action<float, float>) Delegate.Combine(this._avatarActor.onMaxSPChanged, new Action<float, float>(this.OnMaxSPChanged));
            this._iconImage.sprite = Miscs.GetSpriteByPrefab(this._avatarActor.avatarDataItem.IconPathInLevel);
            Transform transform = base.transform.Find("Attr");
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.Find(this._avatarActor.avatarDataItem.Attribute.ToString()).gameObject.SetActive(true);
            this.RefreshBarView();
        }

        private void Update()
        {
            if (!this.onlyForShow)
            {
                if (!this._avatar.IsAlive())
                {
                    if (!this._button.interactable)
                    {
                        this._button.interactable = true;
                    }
                    base.transform.Find("CDMask").gameObject.SetActive(true);
                    this._frameLightImg.gameObject.SetActive(false);
                    this._buttonQTEEffect.gameObject.SetActive(false);
                    this._buttonSwitchInEffect.gameObject.SetActive(false);
                    this._CDMaskImg.fillAmount = 1f;
                }
                else
                {
                    if (this._avatarActor.AllowOtherSwitchIn)
                    {
                    }
                    if (this._avatarActor.IsSwitchInCD())
                    {
                        base.transform.Find("CDMask").gameObject.SetActive(true);
                        if (this._button.interactable)
                        {
                            this._button.interactable = false;
                        }
                        float swtichCDRatio = this._avatarActor.GetSwtichCDRatio();
                        this._CDMaskImg.fillAmount = 1f - swtichCDRatio;
                    }
                    else
                    {
                        if (!this._button.interactable)
                        {
                            this._button.interactable = true;
                        }
                        base.transform.Find("CDMask").gameObject.SetActive(false);
                    }
                    if (this._avatarActor.IsSPEnough("SKL02"))
                    {
                        this._frameLightImg.gameObject.SetActive(true);
                        this._frameLightAnimation.Play("FrameLight", PlayMode.StopAll);
                    }
                    else
                    {
                        this._frameLightImg.gameObject.SetActive(false);
                        this._frameLightAnimation.Stop("FrameLight");
                    }
                    this._buttonQTEEffect.gameObject.SetActive(this.CheckCanShowButtonQTEEffect());
                    this._buttonSwitchInEffect.gameObject.SetActive(this.CheckCanShowButtonAllowSwitchEffect());
                }
            }
        }
    }
}

