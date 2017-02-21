namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoMonsterStatus : MonoBehaviour
    {
        private MonsterActor _currentMonster;
        private MonoSliderGroupWithPhase _hpSlider;
        private MonoMaskSlider _shieldSlider;
        private const string ELITE_ICON_PREFAB = "UI/Menus/Widget/InLevel/EliteIcon";
        private int RED_NAME_DIFF_LEVEL;
        private int WHITE_NAME_DIFF_LEVEL;
        private int YELLOW_NAME_DIFF_LEVEL;

        private void Awake()
        {
            this._hpSlider = base.transform.Find("HPBar").GetComponent<MonoSliderGroupWithPhase>();
            this._shieldSlider = base.transform.Find("ShieldBar").GetComponent<MonoMaskSlider>();
            this.WHITE_NAME_DIFF_LEVEL = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepOne;
            this.YELLOW_NAME_DIFF_LEVEL = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepTwo;
            this.RED_NAME_DIFF_LEVEL = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepThree;
        }

        private bool SetupEliteIcon(MonsterActor monsterActor)
        {
            Transform trans = base.transform.Find("EliteIcons");
            trans.DestroyChildren();
            bool flag = false;
            if (monsterActor.uniqueMonsterID > 0)
            {
                return false;
            }
            foreach (ActorAbility ability in monsterActor.abilityPlugin.GetAppliedAbilities())
            {
                if ((ability != null) && (ability.config != null))
                {
                    string abilityName = ability.config.AbilityName;
                    if (MiscData.Config.EliteAbilityIcon.ContainsKey(abilityName))
                    {
                        string str2 = MiscData.Config.EliteAbilityIcon[abilityName].ToString();
                        string textID = MiscData.Config.EliteAbilityText[abilityName].ToString();
                        if (!string.IsNullOrEmpty(str2))
                        {
                            GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/InLevel/EliteIcon", BundleType.RESOURCE_FILE));
                            obj2.transform.SetParent(trans, false);
                            obj2.transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str2);
                            obj2.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
                            flag = true;
                        }
                    }
                }
            }
            trans.gameObject.SetActive(flag);
            return flag;
        }

        private void SetupHPBar(MonsterActor monsterActor)
        {
            int newMaxPhase = 1;
            if (monsterActor.uniqueMonsterID > 0)
            {
                UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(monsterActor.uniqueMonsterID);
                newMaxPhase = (uniqueMonsterMetaData != null) ? uniqueMonsterMetaData.hpPhaseNum : 1;
            }
            this._hpSlider.UpdateMaxPhase(newMaxPhase);
            this._hpSlider.UpdateValue((float) monsterActor.HP, (float) monsterActor.maxHP, 0f);
        }

        public void SetupMonsterNameByLevelPunish()
        {
            Text component = base.transform.Find("NameText").GetComponent<Text>();
            Gradient gradient = base.transform.Find("NameText").GetComponent<Gradient>();
            Outline outline = base.transform.Find("NameText").GetComponent<Outline>();
            if ((this._currentMonster != null) && (((gradient != null) && (outline != null)) && (component != null)))
            {
                if (Singleton<LevelScoreManager>.Instance.IsAllowLevelPunish())
                {
                    int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
                    int num2 = Mathf.Clamp(((int) this._currentMonster.level) - teamLevel, 0, 10);
                    if ((num2 >= this.WHITE_NAME_DIFF_LEVEL) && (num2 < this.YELLOW_NAME_DIFF_LEVEL))
                    {
                        component.color = MiscData.GetColor("PunishWhiteTopColor");
                        outline.effectColor = MiscData.GetColor("PunishWhiteOutlineColor");
                    }
                    else if ((num2 >= this.YELLOW_NAME_DIFF_LEVEL) && (num2 < this.RED_NAME_DIFF_LEVEL))
                    {
                        component.color = MiscData.GetColor("PunishYellowTopColor");
                        outline.effectColor = MiscData.GetColor("PunishYellowOutlineColor");
                    }
                    else
                    {
                        component.color = MiscData.GetColor("PunishRedTopColor");
                        outline.effectColor = MiscData.GetColor("PunishRedOutlineColor");
                    }
                }
                else
                {
                    gradient.topColor = Color.white;
                    gradient.bottomColor = Color.white;
                    outline.effectColor = MiscData.GetColor("PunishWhiteOutlineColor");
                }
            }
        }

        private void SetupNameText(MonsterActor monsterActor)
        {
            base.transform.Find("NameText").gameObject.SetActive(true);
            string str = string.Empty;
            if (monsterActor.uniqueMonsterID > 0)
            {
                UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(monsterActor.uniqueMonsterID);
                str = (uniqueMonsterMetaData != null) ? LocalizationGeneralLogic.GetText(uniqueMonsterMetaData.name, new object[0]) : monsterActor.uniqueMonsterID.ToString();
            }
            else
            {
                MonsterUIMetaData monsterUIMetaDataByName = MonsterUIMetaDataReaderExtend.GetMonsterUIMetaDataByName(monsterActor.metaConfig.subTypeName);
                str = (monsterUIMetaDataByName != null) ? LocalizationGeneralLogic.GetText(monsterUIMetaDataByName.displayTitle, new object[0]) : monsterActor.metaConfig.subTypeName;
            }
            base.transform.Find("NameText").GetComponent<Text>().text = str;
        }

        public void SetupNatureBonus()
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            EntityNature attribute = (EntityNature) Singleton<EventManager>.Instance.GetActor<AvatarActor>(localAvatar.GetRuntimeID()).avatarDataItem.Attribute;
            EntityNature nature = (EntityNature) this._currentMonster.metaConfig.nature;
            int natureBonusType = DamageModelLogic.GetNatureBonusType(attribute, nature);
            base.transform.Find("DamageMark/Up").gameObject.SetActive(natureBonusType == 1);
            base.transform.Find("DamageMark/Down").gameObject.SetActive(natureBonusType == -1);
        }

        private void SetupShieldBar(MonsterActor monsterActor)
        {
            bool flag = monsterActor.abilityPlugin.HasDisplayFloat("Shield");
            base.transform.Find("ShieldBar").gameObject.SetActive(flag);
            if (flag)
            {
                float curValue = 0f;
                float ceiling = 0f;
                float floor = 0f;
                monsterActor.abilityPlugin.SubAttachDisplayFloat("Shield", new Action<float, float>(this.UpdateMonsterShieldBar), ref curValue, ref floor, ref ceiling);
                this._shieldSlider.UpdateValue(curValue, ceiling, floor);
            }
        }

        public void SetupView(MonsterActor monsterBefore, MonsterActor monsterAfter)
        {
            if (monsterBefore != null)
            {
                monsterBefore.onHPChanged = (Action<float, float, float>) Delegate.Remove(monsterBefore.onHPChanged, new Action<float, float, float>(this.UpdateMonsterHPBar));
                monsterBefore.onMaxHPChanged = (Action<float, float>) Delegate.Remove(monsterBefore.onMaxHPChanged, new Action<float, float>(this.UpdateMonsterMaxHP));
                if (monsterBefore.abilityPlugin.HasDisplayFloat("Shield"))
                {
                    monsterBefore.abilityPlugin.SubDetachDisplayFloat("Shield", new Action<float, float>(this.UpdateMonsterShieldBar));
                }
            }
            if (monsterAfter != null)
            {
                monsterAfter.onHPChanged = (Action<float, float, float>) Delegate.Combine(monsterAfter.onHPChanged, new Action<float, float, float>(this.UpdateMonsterHPBar));
                monsterAfter.onMaxHPChanged = (Action<float, float>) Delegate.Combine(monsterAfter.onMaxHPChanged, new Action<float, float>(this.UpdateMonsterMaxHP));
                base.gameObject.SetActive(true);
                this.SetupHPBar(monsterAfter);
                this.SetupShieldBar(monsterAfter);
                if (!this.SetupEliteIcon(monsterAfter))
                {
                    this.SetupNameText(monsterAfter);
                }
                else
                {
                    base.transform.Find("NameText").gameObject.SetActive(false);
                }
            }
            this._currentMonster = monsterAfter;
            this.SetupNatureBonus();
            this.SetupMonsterNameByLevelPunish();
        }

        private void UpdateMonsterHPBar(float from, float to, float delta)
        {
            this._hpSlider.UpdateValue(to, this._hpSlider.maxValue, 0f);
            if (to <= 0f)
            {
                base.gameObject.SetActive(false);
            }
        }

        private void UpdateMonsterMaxHP(float from, float to)
        {
            this._hpSlider.UpdateValue(this._hpSlider.value, to, 0f);
        }

        private void UpdateMonsterShieldBar(float from, float to)
        {
            this._shieldSlider.UpdateValue(to, this._shieldSlider.maxValue, 0f);
        }
    }
}

