namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoKeyButton : MonoBehaviour
    {
        public Button button;
        public string KeyButtonCode;

        private void DoSomeThing()
        {
            this.button.onClick.Invoke();
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (GlobalVars.KEYBOARD_FUNCTION_BUTTON_CONTROL && !this.button.interactable)
            {
            }
        }
    }
}

