namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class NewbieHighlightInfo
    {
        private List<MonoBehaviour> _enabledScripts;
        private List<EventTrigger.Entry> _eventTriggers;
        private Transform _highlightTrans;
        private List<string> _ignoreScriptList = new List<string>();
        private NewbieDialogContext _newbieDialogContext;
        public Transform originTrans;

        public NewbieHighlightInfo(NewbieDialogContext newbieDialogContext, Transform originTrans, Transform newParent, bool disableHighlightInvoke, Action<BaseEventData> preCallback, Action<BaseEventData> pointerDownCallback, Action<BaseEventData> pointerUpCallback)
        {
            this._ignoreScriptList.Add("ImageForSmoothMask");
            this._ignoreScriptList.Add("MonoButtonWwiseEvent");
            this._newbieDialogContext = newbieDialogContext;
            this.originTrans = originTrans;
            GameObject obj2 = this.CopyOriginTransform(originTrans);
            this._highlightTrans = obj2.transform;
            this._highlightTrans.SetParent(newParent);
            this._highlightTrans.SetSiblingIndex(1);
            this._highlightTrans.localScale = Vector3.one;
            this.BindOriginTransformHelper();
            this.BindViewCallback(disableHighlightInvoke, preCallback, pointerDownCallback, pointerUpCallback);
        }

        private void BindOriginTransformHelper()
        {
            if (this.originTrans.gameObject.GetComponent<MonoNewbieOriginTransformHelper>() == null)
            {
                this.originTrans.gameObject.AddComponent<MonoNewbieOriginTransformHelper>().newbieDialogContext = this._newbieDialogContext;
            }
        }

        private void BindViewCallback(bool disableHighlightInvoke, Action<BaseEventData> preCallback, Action<BaseEventData> pointerDownCallback, Action<BaseEventData> pointerUpCallback)
        {
            Button componentInChildren = this._highlightTrans.GetComponentInChildren<Button>();
            if (componentInChildren != null)
            {
                componentInChildren.onClick = new Button.ButtonClickedEvent();
            }
            this.BindViewCallback(disableHighlightInvoke, (componentInChildren == null) ? this._highlightTrans : componentInChildren.gameObject.transform, preCallback, pointerDownCallback, pointerUpCallback, componentInChildren != null);
        }

        private void BindViewCallback(bool disableHighlightInvoke, Transform trans, Action<BaseEventData> preCallback, Action<BaseEventData> pointerDownCallback, Action<BaseEventData> pointerUpCallback, bool isButton)
        {
            <BindViewCallback>c__AnonStoreyE3 ye = new <BindViewCallback>c__AnonStoreyE3 {
                preCallback = preCallback,
                pointerDownCallback = pointerDownCallback,
                isButton = isButton,
                pointerUpCallback = pointerUpCallback,
                <>f__this = this
            };
            if (trans != null)
            {
                EventTrigger component = trans.gameObject.GetComponent<EventTrigger>();
                if (component != null)
                {
                    UnityEngine.Object.Destroy(component);
                }
                MonoEventTrigger trigger2 = trans.gameObject.GetComponent<MonoEventTrigger>();
                if (trigger2 == null)
                {
                    trigger2 = trans.gameObject.AddComponent<MonoEventTrigger>();
                }
                List<EventTrigger.Entry> list = new List<EventTrigger.Entry>();
                EventTrigger.Entry item = new EventTrigger.Entry {
                    eventID = EventTriggerType.PointerDown
                };
                item.callback.AddListener(new UnityAction<BaseEventData>(ye.<>m__113));
                if (!disableHighlightInvoke)
                {
                    item.callback.AddListener(new UnityAction<BaseEventData>(ye.<>m__114));
                }
                item.callback.AddListener(new UnityAction<BaseEventData>(ye.<>m__115));
                list.Add(item);
                EventTrigger.Entry entry2 = new EventTrigger.Entry {
                    eventID = EventTriggerType.PointerUp
                };
                if (!disableHighlightInvoke)
                {
                    entry2.callback.AddListener(new UnityAction<BaseEventData>(ye.<>m__116));
                }
                entry2.callback.AddListener(new UnityAction<BaseEventData>(ye.<>m__117));
                list.Add(entry2);
                trigger2.triggers = list;
                this._eventTriggers = list;
            }
        }

        private GameObject CopyOriginTransform(Transform originTrans)
        {
            this._enabledScripts = new List<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in originTrans.gameObject.GetComponentsInChildren<MonoBehaviour>())
            {
                if (((behaviour.GetType().Namespace == "MoleMole") && behaviour.enabled) && !this._ignoreScriptList.Contains(behaviour.GetType().Name))
                {
                    this._enabledScripts.Add(behaviour);
                    behaviour.enabled = false;
                }
            }
            Vector3 position = new Vector3(originTrans.position.x, originTrans.position.y, originTrans.position.z);
            MonoBehaviour[] components = originTrans.parent.gameObject.GetComponents<MonoBehaviour>();
            List<MonoBehaviour> list = new List<MonoBehaviour>();
            foreach (MonoBehaviour behaviour2 in components)
            {
                if ((behaviour2 is LayoutGroup) && behaviour2.enabled)
                {
                    list.Add(behaviour2);
                    behaviour2.enabled = false;
                }
            }
            GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(originTrans.gameObject, position, originTrans.rotation);
            foreach (MonoBehaviour behaviour3 in this._enabledScripts)
            {
                behaviour3.enabled = true;
            }
            foreach (MonoBehaviour behaviour4 in list)
            {
                behaviour4.enabled = true;
            }
            Button componentInChildren = obj2.transform.GetComponentInChildren<Button>();
            if (componentInChildren != null)
            {
                componentInChildren.interactable = true;
            }
            Image component = obj2.transform.GetComponent<Image>();
            if (component != null)
            {
                component.set_raycastTarget(true);
            }
            foreach (Image image2 in obj2.transform.GetComponentsInChildren<Image>())
            {
                image2.set_raycastTarget(true);
            }
            return obj2;
        }

        private void InvokeOriginButtonEvent()
        {
            if ((this.originTrans != null) && (this.originTrans.gameObject != null))
            {
                Button[] componentsInChildren = this.originTrans.gameObject.GetComponentsInChildren<Button>(true);
                if (componentsInChildren.Length > 0)
                {
                    componentsInChildren[0].onClick.Invoke();
                    MonoButtonWwiseEvent component = componentsInChildren[0].GetComponent<MonoButtonWwiseEvent>();
                    if (component != null)
                    {
                        component.OnPointerClick(null);
                    }
                }
                this.InvokeOriginPointerUpEvent(null);
            }
        }

        private void InvokeOriginPanelEvent(BaseEventData data = null)
        {
            if ((this.originTrans != null) && (this.originTrans.gameObject != null))
            {
                ExecuteEvents.Execute<IPointerClickHandler>(this.originTrans.gameObject, data, ExecuteEvents.pointerClickHandler);
                this.InvokeOriginPointerUpEvent(null);
            }
        }

        private void InvokeOriginPointerDownEvent(PointerEventData eventData)
        {
            if ((this.originTrans != null) && (this._enabledScripts != null))
            {
                foreach (MonoBehaviour behaviour in this._enabledScripts)
                {
                    if (behaviour is IPointerDownHandler)
                    {
                        ((IPointerDownHandler) behaviour).OnPointerDown(eventData);
                    }
                }
            }
        }

        private void InvokeOriginPointerUpEvent(PointerEventData eventData)
        {
            if ((this.originTrans != null) && (this._enabledScripts != null))
            {
                foreach (MonoBehaviour behaviour in this._enabledScripts)
                {
                    if (behaviour is IPointerUpHandler)
                    {
                        ((IPointerUpHandler) behaviour).OnPointerUp(eventData);
                    }
                }
            }
        }

        public void Recover()
        {
            this.UnbindOriginTransformHelper();
            this.UnbindViewCallback();
            if ((this._highlightTrans != null) && (this._highlightTrans.gameObject != null))
            {
                UnityEngine.Object.Destroy(this._highlightTrans.gameObject);
            }
        }

        private void UnbindOriginTransformHelper()
        {
            MonoNewbieOriginTransformHelper component = this.originTrans.gameObject.GetComponent<MonoNewbieOriginTransformHelper>();
            if (component != null)
            {
                UnityEngine.Object.Destroy(component);
            }
        }

        private void UnbindViewCallback()
        {
            if (this._eventTriggers != null)
            {
                this._eventTriggers.Clear();
                this._eventTriggers = null;
            }
        }

        [CompilerGenerated]
        private sealed class <BindViewCallback>c__AnonStoreyE3
        {
            internal NewbieHighlightInfo <>f__this;
            internal bool isButton;
            internal Action<BaseEventData> pointerDownCallback;
            internal Action<BaseEventData> pointerUpCallback;
            internal Action<BaseEventData> preCallback;

            internal void <>m__113(BaseEventData evtData)
            {
                this.preCallback(evtData);
            }

            internal void <>m__114(BaseEventData evtData)
            {
                this.<>f__this.InvokeOriginPointerDownEvent((PointerEventData) evtData);
            }

            internal void <>m__115(BaseEventData evtData)
            {
                this.pointerDownCallback(evtData);
            }

            internal void <>m__116(BaseEventData evtData)
            {
                if (this.isButton)
                {
                    this.<>f__this.InvokeOriginButtonEvent();
                }
                else
                {
                    this.<>f__this.InvokeOriginPanelEvent(evtData);
                }
            }

            internal void <>m__117(BaseEventData evtData)
            {
                this.pointerUpCallback(evtData);
            }
        }

        private enum CallbackType
        {
            ButtonCallback,
            PanelCallback
        }
    }
}

