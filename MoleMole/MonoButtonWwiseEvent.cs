namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoButtonWwiseEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
    {
        public string eventName;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!string.IsNullOrEmpty(this.eventName))
            {
                Singleton<WwiseAudioManager>.Instance.Post(this.eventName, null, null, null);
            }
        }
    }
}

