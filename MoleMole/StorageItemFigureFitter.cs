namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StorageItemFigureFitter : MonoBehaviour
    {
        public void SetTheFigureFitTheContent(string imagePath)
        {
            GameObject obj2 = Miscs.LoadResource<GameObject>(imagePath, BundleType.RESOURCE_FILE);
            float width = obj2.GetComponent<SpriteRenderer>().sprite.rect.width;
            float height = obj2.GetComponent<SpriteRenderer>().sprite.rect.height;
            float num3 = (base.transform as RectTransform).rect.width;
            if (width > num3)
            {
                height *= num3 / width;
                width = num3;
            }
            base.transform.Find("Image").GetComponent<Image>().sprite = obj2.GetComponent<SpriteRenderer>().sprite;
            (base.transform.Find("Image") as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            (base.transform.Find("Image") as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}

