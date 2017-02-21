namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoDialogHeightGrow : MonoBehaviour
    {
        private RectTransform.Axis _axis;
        private int _currentStep;
        private float _delayTimer;
        private Action _growEnd;
        private bool _playGrowAnimation;
        private RectTransform _rectTrans;
        private float _targetSize;
        public RectTransform contentTrans;
        public float delayTime;
        public MovementDirection direction;
        public float speed;

        private void Init()
        {
            this._rectTrans = base.transform.GetComponent<RectTransform>();
            this._axis = (this.direction != MovementDirection.Vertical) ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
            if (this.contentTrans == null)
            {
                this.contentTrans = base.transform.Find("Content").GetComponent<RectTransform>();
                if (this.contentTrans != null)
                {
                }
            }
            this._targetSize = this.contentTrans.rect.height;
            if (this.direction == MovementDirection.Horizontal)
            {
                this._targetSize = this.contentTrans.rect.width;
            }
            this._rectTrans.SetSizeWithCurrentAnchors(this._axis, 0f);
            this._delayTimer = 0f;
            base.transform.GetComponent<CanvasGroup>().alpha = 0f;
        }

        public void PlayGrow(Action growEnd = null)
        {
            this._playGrowAnimation = true;
            this._currentStep = 0;
            this._growEnd = growEnd;
            this.Init();
        }

        private void Update()
        {
            if (this._playGrowAnimation)
            {
                float height = this._rectTrans.rect.height;
                if (this.direction == MovementDirection.Horizontal)
                {
                    height = this._rectTrans.rect.width;
                }
                if ((this._currentStep >= 3) && (height >= this._targetSize))
                {
                    this._playGrowAnimation = false;
                    if (this._growEnd != null)
                    {
                        this._growEnd();
                    }
                }
                switch (this._currentStep)
                {
                    case 0:
                        this.Init();
                        if (base.transform.GetComponent<ContentSizeFitter>() != null)
                        {
                            base.transform.GetComponent<ContentSizeFitter>().enabled = false;
                        }
                        this._currentStep++;
                        break;

                    case 1:
                        if (this.contentTrans.GetComponent<ContentSizeFitter>() != null)
                        {
                            this.contentTrans.GetComponent<ContentSizeFitter>().enabled = false;
                        }
                        if (this.contentTrans.GetComponent<VerticalLayoutGroup>() != null)
                        {
                            this.contentTrans.GetComponent<VerticalLayoutGroup>().enabled = false;
                        }
                        this._currentStep++;
                        break;

                    case 2:
                    {
                        IEnumerator enumerator = this.contentTrans.transform.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                Transform current = (Transform) enumerator.Current;
                                current.SetLocalPositionX(current.localPosition.x + this.contentTrans.rect.width);
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
                        this._currentStep++;
                        break;
                    }
                    case 3:
                        if (this._delayTimer >= this.delayTime)
                        {
                            base.transform.GetComponent<CanvasGroup>().alpha = 1f;
                            this._rectTrans.SetSizeWithCurrentAnchors(this._axis, height + (this.speed * Time.deltaTime));
                            break;
                        }
                        this._delayTimer += Time.deltaTime;
                        break;
                }
            }
        }
    }
}

