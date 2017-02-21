namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class InputFieldHelper : MonoBehaviour
    {
        public int mCharacterlimit;

        public void OnEndEdit(InputField vInput)
        {
            if (this.mCharacterlimit > 0)
            {
                string str = vInput.text.Trim();
                int length = Mathf.Min(this.mCharacterlimit, str.Length);
                str = str.Substring(0, length);
                vInput.text = str;
            }
        }
    }
}

