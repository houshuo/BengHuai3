namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class EquipPowerUpEffectDialogContext : BaseDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private CanvasTimer _effectDelayTimer;
        private int _effectIndex;
        private List<Vector2> _effectPosition = new List<Vector2> { new Vector2(50f, 0f), new Vector2(-50f, 0f), new Vector2(0f, 50f), new Vector2(0f, -50f), new Vector2(50f, 50f), new Vector2(0f, 0f) };
        private CanvasTimer _flashResetTimer;
        private CanvasTimer _flashStartTimer;
        private List<StorageDataItemBase> _materialList;
        private int _powerUpResultIndex;
        private const string BIG_SUCCESS_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
        public readonly int boostRate;
        public readonly DialogType dialogType;
        private const float EFFECT_POSITION_RANGE = 50f;
        private const string EVOLUTION_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/Evolution";
        public readonly StorageDataItemBase itemDataAfter;
        public readonly StorageDataItemBase itemDataBefore;
        private const string LARGE_SUCCESS_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/UpgradingLargeSuccess";
        private const string POWERUP_RESULT_PREFAB_PRE = "SpriteOutput/EquipPowerUpResult/Success";
        private const string SMALL_SPOT_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/SmallSpot";
        private const string STAR_GREY_ICON_PATH = "SpriteOutput/StarBigGray";
        private const string STAR_ICON_PATH = "SpriteOutput/StarBig";
        private const string SUB_STAR_GREY_ICON_PATH = "SpriteOutput/SubStarActiveGray";
        private const string SUB_STAR_ICON_PATH = "SpriteOutput/SubStarActive";
        private const string SUCCESS_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/UpgradeSuccess";

        public EquipPowerUpEffectDialogContext(StorageDataItemBase itemDataBefore, StorageDataItemBase itemDataAfter, List<StorageDataItemBase> materialList, DialogType type = 0, int boostRate = 100)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EquipPowerUpEffectDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/EquipPowerUpEffectDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            this.itemDataAfter = itemDataAfter;
            this.itemDataBefore = itemDataBefore;
            this._materialList = new List<StorageDataItemBase>();
            foreach (StorageDataItemBase base2 in materialList)
            {
                this._materialList.Add(base2.Clone());
            }
            this.boostRate = boostRate;
            this.dialogType = type;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Button").GetComponent<Button>(), new UnityAction(this.OnBGClick));
            if (this.dialogType == DialogType.NewAffix)
            {
                base.BindViewCallback(base.view.transform.Find("SkillPopup/SkillBtn").GetComponent<Button>(), new UnityAction(this.OnStigmataSkillBtnClick));
                base.BindViewCallback(base.view.transform.Find("StigmataAffixInfo/OldAffix/OkBtn").GetComponent<Button>(), new UnityAction(this.OnOldAffixBtnClick));
                base.BindViewCallback(base.view.transform.Find("StigmataAffixInfo/NewAffix/OkBtn").GetComponent<Button>(), new UnityAction(this.OnNewAffixBtnClick));
            }
        }

        public override void Destroy()
        {
            if (this._flashStartTimer != null)
            {
                this._flashStartTimer.Destroy();
            }
            if (this._flashResetTimer != null)
            {
                this._flashResetTimer.Destroy();
            }
            base.Destroy();
        }

        private void EnableBGClick()
        {
            base.view.transform.Find("Button").gameObject.SetActive(true);
            Transform transform = base.view.transform.Find("Result");
            switch (this.dialogType)
            {
                case DialogType.PowerUp:
                    transform.Find("Particle1").gameObject.SetActive(true);
                    transform.Find("Particle1/Spot").gameObject.SetActive(false);
                    switch (this._powerUpResultIndex)
                    {
                        case 1:
                            transform.Find("Particle1").GetComponent<ParticleSystem>().startColor = MiscData.GetColor("PowerUpSuccess1");
                            return;

                        case 2:
                            transform.Find("Particle1").GetComponent<ParticleSystem>().startColor = MiscData.GetColor("PowerUpSuccess2");
                            return;

                        case 3:
                            transform.Find("Particle1").GetComponent<ParticleSystem>().startColor = MiscData.GetColor("PowerUpSuccess3");
                            transform.Find("Particle1/Spot").gameObject.SetActive(true);
                            return;
                    }
                    break;

                case DialogType.Evo:
                    transform.Find("Particle1").gameObject.SetActive(false);
                    transform.Find("Particle2").gameObject.SetActive(true);
                    break;
            }
        }

        private ParticleSystem GetSmallEffect()
        {
            Transform transform = base.view.transform.Find("ItemPanel/Effects/Small/" + this._effectIndex);
            transform.gameObject.SetActive(true);
            if (this._effectIndex == (this._materialList.Count - 1))
            {
                transform.localPosition = this._effectPosition[this._effectPosition.Count - 1];
            }
            else
            {
                transform.localPosition = this._effectPosition[this._effectIndex];
            }
            this._effectIndex++;
            foreach (ParticleSystem system in transform.GetComponentsInChildren<ParticleSystem>())
            {
                system.Play();
            }
            if (this._flashStartTimer != null)
            {
                this._flashStartTimer.Destroy();
            }
            this._flashStartTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.01666667f, 0f);
            this._flashStartTimer.timeUpCallback = new Action(this.StartImageColorScaler);
            return transform.GetComponent<ParticleSystem>();
        }

        public void OnBGClick()
        {
            this.Destroy();
            Singleton<MainUIManager>.Instance.BackPage();
            if ((this.dialogType == DialogType.Evo) || (this.dialogType == DialogType.PowerUp))
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EquipPowerupOrEvo, this.itemDataAfter));
            }
            else if (this.dialogType == DialogType.NewAffix)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.StigmataNewAffix, null));
            }
        }

        public void OnNewAffixBtnClick()
        {
            Singleton<NetworkManager>.Instance.RequestSelectNewStigmataAffix();
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.PowerUpAndEvoEffect) && this.PlayEffect(ntf));
        }

        public void OnOldAffixBtnClick()
        {
            this.Destroy();
            Singleton<MainUIManager>.Instance.BackPage();
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0xc4) && this.OnSelectNewStigmataAffixRsp(pkt.getData<SelectNewStigmataAffixRsp>()));
        }

        public bool OnSelectNewStigmataAffixRsp(SelectNewStigmataAffixRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.OnBGClick();
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        public void OnStigmataSkillBtnClick()
        {
            GameObject gameObject = base.view.transform.Find("SkillPopup/StigmataSkills").gameObject;
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public bool PlayEffect(Notify ntf)
        {
            string str = ntf.body.ToString();
            ParticleSystem smallEffect = this.GetSmallEffect();
            if (str == "EatAll")
            {
                this.PlayEffectWithDelay(smallEffect.duration / 8f);
            }
            return false;
        }

        private void PlayEffectWithDelay(float delay)
        {
            this._effectDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(delay, 0f);
            this._effectDelayTimer.timeUpCallback = new Action(this.PlayPowerUpEffect);
        }

        private void PlayPowerUpEffect()
        {
            Transform transform;
            ParticleSystem[] systemArray;
            if (this.dialogType == DialogType.PowerUp)
            {
                this._powerUpResultIndex = MiscData.GetEquipPowerUpResultIndex(this.boostRate);
                switch (this._powerUpResultIndex)
                {
                    case 2:
                        Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Small", null, null, null);
                        transform = base.view.transform.Find("ItemPanel/Effects/UpgradingBigSuccess");
                        transform.gameObject.SetActive(true);
                        goto Label_011D;

                    case 3:
                        Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Large", null, null, null);
                        transform = base.view.transform.Find("ItemPanel/Effects/UpgradingLargeSuccess");
                        transform.gameObject.SetActive(true);
                        goto Label_011D;
                }
                Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Small", null, null, null);
                transform = base.view.transform.Find("ItemPanel/Effects/UpgradeSuccess");
                transform.gameObject.SetActive(true);
            }
            else
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Large", null, null, null);
                transform = base.view.transform.Find("ItemPanel/Effects/Evolution");
                transform.gameObject.SetActive(true);
            }
        Label_011D:
            systemArray = transform.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < systemArray.Length; i++)
            {
                systemArray[i].Play();
            }
            if (this.dialogType == DialogType.PowerUp)
            {
                this.SetupLvInfoPanel(0f);
            }
            else if (this.dialogType == DialogType.Evo)
            {
                this.SetupTitleAndStar(0f, false);
            }
            else if (this.dialogType == DialogType.NewAffix)
            {
                this.SetupNewAffixInfo(0f);
            }
        }

        private void ResetImageColorScaler()
        {
            this.SetImageColorScaler(1f);
        }

        private void SetEquipImageFlash(float scaler, float timeSpan)
        {
            if (this._flashResetTimer != null)
            {
                this._flashResetTimer.Destroy();
            }
            this._flashResetTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(timeSpan, 0f);
            this._flashResetTimer.timeUpCallback = new Action(this.ResetImageColorScaler);
            this.SetImageColorScaler(scaler);
        }

        private void SetImageColorScaler(float scaler)
        {
            if (this.itemDataBefore is WeaponDataItem)
            {
                base.view.transform.Find("ItemPanel/3dModel").GetComponent<RawImage>().material.SetFloat("_ColorScaler", scaler);
            }
        }

        private void SetStarColor(Transform starTrans)
        {
            if ((starTrans != null) && (starTrans.GetComponent<Image>() != null))
            {
                starTrans.GetComponent<Image>().color = MiscData.GetColor("TotalWhite");
            }
        }

        private void SetupLvInfoPanel(float starDelay = 0f)
        {
            this._animationManager = new SequenceAnimationManager(new Action(this.EnableBGClick), null);
            base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(false);
            base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(false);
            base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(false);
            if (this.itemDataBefore is WeaponDataItem)
            {
                base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(this.itemDataAfter as WeaponDataItem, false, 0);
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoAnimationinSequence>(), null);
            }
            else if (this.itemDataBefore is StigmataDataItem)
            {
                base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(this.itemDataBefore as StigmataDataItem);
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoAnimationinSequence>(), null);
            }
            else
            {
                base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this.itemDataAfter.GetImagePath());
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<MonoAnimationinSequence>(), null);
            }
            base.view.transform.Find("Result").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/EquipPowerUpResult/Success" + MiscData.GetEquipPowerUpResultIndex(this.boostRate).ToString());
            this._animationManager.AddAnimation(base.view.transform.Find("Result").GetComponent<MonoAnimationinSequence>(), null);
            base.view.transform.Find("InfoRowLv").GetComponent<MonoEquipExpGrow>().SetData(this.itemDataBefore.level, this.itemDataBefore.GetMaxExp(), this.itemDataBefore.exp, this.itemDataAfter.exp, UIUtil.GetEquipmentMaxExpList(this.itemDataBefore, this.itemDataBefore.level, this.itemDataAfter.level));
            this._animationManager.AddAnimation(base.view.transform.Find("InfoRowLv").GetComponent<MonoAnimationinSequence>(), null);
            this._animationManager.StartPlay(starDelay, true);
        }

        private void SetupMaterialList()
        {
            if ((this._materialList != null) && (this._materialList.Count > 0))
            {
                base.view.transform.Find("MaterialListPanel").GetComponent<MonoMaterialPanel>().SetupView(this._materialList);
            }
        }

        private void SetupNewAffixInfo(float delay = 0f)
        {
            StigmataDataItem itemDataBefore = this.itemDataBefore as StigmataDataItem;
            StigmataDataItem itemDataAfter = this.itemDataAfter as StigmataDataItem;
            base.view.transform.Find("StigmataAffixInfo").gameObject.SetActive(true);
            if (itemDataBefore.GetAffixSkillList().Count > 0)
            {
                base.view.transform.Find("StigmataAffixInfo/OldAffix").gameObject.SetActive(true);
                base.view.transform.Find("StigmataAffixInfo/OldAffix/Skills/Content").GetComponent<MonoStigmataAffixSkillPanel>().SetupView(itemDataBefore, itemDataBefore.GetAffixSkillList());
            }
            else
            {
                base.view.transform.Find("StigmataAffixInfo/OldAffix").gameObject.SetActive(false);
            }
            base.view.transform.Find("StigmataAffixInfo/NewAffix/Skills/Content").GetComponent<MonoStigmataAffixSkillPanel>().SetupView(itemDataAfter, itemDataAfter.GetAffixSkillList());
            this.SetupStigmataSkillInfo();
            base.view.transform.Find("SkillPopup").gameObject.SetActive(true);
        }

        private void SetupStigmataSkillInfo()
        {
            int num = 3;
            StigmataDataItem itemDataBefore = this.itemDataBefore as StigmataDataItem;
            List<EquipSkillDataItem> skills = itemDataBefore.skills;
            Transform transform = base.view.transform.Find("SkillPopup/StigmataSkills/ScrollerView/Content/NaturalSkills");
            transform.gameObject.SetActive(skills.Count > 0);
            string text = LocalizationGeneralLogic.GetText("Menu_Title_StigmataSkill", new object[0]);
            transform.Find("Name/Label").GetComponent<Text>().text = text;
            for (int i = 1; i <= num; i++)
            {
                Transform trans = base.view.transform.Find("SkillPopup/StigmataSkills/ScrollerView/Content/NaturalSkills/Desc/Skill_" + i);
                trans.gameObject.SetActive(true);
                if (i > skills.Count)
                {
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    EquipSkillDataItem skillData = skills[i - 1];
                    this.UpdateSkillContent(trans, skillData);
                }
            }
            Transform transform3 = base.view.transform.Find("SkillPopup/StigmataSkills/ScrollerView/Content/SetSkills");
            SortedDictionary<int, EquipSkillDataItem> allSetSkills = itemDataBefore.GetAllSetSkills();
            if (allSetSkills.Count == 0)
            {
                transform3.gameObject.SetActive(false);
            }
            else
            {
                transform3.gameObject.SetActive(true);
                transform3.Find("Name/Text").GetComponent<Text>().text = itemDataBefore.GetEquipSetName();
                Transform transform4 = transform3.Find("Desc");
                for (int j = 0; j < transform3.Find("Desc").childCount; j++)
                {
                    int key = j + 2;
                    Transform child = transform4.GetChild(j);
                    if (child != null)
                    {
                        EquipSkillDataItem item3;
                        allSetSkills.TryGetValue(key, out item3);
                        if (item3 == null)
                        {
                            child.gameObject.SetActive(false);
                        }
                        else
                        {
                            child.Find("Desc").GetComponent<Text>().text = item3.GetSkillDisplay(1);
                        }
                    }
                }
            }
            base.view.transform.Find("SkillPopup/StigmataSkills").gameObject.SetActive(false);
        }

        private void SetupTitleAndStar(float delay = 0f, bool showAfterItem = false)
        {
            base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(false);
            base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(false);
            base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(false);
            this._animationManager = new SequenceAnimationManager(new Action(this.EnableBGClick), null);
            if (this.itemDataBefore is WeaponDataItem)
            {
                base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(this.itemDataAfter as WeaponDataItem, false, 0);
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoAnimationinSequence>(), null);
            }
            else if (this.itemDataBefore is StigmataDataItem)
            {
                base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(this.itemDataAfter as StigmataDataItem);
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoAnimationinSequence>(), null);
            }
            else
            {
                base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this.itemDataAfter.GetImagePath());
                this._animationManager.AddAnimation(base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<MonoAnimationinSequence>(), null);
            }
            base.view.transform.Find("Title").gameObject.SetActive(false);
            base.view.transform.Find("Result").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/EquipPowerUpResult/Success4");
            this._animationManager.AddAnimation(base.view.transform.Find("Result").GetComponent<MonoAnimationinSequence>(), null);
            this._animationManager.AddAnimation(base.view.transform.Find("StarBG").GetComponent<MonoAnimationinSequence>(), null);
            Transform transform = base.view.transform.Find("Stars");
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(i < this.itemDataAfter.GetMaxRarity());
                if (i < this.itemDataAfter.GetMaxRarity())
                {
                    child.GetComponent<Image>().color = MiscData.GetColor("RarityStarGrey");
                    child.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/StarBigGray");
                    child.GetComponent<CanvasGroup>().alpha = 1f;
                    if (i < this.itemDataAfter.rarity)
                    {
                        this._animationManager.AddAnimation(child.GetComponent<MonoAnimationinSequence>(), new Action<Transform>(this.SetStarColor));
                        child.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/StarBig");
                    }
                }
            }
            Transform transform3 = base.view.transform.Find("StarsMini");
            if (this.itemDataAfter.GetMaxSubRarity() > 0)
            {
                this._animationManager.AddAnimation(transform3.GetComponent<MonoAnimationinSequence>(), null);
            }
            for (int j = 0; j < transform3.childCount; j++)
            {
                Transform transform4 = transform3.GetChild(j);
                transform4.gameObject.SetActive(j < (this.itemDataAfter.GetMaxSubRarity() - 1));
                if (j < (this.itemDataAfter.GetMaxSubRarity() - 1))
                {
                    transform4.GetComponent<Image>().color = MiscData.GetColor("RarityStarGrey");
                    transform4.GetComponent<CanvasGroup>().alpha = 1f;
                    transform4.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/SubStarActiveGray");
                    if (j < this.itemDataAfter.GetSubRarity())
                    {
                        this._animationManager.AddAnimation(transform4.GetComponent<MonoAnimationinSequence>(), new Action<Transform>(this.SetStarColor));
                        transform4.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/SubStarActive");
                    }
                }
            }
            this._animationManager.StartPlay(delay, true);
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(null, null);
            foreach (ParticleSystem system in base.view.transform.Find("ItemPanel/Effects").GetComponentsInChildren<ParticleSystem>())
            {
                system.Stop();
            }
            foreach (ParticleSystem system2 in base.view.transform.Find("Result").GetComponentsInChildren<ParticleSystem>())
            {
                system2.Stop();
            }
            base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(false);
            base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(false);
            base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(false);
            base.view.transform.Find("SkillPopup").gameObject.SetActive(false);
            if (this.itemDataBefore is WeaponDataItem)
            {
                base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(this.itemDataBefore as WeaponDataItem, false, 0);
            }
            else if (this.itemDataBefore is StigmataDataItem)
            {
                base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(this.itemDataBefore as StigmataDataItem);
            }
            else
            {
                base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(true);
                base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this.itemDataBefore.GetImagePath());
            }
            this.SetupMaterialList();
            base.view.transform.Find("Button").gameObject.SetActive(false);
            base.view.transform.Find("StarBG").GetComponent<CanvasGroup>().alpha = 0f;
            this._effectIndex = 0;
            base.view.transform.Find("StigmataAffixInfo").gameObject.SetActive(false);
            return false;
        }

        private void StartImageColorScaler()
        {
            this.SetEquipImageFlash(3f, 0.05f);
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent)
        {
            base.StartUp(canvasTrans, viewParent);
            if (this.dialogType == DialogType.Evo)
            {
                string evtName = null;
                if (this.itemDataAfter is WeaponDataItem)
                {
                    evtName = "VO_M_Con_05_Weapon_Upgread";
                }
                else if (this.itemDataAfter is StigmataDataItem)
                {
                    evtName = "VO_M_Con_06_Stigmata_Upgread";
                }
                if (evtName != null)
                {
                    Singleton<WwiseAudioManager>.Instance.Post(evtName, null, null, null);
                }
            }
        }

        private void UpdateSkillContent(Transform trans, EquipSkillDataItem skillData)
        {
            trans.Find("Label").GetComponent<Text>().text = skillData.skillName;
            trans.Find("Desc").GetComponent<Text>().text = skillData.GetSkillDisplay(this.itemDataBefore.level);
        }

        public enum DialogType
        {
            PowerUp,
            Evo,
            NewAffix
        }

        public enum EffectType
        {
            Small,
            Large
        }
    }
}

