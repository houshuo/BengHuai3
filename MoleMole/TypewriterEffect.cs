namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [RequireComponent(typeof(Text)), AddComponentMenu("Typewriter Effect")]
    public class TypewriterEffect : MonoBehaviour
    {
        public int charsPerSecond;
        private bool isActive;
        private Text mText;
        public UnityEvent myEvent;
        private float timer;
        private string words;

        public void Finish()
        {
            this.OnFinish();
        }

        public void OnEnable()
        {
            this.ReloadText();
            this.isActive = true;
        }

        private void OnFinish()
        {
            this.isActive = false;
            this.timer = 0f;
            base.GetComponent<Text>().text = this.words;
            try
            {
                this.myEvent.Invoke();
            }
            catch (Exception)
            {
            }
        }

        public void OnStart()
        {
            this.ReloadText();
            this.isActive = true;
        }

        private void OnStartWriter()
        {
            if (this.isActive)
            {
                try
                {
                    this.mText.text = this.words.Substring(0, (int) (this.charsPerSecond * this.timer));
                    this.timer += Time.deltaTime;
                }
                catch (Exception)
                {
                    this.OnFinish();
                }
            }
        }

        private void ReloadText()
        {
            this.words = base.GetComponent<Text>().text;
            this.mText = base.GetComponent<Text>();
            this.timer = 0f;
        }

        public void RestartRead()
        {
            this.ReloadText();
            this.isActive = true;
        }

        private void Start()
        {
            if (this.myEvent == null)
            {
                this.myEvent = new UnityEvent();
            }
            this.words = base.GetComponent<Text>().text;
            base.GetComponent<Text>().text = string.Empty;
            this.timer = 0f;
            this.isActive = true;
            this.charsPerSecond = Mathf.Max(1, this.charsPerSecond);
            this.mText = base.GetComponent<Text>();
        }

        private void Update()
        {
            this.OnStartWriter();
        }
    }
}

