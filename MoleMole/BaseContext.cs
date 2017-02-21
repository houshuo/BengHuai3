namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public abstract class BaseContext
    {
        private List<UnityEventBase> _bindedEvents = new List<UnityEventBase>();
        protected Queue<Notify> _notifyQueue = new Queue<Notify>();
        public ContextPattern config;
        protected bool findViewSavedInScene;
        public UIType uiType;

        public BaseContext()
        {
            this.EnableTutorial = true;
        }

        private void AddContextIdentifier()
        {
            if (this.view.GetComponent<ContextIdentifier>() == null)
            {
                this.view.AddComponent<ContextIdentifier>().context = this;
            }
        }

        protected void BindViewCallback(Button button, UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
            this._bindedEvents.Add(button.onClick);
        }

        protected void BindViewCallback(Toggle toggle, UnityAction<bool> callback)
        {
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(callback);
            this._bindedEvents.Add(toggle.onValueChanged);
        }

        protected void BindViewCallback(Transform trans, EventTriggerType eventType, Action<BaseEventData> callback)
        {
            <BindViewCallback>c__AnonStoreyDE yde = new <BindViewCallback>c__AnonStoreyDE {
                callback = callback
            };
            MonoEventTrigger component = trans.gameObject.GetComponent<MonoEventTrigger>();
            if (component == null)
            {
                component = trans.gameObject.AddComponent<MonoEventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry {
                eventID = eventType
            };
            entry.callback.AddListener(new UnityAction<BaseEventData>(yde.<>m__109));
            component.AddTrigger(entry);
        }

        protected virtual void BindViewCallbacks()
        {
        }

        public virtual void Destroy()
        {
            this.UnbindView();
            Singleton<MainUIManager>.Instance.ReleaseInstancedView(this);
            this.IsActive = false;
            Singleton<NotifyManager>.Instance.RemoveContext(this);
            if (((this.uiType == UIType.Page) || (this.uiType == UIType.SpecialDialog)) && (this.config.cacheType != ViewCacheType.AlwaysCached))
            {
                Singleton<MainUIManager>.Instance.CheckResouceBeforeLoad();
            }
        }

        public virtual void DestroyContextOnly()
        {
            this.UnbindView();
            this.IsActive = false;
            Singleton<NotifyManager>.Instance.RemoveContext(this);
        }

        private bool DoHandleNotify(Notify cmd)
        {
            try
            {
                bool flag = false;
                if (cmd.type == NotifyTypes.NetwrokPacket)
                {
                    flag |= this.OnPacket(cmd.body as NetPacketV1);
                }
                else
                {
                    flag |= this.OnNotify(cmd);
                }
                return flag;
            }
            catch (Exception exception)
            {
                SuperDebug.VeryImportantError("Exception: " + exception.ToString());
                return false;
            }
        }

        protected virtual void FindViewSavedInScene(Transform canvasTrans, Transform parentTrans)
        {
            Transform parentTransform = this.GetParentTransform(canvasTrans, parentTrans);
            if (parentTransform == null)
            {
            }
            parentTrans = canvasTrans;
            string fileName = Path.GetFileName(this.config.viewPrefabPath);
            Transform transform = parentTrans.Find(fileName);
            if (transform != null)
            {
                this.view = transform.gameObject;
                GameObject view = this.view;
                view.name = view.name + "(Clone)";
                this.view.SetActive(true);
            }
        }

        public virtual Transform GetParentTransform(Transform canvasTrans, Transform viewParent)
        {
            if (viewParent == null)
            {
                return this.GetUIHolder(canvasTrans, this.uiType);
            }
            return viewParent;
        }

        private Transform GetUIHolder(Transform canvasTrans, UIType uiType)
        {
            switch (uiType)
            {
                case UIType.Page:
                    return canvasTrans.Find("Pages");

                case UIType.SpecialDialog:
                    return canvasTrans.Find("SpecialDialogs");

                case UIType.SuspendBar:
                    return canvasTrans.Find("SuspendBars");

                case UIType.Dialog:
                    return canvasTrans.Find("Dialogs");

                case UIType.Root:
                    return null;

                case UIType.MostFront:
                    return canvasTrans;
            }
            return null;
        }

        public bool HandleNotify(Notify cmd)
        {
            if (this.IsActive)
            {
                if (this.view == null)
                {
                    return false;
                }
                return this.DoHandleNotify(cmd);
            }
            Queue<Notify> queue = this._notifyQueue;
            lock (queue)
            {
                this._notifyQueue.Enqueue(cmd);
            }
            return false;
        }

        private void InstantiateView(GameObject go, Transform viewParent = null)
        {
            this.view = go;
            if (viewParent != null)
            {
                this.view.transform.SetParent(viewParent, false);
            }
            this.AddContextIdentifier();
            this.OnViewSet();
        }

        public virtual bool OnNotify(Notify ntf)
        {
            return false;
        }

        public virtual bool OnPacket(NetPacketV1 ntf)
        {
            return false;
        }

        protected virtual void OnSetActive(bool enabled)
        {
        }

        private void OnViewSet()
        {
            if (this is BasePageContext)
            {
                this.view.transform.SetAsFirstSibling();
            }
            this.IsActive = true;
            Singleton<NotifyManager>.Instance.RegisterContext(this);
            this.SetupView();
            this.BindViewCallbacks();
            this.SetViewButtonSoundEffects();
            if (this.EnableTutorial)
            {
                this.TryToDoTutorial();
            }
        }

        private void PostProcessOfSetActive(bool enabled)
        {
            if (enabled)
            {
                Notify[] notifyArray;
                if (this._notifyQueue.Count == 0)
                {
                    return;
                }
                Queue<Notify> queue = this._notifyQueue;
                lock (queue)
                {
                    notifyArray = this._notifyQueue.ToArray();
                    this._notifyQueue.Clear();
                }
                for (int i = 0; i < notifyArray.Length; i++)
                {
                    this.DoHandleNotify(notifyArray[i]);
                }
            }
            this.OnSetActive(enabled);
        }

        public virtual void SetActive(bool enabled)
        {
            this.IsActive = enabled;
            if (this.view != null)
            {
                this.view.SetActive(enabled);
                this.PostProcessOfSetActive(enabled);
                if (enabled)
                {
                    this.TryToDoTutorial();
                }
            }
        }

        protected virtual bool SetupView()
        {
            return false;
        }

        public void SetView(GameObject newView)
        {
            this.view = newView;
            this.OnViewSet();
        }

        private void SetViewButtonSoundEffects()
        {
            Button[] componentsInChildren = this.view.GetComponentsInChildren<Button>(true);
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                if (componentsInChildren[index].GetComponent<MonoButtonWwiseEvent>() == null)
                {
                    MonoButtonWwiseEvent event2 = componentsInChildren[index].gameObject.AddComponent<MonoButtonWwiseEvent>();
                    componentsInChildren[index].GetComponent<MonoButtonWwiseEvent>().eventName = "UI_Click";
                }
                index++;
            }
        }

        public virtual void StartUp(Transform canvasTrans, Transform parentTrans = null)
        {
            if ((this.view == null) && this.findViewSavedInScene)
            {
                this.FindViewSavedInScene(canvasTrans, parentTrans);
            }
            if (this.view == null)
            {
                Transform parentTransform = this.GetParentTransform(canvasTrans, parentTrans);
                GameObject go = Singleton<MainUIManager>.Instance.LoadInstancedView(this);
                this.InstantiateView(go, parentTransform);
            }
            else
            {
                this.AddContextIdentifier();
                this.OnViewSet();
            }
            if (Singleton<MainUIManager>.Instance.GetMainCanvas() is MonoMainCanvas)
            {
                Singleton<MiHoYoGameData>.Instance.GeneralLocalData.AddContextShowCount(this);
            }
        }

        protected void TryToDoTutorial()
        {
            if (Singleton<TutorialModule>.Instance != null)
            {
                Singleton<TutorialModule>.Instance.TryToDoTutoialWhenShowContext(this);
            }
        }

        private void UnbindView()
        {
            for (int i = 0; i < this._bindedEvents.Count; i++)
            {
                this._bindedEvents[i].RemoveAllListeners();
            }
            this._bindedEvents.Clear();
        }

        public bool EnableTutorial { get; set; }

        public bool IsActive { get; private set; }

        public GameObject view { get; protected set; }

        [CompilerGenerated]
        private sealed class <BindViewCallback>c__AnonStoreyDE
        {
            internal Action<BaseEventData> callback;

            internal void <>m__109(BaseEventData evtData)
            {
                this.callback(evtData);
            }
        }
    }
}

