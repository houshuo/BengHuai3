namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoLevelDebugToggle : MonoBehaviour
    {
        public MonoLevelDebug levelDebug;
        public string luaName;
        public Text luaNameText;
        public Toggle toggle;

        public void OnValueChanged()
        {
            if (this.toggle.isOn)
            {
                this.levelDebug.luaName = this.luaName;
                this.levelDebug.Refresh(this);
            }
        }
    }
}

