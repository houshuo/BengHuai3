namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoGMTalkButton : MonoBehaviour
    {
        private ButtonCallBack _buttonCallback;
        private string _command;

        public void OnButtonCallBack()
        {
            if (this._buttonCallback != null)
            {
                this._buttonCallback(this._command);
            }
        }

        public void SetupView(string command, ButtonCallBack buttonCallback = null)
        {
            this._command = command;
            this._buttonCallback = buttonCallback;
            base.transform.Find("Text").GetComponent<Text>().text = command;
        }

        public delegate void ButtonCallBack(string command);
    }
}

