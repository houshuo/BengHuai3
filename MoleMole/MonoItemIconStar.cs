namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoItemIconStar : MonoBehaviour
    {
        private const string ITEM_STAR_GREY_PREFAB_PATH = "SpriteOutput/GeneralUI/StarGray";
        private const string ITEM_STAR_PREFAB_PATH = "SpriteOutput/GeneralUI/Star";

        public void SetupView(int star, int maxStar)
        {
            for (int i = 0; i < base.transform.childCount; i++)
            {
                Image component = base.transform.GetChild(i).GetComponent<Image>();
                component.gameObject.SetActive(i < maxStar);
                if (i < maxStar)
                {
                    if (i < star)
                    {
                        component.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/GeneralUI/Star");
                    }
                    else
                    {
                        component.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/GeneralUI/StarGray");
                    }
                }
            }
        }
    }
}

