namespace MoleMole
{
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoRemainHP : MonoBehaviour
    {
        private EndlessAvatarHp _avatarHPData;
        private float _checkInterval = 5f;
        private float _timer;
        private bool _update;
        [SerializeField]
        private Action<bool> avatarDieCallBack;

        private void CheckAndSetupView()
        {
            this._timer = 0f;
            DateTime time = Singleton<EndlessModule>.Instance.CheckAvatarHPChanged(this._avatarHPData);
            if (time == DateTime.MinValue)
            {
                this._checkInterval = 300f;
            }
            else
            {
                if (time > TimeUtil.Now)
                {
                    TimeSpan span = (TimeSpan) (time - TimeUtil.Now);
                    if (span.TotalSeconds > 60.0)
                    {
                        this._checkInterval = 60f;
                        goto Label_0088;
                    }
                }
                this._checkInterval = 5f;
            }
        Label_0088:
            base.transform.Find("HPSlider/Slider").GetComponent<Image>().fillAmount = ((float) Mathf.Clamp((int) this._avatarHPData.get_hp_percent(), 0, 100)) / 100f;
            bool flag = this._avatarHPData.get_is_dieSpecified() && this._avatarHPData.get_is_die();
            if (this.avatarDieCallBack != null)
            {
                this.avatarDieCallBack(flag);
            }
            base.transform.Find("HPSlider/HpRecovery").gameObject.SetActive(this._avatarHPData.get_hp_percent() < 100);
        }

        public void SetAvatarHPData(EndlessAvatarHp avatarHPData, Action<bool> avatarDie = null)
        {
            this._avatarHPData = avatarHPData;
            this.avatarDieCallBack = avatarDie;
            this._update = true;
            this.CheckAndSetupView();
        }

        private void Update()
        {
            if (this._update)
            {
                this._timer += Time.deltaTime;
                if (this._timer > this._checkInterval)
                {
                    this.CheckAndSetupView();
                }
            }
        }
    }
}

