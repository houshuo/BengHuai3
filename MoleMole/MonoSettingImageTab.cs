namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoSettingImageTab : MonoBehaviour
    {
        private float _contrastDelta;
        private int _contrastShowMaxValue = 20;
        private int _contrastShowMinValue = -20;
        private Slider _contrastSlider;

        public bool CheckNeedSave()
        {
            return !GraphicsSettingData.IsEqualToPersonalContrastDelta(this._contrastDelta);
        }

        public void OnContrastDeltaChanged()
        {
            this._contrastDelta = ((this._contrastSlider.value - this._contrastShowMinValue) / 20f) - 1f;
            GraphicsSettingUtil.SetPostFXContrast(this._contrastDelta);
            this.ShowContrast((int) this._contrastSlider.value);
        }

        public void OnNoSaveBtnClick()
        {
            this.RecoverOriginState();
        }

        public void OnSaveBtnClick()
        {
            GraphicsSettingData.SavePersonalContrastDelta(this._contrastDelta);
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess", new object[0]), 2f), UIType.Any);
        }

        private void RecoverOriginState()
        {
            GraphicsSettingData.ApplyPersonalContrastDelta();
            GraphicsSettingData.CopyPersonalContrastDelta(ref this._contrastDelta);
            int showValue = ((int) ((((float) (this._contrastShowMaxValue - this._contrastShowMinValue)) / 2f) * (this._contrastDelta + 1f))) + this._contrastShowMinValue;
            this._contrastSlider.value = showValue;
            this.ShowContrast(showValue);
        }

        public void SetupView()
        {
            base.transform.FindChild("Content/RT/3dModel").GetComponent<MonoGammaSettingRenderImage>().SetupView();
            GraphicsSettingData.ApplyPersonalContrastDelta();
            this._contrastSlider = base.transform.FindChild("Content/Contrast/Slider").GetComponent<Slider>();
            this.RecoverOriginState();
        }

        private void ShowContrast(int showValue)
        {
            if (showValue == this._contrastShowMinValue)
            {
                this._contrastSlider.transform.FindChild("Fill Area/Fill").GetComponent<Image>().enabled = false;
            }
            else
            {
                this._contrastSlider.transform.FindChild("Fill Area/Fill").GetComponent<Image>().enabled = true;
            }
            this._contrastSlider.transform.FindChild("Handle Slide Area/Handle/Popup/PopupSmall/Text").GetComponent<Text>().text = showValue.ToString();
        }
    }
}

