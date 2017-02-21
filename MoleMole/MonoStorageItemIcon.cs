namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoStorageItemIcon : MonoBehaviour
    {
        public StorageDataItemBase _data;
        private int _loadPosition;
        private Color _originColor;
        private bool _setupAlready;
        private StorageItemAction _storageItemProcess;
        private System.Type _type;

        public void ClearContent()
        {
            if (this._data != null)
            {
                this.RealClearContent();
                this._data = null;
            }
        }

        public void OnClick()
        {
            if (this._storageItemProcess != null)
            {
                this._storageItemProcess(this._data, base.transform, this._loadPosition, this._type);
            }
        }

        public void ProcessEquipmentCost(int avatarCostLeft)
        {
            if (avatarCostLeft < this._data.GetCost())
            {
                base.transform.GetComponent<Image>().color = Color.gray;
                base.transform.Find("Image").GetComponent<Image>().color = Color.gray;
                base.transform.GetComponent<Button>().interactable = false;
            }
        }

        public bool ProcessEvoMaterial()
        {
            if (this._data == null)
            {
                this.RealClearContent();
                return true;
            }
            MaterialDataItem storageItemByTypeAndID = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(this._data.GetType(), this._data.ID) as MaterialDataItem;
            if ((storageItemByTypeAndID != null) && (storageItemByTypeAndID.number >= this._data.number))
            {
                return true;
            }
            base.transform.GetComponent<Image>().color = Color.gray;
            base.transform.Find("Image").GetComponent<Image>().color = Color.gray;
            return false;
        }

        public void ProcessLoadedItem()
        {
            if ((this._data.avatarID != -1) && (Singleton<AvatarModule>.Instance.GetAvatarByID(this._data.avatarID) != null))
            {
                base.transform.GetComponent<Image>().color = Color.red;
            }
        }

        public void ProcessSelectedItem()
        {
            base.transform.GetComponent<Image>().color = Color.green;
        }

        private void RealClearContent()
        {
            base.transform.Find("Image").gameObject.SetActive(false);
            base.transform.Find("LevelAndCost").gameObject.SetActive(false);
            base.transform.Find("Number").gameObject.SetActive(false);
            this.ResetColor();
        }

        public void ResetColor()
        {
            base.transform.GetComponent<Image>().color = this._originColor;
        }

        public void SetupView(StorageDataItemBase data, Transform parentTrans, StorageItemAction action, int loadPosition, System.Type type, bool interactable = true)
        {
            this._originColor = base.transform.GetComponent<Image>().color;
            this._data = data;
            this._storageItemProcess = action;
            this._type = type;
            this._loadPosition = loadPosition;
            base.transform.SetParent(parentTrans, false);
            if (this._data == null)
            {
                this.RealClearContent();
            }
            else
            {
                base.transform.Find("Image").gameObject.SetActive(true);
                GameObject obj2 = Miscs.LoadResource<GameObject>(data.GetIconPath(), BundleType.RESOURCE_FILE);
                base.transform.Find("Image").GetComponent<Image>().sprite = obj2.GetComponent<SpriteRenderer>().sprite;
                if (this._data is MaterialDataItem)
                {
                    base.transform.Find("LevelAndCost").gameObject.SetActive(false);
                    base.transform.Find("Number").gameObject.SetActive(true);
                    base.transform.Find("Number/Number").GetComponent<Text>().text = this._data.number.ToString();
                }
                else
                {
                    base.transform.Find("LevelAndCost").gameObject.SetActive(true);
                    base.transform.Find("Number").gameObject.SetActive(false);
                    base.transform.Find("LevelAndCost/LevelNumber").GetComponent<Text>().text = this._data.level.ToString();
                    base.transform.Find("LevelAndCost/CostNumber").GetComponent<Text>().text = this._data.GetCost().ToString();
                }
                base.transform.GetComponent<Button>().interactable = interactable;
                this._setupAlready = true;
            }
        }

        public void SetupViewWithExtraInfo(StorageDataItemBase data, Transform parentTrans, StorageItemAction action, int loadPosition, System.Type type, bool isAlreadyLoaded, bool interactable)
        {
            this.SetupView(data, parentTrans, action, loadPosition, type, interactable);
            this.ProcessLoadedItem();
        }

        private void Start()
        {
            if (!this._setupAlready)
            {
                this.ClearContent();
            }
        }
    }
}

