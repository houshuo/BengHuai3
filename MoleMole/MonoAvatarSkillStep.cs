namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class MonoAvatarSkillStep : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        public Transform iconPrefab;
        private const string PATTERN = @"\[(\d+)\]";
        public Transform textPrefab;

        private void AddIcon(int skillId)
        {
            base.transform.AddChildFromPrefab(this.iconPrefab, "Icon").Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._avatarData.GetAvatarSkillBySkillID(skillId).IconPath);
        }

        private void AddText(string text)
        {
            base.transform.AddChildFromPrefab(this.textPrefab, "Text").GetComponent<Text>().text = text;
        }

        private void Clear()
        {
            base.transform.DestroyChildren();
        }

        public void SetupView(AvatarDataItem avatar, string input)
        {
            this.Clear();
            this._avatarData = avatar;
            int startIndex = 0;
            IEnumerator enumerator = Regex.Matches(input, @"\[(\d+)\]").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Match current = (Match) enumerator.Current;
                    if (startIndex < current.Index)
                    {
                        string text = input.Substring(startIndex, current.Index - startIndex);
                        this.AddText(text);
                    }
                    startIndex = current.Index + current.Value.Length;
                    int skillId = int.Parse(current.Groups[1].Value);
                    this.AddIcon(skillId);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (startIndex < input.Length)
            {
                string str2 = input.Substring(startIndex, input.Length - startIndex);
                this.AddText(str2);
            }
        }
    }
}

