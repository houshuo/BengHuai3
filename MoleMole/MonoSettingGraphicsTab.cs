namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoSettingGraphicsTab : MonoBehaviour
    {
        private ConfigGraphicsPersonalSetting _modifiedPersonalSetting;
        public Button[] configButtons;
        public Transform ecoMode;
        public Transform[] grades;
        private bool isInLevelSimpleSetting;
        public Transform[] processes;

        public bool CheckNeedSave()
        {
            return !GraphicsSettingData.IsEqualToPersonalConfigIgnoreContrast(this._modifiedPersonalSetting);
        }

        private void EnableAllConfigBtns(bool enable)
        {
            foreach (Button button in this.configButtons)
            {
                if (button != null)
                {
                    button.interactable = enable;
                }
            }
        }

        public void OnAABtnClick()
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            bool useFXAA = this._modifiedPersonalSetting.VolatileSetting.UseFXAA;
            this._modifiedPersonalSetting.VolatileSetting.UseFXAA = !useFXAA;
            GraphicsSettingUtil.EnableFXAA(useFXAA);
            this.ShowAA(this._modifiedPersonalSetting.VolatileSetting.UseFXAA, false, true);
        }

        public void OnDistortionBtnClick()
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            bool useDistortion = this._modifiedPersonalSetting.VolatileSetting.UseDistortion;
            this._modifiedPersonalSetting.VolatileSetting.UseDistortion = !useDistortion;
            GraphicsSettingUtil.EnableDistortion(useDistortion);
            this.ShowDistortion(this._modifiedPersonalSetting.VolatileSetting.UseDistortion, false, true);
        }

        public void OnDynamicBoneBtnClick(bool willBeOn)
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            this._modifiedPersonalSetting.VolatileSetting.UseDynamicBone = willBeOn;
            GraphicsSettingUtil.EnableDynamicBone(willBeOn);
            this.ShowDynamicBone(this._modifiedPersonalSetting.VolatileSetting.UseDynamicBone, false);
        }

        public void OnEcoModeBtnClick(bool willBeOn)
        {
            GraphicsSettingData.CopyPersonalGraphicsConfig(willBeOn, ref this._modifiedPersonalSetting);
            this.ResetView();
        }

        public void OnHDRBtnClick()
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            bool useHDR = this._modifiedPersonalSetting.VolatileSetting.UseHDR;
            this._modifiedPersonalSetting.VolatileSetting.UseHDR = !useHDR;
            GraphicsSettingUtil.EnableHDR(useHDR);
            this.ShowHDR(this._modifiedPersonalSetting.VolatileSetting.UseHDR, false, true);
        }

        public void OnNoSaveBtnClick()
        {
            GraphicsSettingData.ApplySettingConfig();
            GraphicsSettingData.CopyPersonalGraphicsConfig(ref this._modifiedPersonalSetting);
            this.ResetView();
        }

        public void OnPersonalGradeBtnClick()
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            if (Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsUserDefinedVolatile)
            {
                GraphicsSettingData.CopyPersonalGraphicsConfig(ref this._modifiedPersonalSetting);
            }
            else
            {
                GraphicsSettingData.CopyToPersonalGraphicsConfig(GraphicsSettingData.GetGraphicsRecommendCompleteConfig(), ref this._modifiedPersonalSetting);
            }
            GraphicsSettingData.ApplySettingConfig(GraphicsSettingData.GetGraphicsPersonalSettingConfig(this._modifiedPersonalSetting));
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            this._modifiedPersonalSetting.IsEcoMode = false;
            this.ShowPersonalConfig();
            this.SetupEcoMode();
        }

        public void OnPostFXBtnClick(bool willBeOn)
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            this._modifiedPersonalSetting.VolatileSetting.UsePostFX = willBeOn;
            bool forceWhenDisable = true;
            GraphicsSettingUtil.EnablePostFX(willBeOn, forceWhenDisable);
            this.ShowPostFX(this._modifiedPersonalSetting, false);
        }

        public void OnPostFXGradeBtnClick()
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            if (this._modifiedPersonalSetting.VolatileSetting.PostFXGrade == PostEffectQualityGrade.Low)
            {
                this._modifiedPersonalSetting.VolatileSetting.PostFXGrade = PostEffectQualityGrade.High;
            }
            else
            {
                this._modifiedPersonalSetting.VolatileSetting.PostFXGrade = PostEffectQualityGrade.Low;
            }
            GraphicsSettingUtil.SetPostEffectBufferSizeByQuality(this._modifiedPersonalSetting.PostFxGradeBufferSize, this._modifiedPersonalSetting.VolatileSetting.PostFXGrade);
            this.ShowPostFXGrade(this._modifiedPersonalSetting.VolatileSetting.PostFXGrade, false, true);
        }

        public void OnRecommendGradeBtnClick(int index)
        {
            GraphicsRecommendGrade grade = (GraphicsRecommendGrade) index;
            if ((this._modifiedPersonalSetting.IsEcoMode || !this._modifiedPersonalSetting.IsUserDefinedGrade) || (grade != this._modifiedPersonalSetting.RecommendGrade))
            {
                this._modifiedPersonalSetting.IsUserDefinedGrade = true;
                this._modifiedPersonalSetting.IsUserDefinedVolatile = false;
                this._modifiedPersonalSetting.IsEcoMode = false;
                this._modifiedPersonalSetting.RecommendGrade = grade;
                GraphicsSettingData.ApplySettingConfig(GraphicsSettingData.GetGraphicsRecommendCompleteConfig(grade));
                this.EnableAllConfigBtns(false);
                this.ShowRecommendCompleteConfig(grade);
                this.SetupEcoMode();
            }
        }

        public void OnReflectionBtnClick(bool willBeOn)
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            this._modifiedPersonalSetting.VolatileSetting.UseReflection = willBeOn;
            GraphicsSettingUtil.EnableReflection(willBeOn);
            this.ShowReflection(this._modifiedPersonalSetting.VolatileSetting.UseReflection, false);
        }

        public void OnResolutionBtnClick(int grade)
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            this._modifiedPersonalSetting.ResolutionQuality = (ResolutionQualityGrade) grade;
            GraphicsSettingUtil.ApplyResolution(this._modifiedPersonalSetting.ResolutionPercentage, this._modifiedPersonalSetting.ResolutionQuality, this._modifiedPersonalSetting.RecommendResolutionX, this._modifiedPersonalSetting.RecommendResolutionY);
            this.ShowResolution(this._modifiedPersonalSetting.ResolutionQuality, false);
        }

        public void OnSaveBtnClick()
        {
            GraphicsSettingData.SavePersonalConfigIgnoreContrast(this._modifiedPersonalSetting);
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess", new object[0]), 2f), UIType.Any);
        }

        public void OnTargetFrameRateBtnClick(bool is30Rate)
        {
            this._modifiedPersonalSetting.IsUserDefinedGrade = false;
            this._modifiedPersonalSetting.IsUserDefinedVolatile = true;
            if (is30Rate)
            {
                this._modifiedPersonalSetting.TargetFrameRate = 30;
            }
            else
            {
                this._modifiedPersonalSetting.TargetFrameRate = 60;
            }
            GraphicsSettingUtil.SetTargetFrameRate(this._modifiedPersonalSetting.TargetFrameRate);
            this.ShowTargetFrameRate(this._modifiedPersonalSetting.TargetFrameRate, false);
        }

        public void ResetView()
        {
            if (this._modifiedPersonalSetting.IsEcoMode)
            {
                this.ShowEcoModeConfig();
            }
            else if (this._modifiedPersonalSetting.IsUserDefinedGrade || this._modifiedPersonalSetting.IsUserDefinedVolatile)
            {
                this.ShowPersonalConfig();
            }
            else
            {
                this.ShowDefaultRecommendCompleteConfig();
            }
            this.ShowRecommendMark();
            this.SetupEcoMode();
        }

        private void SetupEcoMode()
        {
            bool isEcoMode = this._modifiedPersonalSetting.IsEcoMode;
            if (this.isInLevelSimpleSetting)
            {
                this.ecoMode.FindChild("Mode/Desc/On").gameObject.SetActive(isEcoMode);
                this.ecoMode.FindChild("Mode/Desc/Off").gameObject.SetActive(!isEcoMode);
                Transform transform = this.ecoMode.Find("Mode/Choice/On");
                Transform transform2 = this.ecoMode.Find("Mode/Choice/Off");
                transform.FindChild("Blue").gameObject.SetActive(isEcoMode);
                transform.FindChild("Grey").gameObject.SetActive(!isEcoMode);
                transform.FindChild("Disable").gameObject.SetActive(false);
                transform.gameObject.SetActive(isEcoMode);
                transform.GetComponent<Button>().interactable = isEcoMode;
                transform2.FindChild("Grey").gameObject.SetActive(!isEcoMode);
                transform2.FindChild("Disable").gameObject.SetActive(false);
                transform2.gameObject.SetActive(!isEcoMode);
                transform2.GetComponent<Button>().interactable = !isEcoMode;
            }
            else
            {
                this.ecoMode.FindChild("Desc/ON").gameObject.SetActive(isEcoMode);
                this.ecoMode.FindChild("Desc/OFF").gameObject.SetActive(!isEcoMode);
                this.ecoMode.FindChild("Choice/Blue").gameObject.SetActive(isEcoMode);
                this.ecoMode.FindChild("Choice/Grey").gameObject.SetActive(!isEcoMode);
                this.ecoMode.FindChild("Choice/Disable").gameObject.SetActive(false);
                this.ecoMode.FindChild("Choice/Text").GetComponent<Text>().color = Color.white;
                Transform transform3 = this.ecoMode.FindChild("Choice/Check");
                transform3.GetComponent<Image>().color = Color.white;
                transform3.gameObject.SetActive(isEcoMode);
            }
        }

        public void SetupView(bool inLevelSimpleSetting = false)
        {
            this.isInLevelSimpleSetting = inLevelSimpleSetting;
            this._modifiedPersonalSetting = new ConfigGraphicsPersonalSetting();
            GraphicsSettingData.CopyPersonalGraphicsConfig(ref this._modifiedPersonalSetting);
            this.ResetView();
        }

        private void ShowAA(bool useFXAA, bool isRecommend, bool usePostFX)
        {
            this.ShowThirdLineElement(base.transform.Find("Content/DetailSetting/ThirdLine/Content/AA"), isRecommend, usePostFX, useFXAA);
        }

        private void ShowDefaultRecommendCompleteConfig()
        {
            this.EnableAllConfigBtns(false);
            GraphicsRecommendGrade graphicsRecommendGrade = GraphicsSettingData.GetGraphicsRecommendGrade();
            this.ShowRecommendCompleteConfig(graphicsRecommendGrade);
        }

        private void ShowDistortion(bool useDistortion, bool isRecommend, bool usePostFX)
        {
            this.ShowThirdLineElement(base.transform.Find("Content/DetailSetting/ThirdLine/Content/Distortion"), isRecommend, usePostFX, useDistortion);
        }

        private void ShowDynamicBone(bool useDynamicBone, bool isRecommend)
        {
            this.ShowSecondLineElement(base.transform.Find("Content/DetailSetting/SecondLine/DynamicBone"), isRecommend, useDynamicBone);
        }

        private void ShowEcoModeConfig()
        {
            this.EnableAllConfigBtns(false);
            this.ShowEcoModeGradeInfo();
            ConfigGraphicsSetting graphicsEcoModeConfig = GraphicsSettingData.GetGraphicsEcoModeConfig();
            this.ShowGraphicsSettingConfig(graphicsEcoModeConfig, true);
        }

        private void ShowEcoModeGradeInfo()
        {
            for (int i = 0; i < this.grades.Length; i++)
            {
                this.grades[i].FindChild("Blue").gameObject.SetActive(true);
                this.grades[i].FindChild("Check").gameObject.SetActive(false);
                this.processes[i].gameObject.SetActive(false);
            }
            this.processes[0].gameObject.SetActive(true);
        }

        private void ShowFirstLineElement(Transform elementTransform, bool isRecommend, bool isHigh)
        {
            Transform transform = elementTransform.Find("Mark");
            transform.FindChild("Enable").gameObject.SetActive(!isRecommend);
            transform.FindChild("Disable").gameObject.SetActive(isRecommend);
            Transform transform2 = elementTransform.FindChild("Choice/Low");
            Transform transform3 = elementTransform.FindChild("Choice/High");
            Transform transform4 = elementTransform.FindChild("Label");
            Transform transform5 = transform2.FindChild("Text");
            Transform transform6 = transform3.FindChild("Text");
            if (isRecommend)
            {
                transform2.FindChild("Blue").gameObject.SetActive(false);
                transform2.FindChild("Grey").gameObject.SetActive(false);
                transform2.FindChild("Disable").gameObject.SetActive(true);
                transform3.FindChild("Blue").gameObject.SetActive(false);
                transform3.FindChild("Grey").gameObject.SetActive(false);
                transform3.FindChild("Disable").gameObject.SetActive(true);
                transform4.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform5.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform6.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform2.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
                transform3.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
            }
            else
            {
                transform2.FindChild("Disable").gameObject.SetActive(false);
                transform3.FindChild("Disable").gameObject.SetActive(false);
                transform4.GetComponent<Text>().color = Color.white;
                transform5.GetComponent<Text>().color = Color.white;
                transform6.GetComponent<Text>().color = Color.white;
                transform2.FindChild("Check").GetComponent<Image>().color = Color.white;
                transform3.FindChild("Check").GetComponent<Image>().color = Color.white;
            }
            if (isHigh)
            {
                transform2.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
                transform3.FindChild("Button").GetComponent<Button>().interactable = false;
                if (!isRecommend)
                {
                    transform2.FindChild("Blue").gameObject.SetActive(false);
                    transform2.FindChild("Grey").gameObject.SetActive(true);
                    transform3.FindChild("Blue").gameObject.SetActive(true);
                    transform3.FindChild("Grey").gameObject.SetActive(false);
                }
                transform2.FindChild("Check").gameObject.SetActive(false);
                transform3.FindChild("Check").gameObject.SetActive(true);
            }
            else
            {
                transform2.FindChild("Button").GetComponent<Button>().interactable = false;
                transform3.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
                if (!isRecommend)
                {
                    transform3.FindChild("Blue").gameObject.SetActive(false);
                    transform3.FindChild("Grey").gameObject.SetActive(true);
                    transform2.FindChild("Blue").gameObject.SetActive(true);
                    transform2.FindChild("Grey").gameObject.SetActive(false);
                }
                transform3.FindChild("Check").gameObject.SetActive(false);
                transform2.FindChild("Check").gameObject.SetActive(true);
            }
        }

        private void ShowGraphicsSettingConfig(ConfigGraphicsSetting setting, bool isRecommend)
        {
            if (!this.isInLevelSimpleSetting)
            {
                Transform transform = base.transform.Find("Content/DetailSetting/BG");
                transform.Find("Grey").gameObject.SetActive(isRecommend);
                transform.Find("Blue").gameObject.SetActive(!isRecommend);
                this.ShowResolution(setting.ResolutionQuality, isRecommend);
                this.ShowTargetFrameRate(setting.TargetFrameRate, isRecommend);
                this.ShowPostFX(setting, isRecommend);
                this.ShowReflection(setting.VolatileSetting.UseReflection, isRecommend);
                this.ShowDynamicBone(setting.VolatileSetting.UseDynamicBone, isRecommend);
                this.ShowPostFXGrade(setting.VolatileSetting.PostFXGrade, isRecommend, setting.VolatileSetting.UsePostFX);
                this.ShowHDR(setting.VolatileSetting.UseHDR, isRecommend, setting.VolatileSetting.UsePostFX);
                this.ShowAA(setting.VolatileSetting.UseFXAA, isRecommend, setting.VolatileSetting.UsePostFX);
                this.ShowDistortion(setting.VolatileSetting.UseDistortion, isRecommend, setting.VolatileSetting.UsePostFX);
            }
        }

        private void ShowHDR(bool useHDR, bool isRecommend, bool usePostFX)
        {
            this.ShowThirdLineElement(base.transform.Find("Content/DetailSetting/ThirdLine/Content/HDR"), isRecommend, usePostFX, useHDR);
        }

        private void ShowPersonalConfig()
        {
            this.EnableAllConfigBtns(true);
            this.ShowPersonalGradeInfo();
            if (!this._modifiedPersonalSetting.IsUserDefinedGrade || !this._modifiedPersonalSetting.IsUserDefinedVolatile)
            {
                if (this._modifiedPersonalSetting.IsUserDefinedGrade || this._modifiedPersonalSetting.IsUserDefinedVolatile)
                {
                    if (this._modifiedPersonalSetting.IsUserDefinedGrade)
                    {
                        this.ShowRecommendCompleteConfig(this._modifiedPersonalSetting.RecommendGrade);
                    }
                    else
                    {
                        this.ShowGraphicsSettingConfig(this._modifiedPersonalSetting, false);
                    }
                }
                else
                {
                    ConfigGraphicsSetting graphicsPersonalSettingConfig = GraphicsSettingData.GetGraphicsPersonalSettingConfig(this._modifiedPersonalSetting);
                    this.ShowGraphicsSettingConfig(graphicsPersonalSettingConfig, false);
                }
            }
        }

        private void ShowPersonalGradeInfo()
        {
            for (int i = 0; i < (this.grades.Length - 1); i++)
            {
                Transform transform = this.grades[i];
                if (transform.gameObject.activeSelf)
                {
                    transform.FindChild("Blue").gameObject.SetActive(true);
                    transform.FindChild("Check").gameObject.SetActive(false);
                    this.processes[i].gameObject.SetActive(false);
                }
            }
            if (!this.isInLevelSimpleSetting)
            {
                Transform transform2 = this.grades[this.grades.Length - 1];
                transform2.FindChild("Blue").gameObject.SetActive(false);
                transform2.FindChild("Check").gameObject.SetActive(true);
                this.processes[this.grades.Length - 1].gameObject.SetActive(true);
            }
        }

        private void ShowPostFX(ConfigGraphicsSetting setting, bool isRecommend)
        {
            this.ShowSecondLineElement(base.transform.Find("Content/DetailSetting/SecondLine/PostFX"), isRecommend, setting.VolatileSetting.UsePostFX);
            this.ShowPostFXGrade(setting.VolatileSetting.PostFXGrade, isRecommend, setting.VolatileSetting.UsePostFX);
            this.ShowHDR(setting.VolatileSetting.UseHDR, isRecommend, setting.VolatileSetting.UsePostFX);
            this.ShowAA(setting.VolatileSetting.UseFXAA, isRecommend, setting.VolatileSetting.UsePostFX);
            this.ShowDistortion(setting.VolatileSetting.UseDistortion, isRecommend, setting.VolatileSetting.UsePostFX);
        }

        private void ShowPostFXGrade(PostEffectQualityGrade postFXGrade, bool isRecommend, bool usePostFX)
        {
            this.ShowThirdLineElement(base.transform.Find("Content/DetailSetting/ThirdLine/Content/PostFXQuality"), isRecommend, usePostFX, postFXGrade == PostEffectQualityGrade.High);
        }

        private void ShowRecommendCompleteConfig(GraphicsRecommendGrade grade)
        {
            this.ShowRecommendGradeInfo(grade);
            ConfigGraphicsSetting graphicsRecommendCompleteConfig = GraphicsSettingData.GetGraphicsRecommendCompleteConfig(grade);
            this.ShowGraphicsSettingConfig(graphicsRecommendCompleteConfig, true);
        }

        private void ShowRecommendGradeInfo(GraphicsRecommendGrade grade)
        {
            int index = (int) grade;
            for (int i = 0; i < this.grades.Length; i++)
            {
                if (i != index)
                {
                    Transform transform = this.grades[i];
                    transform.FindChild("Blue").gameObject.SetActive(true);
                    transform.FindChild("Check").gameObject.SetActive(false);
                    this.processes[i].gameObject.SetActive(false);
                }
            }
            Transform transform2 = this.grades[index];
            transform2.FindChild("Blue").gameObject.SetActive(false);
            transform2.FindChild("Check").gameObject.SetActive(true);
            this.processes[index].gameObject.SetActive(true);
        }

        private void ShowRecommendMark()
        {
            GraphicsRecommendGrade graphicsRecommendGrade = GraphicsSettingData.GetGraphicsRecommendGrade();
            for (int i = 0; i < (this.grades.Length - 1); i++)
            {
                Transform transform = this.grades[i];
                transform.FindChild("Recommend").gameObject.SetActive(false);
            }
            Transform transform2 = this.grades[(int) graphicsRecommendGrade];
            transform2.FindChild("Recommend").gameObject.SetActive(true);
        }

        private void ShowReflection(bool useReflection, bool isRecommend)
        {
            this.ShowSecondLineElement(base.transform.Find("Content/DetailSetting/SecondLine/Reflection"), isRecommend, useReflection);
        }

        private void ShowResolution(ResolutionQualityGrade resolutionGrade, bool isRecommend)
        {
            Transform transform = base.transform.Find("Content/DetailSetting/FirstLine/Resolution");
            Transform transform2 = transform.Find("Mark");
            transform2.FindChild("Enable").gameObject.SetActive(!isRecommend);
            transform2.FindChild("Disable").gameObject.SetActive(isRecommend);
            Transform transform3 = transform.FindChild("Choice/Low");
            Transform transform4 = transform.FindChild("Choice/Middle");
            Transform transform5 = transform.FindChild("Choice/High");
            Transform transform6 = transform.FindChild("Label");
            Transform transform7 = transform3.FindChild("Text");
            Transform transform8 = transform4.FindChild("Text");
            Transform transform9 = transform5.FindChild("Text");
            if (isRecommend)
            {
                transform3.FindChild("Blue").gameObject.SetActive(false);
                transform3.FindChild("Grey").gameObject.SetActive(false);
                transform3.FindChild("Disable").gameObject.SetActive(true);
                transform4.FindChild("Blue").gameObject.SetActive(false);
                transform4.FindChild("Grey").gameObject.SetActive(false);
                transform4.FindChild("Disable").gameObject.SetActive(true);
                transform5.FindChild("Blue").gameObject.SetActive(false);
                transform5.FindChild("Grey").gameObject.SetActive(false);
                transform5.FindChild("Disable").gameObject.SetActive(true);
                transform6.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform7.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform8.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform9.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform3.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
                transform4.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
                transform5.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
            }
            else
            {
                transform3.FindChild("Disable").gameObject.SetActive(false);
                transform4.FindChild("Disable").gameObject.SetActive(false);
                transform5.FindChild("Disable").gameObject.SetActive(false);
                transform6.GetComponent<Text>().color = Color.white;
                transform7.GetComponent<Text>().color = Color.white;
                transform8.GetComponent<Text>().color = Color.white;
                transform9.GetComponent<Text>().color = Color.white;
                transform3.FindChild("Check").GetComponent<Image>().color = Color.white;
                transform4.FindChild("Check").GetComponent<Image>().color = Color.white;
                transform5.FindChild("Check").GetComponent<Image>().color = Color.white;
            }
            if (resolutionGrade == ResolutionQualityGrade.High)
            {
                transform3.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
                transform4.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
                transform5.FindChild("Button").GetComponent<Button>().interactable = false;
                if (!isRecommend)
                {
                    transform3.FindChild("Blue").gameObject.SetActive(false);
                    transform3.FindChild("Grey").gameObject.SetActive(true);
                    transform4.FindChild("Blue").gameObject.SetActive(false);
                    transform4.FindChild("Grey").gameObject.SetActive(true);
                    transform5.FindChild("Blue").gameObject.SetActive(true);
                    transform5.FindChild("Grey").gameObject.SetActive(false);
                }
                transform3.FindChild("Check").gameObject.SetActive(false);
                transform4.FindChild("Check").gameObject.SetActive(false);
                transform5.FindChild("Check").gameObject.SetActive(true);
            }
            else if (resolutionGrade == ResolutionQualityGrade.Middle)
            {
                transform3.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
                transform4.FindChild("Button").GetComponent<Button>().interactable = false;
                transform5.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
                if (!isRecommend)
                {
                    transform3.FindChild("Blue").gameObject.SetActive(false);
                    transform3.FindChild("Grey").gameObject.SetActive(true);
                    transform4.FindChild("Blue").gameObject.SetActive(true);
                    transform4.FindChild("Grey").gameObject.SetActive(false);
                    transform5.FindChild("Blue").gameObject.SetActive(false);
                    transform5.FindChild("Grey").gameObject.SetActive(true);
                }
                transform3.FindChild("Check").gameObject.SetActive(false);
                transform4.FindChild("Check").gameObject.SetActive(true);
                transform5.FindChild("Check").gameObject.SetActive(false);
            }
            else
            {
                transform3.FindChild("Button").GetComponent<Button>().interactable = false;
                transform4.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
                transform5.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
                if (!isRecommend)
                {
                    transform5.FindChild("Blue").gameObject.SetActive(false);
                    transform5.FindChild("Grey").gameObject.SetActive(true);
                    transform4.FindChild("Blue").gameObject.SetActive(false);
                    transform4.FindChild("Grey").gameObject.SetActive(true);
                    transform3.FindChild("Blue").gameObject.SetActive(true);
                    transform3.FindChild("Grey").gameObject.SetActive(false);
                }
                transform5.FindChild("Check").gameObject.SetActive(false);
                transform4.FindChild("Check").gameObject.SetActive(false);
                transform3.FindChild("Check").gameObject.SetActive(true);
            }
        }

        private void ShowSecondLineElement(Transform elementTransform, bool isRecommend, bool use)
        {
            Transform transform = elementTransform.Find("Mark");
            transform.FindChild("Enable").gameObject.SetActive(!isRecommend);
            transform.FindChild("Disable").gameObject.SetActive(isRecommend);
            Transform transform2 = elementTransform.Find("Choice/On");
            Transform transform3 = elementTransform.Find("Choice/Off");
            Transform transform4 = elementTransform.FindChild("Label");
            Transform transform5 = transform2.FindChild("Text");
            Transform transform6 = transform3.FindChild("Text");
            if (isRecommend)
            {
                transform2.FindChild("Blue").gameObject.SetActive(false);
                transform2.FindChild("Grey").gameObject.SetActive(false);
                transform2.FindChild("Disable").gameObject.SetActive(true);
                transform3.FindChild("Grey").gameObject.SetActive(false);
                transform3.FindChild("Disable").gameObject.SetActive(true);
                transform4.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform5.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform6.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
            }
            else
            {
                transform2.FindChild("Blue").gameObject.SetActive(use);
                transform2.FindChild("Grey").gameObject.SetActive(!use);
                transform3.FindChild("Grey").gameObject.SetActive(!use);
                transform2.FindChild("Disable").gameObject.SetActive(false);
                transform3.FindChild("Disable").gameObject.SetActive(false);
                transform4.GetComponent<Text>().color = Color.white;
                transform5.GetComponent<Text>().color = Color.white;
                transform6.GetComponent<Text>().color = Color.white;
            }
            transform2.gameObject.SetActive(use);
            transform3.gameObject.SetActive(!use);
            transform2.GetComponent<Button>().interactable = !isRecommend && use;
            transform3.GetComponent<Button>().interactable = !isRecommend && !use;
        }

        private void ShowTargetFrameRate(int targetFrameRate, bool isRecommend)
        {
            this.ShowFirstLineElement(base.transform.Find("Content/DetailSetting/FirstLine/FrameLimit"), isRecommend, targetFrameRate == 60);
        }

        private void ShowThirdLineElement(Transform elementTransform, bool isRecommend, bool usePostFX, bool use)
        {
            Transform transform = elementTransform.FindChild("Text");
            Transform transform2 = elementTransform.FindChild("Check");
            if (isRecommend)
            {
                elementTransform.FindChild("Blue").gameObject.SetActive(false);
                elementTransform.FindChild("Grey").gameObject.SetActive(false);
                elementTransform.FindChild("Disable").gameObject.SetActive(true);
                transform.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform2.GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
            }
            else
            {
                elementTransform.FindChild("Blue").gameObject.SetActive(usePostFX && use);
                elementTransform.FindChild("Grey").gameObject.SetActive(!usePostFX || !use);
                elementTransform.FindChild("Disable").gameObject.SetActive(false);
                transform.GetComponent<Text>().color = Color.white;
                transform2.GetComponent<Image>().color = Color.white;
            }
            transform2.gameObject.SetActive(use);
            elementTransform.FindChild("Button").GetComponent<Button>().interactable = !isRecommend && usePostFX;
        }

        public void SwitchEcoMode()
        {
            GraphicsSettingData.CopyPersonalGraphicsConfig(!this._modifiedPersonalSetting.IsEcoMode, ref this._modifiedPersonalSetting);
            GraphicsSettingData.SavePersonalConfigIgnoreContrast(this._modifiedPersonalSetting);
            this.ResetView();
            string textID = !this._modifiedPersonalSetting.IsEcoMode ? "Menu_SettingEcoModeOffTip" : "Menu_SettingEcoModeOnTip";
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText(textID, new object[0]), 2f), UIType.Any);
        }
    }
}

