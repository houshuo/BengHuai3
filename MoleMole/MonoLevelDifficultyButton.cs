namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    public class MonoLevelDifficultyButton : MonoBehaviour
    {
        private Action<LevelDiffculty> _clickCallBack;
        private LevelDiffculty _difficulty;

        private void OnClick()
        {
            if (this._clickCallBack != null)
            {
                this._clickCallBack(this._difficulty);
            }
        }

        public void SetupClickCallBack(Action<LevelDiffculty> callBack)
        {
            this._clickCallBack = callBack;
            base.transform.Find("Btn").GetComponent<Button>().onClick.RemoveAllListeners();
            base.transform.Find("Btn").GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
        }

        public void SetupDifficulty(LevelDiffculty difficulty)
        {
            this._difficulty = difficulty;
        }
    }
}

