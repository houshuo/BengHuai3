namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class DragObject : MonoBehaviour, IEventSystemHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        private const float _acceleration = 400f;
        private Vector2 _beginDragPoint;
        private Vector2 _beginPoint;
        private float _boundBackDelta;
        private float _boundBackTimer;
        private float _fadeOutTimer;
        private bool _isIdentifySucc;
        private StorageDataItemBase _item;
        private float[] _sequenceOffset;
        private Status _status;
        private Vector2 _targetPos;
        private const float BOUND_BACK_TIME = 0.2f;
        private const float defaultMaskHeight = 609f;
        public RectTransform dragObject;
        private const float dragObjectBeginY = 241f;
        public RectTransform[] dragSequence;
        private float dragSpeed;
        private const float FADE_OUT_TIME = 0.3f;
        private const float maskHeightOrig = 720f;
        public RectTransform maskRect;
        private const float maxDeltaY = 300f;
        private const float maxDragSpeed = 600f;
        public Transform pageTrans;
        public string successAudioName;

        public void Init(StorageDataItemBase item)
        {
            this._item = item;
            this._isIdentifySucc = false;
            this._status = Status.Default;
            this.SetInfoActive(true);
            this.pageTrans.Find("Info/IdentifyNotice").gameObject.SetActive(false);
            this.maskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 720f);
            this.dragObject.anchoredPosition = new Vector2(this.dragObject.anchoredPosition.x, 241f);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetPlayerStatusWidgetDisplay, true));
        }

        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            this._beginPoint = this.dragObject.anchoredPosition;
            this._beginDragPoint = pointerEventData.position;
            this._sequenceOffset = new float[this.dragSequence.Length];
            for (int i = 0; i < this.dragSequence.Length; i++)
            {
                this._sequenceOffset[i] = this.dragSequence[i].anchoredPosition.y;
            }
            this.SetInfoActive(false);
            this.pageTrans.Find("Info/IdentifyNotice").gameObject.SetActive(true);
            this.pageTrans.Find("Info/Figure/PrefContainer").gameObject.SetActive(true);
            this.UpdateImageByDelta(0f);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetPlayerStatusWidgetDisplay, false));
            this._status = Status.Drag;
            this._targetPos = pointerEventData.position;
            this.dragSpeed = 0f;
        }

        public void OnDrag(PointerEventData pointerEventData)
        {
            if (this._status == Status.Drag)
            {
                this._targetPos = pointerEventData.position;
            }
        }

        public void OnEndDrag(PointerEventData pointerEventData)
        {
            if (this._status == Status.Drag)
            {
                float num = pointerEventData.position.y - this._beginDragPoint.y;
                if (num < 0f)
                {
                    this._status = Status.BoundBack;
                    this._boundBackDelta = Mathf.Abs((float) (this.dragObject.anchoredPosition.y - 241f));
                    this._boundBackTimer = 0f;
                }
                else
                {
                    this.Reset();
                }
            }
        }

        public void OnIdentifyStigmataAffixSucc()
        {
            this._isIdentifySucc = true;
        }

        private void Reset()
        {
            this._status = Status.Default;
            this.dragObject.anchoredPosition = new Vector2(this.dragObject.anchoredPosition.x, 241f);
            this.SetInfoActive(true);
            this.pageTrans.Find("Info/IdentifyNotice").gameObject.SetActive(false);
            this.pageTrans.Find("Info/Figure/PrefContainer").gameObject.SetActive(false);
            this.maskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 720f);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetPlayerStatusWidgetDisplay, true));
        }

        public void SetInfoActive(bool active)
        {
            List<string> list = new List<string> { "Info/Content", "Attributes", "Skills", "Lv", "ActionBtns", "Info/IdentifyBtn/Text" };
            int num = 0;
            int count = list.Count;
            while (num < count)
            {
                this.pageTrans.Find(list[num]).gameObject.SetActive(active);
                num++;
            }
        }

        public void Update()
        {
            if (this._status == Status.Drag)
            {
                BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
                if (mainCanvas != null)
                {
                    float scaleFactor = mainCanvas.GetComponent<Canvas>().scaleFactor;
                    float a = ((this._targetPos.y - this._beginDragPoint.y) / scaleFactor) - (this.dragObject.anchoredPosition.y - this._beginPoint.y);
                    if (a < 0f)
                    {
                        this.dragSpeed += Time.unscaledDeltaTime * 400f;
                        this.dragSpeed = Mathf.Min(600f, this.dragSpeed);
                    }
                    else
                    {
                        this.dragSpeed = 0f;
                    }
                    float y = Mathf.Max(a, -this.dragSpeed * Time.unscaledDeltaTime) + this.dragObject.anchoredPosition.y;
                    if (y > 241f)
                    {
                        y = 241f;
                    }
                    this.dragObject.anchoredPosition = new Vector2(this.dragObject.anchoredPosition.x, y);
                    for (int i = 0; i < this._sequenceOffset.Length; i++)
                    {
                        float num5 = (a >= 0f) ? 0f : ((1f - (1f / Mathf.Pow(2f, (float) (i + 1)))) * a);
                        this.dragSequence[i].anchoredPosition = new Vector2(this.dragSequence[i].anchoredPosition.x, this._sequenceOffset[i] + num5);
                    }
                    this.UpdateImageByDelta(y - 241f);
                    if (y <= -59f)
                    {
                        Singleton<NetworkManager>.Instance.RequestIdentifyStigmataAffix(this._item);
                        this._status = Status.Identify;
                        this._fadeOutTimer = 0f;
                        this.pageTrans.Find("Info/Figure/IdentifySuccEffect").GetComponent<ParticleSystem>().Play();
                        if (!string.IsNullOrEmpty(this.successAudioName))
                        {
                            Singleton<WwiseAudioManager>.Instance.Post(this.successAudioName, null, null, null);
                        }
                    }
                }
            }
            else if (this._status == Status.Identify)
            {
                this._fadeOutTimer += Time.unscaledDeltaTime;
                if (this._fadeOutTimer <= 0.3f)
                {
                    float delta = -Mathf.Lerp(300f, 720f, this._fadeOutTimer / 0.3f);
                    this.dragObject.anchoredPosition = new Vector2(this.dragObject.anchoredPosition.x, 241f + delta);
                    for (int j = 0; j < this._sequenceOffset.Length; j++)
                    {
                        this.dragSequence[j].anchoredPosition = new Vector2(this.dragSequence[j].anchoredPosition.x, this._sequenceOffset[j]);
                    }
                    this.UpdateImageByDelta(delta);
                }
                else if (this._isIdentifySucc)
                {
                    this.Reset();
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RefreshStigmataDetailView, null));
                }
            }
            else if (this._status == Status.BoundBack)
            {
                this._boundBackTimer += Time.unscaledDeltaTime;
                if (this._boundBackTimer <= 0.2f)
                {
                    float num8 = -Mathf.Lerp(this._boundBackDelta, 0f, this._boundBackTimer / 0.2f);
                    this.dragObject.anchoredPosition = new Vector2(this.dragObject.anchoredPosition.x, 241f + num8);
                    for (int k = 0; k < this._sequenceOffset.Length; k++)
                    {
                        this.dragSequence[k].anchoredPosition = new Vector2(this.dragSequence[k].anchoredPosition.x, this._sequenceOffset[k]);
                    }
                    this.UpdateImageByDelta(num8);
                }
                else
                {
                    this.Reset();
                }
            }
        }

        public void UpdateImageByDelta(float delta)
        {
            this.maskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 609f + delta);
        }

        private enum Status
        {
            Default,
            Drag,
            BoundBack,
            Identify
        }
    }
}

