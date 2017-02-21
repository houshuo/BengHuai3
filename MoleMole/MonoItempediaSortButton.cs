namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class MonoItempediaSortButton : MonoBehaviour
    {
        private Action Clicked;

        public void OnClick()
        {
            if (this.Clicked != null)
            {
                this.Clicked();
            }
        }

        public void SetClickCallback(Action cb)
        {
            this.Clicked = cb;
        }

        public void SetupView(bool selected, bool asent)
        {
            Image component = base.transform.GetComponent<Image>();
            bool flag = selected;
            component.enabled = flag;
            component.color = !flag ? Color.white : MiscData.GetColor("Yellow");
            base.transform.Find("Text").GetComponent<Text>().color = !flag ? Color.white : MiscData.GetColor("Black");
            base.transform.Find("Order").gameObject.SetActive(flag);
            base.transform.Find("Order/UpImg").gameObject.SetActive(asent);
            base.transform.Find("Order/DownImg").gameObject.SetActive(!asent);
        }
    }
}

