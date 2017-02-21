namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class PressWithSE : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
    {
        public string SEEventName;

        public void OnPointerDown(PointerEventData eventData)
        {
            Singleton<WwiseAudioManager>.Instance.Post(this.SEEventName, null, null, null);
        }
    }
}

