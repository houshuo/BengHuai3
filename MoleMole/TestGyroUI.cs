namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class TestGyroUI : MonoBehaviour
    {
        private Gyroscope _gyro;
        public Text attitudeText;
        public Text gravityText;
        public Text rotationRateText;

        public void Start()
        {
            this._gyro = Input.gyro;
            this._gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
        }

        public void Update()
        {
            if (this._gyro != null)
            {
                this.gravityText.text = this._gyro.gravity.ToString();
                this.rotationRateText.text = this._gyro.rotationRate.ToString();
                this.attitudeText.text = this._gyro.attitude.ToString() + "\n" + this._gyro.attitude.eulerAngles.ToString();
            }
        }
    }
}

