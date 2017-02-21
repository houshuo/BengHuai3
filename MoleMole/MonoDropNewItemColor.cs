namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoDropNewItemColor : MonoBehaviour
    {
        public void SetColorBlack()
        {
            Color color = MiscData.GetColor("TotalBlack");
            this.TrySetImageColorWithChildren(color);
            this.TrySetRawImageColorWithChildren(color);
        }

        public void SetColorWhite()
        {
            Color color = MiscData.GetColor("TotalWhite");
            this.TrySetImageColorWithChildren(color);
            this.TrySetRawImageColorWithChildren(color);
        }

        private void TrySetImageColorWithChildren(Color color)
        {
            Image component = base.GetComponent<Image>();
            if (component != null)
            {
                component.color = color;
            }
            Image[] componentsInChildren = base.GetComponentsInChildren<Image>();
            if (componentsInChildren != null)
            {
                int index = 0;
                int length = componentsInChildren.Length;
                while (index < length)
                {
                    componentsInChildren[index].color = color;
                    index++;
                }
            }
        }

        private void TrySetRawImageColorWithChildren(Color color)
        {
            RawImage component = base.GetComponent<RawImage>();
            if (component != null)
            {
                component.color = color;
            }
            RawImage[] componentsInChildren = base.GetComponentsInChildren<RawImage>();
            if (componentsInChildren != null)
            {
                int index = 0;
                int length = componentsInChildren.Length;
                while (index < length)
                {
                    componentsInChildren[index].color = color;
                    index++;
                }
            }
        }
    }
}

