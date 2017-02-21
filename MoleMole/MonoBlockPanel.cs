namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoBlockPanel : MonoBehaviour
    {
        private float _intervalTimer;
        private bool _isCounting;
        private float INTERVAL_SPAN = 3f;

        public void SetTimeSpanTakeEffect(float timeSpan)
        {
            base.gameObject.SetActive(true);
            this.INTERVAL_SPAN = timeSpan;
            this._intervalTimer = 0f;
            this._isCounting = true;
        }

        private void Update()
        {
            if (this._isCounting)
            {
                this._intervalTimer += Time.deltaTime;
                if (this._intervalTimer > this.INTERVAL_SPAN)
                {
                    this._intervalTimer = 0f;
                    this._isCounting = false;
                    base.gameObject.SetActive(false);
                }
            }
        }
    }
}

