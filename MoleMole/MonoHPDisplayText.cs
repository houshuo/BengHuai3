namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoHPDisplayText : MonoBehaviour
    {
        private void PlayDisplayAnimation(Mode mode)
        {
            Animation component = base.transform.Find("DisplayText").GetComponent<Animation>();
            if (component != null)
            {
                if (component.isPlaying)
                {
                    component.Rewind();
                    component.Stop();
                    this.ResetImageInvisible();
                }
                if (mode == Mode.Up)
                {
                    component.Play("DisplayHPUp", PlayMode.StopAll);
                }
                else if (mode == Mode.Down)
                {
                    component.Play("DisplayHPDown", PlayMode.StopAll);
                }
            }
        }

        private void ResetImageInvisible()
        {
            base.transform.Find("DisplayText/DownText").GetComponent<CanvasGroup>().alpha = 0f;
            base.transform.Find("DisplayText/UpText").GetComponent<CanvasGroup>().alpha = 0f;
        }

        public void SetupView(int hpBefore, int hpAfter, int delta)
        {
            Text component = base.transform.Find("DisplayText/DownText").GetComponent<Text>();
            Text text2 = base.transform.Find("DisplayText/UpText").GetComponent<Text>();
            if (delta > 0)
            {
                text2.text = string.Format("+{0}", delta);
                this.PlayDisplayAnimation(Mode.Up);
            }
            if (delta < 0)
            {
                component.text = string.Format("{0}", delta);
                this.PlayDisplayAnimation(Mode.Down);
            }
        }

        private enum Mode
        {
            Up,
            Down
        }
    }
}

