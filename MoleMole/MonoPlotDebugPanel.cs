namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoPlotDebugPanel : MonoBehaviour
    {
        private int _currentPlotID = 0x4e21;
        private const int DEFAULT_PLOT_ID = 0x4e21;

        private void Awake()
        {
        }

        public void OnClosePlotBtnClick()
        {
            base.transform.gameObject.SetActive(false);
        }

        public void OnShowPlotPanelClick()
        {
            this.SetupView();
            base.transform.gameObject.SetActive(true);
        }

        public void SetupView()
        {
        }

        public void ShowPlot()
        {
            Text component = base.transform.Find("InputField/Text").GetComponent<Text>();
            if (component != null)
            {
                if (!int.TryParse(component.text, out this._currentPlotID) || (this._currentPlotID < 0x4e21))
                {
                    this._currentPlotID = 0x4e21;
                }
                if ((PlotMetaDataReader.TryGetPlotMetaDataByKey(this._currentPlotID) != null) && !Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
                {
                    bool isOn = base.transform.Find("LerpInToggle").GetComponent<Toggle>().isOn;
                    bool exitLerp = base.transform.Find("LerpOutToggle").GetComponent<Toggle>().isOn;
                    Singleton<CameraManager>.Instance.GetMainCamera().PlayStoryCameraState(this._currentPlotID, isOn, exitLerp, true, true, false);
                    base.transform.gameObject.SetActive(false);
                }
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
        }
    }
}

