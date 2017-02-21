namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoEventTrigger : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IEndDragHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
    {
        [NonSerialized]
        public List<EventTrigger.Entry> triggers = new List<EventTrigger.Entry>();

        public void AddTrigger(EventTrigger.Entry entry)
        {
            int num = this.triggers.SeekAddPosition<EventTrigger.Entry>();
            this.triggers[num] = entry;
        }

        public void ClearTriggers()
        {
            for (int i = 0; i < this.triggers.Count; i++)
            {
                this.triggers[i] = null;
            }
        }

        private void Execute(EventTriggerType id, BaseEventData eventData)
        {
            for (int i = 0; i < this.triggers.Count; i++)
            {
                EventTrigger.Entry entry = this.triggers[i];
                if (((entry != null) && (entry.eventID == id)) && (entry.callback != null))
                {
                    entry.callback.Invoke(eventData);
                }
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.BeginDrag, eventData);
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.Cancel, eventData);
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.Deselect, eventData);
        }

        private void OnDisable()
        {
            this.ClearTriggers();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.Drag, eventData);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.Drop, eventData);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.EndDrag, eventData);
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.InitializePotentialDrag, eventData);
        }

        public virtual void OnMove(AxisEventData eventData)
        {
            this.Execute(EventTriggerType.Move, eventData);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerClick, eventData);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerDown, eventData);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerEnter, eventData);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerExit, eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerUp, eventData);
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.Scroll, eventData);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.Select, eventData);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.Submit, eventData);
        }

        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.UpdateSelected, eventData);
        }
    }
}

