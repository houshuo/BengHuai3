namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class InLevelMainPageContext : BasePageContext
    {
        private Text _addTimeText;
        private Transform _buttonOverHeatPlugin;
        private MonoComboText _comboText;
        private bool _hasShowTeamBuff;
        private LocalAvatarHealthMode _healthMode;
        private MonoHPDisplayText _hpDisplayText;
        private float _hurtRatio;
        private bool _isOverheat;
        private const float _localAvatarHPWarningRatio = 0.3f;
        private Animation _mainPageFadeAnim;
        private const float _maxHurtWarningRatio = 2f;
        private const float _minHurtWarningRatio = 0.5f;
        private float _overheatRatio;
        private bool _pauseBtnEnable = true;
        private bool _pauseDialogShown;
        private InLevelMainPageShowState _showState;
        private MonoSPDisplayText _spDisplayText;
        private Text _timeCountDownText;
        private Text _timerText;
        private float _tutorialDelayInputTime = 0.5f;
        public MonoAvatarButtonContainer avatarButtonContainer;
        private const int COUNT_DOWN_TEXT_WARNING_SECOND = 10;
        private const int MIN_SP_DISPLAY_TEXT_INT_VALUE = 2;
        public Dictionary<string, MonoSkillButton> skillButtonDict;

        public InLevelMainPageContext(GameObject view = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "InLevelMainPageContext",
                viewPrefabPath = "UI/Menus/Page/InLevel/InLevelMainPage"
            };
            base.config = pattern;
            base.view = view;
            this.skillButtonDict = new Dictionary<string, MonoSkillButton>();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("FuncBtns/PauseBtn").GetComponent<Button>(), new UnityAction(this.OnPauseBtnClick));
        }

        public override void Destroy()
        {
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Remove(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.UpdateComboText));
            base.Destroy();
        }

        public MonoSkillButton GetSkillButtonBySkillID(string skillID)
        {
            return this.skillButtonDict[skillID];
        }

        public Transform GetSPBar()
        {
            return base.view.transform.Find("LocalAvatarStatus/SP");
        }

        public void HideMonsterStatus()
        {
            base.view.transform.Find("MonsterStatus").gameObject.SetActive(false);
        }

        private bool IsEcoMode()
        {
            if (Singleton<PlayerModule>.Instance.playerData.userId == 0)
            {
                return false;
            }
            return Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode;
        }

        private LocalAvatarHealthMode IsLocalAvatarInLowHP()
        {
            uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
            if ((actor != null) && (actor.HP < (actor.maxHP * 0.3f)))
            {
                this._hurtRatio = 1f - (actor.HP / (actor.maxHP * 0.3f));
                return LocalAvatarHealthMode.Unhealthy;
            }
            return LocalAvatarHealthMode.Healthy;
        }

        private bool OnAttackBtnVisibleControl(bool visible)
        {
            Transform transform = base.view.transform.Find("InputController/SkillButton_1");
            if (transform != null)
            {
                transform.gameObject.SetActive(visible);
            }
            return false;
        }

        private bool OnAttackLandedNotify(EvtAttackLanded evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.attackeeID) == 4)
            {
                MonsterActor actor = Singleton<EventManager>.Instance.GetActor(evt.attackeeID) as MonsterActor;
                if ((actor != null) && (actor.showSubHpBarWhenAttackLanded || (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi)))
                {
                    base.view.transform.Find("SubMonsterHPBarContainer").GetComponent<MonoSubMonsterHPBarContainer>().OnAttackLandedEvt(evt);
                }
            }
            return false;
        }

        public void OnAutoToggleValueChange(bool value)
        {
            Singleton<AvatarManager>.Instance.SetAutoBattle(value);
        }

        private bool OnAvatarCreate(uint runtimeId)
        {
            this.avatarButtonContainer.AddAvatarButton(runtimeId);
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(runtimeId))
            {
                Singleton<MainUIManager>.Instance.GetInLevelUICanvas().OnUpdateLocalAvatar(runtimeId, 0);
            }
            return false;
        }

        private bool OnBattleBegin()
        {
            if (!this._hasShowTeamBuff)
            {
                this.SetupTeamBuff();
                base.view.transform.Find("TeamBuff").gameObject.SetActive(true);
                base.view.transform.Find("TeamBuff").GetComponent<Animation>().Play();
                this._hasShowTeamBuff = true;
            }
            return false;
        }

        private bool OnEcoModeVisible(bool visible)
        {
            Transform transform = base.view.transform.Find("Ecomode");
            if (transform != null)
            {
                transform.gameObject.SetActive(visible);
            }
            return false;
        }

        private bool OnEvadeBtnVisibleControl(bool visible)
        {
            Transform transform = base.view.transform.Find("InputController/SkillButton_2");
            if (transform != null)
            {
                transform.gameObject.SetActive(visible);
            }
            return false;
        }

        public void OnIsOverheatChanged(float wasOverheat, float isOverheat)
        {
            this._isOverheat = isOverheat > 0f;
            this.UpdateOverHeatView(this._isOverheat, this._overheatRatio);
        }

        private bool OnJoystickVisibleControl(bool visible)
        {
            Transform transform = base.view.transform.Find("InputController/MoveJoystick");
            if (transform != null)
            {
                transform.gameObject.SetActive(visible);
            }
            return false;
        }

        private bool OnLoadingSceneDestroyed()
        {
            Singleton<EventManager>.Instance.FireEvent(new EvtLoadingState(EvtLoadingState.State.Destroy), MPEventDispatchMode.Normal);
            return false;
        }

        public void OnLocalAvatarChanged(AvatarActor avatarBefore, AvatarActor avatarAfter)
        {
            this.SetupLocalAvatarStatus(avatarAfter);
            avatarAfter.onHPChanged = (Action<float, float, float>) Delegate.Combine(avatarAfter.onHPChanged, new Action<float, float, float>(this.UpdateHPView));
            avatarAfter.onMaxHPChanged = (Action<float, float>) Delegate.Combine(avatarAfter.onMaxHPChanged, new Action<float, float>(this.UpdateMaxHPView));
            avatarAfter.onSPChanged = (Action<float, float, float>) Delegate.Combine(avatarAfter.onSPChanged, new Action<float, float, float>(this.UpdateSPView));
            avatarAfter.onMaxSPChanged = (Action<float, float>) Delegate.Combine(avatarAfter.onMaxSPChanged, new Action<float, float>(this.UpdateMaxSPView));
            if (avatarBefore != null)
            {
                avatarBefore.onHPChanged = (Action<float, float, float>) Delegate.Remove(avatarBefore.onHPChanged, new Action<float, float, float>(this.UpdateHPView));
                avatarBefore.onMaxHPChanged = (Action<float, float>) Delegate.Remove(avatarBefore.onMaxHPChanged, new Action<float, float>(this.UpdateMaxHPView));
                avatarBefore.onSPChanged = (Action<float, float, float>) Delegate.Remove(avatarBefore.onSPChanged, new Action<float, float, float>(this.UpdateSPView));
                avatarBefore.onMaxSPChanged = (Action<float, float>) Delegate.Remove(avatarBefore.onMaxSPChanged, new Action<float, float>(this.UpdateMaxSPView));
            }
            if ((avatarBefore != null) && (avatarAfter != null))
            {
                this.avatarButtonContainer.PlaySwapAvatarAnim(avatarBefore.runtimeID, avatarAfter.runtimeID);
            }
            if (avatarAfter != null)
            {
                bool flag = !avatarAfter.IsSkillLocked("SKL02");
                base.view.transform.Find("LocalAvatarStatus/SP").gameObject.SetActive(flag);
            }
            MonoMonsterStatus component = base.view.transform.Find("MonsterStatus").GetComponent<MonoMonsterStatus>();
            if (component.gameObject.activeSelf)
            {
                component.SetupNatureBonus();
                component.SetupMonsterNameByLevelPunish();
            }
            this._healthMode = this.IsLocalAvatarInLowHP();
            this.SetHPWarningByHealthMode(this._healthMode);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.OnAvatarCreate)
            {
                return this.OnAvatarCreate((uint) ntf.body);
            }
            if (ntf.type == NotifyTypes.PostStageReady)
            {
                return this.PostStageReady();
            }
            if (ntf.type == NotifyTypes.ShowDamegeText)
            {
                return this.ShowDamegeText((EvtBeingHit) ntf.body);
            }
            if (ntf.type == NotifyTypes.AttackLanded)
            {
                return this.OnAttackLandedNotify((EvtAttackLanded) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetTimeCountDownText)
            {
                return this.SetTimeCountDownText((float) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetTimesUpText)
            {
                return this.SetTimesUpText((string) ntf.body);
            }
            if (ntf.type == NotifyTypes.ShowAddTimeText)
            {
                return this.ShowAddTimeText((float) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetDefendModeText)
            {
                return this.SetDefendModeText((string) ntf.body);
            }
            if (ntf.type == NotifyTypes.ShowDefendModeText)
            {
                return this.SetDefendModeTextEnable((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetTimeCountDownTextActive)
            {
                return this.SetTimeCountDownTextActive((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.ShowHelperCutIn)
            {
                return this.ShowHelperCutIn();
            }
            if (ntf.type == NotifyTypes.DropItemConutChanged)
            {
                return this.SetDropItemCount((int) ntf.body);
            }
            if (ntf.type == NotifyTypes.ShowLevelDisplayText)
            {
                return this.ShowLevelDisplayText((string) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetTimerText)
            {
                return this.SetTimerText((float) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetupLocalAvatarStatus)
            {
                return this.SetupLocalAvatarStatus(ntf.body as AvatarActor);
            }
            if (ntf.type == NotifyTypes.TutorialPlayerTeaching)
            {
                return this.OnTutorialPlayerTeaching((LevelTutorialPlayerTeaching) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialUltraAttack)
            {
                return this.OnTutorialUltraAttackNotify((LevelTutorialUltraAttack) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialBranchAttack)
            {
                return this.OnTutorialBranchAttackNotify((LevelTutorialBranchAttack) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialEliteAttack)
            {
                return this.OnTutorialEliteAttackNotify((LevelTutorialEliteAttack) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialSwapAttack)
            {
                return this.OnTutorialSwapAttackNotify((LevelTutorialSwapAttack) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialSwapAndRestrain)
            {
                return this.OnTutorialSwapAndRestrain((LevelTutorialSwapAndRestrain) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialMonsterBlock)
            {
                return this.OnTutorialMonsterBlock((LevelTutorialMonsterBlock) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialMonsterTeleport)
            {
                return this.OnTutorialMonsterTeleport((LevelTutorialMonsterTeleport) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialNatureRestrain)
            {
                return this.OnTutorialNatureRestrain((LevelTutorialNatureRestrain) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialMonsterShield)
            {
                return this.OnTutorialMonsterShield((LevelTutorialMonsterShield) ntf.body);
            }
            if (ntf.type == NotifyTypes.TutorialMonsterRobotDodge)
            {
                return this.OnTutorialMonsterRobotDodge((LevelTutorialMonsterRobotDodge) ntf.body);
            }
            if (ntf.type == NotifyTypes.EvadeBtnVisible)
            {
                return this.OnEvadeBtnVisibleControl((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.AttackBtnVisible)
            {
                return this.OnAttackBtnVisibleControl((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.JoystickVisible)
            {
                return this.OnJoystickVisibleControl((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.PauseBtnEnable)
            {
                return this.OnPauseBtnEnable((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.PauseBtnVisible)
            {
                return this.OnPauseBtnVisible((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.SwapBtnVisible)
            {
                return this.OnSwapBtnVisible((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.EcoModeVisible)
            {
                return this.OnEcoModeVisible((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.OnSocketConnect)
            {
                return this.OnSocketConnect();
            }
            if (ntf.type == NotifyTypes.OnSocketDisconnect)
            {
                return this.OnSocketDisconnect();
            }
            if (ntf.type == NotifyTypes.ResitComboClear)
            {
                return this.OnResistComboClear();
            }
            if (ntf.type == NotifyTypes.OnQuitGameDialogShow)
            {
                return this.OnQuitGameDialogShow();
            }
            if (ntf.type == NotifyTypes.OnQuitGameDialogDestroy)
            {
                return this.OnQuitGameDialogDestroy();
            }
            if (ntf.type == NotifyTypes.BattleBegin)
            {
                return this.OnBattleBegin();
            }
            return ((ntf.type == NotifyTypes.LoadingSceneDestroyed) && this.OnLoadingSceneDestroyed());
        }

        public void OnOverheatRatioChanged(float oldRatio, float newRatio)
        {
            this._overheatRatio = newRatio;
            this.UpdateOverHeatView(this._isOverheat, this._overheatRatio);
        }

        public void OnPauseBtnClick()
        {
            if (this._pauseBtnEnable && !Singleton<LevelManager>.Instance.IsPaused())
            {
                Singleton<LevelManager>.Instance.SetPause(true);
                InLevelPauseDialogContext dialogContext = new InLevelPauseDialogContext("StatusTab");
                dialogContext.OnClosed += () => (this._pauseDialogShown = false);
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                this._pauseDialogShown = true;
            }
        }

        private bool OnPauseBtnEnable(bool enable)
        {
            this.PauseBtnEnabled = enable;
            return false;
        }

        private bool OnPauseBtnVisible(bool visible)
        {
            Transform transform = base.view.transform.Find("FuncBtns/PauseBtn");
            if (transform != null)
            {
                transform.gameObject.SetActive(visible);
            }
            return false;
        }

        private bool OnQuitGameDialogDestroy()
        {
            this.TryPauseGameByOthers(false);
            return false;
        }

        private bool OnQuitGameDialogShow()
        {
            this.TryPauseGameByOthers(true);
            return false;
        }

        private bool OnResistComboClear()
        {
            this._comboText.ActBlingEffect();
            return false;
        }

        private bool OnSocketConnect()
        {
            this.TryPauseGameByOthers(false);
            return false;
        }

        private bool OnSocketDisconnect()
        {
            this.TryPauseGameByOthers(true);
            return false;
        }

        private bool OnSwapBtnVisible(bool visible)
        {
            Transform transform = base.view.transform.Find("AvatarBtns");
            if (transform != null)
            {
                transform.gameObject.SetActive(visible);
            }
            return false;
        }

        public void OnTargetMonsterChange(MonsterActor targetBefore, MonsterActor targetAfter)
        {
            MonoMonsterStatus component = base.view.transform.Find("MonsterStatus").GetComponent<MonoMonsterStatus>();
            component.SetupView(targetBefore, targetAfter);
            component.SetupNatureBonus();
            component.SetupMonsterNameByLevelPunish();
            base.view.transform.Find("SubMonsterHPBarContainer").GetComponent<MonoSubMonsterHPBarContainer>().OnTargetMonsterChange(targetBefore, targetAfter);
        }

        private bool OnTutorialBranchAttackNotify(LevelTutorialBranchAttack tutorial)
        {
            NewbieDialogContext context;
            <OnTutorialBranchAttackNotify>c__AnonStoreyEE yee = new <OnTutorialBranchAttackNotify>c__AnonStoreyEE {
                tutorial = tutorial
            };
            if (yee.tutorial.step == 0)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yee.tutorial.GetDisplayTarget(yee.tutorial.step),
                    pointerUpCallback = new Func<bool>(yee.<>m__14C)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else
            {
                string str;
                if (yee.tutorial.step == 1)
                {
                    str = "SpriteOutput/GuideImgs/PicSkillGuide 1";
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        guideSprite = Miscs.GetSpriteByPrefab(str),
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yee.tutorial.GetDisplayTarget(yee.tutorial.step),
                        pointerUpCallback = new Func<bool>(yee.<>m__14D)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yee.tutorial.step == 2)
                {
                    str = "SpriteOutput/GuideImgs/PicSkillGuide 1";
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        guideSprite = Miscs.GetSpriteByPrefab(str),
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yee.tutorial.GetDisplayTarget(yee.tutorial.step),
                        pointerUpCallback = new Func<bool>(yee.<>m__14E)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yee.tutorial.step == 3)
                {
                    str = "SpriteOutput/GuideImgs/PicSkillGuide 1";
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        guideSprite = Miscs.GetSpriteByPrefab(str),
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yee.tutorial.GetDisplayTarget(yee.tutorial.step),
                        pointerUpCallback = new Func<bool>(yee.<>m__14F)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yee.tutorial.step == 4)
                {
                    Transform transform = base.view.transform.Find("InputController/SkillButton_1");
                    context = new NewbieDialogContext {
                        highlightTrans = transform,
                        delayInputTime = this._tutorialDelayInputTime,
                        destroyByOthers = true,
                        disableMask = true,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yee.tutorial.GetDisplayTarget(yee.tutorial.step),
                        pointerDownCallback = new Action(yee.tutorial.OnStep5PointerDown),
                        pointerUpCallback = new Func<bool>(yee.tutorial.OnStep5PoointerUp)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yee.tutorial.step == 5)
                {
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yee.tutorial.GetDisplayTarget(yee.tutorial.step),
                        pointerUpCallback = new Func<bool>(yee.<>m__150)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
            }
            return false;
        }

        private bool OnTutorialEliteAttackNotify(LevelTutorialEliteAttack tutorial)
        {
            NewbieDialogContext context;
            <OnTutorialEliteAttackNotify>c__AnonStoreyEF yef = new <OnTutorialEliteAttackNotify>c__AnonStoreyEF {
                tutorial = tutorial
            };
            if (yef.tutorial.step == 0)
            {
                Transform transform = base.view.transform.Find("MonsterStatus/ShieldBar");
                context = new NewbieDialogContext {
                    disableHighlightEffect = true,
                    highlightTrans = transform,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.RightUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yef.tutorial.GetDisplayTarget(yef.tutorial.step),
                    pointerUpCallback = new Func<bool>(yef.<>m__151)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yef.tutorial.step == 1)
            {
                Transform transform2 = base.view.transform.Find("MonsterStatus/ShieldBar");
                context = new NewbieDialogContext {
                    disableHighlightEffect = true,
                    highlightTrans = transform2,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.RightUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yef.tutorial.GetDisplayTarget(yef.tutorial.step),
                    pointerUpCallback = new Func<bool>(yef.<>m__152)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        private bool OnTutorialMonsterBlock(LevelTutorialMonsterBlock tutorial)
        {
            NewbieDialogContext context;
            <OnTutorialMonsterBlock>c__AnonStoreyF2 yf = new <OnTutorialMonsterBlock>c__AnonStoreyF2 {
                tutorial = tutorial
            };
            if (yf.tutorial.step == 0)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__166)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 1)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    isMaskClickable = true,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__167)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 2)
            {
                Transform transform = base.view.transform.Find("InputController/SkillButton_2");
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    highlightTrans = transform,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__168)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        private bool OnTutorialMonsterRobotDodge(LevelTutorialMonsterRobotDodge tutorial)
        {
            <OnTutorialMonsterRobotDodge>c__AnonStoreyF6 yf = new <OnTutorialMonsterRobotDodge>c__AnonStoreyF6 {
                tutorial = tutorial
            };
            if (yf.tutorial.step == 0)
            {
                NewbieDialogContext dialogContext = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    isMaskClickable = true,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__173)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private bool OnTutorialMonsterShield(LevelTutorialMonsterShield tutorial)
        {
            NewbieDialogContext context;
            <OnTutorialMonsterShield>c__AnonStoreyF5 yf = new <OnTutorialMonsterShield>c__AnonStoreyF5 {
                tutorial = tutorial
            };
            if (yf.tutorial.step == 0)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__170)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 1)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    isMaskClickable = true,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__171)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 2)
            {
                Transform transform = base.view.transform.Find("InputController/SkillButton_2");
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    highlightTrans = transform,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__172)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        private bool OnTutorialMonsterTeleport(LevelTutorialMonsterTeleport tutorial)
        {
            NewbieDialogContext context;
            <OnTutorialMonsterTeleport>c__AnonStoreyF4 yf = new <OnTutorialMonsterTeleport>c__AnonStoreyF4 {
                tutorial = tutorial
            };
            if (yf.tutorial.step == 0)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__16E)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 1)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__16F)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        private bool OnTutorialNatureRestrain(LevelTutorialNatureRestrain tutorial)
        {
            NewbieDialogContext context;
            <OnTutorialNatureRestrain>c__AnonStoreyF3 yf = new <OnTutorialNatureRestrain>c__AnonStoreyF3 {
                tutorial = tutorial
            };
            if (yf.tutorial.step == 0)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__169)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 1)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__16A)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 2)
            {
                Transform transform = base.view.transform.Find("MonsterStatus/DamageMark");
                context = new NewbieDialogContext {
                    disableHighlightEffect = true,
                    delayInputTime = this._tutorialDelayInputTime,
                    highlightTrans = transform,
                    isMaskClickable = true,
                    disableHighlightInvoke = true,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__16B)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 3)
            {
                string prefabPath = "SpriteOutput/GuideImgs/PicNatureGuide1";
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__16C)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yf.tutorial.step == 4)
            {
                string str2 = "SpriteOutput/GuideImgs/PicNatureGuide1";
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    guideSprite = Miscs.GetSpriteByPrefab(str2),
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                    pointerUpCallback = new Func<bool>(yf.<>m__16D)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        private bool OnTutorialPlayerTeaching(LevelTutorialPlayerTeaching tutorial)
        {
            NewbieDialogContext context;
            <OnTutorialPlayerTeaching>c__AnonStoreyEC yec = new <OnTutorialPlayerTeaching>c__AnonStoreyEC {
                tutorial = tutorial
            };
            if (yec.tutorial.step == 0)
            {
                context = new NewbieDialogContext {
                    isMaskClickable = true,
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__138)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 1)
            {
                context = new NewbieDialogContext {
                    isMaskClickable = true,
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__139)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 2)
            {
                Transform transform = base.view.transform.Find("InputController/MoveJoystick");
                context = new NewbieDialogContext {
                    highlightTrans = transform,
                    delayInputTime = this._tutorialDelayInputTime,
                    disableHighlightInvoke = true,
                    isMaskClickable = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Up,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__13A)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 3)
            {
                Transform transform2 = base.view.transform.Find("InputController/MoveJoystick");
                context = new NewbieDialogContext {
                    highlightTrans = transform2,
                    delayInputTime = this._tutorialDelayInputTime,
                    disableHighlightInvoke = true,
                    isMaskClickable = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Up,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__13B)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 4)
            {
                context = new NewbieDialogContext {
                    isMaskClickable = true,
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__13C)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 5)
            {
                Transform transform3 = base.view.transform.Find("LocalAvatarStatus/HP/Bar");
                context = new NewbieDialogContext {
                    disableHighlightEffect = true,
                    highlightTrans = transform3,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__13D)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 6)
            {
                Transform transform4 = base.view.transform.Find("InputController/SkillButton_1");
                context = new NewbieDialogContext {
                    highlightTrans = transform4,
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__13E)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 7)
            {
                context = new NewbieDialogContext {
                    isMaskClickable = true,
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__13F)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 8)
            {
                Transform transform5 = base.view.transform.Find("MonsterStatus/HPBar");
                context = new NewbieDialogContext {
                    disableHighlightEffect = true,
                    highlightTrans = transform5,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = true,
                    disableHighlightInvoke = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__140)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 9)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__141)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 10)
            {
                context = new NewbieDialogContext {
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__142)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 11)
            {
                Transform transform6 = base.view.transform.Find("InputController/SkillButton_2");
                context = new NewbieDialogContext {
                    highlightTrans = transform6,
                    delayInputTime = this._tutorialDelayInputTime,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__143)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 12)
            {
                context = new NewbieDialogContext {
                    isMaskClickable = true,
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__144)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 13)
            {
                Transform transform7 = base.view.transform.Find("InputController/SkillButton_2");
                context = new NewbieDialogContext {
                    highlightTrans = transform7,
                    isMaskClickable = true,
                    disableHighlightEffect = true,
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__145)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 14)
            {
                Transform transform8 = base.view.transform.Find("InputController/SkillButton_2");
                context = new NewbieDialogContext {
                    highlightTrans = transform8,
                    isMaskClickable = true,
                    disableHighlightEffect = true,
                    delayInputTime = this._tutorialDelayInputTime,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__146)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yec.tutorial.step == 15)
            {
                Transform transform9 = base.view.transform.Find("InputController/SkillButton_2");
                context = new NewbieDialogContext {
                    highlightTrans = transform9,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = false,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yec.tutorial.GetDisplayTarget(yec.tutorial.step),
                    pointerUpCallback = new Func<bool>(yec.<>m__147)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        private bool OnTutorialSwapAndRestrain(LevelTutorialSwapAndRestrain tutorial)
        {
            <OnTutorialSwapAndRestrain>c__AnonStoreyF1 yf = new <OnTutorialSwapAndRestrain>c__AnonStoreyF1 {
                tutorial = tutorial
            };
            Transform transform = null;
            transform = base.view.transform.Find("AvatarBtns").GetComponent<MonoAvatarButtonContainer>().GetAvatarButtonByRuntimeID(yf.tutorial.targetSwapAvatarId).gameObject.transform;
            string prefabPath = "SpriteOutput/GuideImgs/PicNatureGuide1";
            if (transform != null)
            {
                NewbieDialogContext context;
                if (yf.tutorial.step == 0)
                {
                    Transform transform3 = base.view.transform.Find("MonsterStatus/DamageMark");
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        delayInputTime = this._tutorialDelayInputTime,
                        highlightTrans = transform3,
                        isMaskClickable = true,
                        disableHighlightInvoke = true,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__158)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 1)
                {
                    Transform transform4 = base.view.transform.Find("MonsterStatus/DamageMark");
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        delayInputTime = this._tutorialDelayInputTime,
                        highlightTrans = transform4,
                        isMaskClickable = true,
                        disableHighlightInvoke = true,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__159)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 2)
                {
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__15A)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 3)
                {
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__15B)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 4)
                {
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__15C)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 5)
                {
                    Transform transform5 = base.view.transform.Find("MonsterStatus/DamageMark");
                    bool flag = !yf.tutorial.isFirstDead;
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        highlightTrans = transform5,
                        delayInputTime = this._tutorialDelayInputTime,
                        disableHighlightInvoke = true,
                        isMaskClickable = true,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        handIconPosType = !flag ? NewbieDialogContext.HandIconPosType.None : NewbieDialogContext.HandIconPosType.Bottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__15D)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 6)
                {
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        delayShowTime = 0.01f,
                        highlightTrans = transform,
                        delayInputTime = this._tutorialDelayInputTime,
                        disableHighlightInvoke = true,
                        isMaskClickable = true,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__15E)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 7)
                {
                    Transform transform6 = transform.FindChild("HPBar");
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        highlightTrans = transform6,
                        delayInputTime = this._tutorialDelayInputTime,
                        disableHighlightInvoke = true,
                        isMaskClickable = true,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__15F)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 8)
                {
                    Transform transform7 = transform.FindChild("SPBar");
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        highlightTrans = transform7,
                        delayInputTime = this._tutorialDelayInputTime,
                        disableHighlightInvoke = true,
                        isMaskClickable = true,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__160)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 9)
                {
                    context = new NewbieDialogContext {
                        highlightTrans = transform,
                        delayInputTime = this._tutorialDelayInputTime,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__161)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 10)
                {
                    Transform transform8 = base.view.transform.Find("MonsterStatus/DamageMark");
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        delayInputTime = this._tutorialDelayInputTime,
                        highlightTrans = transform8,
                        isMaskClickable = true,
                        disableHighlightInvoke = true,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__162)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 11)
                {
                    string str2 = "SpriteOutput/GuideImgs/PicSkillGuide 2";
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        guideSprite = Miscs.GetSpriteByPrefab(str2),
                        isMaskClickable = true,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__163)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 12)
                {
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        isMaskClickable = true,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__164)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 13)
                {
                    context = new NewbieDialogContext {
                        delayInputTime = this._tutorialDelayInputTime,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__165)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
            }
            return false;
        }

        private bool OnTutorialSwapAttackNotify(LevelTutorialSwapAttack tutorial)
        {
            <OnTutorialSwapAttackNotify>c__AnonStoreyF0 yf = new <OnTutorialSwapAttackNotify>c__AnonStoreyF0 {
                tutorial = tutorial
            };
            Transform transform = null;
            transform = base.view.transform.Find("AvatarBtns").GetComponent<MonoAvatarButtonContainer>().GetAvatarButtonByRuntimeID(yf.tutorial.targetSwapAvatarId).gameObject.transform;
            if (transform != null)
            {
                NewbieDialogContext context;
                if (yf.tutorial.step == 0)
                {
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        highlightTrans = transform,
                        delayInputTime = this._tutorialDelayInputTime,
                        disableHighlightInvoke = true,
                        isMaskClickable = true,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__153)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 1)
                {
                    Transform transform3 = transform.FindChild("HPBar");
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        highlightTrans = transform3,
                        delayInputTime = this._tutorialDelayInputTime,
                        disableHighlightInvoke = true,
                        isMaskClickable = true,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__154)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 2)
                {
                    Transform transform4 = transform.FindChild("SPBar");
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        highlightTrans = transform4,
                        delayInputTime = this._tutorialDelayInputTime,
                        disableHighlightInvoke = true,
                        isMaskClickable = true,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__155)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 3)
                {
                    context = new NewbieDialogContext {
                        highlightTrans = transform,
                        delayInputTime = this._tutorialDelayInputTime,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__156)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (yf.tutorial.step == 4)
                {
                    context = new NewbieDialogContext {
                        disableHighlightEffect = true,
                        highlightTrans = transform,
                        delayInputTime = this._tutorialDelayInputTime,
                        isMaskClickable = true,
                        disableHighlightInvoke = true,
                        handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                        bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
                        guideDesc = yf.tutorial.GetDisplayTarget(yf.tutorial.step),
                        pointerUpCallback = new Func<bool>(yf.<>m__157)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
            }
            return false;
        }

        private bool OnTutorialUltraAttackNotify(LevelTutorialUltraAttack tutorial)
        {
            NewbieDialogContext context;
            <OnTutorialUltraAttackNotify>c__AnonStoreyED yed = new <OnTutorialUltraAttackNotify>c__AnonStoreyED {
                tutorial = tutorial
            };
            if (yed.tutorial.step == 0)
            {
                Transform transform = base.view.transform.Find("InputController/SkillButton_3");
                context = new NewbieDialogContext {
                    highlightTrans = transform,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yed.tutorial.GetDisplayTarget(yed.tutorial.step),
                    pointerUpCallback = new Func<bool>(yed.<>m__148)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yed.tutorial.step == 1)
            {
                Transform transform2 = base.view.transform.Find("LocalAvatarStatus/SP");
                context = new NewbieDialogContext {
                    disableHighlightEffect = true,
                    highlightTrans = transform2,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = true,
                    disableHighlightInvoke = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yed.tutorial.GetDisplayTarget(yed.tutorial.step),
                    pointerUpCallback = new Func<bool>(yed.<>m__149)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yed.tutorial.step == 2)
            {
                Transform transform3 = base.view.transform.Find("LocalAvatarStatus/SP");
                context = new NewbieDialogContext {
                    disableHighlightEffect = true,
                    highlightTrans = transform3,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = true,
                    disableHighlightInvoke = true,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yed.tutorial.GetDisplayTarget(yed.tutorial.step),
                    pointerUpCallback = new Func<bool>(yed.<>m__14A)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yed.tutorial.step == 3)
            {
                Transform transform4 = base.view.transform.Find("InputController/SkillButton_3");
                transform4.GetComponent<Button>().interactable = true;
                context = new NewbieDialogContext {
                    highlightTrans = transform4,
                    delayInputTime = this._tutorialDelayInputTime,
                    isMaskClickable = false,
                    bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
                    handIconPosType = NewbieDialogContext.HandIconPosType.Left,
                    guideDesc = yed.tutorial.GetDisplayTarget(yed.tutorial.step),
                    pointerUpCallback = new Func<bool>(yed.<>m__14B)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (yed.tutorial.step == 4)
            {
            }
            return false;
        }

        private bool PostStageReady()
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().OnUpdateLocalAvatarAbilityDisplay(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), 0);
            return false;
        }

        public void RefreshSPView(float from, float to, float delta)
        {
            this.UpdateSPView(from, to, delta);
        }

        private void SetContoller(AvatarActor avatarActor)
        {
            BaseAvatarInputController inputController = avatarActor.avatar.GetInputController();
            this.skillButtonDict.Clear();
            foreach (MonoSkillButton button in base.view.transform.Find("InputController").GetComponentsInChildren<MonoSkillButton>(true))
            {
                if (avatarActor.GetSkillInfo(button.SkillName) != null)
                {
                    this.skillButtonDict.Add(button.SkillName, button);
                    button.InitSkillButton(inputController);
                    button.gameObject.SetActive(!avatarActor.IsSkillLocked(button.SkillName));
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
            base.view.transform.Find("InputController/MoveJoystick").GetComponent<MonoJoystick>().InitJoystick(inputController);
            base.view.transform.Find("InputController").gameObject.SetActive(true);
        }

        private bool SetDefendModeText(string msg)
        {
            Text component = base.view.transform.Find("DefenseMode/Timing/Text").GetComponent<Text>();
            if (component != null)
            {
                component.text = msg;
                return true;
            }
            return false;
        }

        private bool SetDefendModeTextEnable(bool enable)
        {
            base.view.transform.Find("DefenseMode").gameObject.SetActive(enable);
            return false;
        }

        private bool SetDropItemCount(int count)
        {
            base.view.transform.Find("LevelInfoPanel/DropItem/NumText").GetComponent<Text>().text = "x" + count;
            return false;
        }

        private void SetHPWarningByHealthMode(LocalAvatarHealthMode mode)
        {
            Animation component = base.view.transform.Find("RedFrame").GetComponent<Animation>();
            CanvasGroup group = base.view.transform.Find("RedFrame").GetComponent<CanvasGroup>();
            MonoSliderGroup group2 = base.view.transform.Find("LocalAvatarStatus/HP/Bar").GetComponent<MonoSliderGroup>();
            if (((component != null) && (group != null)) && (group2 != null))
            {
                if (mode == LocalAvatarHealthMode.Unhealthy)
                {
                    base.view.transform.Find("RedFrame").gameObject.SetActive(true);
                    IEnumerator enumerator = component.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            AnimationState current = (AnimationState) enumerator.Current;
                            current.speed = Mathf.Lerp(0.5f, 2f, this._hurtRatio);
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
                    component.Play();
                    group2.SetupInDanageView(mode);
                }
                else
                {
                    component.Stop();
                    group.alpha = 0f;
                    group2.SetupInDanageView(mode);
                    base.view.transform.Find("RedFrame").gameObject.SetActive(false);
                }
            }
        }

        public void SetInLevelMainPageActive(bool active, bool instant = false, bool force = false)
        {
            if ((this._mainPageFadeAnim != null) && ((this._showState != InLevelMainPageShowState.Changing) || force))
            {
                this.SetActive(true);
                InLevelMainPageShowState state = !active ? InLevelMainPageShowState.Hide : InLevelMainPageShowState.Show;
                if (instant)
                {
                    this.SetActive(active);
                    this._showState = !active ? InLevelMainPageShowState.Hide : InLevelMainPageShowState.Show;
                }
                else if ((this._showState != state) || force)
                {
                    string animation = !active ? "InlevelmainPageFadeout" : "InlevelmainPageFadein";
                    this._mainPageFadeAnim.Play(animation);
                    this._showState = InLevelMainPageShowState.Changing;
                    if (!base.IsActive)
                    {
                        this.SetActive(true);
                    }
                    Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.WaitFadeAnimation(this._mainPageFadeAnim, active, null));
                }
            }
        }

        public bool SetOverHeatViewActive(bool active)
        {
            this._buttonOverHeatPlugin.gameObject.SetActive(active);
            return false;
        }

        private bool SetTimeCountDownText(float remainTime)
        {
            int num = Mathf.CeilToInt(remainTime) / 60;
            int num2 = Mathf.CeilToInt(remainTime) - (60 * num);
            this._timeCountDownText.text = string.Format("{0:D2}:{1:D2}", num, num2);
            if (remainTime <= 10f)
            {
                base.view.transform.Find("TimeCountDown").GetComponent<Animation>().Play();
                this._timeCountDownText.color = MiscData.GetColor("WarningRed");
            }
            else
            {
                this._timeCountDownText.color = MiscData.GetColor("TotalWhite");
            }
            return false;
        }

        private bool SetTimeCountDownTextActive(bool active)
        {
            base.view.transform.Find("TimeCountDown").gameObject.SetActive(active);
            return false;
        }

        private bool SetTimerText(float time)
        {
            int num = Mathf.CeilToInt(time) / 60;
            int num2 = Mathf.CeilToInt(time) - (60 * num);
            this._timerText.text = string.Format("{0:D2}:{1:D2}", num, num2);
            return false;
        }

        private bool SetTimesUpText(string msg)
        {
            this.ShowLevelDisplayText(LocalizationGeneralLogic.GetText("LevelDisplay_TimeUp", new object[0]));
            return false;
        }

        private void SetupDebugMenu()
        {
            base.view.transform.Find("DebugWidget").gameObject.SetActive(GlobalVars.LEVEL_MODE_DEBUG);
        }

        private bool SetupLocalAvatarStatus(AvatarActor avatar)
        {
            this.UpdateHPView((float) avatar.HP, (float) avatar.HP, 0f);
            this.UpdateSPView((float) avatar.SP, (float) avatar.SP, 0f);
            this.SetContoller(avatar);
            return false;
        }

        private void SetupTeamBuff()
        {
            AvatarSkillDataItem leaderSkill = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetTeamLeader().GetRuntimeID()).avatarDataItem.GetLeaderSkill();
            base.view.transform.Find("TeamBuff/Self/SkillName").GetComponent<Text>().text = leaderSkill.SkillName;
            base.view.transform.Find("TeamBuff/Self/Desc").GetComponent<Text>().text = leaderSkill.SkillShortInfo;
            base.view.transform.Find("TeamBuff/Friend").gameObject.SetActive(false);
            BaseMonoAvatar helperAvatar = Singleton<AvatarManager>.Instance.GetHelperAvatar();
            if ((helperAvatar != null) && Singleton<FriendModule>.Instance.IsMyFriend(Singleton<LevelScoreManager>.Instance.friendDetailItem.uid))
            {
                AvatarSkillDataItem item2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(helperAvatar.GetRuntimeID()).avatarDataItem.GetLeaderSkill();
                base.view.transform.Find("TeamBuff/Friend").gameObject.SetActive(true);
                base.view.transform.Find("TeamBuff/Friend/SkillName").GetComponent<Text>().text = item2.SkillName;
                base.view.transform.Find("TeamBuff/Friend/Desc").GetComponent<Text>().text = item2.SkillShortInfo;
                base.view.transform.Find("TeamBuff/Friend/SkillName/Hint").gameObject.SetActive(false);
            }
        }

        protected override bool SetupView()
        {
            this._comboText = base.view.transform.Find("LocalAvatarStatus/ComboText").GetComponent<MonoComboText>();
            this._hpDisplayText = base.view.transform.Find("LocalAvatarStatus/HP").GetComponent<MonoHPDisplayText>();
            this._spDisplayText = base.view.transform.Find("LocalAvatarStatus/SP").GetComponent<MonoSPDisplayText>();
            this._mainPageFadeAnim = base.view.gameObject.GetComponent<Animation>();
            this._timeCountDownText = base.view.transform.Find("TimeCountDown/Timing/Text").GetComponent<Text>();
            this._addTimeText = base.view.transform.Find("TimeCountDown/AddTimeText").GetComponent<Text>();
            this._timerText = base.view.transform.Find("LevelInfoPanel/Timer/Text").GetComponent<Text>();
            this._buttonOverHeatPlugin = base.view.transform.Find("InputController/SkillButton_1/ButtonOverHeatPlugin");
            this.avatarButtonContainer = base.view.transform.Find("AvatarBtns").GetComponent<MonoAvatarButtonContainer>();
            base.view.transform.Find("MonsterStatus").gameObject.SetActive(false);
            base.view.transform.Find("TimeCountDown").gameObject.SetActive(false);
            this._addTimeText.gameObject.SetActive(false);
            this._timerText.text = "00:00";
            this.SetupDebugMenu();
            base.view.transform.Find("InputController").gameObject.SetActive(false);
            base.view.transform.Find("HelperCutIn").gameObject.SetActive(false);
            base.view.transform.Find("LevelDisplayText").gameObject.SetActive(false);
            base.view.transform.Find("LevelInfoPanel/DropItem/NumText").GetComponent<Text>().text = "x0";
            if (GlobalVars.DEBUG_FEATURE_ON)
            {
                base.view.transform.Find("DebugWidget").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
            }
            else
            {
                UnityEngine.Object.Destroy(base.view.transform.Find("DebugWidget").gameObject);
            }
            this._showState = InLevelMainPageShowState.Show;
            this.OnEcoModeVisible(this.IsEcoMode());
            return false;
        }

        private bool ShowAddTimeText(float addTime)
        {
            if (addTime > 0f)
            {
                this._addTimeText.text = "+" + addTime.ToString() + "s";
            }
            else
            {
                this._addTimeText.text = addTime.ToString() + "s";
            }
            this._addTimeText.gameObject.SetActive(true);
            base.view.transform.Find("TimeCountDown").GetComponent<Animation>().Play();
            return false;
        }

        private bool ShowDamegeText(EvtBeingHit evt)
        {
            bool flag = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 4;
            BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.targetID);
            if (entity != null)
            {
                if (!Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(entity))
                {
                    return false;
                }
                if (flag && !GlobalVars.LEVEL_MODE_DEBUG)
                {
                    return false;
                }
                if (evt.attackData.rejected)
                {
                    return false;
                }
                if (((evt.attackData == null) || (evt.attackData.hitCollision == null)) || (evt.attackData.resolveStep != AttackData.AttackDataStep.FinalResolved))
                {
                    return false;
                }
                base.view.transform.Find("DamageTextContainer").GetComponent<MonoDamageTextContainer>().ShowDamageText(evt.attackData, entity);
            }
            return false;
        }

        private bool ShowHelperCutIn()
        {
            base.view.transform.Find("HelperCutIn/Content/Name").GetComponent<Text>().text = Singleton<LevelScoreManager>.Instance.friendDetailItem.nickName;
            string iconPath = Singleton<LevelScoreManager>.Instance.friendDetailItem.leaderAvatar.IconPath;
            base.view.transform.Find("HelperCutIn/Content/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(iconPath);
            base.view.transform.Find("HelperCutIn").gameObject.SetActive(true);
            return false;
        }

        private bool ShowLevelDisplayText(string text)
        {
            base.view.transform.Find("LevelDisplayText/Text").GetComponent<Text>().text = text;
            base.view.transform.Find("LevelDisplayText").gameObject.SetActive(true);
            return false;
        }

        private bool ShowVerticalDrawing(string text)
        {
            return this.ShowLevelDisplayText(text);
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent = null)
        {
            base.StartUp(canvasTrans, viewParent);
            this._comboText.transform.localScale = Vector3.zero;
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Combine(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.UpdateComboText));
        }

        private void TryPauseGameByOthers(bool pause)
        {
            if ((((Singleton<LevelManager>.Instance != null) && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning)) && ((Singleton<LevelManager>.Instance.IsPaused() != pause) && !this._pauseDialogShown)) && (this._showState != InLevelMainPageShowState.Changing))
            {
                Singleton<LevelManager>.Instance.SetPause(pause);
            }
        }

        private void UpdateComboText(int from, int to)
        {
            this._comboText.SetupView(from, to);
        }

        private void UpdateHPView(float from, float to, float delta)
        {
            uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
            int hpAfter = UIUtil.FloorToIntCustom(to);
            int num3 = UIUtil.FloorToIntCustom((float) actor.maxHP);
            base.view.transform.Find("LocalAvatarStatus/HP/Bar").GetComponent<MonoSliderGroup>().UpdateValue((float) hpAfter, (float) num3, 0f);
            base.view.transform.Find("LocalAvatarStatus/HP/NumText/Num").GetComponent<Text>().text = hpAfter.ToString();
            base.view.transform.Find("LocalAvatarStatus/HP/NumText/MaxNum").GetComponent<Text>().text = num3.ToString();
            this._healthMode = this.IsLocalAvatarInLowHP();
            this.SetHPWarningByHealthMode(this._healthMode);
            if (this._hpDisplayText != null)
            {
                int num4 = UIUtil.FloorToIntCustom(delta);
                this._hpDisplayText.SetupView((int) from, hpAfter, num4);
            }
        }

        private void UpdateMaxHPView(float from, float to)
        {
            uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
            this.UpdateHPView((float) actor.HP, (float) actor.HP, 0f);
        }

        private void UpdateMaxSPView(float from, float to)
        {
            uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
            this.UpdateSPView((float) actor.SP, (float) actor.SP, 0f);
        }

        public void UpdateOverHeatView(bool isOverHeat, float ratio)
        {
            float num = 0.5f;
            CanvasGroup component = this._buttonOverHeatPlugin.GetComponent<CanvasGroup>();
            if (!isOverHeat && (ratio <= num))
            {
                component.alpha = ratio / num;
            }
            else
            {
                component.alpha = 1f;
            }
            this._buttonOverHeatPlugin.Find("FrameWhite").gameObject.SetActive(!isOverHeat);
            this._buttonOverHeatPlugin.Find("FrameRed").gameObject.SetActive(isOverHeat);
            Image image = this._buttonOverHeatPlugin.Find("SpColor").GetComponent<Image>();
            Image image2 = this._buttonOverHeatPlugin.Find("SpDisable").GetComponent<Image>();
            image.gameObject.SetActive(!isOverHeat);
            image2.gameObject.SetActive(isOverHeat);
            if (isOverHeat)
            {
                image2.fillAmount = ratio;
            }
            else
            {
                image.fillAmount = ratio;
            }
        }

        private void UpdateSPView(float from, float to, float delta)
        {
            uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
            if (actor.IsSkillLocked("SKL02"))
            {
                base.view.transform.Find("LocalAvatarStatus/SP").gameObject.SetActive(false);
            }
            else
            {
                base.view.transform.Find("LocalAvatarStatus/SP").gameObject.SetActive(true);
                int num2 = UIUtil.FloorToIntCustom(to);
                int num3 = UIUtil.FloorToIntCustom((float) actor.maxSP);
                base.view.transform.Find("LocalAvatarStatus/SP/Bar/MaskSlider").GetComponent<MonoMaskSlider>().UpdateValue((float) num2, (float) num3, 0f);
                base.view.transform.Find("LocalAvatarStatus/SP/NumText/MaxNum").GetComponent<Text>().text = num3.ToString();
                bool flag = num2 == num3;
                Text component = base.view.transform.Find("LocalAvatarStatus/SP/NumText/Num").GetComponent<Text>();
                component.text = !flag ? num2.ToString() : "MAX";
                component.color = !flag ? Color.white : MiscData.GetColor("TextOrange");
                if (this._spDisplayText != null)
                {
                    bool showText = false;
                    if ((delta > 0f) && (UIUtil.FloorToIntCustom(delta) >= 2))
                    {
                        showText = true;
                    }
                    this._spDisplayText.SetupView(from, to, delta, showText);
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator WaitFadeAnimation(Animation animation, bool active, Action callback)
        {
            return new <WaitFadeAnimation>c__Iterator64 { animation = animation, active = active, callback = callback, <$>animation = animation, <$>active = active, <$>callback = callback, <>f__this = this };
        }

        public bool PauseBtnEnabled
        {
            get
            {
                return this._pauseBtnEnable;
            }
            set
            {
                this._pauseBtnEnable = value;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialBranchAttackNotify>c__AnonStoreyEE
        {
            internal LevelTutorialBranchAttack tutorial;

            internal bool <>m__14C()
            {
                this.tutorial.OnStep1Done();
                return false;
            }

            internal bool <>m__14D()
            {
                this.tutorial.OnStep2Done();
                return false;
            }

            internal bool <>m__14E()
            {
                this.tutorial.OnStep3Done();
                return false;
            }

            internal bool <>m__14F()
            {
                this.tutorial.OnStep4Done();
                return false;
            }

            internal bool <>m__150()
            {
                this.tutorial.OnStep6Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialEliteAttackNotify>c__AnonStoreyEF
        {
            internal LevelTutorialEliteAttack tutorial;

            internal bool <>m__151()
            {
                this.tutorial.OnTutorialStep1Done();
                return false;
            }

            internal bool <>m__152()
            {
                this.tutorial.OnTutorialStep2Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialMonsterBlock>c__AnonStoreyF2
        {
            internal LevelTutorialMonsterBlock tutorial;

            internal bool <>m__166()
            {
                this.tutorial.OnTutoriaStep1Done();
                return false;
            }

            internal bool <>m__167()
            {
                this.tutorial.OnTutorialStep2Done();
                return false;
            }

            internal bool <>m__168()
            {
                this.tutorial.OnTutorialStep3Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialMonsterRobotDodge>c__AnonStoreyF6
        {
            internal LevelTutorialMonsterRobotDodge tutorial;

            internal bool <>m__173()
            {
                this.tutorial.OnTutoriaStep1Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialMonsterShield>c__AnonStoreyF5
        {
            internal LevelTutorialMonsterShield tutorial;

            internal bool <>m__170()
            {
                this.tutorial.OnTutoriaStep1Done();
                return false;
            }

            internal bool <>m__171()
            {
                this.tutorial.OnTutorialStep2Done();
                return false;
            }

            internal bool <>m__172()
            {
                this.tutorial.OnTutorialStep3Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialMonsterTeleport>c__AnonStoreyF4
        {
            internal LevelTutorialMonsterTeleport tutorial;

            internal bool <>m__16E()
            {
                this.tutorial.OnTutorialStep1Done();
                return false;
            }

            internal bool <>m__16F()
            {
                this.tutorial.OnTutorialStep2Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialNatureRestrain>c__AnonStoreyF3
        {
            internal LevelTutorialNatureRestrain tutorial;

            internal bool <>m__169()
            {
                this.tutorial.OnTutorialStep1Done();
                return false;
            }

            internal bool <>m__16A()
            {
                this.tutorial.OnTutorialStep2Done();
                return false;
            }

            internal bool <>m__16B()
            {
                this.tutorial.OnTutorialStep3Done();
                return false;
            }

            internal bool <>m__16C()
            {
                this.tutorial.OnTutorialStep4Done();
                return false;
            }

            internal bool <>m__16D()
            {
                this.tutorial.OnTutorialStep5Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialPlayerTeaching>c__AnonStoreyEC
        {
            internal LevelTutorialPlayerTeaching tutorial;

            internal bool <>m__138()
            {
                this.tutorial.OnTutorialStep1Done();
                return false;
            }

            internal bool <>m__139()
            {
                this.tutorial.OnTutorialStep2Done();
                return false;
            }

            internal bool <>m__13A()
            {
                this.tutorial.OnTutorialStep3Done();
                return false;
            }

            internal bool <>m__13B()
            {
                this.tutorial.OnTutorialStpe4Done();
                return false;
            }

            internal bool <>m__13C()
            {
                this.tutorial.OnTutorialStep5Done();
                return false;
            }

            internal bool <>m__13D()
            {
                this.tutorial.OnTutorialStep6Done();
                return false;
            }

            internal bool <>m__13E()
            {
                this.tutorial.OnTutorialStpe7Done();
                return false;
            }

            internal bool <>m__13F()
            {
                this.tutorial.OnTutoriaStep8Done();
                return false;
            }

            internal bool <>m__140()
            {
                this.tutorial.OnTutorialStep8_1Done();
                return false;
            }

            internal bool <>m__141()
            {
                this.tutorial.OnTutoriaStep9Done();
                return false;
            }

            internal bool <>m__142()
            {
                this.tutorial.OnTutoriaStep10Done();
                return false;
            }

            internal bool <>m__143()
            {
                this.tutorial.OnTutoriaStep11Done();
                return false;
            }

            internal bool <>m__144()
            {
                this.tutorial.OnTutoriaStep12Done();
                return false;
            }

            internal bool <>m__145()
            {
                this.tutorial.OnTutorialStep13Done();
                return false;
            }

            internal bool <>m__146()
            {
                this.tutorial.OnTutorialStep14Done();
                return false;
            }

            internal bool <>m__147()
            {
                this.tutorial.OnTutorialStep15Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialSwapAndRestrain>c__AnonStoreyF1
        {
            internal LevelTutorialSwapAndRestrain tutorial;

            internal bool <>m__158()
            {
                this.tutorial.OnTutorialStep0Done();
                return false;
            }

            internal bool <>m__159()
            {
                this.tutorial.OnTutorialStep1Done();
                return false;
            }

            internal bool <>m__15A()
            {
                this.tutorial.OnTutorialStep2Done();
                return false;
            }

            internal bool <>m__15B()
            {
                this.tutorial.OnTutorialStep3Done();
                return false;
            }

            internal bool <>m__15C()
            {
                this.tutorial.OnTutorialStep4Done();
                return false;
            }

            internal bool <>m__15D()
            {
                this.tutorial.OnTutorialStep5Done();
                return false;
            }

            internal bool <>m__15E()
            {
                this.tutorial.OnTutorialStep6Done();
                return false;
            }

            internal bool <>m__15F()
            {
                this.tutorial.OnTutorialStep7Done();
                return false;
            }

            internal bool <>m__160()
            {
                this.tutorial.OnTutorialStep8Done();
                return false;
            }

            internal bool <>m__161()
            {
                this.tutorial.OnTutorialStep9Done();
                return false;
            }

            internal bool <>m__162()
            {
                this.tutorial.OnTutorialStep10Done();
                return false;
            }

            internal bool <>m__163()
            {
                this.tutorial.OnTutorialStep11Done();
                return false;
            }

            internal bool <>m__164()
            {
                this.tutorial.OnTutorialStep12Done();
                return false;
            }

            internal bool <>m__165()
            {
                this.tutorial.OnTutorialStep13Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialSwapAttackNotify>c__AnonStoreyF0
        {
            internal LevelTutorialSwapAttack tutorial;

            internal bool <>m__153()
            {
                this.tutorial.OnTutorialStep1Done();
                return false;
            }

            internal bool <>m__154()
            {
                this.tutorial.OnTutorialStep2Done();
                return false;
            }

            internal bool <>m__155()
            {
                this.tutorial.OnTutorialStep3Done();
                return false;
            }

            internal bool <>m__156()
            {
                this.tutorial.OnTutorialStep4Done();
                return false;
            }

            internal bool <>m__157()
            {
                this.tutorial.OnTutorialStep5Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <OnTutorialUltraAttackNotify>c__AnonStoreyED
        {
            internal LevelTutorialUltraAttack tutorial;

            internal bool <>m__148()
            {
                this.tutorial.OnStep1Done();
                return false;
            }

            internal bool <>m__149()
            {
                this.tutorial.OnStep2Done();
                return false;
            }

            internal bool <>m__14A()
            {
                this.tutorial.OnStep3Done();
                return false;
            }

            internal bool <>m__14B()
            {
                this.tutorial.OnStep4Done();
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <WaitFadeAnimation>c__Iterator64 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal bool <$>active;
            internal Animation <$>animation;
            internal Action <$>callback;
            internal InLevelMainPageContext <>f__this;
            internal bool active;
            internal Animation animation;
            internal Action callback;

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
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PauseBtnEnable, false));
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00DB;
                }
                if ((this.animation != null) && this.animation.isPlaying)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PauseBtnEnable, true));
                if (this.active)
                {
                    this.<>f__this._showState = InLevelMainPageContext.InLevelMainPageShowState.Show;
                }
                else
                {
                    this.<>f__this._showState = InLevelMainPageContext.InLevelMainPageShowState.Hide;
                    this.<>f__this.SetActive(false);
                }
                if (this.callback != null)
                {
                    this.callback();
                }
                this.$PC = -1;
            Label_00DB:
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

        public enum InLevelMainPageShowState
        {
            Show,
            Hide,
            Changing
        }
    }
}

