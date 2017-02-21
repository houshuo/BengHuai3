namespace MoleMole
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class MonoComboText : MonoBehaviour
    {
        private readonly string[] _evaluationList = new string[] { string.Empty, "Good", "Great", "Terrific", "Splendid", "Mavelous" };

        public void ActBlingEffect()
        {
            if (base.gameObject.activeInHierarchy && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning))
            {
                Transform transform = base.transform.Find("NumText/Combo");
                Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Combo_Clear_Resist_Effect", transform.position, transform.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
            }
        }

        private int GetComboEvaluation(int combo)
        {
            for (int i = MiscData.Config.ComboEvaluation.Count - 1; i >= 0; i--)
            {
                if (combo >= MiscData.Config.ComboEvaluation[i])
                {
                    return (i + 1);
                }
            }
            return 0;
        }

        public void SetupView(int comboBefore, int comboAfter)
        {
            if (comboAfter > 0)
            {
                base.transform.Find("NumText/Combo/ComboNum").GetComponent<Text>().text = comboAfter + string.Empty;
                if (comboBefore == 0)
                {
                    base.transform.localScale = Vector3.one;
                }
                if (base.transform.Find("NumText").GetComponent<Animation>().isPlaying)
                {
                    base.transform.Find("NumText").GetComponent<Animation>().Rewind();
                }
                base.transform.Find("NumText").GetComponent<Animation>().Play();
                int comboEvaluation = this.GetComboEvaluation(comboAfter);
                string str = this._evaluationList[comboEvaluation];
                IEnumerator enumerator = base.transform.Find("NumText/Evaluation").GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        current.gameObject.SetActive(current.name == str);
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
                base.transform.Find("NumText/Combo/ComboNumBG").GetComponent<Image>().color = Miscs.ParseColor(MiscData.Config.ComboNumFrameColor[comboEvaluation]);
            }
            else
            {
                base.transform.localScale = Vector3.zero;
            }
        }
    }
}

