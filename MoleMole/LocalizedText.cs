namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        private Text _label;
        [SerializeField]
        private string _textID;
        [SerializeField]
        private string _textPattern;

        public void SetupTextID(string textID, string textPattern)
        {
            this._label.text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            if (!string.IsNullOrEmpty(textPattern))
            {
                this._label.text = textPattern.Replace("#1", this._label.text);
            }
        }

        public void SetupTextID(string textID, params object[] replaceParams)
        {
            this._label.text = LocalizationGeneralLogic.GetText(textID, replaceParams);
        }

        public void Start()
        {
            this._label = base.GetComponent<Text>();
            this.SetupTextID(this._textID, this._textPattern);
        }

        public string TextID
        {
            get
            {
                return this._textID;
            }
            set
            {
                this._textID = value;
            }
        }

        public string TextPattern
        {
            get
            {
                return this._textPattern;
            }
        }
    }
}

