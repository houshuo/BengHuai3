namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoMainMessages : MonoBehaviour
    {
        private Transform _currentMessage;
        private MainPageContext.PopShowMessage _currentMessageData;
        private float _messageHoldTimer;
        private float _messageHoldTimeSpan;
        private Action _moveEndCallBack;
        private Transform _preMessage;
        private const float DEFAULT_HOLD_HEIGHT = 10f;
        public float fadeSpeed;
        public GameObject messageGO;
        public float moveSpeed;
        private Status status;

        public void OnMessagePanelClick()
        {
            if ((this._currentMessage != null) && (this._currentMessageData != null))
            {
                if (this._currentMessageData.source == MainPageContext.MessageSource.Friend)
                {
                    if (this._currentMessageData.talkingUid != 0)
                    {
                        Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext((int) this._currentMessageData.talkingUid), UIType.Any);
                    }
                }
                else if (this._currentMessageData.source == MainPageContext.MessageSource.World)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext(), UIType.Any);
                }
                else if ((this._currentMessageData.source != MainPageContext.MessageSource.Guild) && (this._currentMessageData.source == MainPageContext.MessageSource.System))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext(), UIType.Any);
                }
            }
        }

        public void ShowMessage(MainPageContext.PopShowMessage message, Action moveEndCallBack = null)
        {
            if (this._currentMessage != null)
            {
                this._preMessage = this._currentMessage;
                this._preMessage.name = "preMessage";
            }
            this._currentMessage = UnityEngine.Object.Instantiate<GameObject>(this.messageGO).transform;
            this._currentMessage.SetParent(base.transform, false);
            this._currentMessageData = message;
            string prefabPath = "SpriteOutput/ChatTypeBackground/PicChatWorldBg";
            if ((this._currentMessageData != null) && (this._currentMessageData.source != MainPageContext.MessageSource.World))
            {
                if (this._currentMessageData.source == MainPageContext.MessageSource.System)
                {
                    prefabPath = "SpriteOutput/ChatTypeBackground/PicChatSystemBg";
                }
                else if (this._currentMessageData.source == MainPageContext.MessageSource.Guild)
                {
                    prefabPath = "SpriteOutput/ChatTypeBackground/PicChatGuildBg";
                }
                else if (this._currentMessageData.source == MainPageContext.MessageSource.Friend)
                {
                    prefabPath = "SpriteOutput/ChatTypeBackground/PicChatFriendBg";
                }
            }
            this._currentMessage.transform.Find("BG/Panel").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
            RectTransform component = this._currentMessage.GetComponent<RectTransform>();
            component.anchoredPosition = new Vector2(component.anchoredPosition.x, -(component.rect.height + 10f));
            component.transform.Find("Content/Text").GetComponent<Text>().text = message.message;
            component.transform.Find("Content/Text").GetComponent<Text>().supportRichText = true;
            component.transform.name = "currentMessage";
            this._moveEndCallBack = moveEndCallBack;
            this.status = Status.Move;
        }

        private void Start()
        {
            this._preMessage = null;
            this._currentMessage = null;
            this._currentMessageData = null;
            this._messageHoldTimeSpan = MiscData.Config.ChatConfig.PopMessageHoldDuration;
            this.status = Status.Idle;
        }

        private void Update()
        {
            switch (this.status)
            {
                case Status.Move:
                    if (this._preMessage != null)
                    {
                        RectTransform component = this._preMessage.GetComponent<RectTransform>();
                        component.anchoredPosition = new Vector2(component.anchoredPosition.x, component.anchoredPosition.y + (this.moveSpeed * Time.deltaTime));
                        float num = Mathf.Clamp((float) (component.GetComponent<CanvasGroup>().alpha - (this.fadeSpeed * Time.deltaTime)), (float) 0f, (float) 1f);
                        component.GetComponent<CanvasGroup>().alpha = num;
                        if (num < 0.1f)
                        {
                            UnityEngine.Object.Destroy(this._preMessage.gameObject);
                        }
                    }
                    if (this._currentMessage != null)
                    {
                        RectTransform transform2 = this._currentMessage.GetComponent<RectTransform>();
                        transform2.anchoredPosition = new Vector2(transform2.anchoredPosition.x, transform2.anchoredPosition.y + (this.moveSpeed * Time.deltaTime));
                        if (transform2.anchoredPosition.y >= 10f)
                        {
                            transform2.anchoredPosition = new Vector2(transform2.anchoredPosition.x, 10f);
                            if (this._preMessage != null)
                            {
                                UnityEngine.Object.Destroy(this._preMessage.gameObject);
                            }
                            this.status = Status.Hold;
                            this._messageHoldTimer = 0f;
                        }
                    }
                    if ((this._preMessage == null) && (this._currentMessage == null))
                    {
                        this.status = Status.Idle;
                    }
                    break;

                case Status.Hold:
                    this._messageHoldTimer += Time.deltaTime;
                    if (this._messageHoldTimer >= this._messageHoldTimeSpan)
                    {
                        this.status = Status.Move;
                        this._preMessage = this._currentMessage;
                        this._preMessage.name = "preMessage";
                        this._currentMessage = null;
                        this._currentMessageData = null;
                        if (this._moveEndCallBack != null)
                        {
                            this._moveEndCallBack();
                        }
                    }
                    break;
            }
        }

        public enum Status
        {
            Move,
            Hold,
            Idle
        }
    }
}

