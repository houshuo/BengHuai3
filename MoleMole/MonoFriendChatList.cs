namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoFriendChatList : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        private float closePosY;
        public float fadeSpeed;
        public float moveSpeed;
        private const float OFFSET_Y = 34.5f;
        private float openPosY;
        private RectTransform rectTrans;
        public Status status;

        public void CloseFriendChatList()
        {
            this.status = Status.Closing;
            this.SetPosY(this.openPosY);
        }

        private float GetCurrentPosY()
        {
            return base.transform.GetComponent<RectTransform>().anchoredPosition.y;
        }

        private void OnClosed()
        {
            if (base.transform.parent.GetComponent<CanvasGroup>() != null)
            {
                base.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

        private void OnOpened()
        {
            if (base.transform.parent.GetComponent<CanvasGroup>() != null)
            {
                base.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }

        public void OpenFriendChatList()
        {
            this.status = Status.Opening;
            this.SetPosY(this.closePosY);
        }

        private void SetOpacity(float opacity)
        {
            this.canvasGroup.alpha = opacity;
        }

        private void SetPosY(float posY)
        {
            base.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(base.transform.GetComponent<RectTransform>().anchoredPosition.x, posY);
        }

        private void Start()
        {
            this.status = Status.Close;
            this.rectTrans = base.transform.GetComponent<RectTransform>();
            this.canvasGroup = base.transform.GetComponent<CanvasGroup>();
            this.openPosY = 0f;
            this.closePosY = -this.rectTrans.sizeDelta.y - 34.5f;
            this.moveSpeed = 1500f;
        }

        private void Update()
        {
            if (this.status == Status.Opening)
            {
                if (this.GetCurrentPosY() > this.openPosY)
                {
                    this.status = Status.Open;
                    this.SetPosY(this.openPosY);
                    this.OnOpened();
                }
                this.SetPosY(this.GetCurrentPosY() + (this.moveSpeed * Time.deltaTime));
            }
            if (this.status == Status.Closing)
            {
                if (this.GetCurrentPosY() < this.closePosY)
                {
                    this.status = Status.Close;
                    this.SetPosY(this.closePosY);
                    this.OnClosed();
                }
                this.SetPosY(this.GetCurrentPosY() - (this.moveSpeed * Time.deltaTime));
            }
            float t = (this.openPosY - this.GetCurrentPosY()) / (this.openPosY - this.closePosY);
            t = Mathf.Clamp((float) (t * t), (float) 0f, (float) 1f);
            float opacity = Mathf.Lerp(0.8f, 0f, t);
            this.SetOpacity(opacity);
        }

        public enum Status
        {
            Open,
            Close,
            Opening,
            Closing
        }
    }
}

