namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoFaceEffect : MonoBehaviour
    {
        public FaceEffectItem[] items;

        private void Awake()
        {
            List<FaceEffectItem> list = new List<FaceEffectItem>();
            IEnumerator enumerator = base.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    FaceEffectItem item = new FaceEffectItem {
                        name = current.gameObject.name,
                        effect = current.gameObject
                    };
                    list.Add(item);
                    item.effect.SetActive(false);
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
            this.items = list.ToArray();
        }
    }
}

