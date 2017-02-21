namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoMaterialPanel : MonoBehaviour
    {
        private List<StorageDataItemBase> _materialList;
        private AnimStep _step;
        private const string ANI_CLIP_PRE = "PowerUpMaterialMove_";
        private const string MaterialRarityIconPrefabPathPre = "SpriteOutput/MaterialRarityIcons/Metial";

        public void EatAll()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PowerUpAndEvoEffect, "EatAll"));
        }

        public void EatMaterial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PowerUpAndEvoEffect, "Small"));
        }

        public void SetupView(List<StorageDataItemBase> materialList)
        {
            this._materialList = materialList;
            base.transform.GetComponent<Animation>().Stop();
            this._step = AnimStep.StepNone;
            for (int i = 0; i < base.transform.childCount; i++)
            {
                Transform child = base.transform.GetChild(i);
                if (i >= this._materialList.Count)
                {
                    child.gameObject.SetActive(false);
                    continue;
                }
                StorageDataItemBase item = this._materialList[i];
                int num2 = 1;
                switch (item.rarity)
                {
                    case 1:
                    case 2:
                        num2 = 1;
                        break;

                    case 3:
                        num2 = 2;
                        break;

                    case 4:
                    case 5:
                        num2 = 3;
                        break;
                }
                string prefabPath = "SpriteOutput/MaterialRarityIcons/Metial" + num2.ToString();
                child.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
                child.GetComponent<MonoItemIconButton>().SetupView(item, MonoItemIconButton.SelectMode.None, false, false, false);
            }
            this._step = AnimStep.StepOne;
        }

        private void Update()
        {
            switch (this._step)
            {
                case AnimStep.StepOne:
                    this._step = AnimStep.StepTwo;
                    break;

                case AnimStep.StepTwo:
                    base.transform.GetComponent<ContentSizeFitter>().enabled = false;
                    this._step = AnimStep.StepThree;
                    break;

                case AnimStep.StepThree:
                    base.transform.GetComponent<HorizontalLayoutGroup>().enabled = false;
                    this._step = AnimStep.StepFour;
                    break;

                case AnimStep.StepFour:
                {
                    Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_Mat_Drop", null, null, null);
                    Animation component = base.transform.GetComponent<Animation>();
                    string animation = "PowerUpMaterialMove_" + this._materialList.Count;
                    component.Play(animation, PlayMode.StopAll);
                    this._step = AnimStep.StepNone;
                    break;
                }
            }
        }

        public enum AnimStep
        {
            StepNone,
            StepOne,
            StepTwo,
            StepThree,
            StepFour
        }
    }
}

