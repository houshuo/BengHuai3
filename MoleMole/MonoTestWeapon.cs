namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoTestWeapon : MonoBehaviour
    {
        public void Awake()
        {
            Transform child = base.transform.GetChild(0);
            if (!child.gameObject.activeSelf)
            {
                child = base.transform.GetChild(1);
            }
            this.SetupWeaponView(child.gameObject);
        }

        private void SetupWeaponView(GameObject weaponGo)
        {
            if (weaponGo.transform.Find("TransformCopy") != null)
            {
                Transform transform = weaponGo.transform.Find("TransformCopy");
                weaponGo.transform.localPosition = transform.localPosition;
                weaponGo.transform.localEulerAngles = transform.localEulerAngles;
                weaponGo.transform.localScale = transform.localScale;
            }
            if ((weaponGo.transform.Find("ShortSword") != null) && (weaponGo.transform.Find("LongSword") != null))
            {
                weaponGo.transform.Find("ShortSword").gameObject.SetActive(false);
                weaponGo.transform.Find("LongSword").gameObject.SetActive(true);
            }
        }
    }
}

