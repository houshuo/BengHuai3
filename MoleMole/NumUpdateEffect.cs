namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Text)), RequireComponent(typeof(Animation))]
    public class NumUpdateEffect : MonoBehaviour
    {
        private Animation _ani;
        private Text _textComponent;
        private string _textContent;

        private void Awake()
        {
            this._textComponent = base.transform.GetComponent<Text>();
            this._textContent = this._textComponent.text;
            this._ani = base.transform.GetComponent<Animation>();
        }

        private void Update()
        {
            if (this._textContent != this._textComponent.text)
            {
                this._textContent = this._textComponent.text;
                this._ani.Play();
            }
        }
    }
}

