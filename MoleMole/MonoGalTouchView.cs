namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoGalTouchView : MonoBehaviour
    {
        private int _animateStep;
        private float _animateTimer;
        private Animator _animator;
        private int _finalExp;
        private float _fromPer;
        private float _inactiveTimer;
        private float _lastRatio = -1f;
        private float _toPer;
        public Text additionalLabel;
        public GameObject additionalObject;
        public float animateTime;
        public GameObject[] fullLevelHideObjects;
        public Text goodFeelLabel;
        public GameObject[] heartObjects;
        public MonoMaskSlider maskSlider;
        public float postAnimateTime;
        public float preAnimateTime;
        public float sliderMax = 1f;
        public float sliderMin;
        public GameObject sliderView;
        public Text todayRemainFeel;

        public event Action Upgrade;

        private void Awake()
        {
            this._animator = base.GetComponent<Animator>();
            this._inactiveTimer = -1f;
        }

        public void Hide()
        {
            if (this._animator != null)
            {
                this._animator.Play("FeelHide");
            }
            this._inactiveTimer = 1f;
            this.shown = false;
        }

        public void PlaySliderAnimation(float from, float to)
        {
            if (this.sliderView.GetComponent<Image>() != null)
            {
                this._animateTimer = (this.preAnimateTime + this.animateTime) + this.postAnimateTime;
                this._animateStep = 1;
                this._fromPer = from;
                this._toPer = to;
                this.SetHeartLevel((int) from);
            }
        }

        public void SetGoodFeel(int val)
        {
            if (this.goodFeelLabel != null)
            {
                char[] separator = new char[] { '/' };
                string[] strArray = this.goodFeelLabel.text.Split(separator);
                this.goodFeelLabel.text = string.Format("{0}/{1}", val.ToString(), strArray[1]);
            }
        }

        private void SetGoodFeelText(float ratio)
        {
            int level = (int) ratio;
            float num2 = ratio - level;
            int num3 = GalTouchData.QueryLevelUpFeelNeed(level);
            int num4 = (int) (num3 * num2);
            this.goodFeelLabel.text = string.Format("{0}/{1}", (ratio != this._toPer) ? num4.ToString() : this._finalExp.ToString(), num3.ToString());
        }

        public void SetHeartLevel(int val)
        {
            if ((this.heartObjects == null) || (this.heartObjects.Length != 5))
            {
                Debug.LogWarning("[GalTouch] heartObjects of MonoGalTouchView is not set correctly");
            }
            else
            {
                val = Mathf.Clamp(val, 0, 5);
                for (int i = 0; i < 5; i++)
                {
                    this.heartObjects[i].SetActive(i < val);
                }
                int index = 0;
                int length = this.fullLevelHideObjects.Length;
                while (index < length)
                {
                    this.fullLevelHideObjects[index].SetActive(val < 5);
                    index++;
                }
            }
        }

        public void SetHintContent(string content)
        {
            if (this.todayRemainFeel != null)
            {
                this.todayRemainFeel.text = content;
            }
        }

        public void SetHintVisible(bool visible)
        {
            if (this.todayRemainFeel != null)
            {
                this.todayRemainFeel.enabled = visible;
            }
        }

        public void SetMaxGoodFeel(int val)
        {
            if (this.goodFeelLabel != null)
            {
                char[] separator = new char[] { '/' };
                string[] strArray = this.goodFeelLabel.text.Split(separator);
                this.goodFeelLabel.text = string.Format("{0}/{1}", strArray[0], val.ToString());
            }
        }

        private void SetSliderRatio(float ratio)
        {
            ratio = Mathf.Clamp(ratio, 0f, 1f);
            ratio = Mathf.Lerp(this.sliderMin, this.sliderMax, ratio);
            this.maskSlider.UpdateValue(ratio, 1f, 0f);
        }

        public void Show(float sliderFrom, float sliderTo, int finalExp, string additionalText)
        {
            base.gameObject.SetActive(true);
            if (this._animator != null)
            {
                this._animator.Play("FeelPopUp");
            }
            this._animateStep = 0;
            this._finalExp = finalExp;
            this.PlaySliderAnimation(sliderFrom, sliderTo);
            this._inactiveTimer = -1f;
            this.shown = true;
            int index = 0;
            int length = this.heartObjects.Length;
            while (index < length)
            {
                IEnumerator enumerator = this.heartObjects[index].transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        current.gameObject.SetActive(!this.heartObjects[index].activeSelf);
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
                index++;
            }
            if (this.additionalObject != null)
            {
                this.additionalObject.SetActive(!string.IsNullOrEmpty(additionalText));
            }
            if (this.additionalLabel != null)
            {
                this.additionalLabel.text = additionalText;
            }
        }

        private void Update()
        {
            if (this._inactiveTimer > 0f)
            {
                this._inactiveTimer -= Time.deltaTime;
                if (this._inactiveTimer <= 0f)
                {
                    this._inactiveTimer = -1f;
                    base.gameObject.SetActive(false);
                }
            }
            this.UpdateSliderAnimation();
        }

        private void UpdateSliderAnimation()
        {
            if (this._animateStep == 1)
            {
                this._animateTimer -= Time.deltaTime;
                float ratio = Mathf.Lerp(this._fromPer, this._toPer, ((this.postAnimateTime + this.animateTime) - this._animateTimer) / this.animateTime);
                this.SetGoodFeelText(ratio);
                if ((this._lastRatio > 0f) && (((int) ratio) > ((int) this._lastRatio)))
                {
                    this.UpgradeAction((int) ratio);
                }
                float num2 = ratio - ((int) ratio);
                this.SetSliderRatio(num2);
                if (this._animateTimer <= 0f)
                {
                    this.Hide();
                    this._animateStep = 2;
                }
                this._lastRatio = ratio;
            }
        }

        private void UpgradeAction(int level)
        {
            this.SetHeartLevel(level);
            if (this.Upgrade != null)
            {
                this.Upgrade();
            }
        }

        public bool shown { get; set; }
    }
}

