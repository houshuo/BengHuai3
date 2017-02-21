namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoStigmataFigure : MonoBehaviour
    {
        private StigmataDataItem _stigmata;

        public void OnStigmataFigureClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new DropNewItemDialogContext(this._stigmata, false, true), UIType.Any);
        }

        private void SetImageAttrForAllChildren(Transform trans, Material mat, Color color)
        {
            Image[] componentsInChildren = trans.GetComponentsInChildren<Image>();
            if (componentsInChildren != null)
            {
                int index = 0;
                int length = componentsInChildren.Length;
                while (index < length)
                {
                    componentsInChildren[index].material = mat;
                    componentsInChildren[index].color = color;
                    index++;
                }
            }
        }

        private void SetupPrefIntoContainer(Transform containerTrans, StigmataDataItem stigmata)
        {
            containerTrans.DestroyChildren();
            GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(stigmata.GetImagePath(), BundleType.RESOURCE_FILE));
            obj2.transform.SetParent(containerTrans, false);
            obj2.gameObject.SetActive(true);
            obj2.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        public void SetupView(StigmataDataItem stigmata)
        {
            this._stigmata = stigmata;
            this.SetupPrefIntoContainer(base.transform.Find("PrefContainer"), stigmata);
        }

        public void SetupViewWithIdentifyStatus(StigmataDataItem stigmata)
        {
            this._stigmata = stigmata;
            this.SetupViewWithIdentifyStatus(stigmata, false);
        }

        public void SetupViewWithIdentifyStatus(StigmataDataItem stigmata, bool forceLock)
        {
            this._stigmata = stigmata;
            Transform containerTrans = base.transform.Find("PrefContainer");
            Transform transform2 = base.transform.Find("Mask/PrefContainer");
            Transform transform3 = base.transform.Find("Mask");
            this.SetupPrefIntoContainer(containerTrans, stigmata);
            if (stigmata.IsAffixIdentify && !forceLock)
            {
                this.SetImageAttrForAllChildren(containerTrans, null, Color.white);
                containerTrans.gameObject.SetActive(true);
                transform3.gameObject.SetActive(false);
            }
            else
            {
                containerTrans.gameObject.SetActive(false);
                Material mat = Miscs.LoadResource<Material>("Material/ImageColorize", BundleType.RESOURCE_FILE);
                this.SetImageAttrForAllChildren(containerTrans, mat, Color.white);
                transform3.gameObject.SetActive(true);
                this.SetupPrefIntoContainer(transform2, stigmata);
                Material material2 = Miscs.LoadResource<Material>("Material/ImageMonoColor", BundleType.RESOURCE_FILE);
                this.SetImageAttrForAllChildren(transform2, material2, MiscData.GetColor("DarkBlue"));
            }
        }
    }
}

