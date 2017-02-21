namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Scrollbar))]
    public class MonoScrollBar : MonoBehaviour
    {
        public void SetVisible(bool isVisible)
        {
            base.gameObject.SetActive(isVisible);
        }
    }
}

