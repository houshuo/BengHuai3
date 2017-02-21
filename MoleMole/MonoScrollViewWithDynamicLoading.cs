namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoScrollViewWithDynamicLoading : MonoBehaviour
    {
        private Transform _contentTrans;
        private int _createdStorageItemNum;
        private GridLayoutGroup _grid;
        private GridLayoutGroup.Constraint _gridConstraint;
        private float _loadPeriod = 2f;
        private RectTransform _maskRectTrans;
        private int _max_storage_item_num_shown;
        private Vector2 _previousPoint;
        private List<IScrollViewItem> _storageItemList;
        private float _timer;
        private CreateGameObjecDelegate CreateGameObjectAndSetupView;

        private void Init()
        {
            this._contentTrans = base.transform.Find("Content");
            this._grid = this._contentTrans.GetComponent<GridLayoutGroup>();
            this._maskRectTrans = base.transform as RectTransform;
            this._createdStorageItemNum = 0;
            this._previousPoint = new Vector2(0f, 1f);
        }

        public bool isEmpty()
        {
            return ((this._contentTrans == null) || (this._contentTrans.childCount <= 0));
        }

        private void LoadStorageItemFromSpecifiedIndex(int beginIndex)
        {
            int num = 0;
            int num2 = beginIndex;
            while ((num2 < this._storageItemList.Count) && (num < this._max_storage_item_num_shown))
            {
                this.CreateGameObjectAndSetupView(this._storageItemList[num2]).transform.SetParent(this._contentTrans, false);
                num2++;
                num++;
                this._createdStorageItemNum++;
            }
        }

        public void MakeAllItemsInteractable()
        {
            IEnumerator enumerator = this._contentTrans.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.GetComponent<Button>().interactable = true;
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
        }

        public void MaskItemsExceptSepcifyList(List<StorageDataItemBase> itemList)
        {
            IEnumerator enumerator = this._contentTrans.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (!itemList.Contains(current.GetComponent<MonoStorageItemIcon>()._data))
                    {
                        current.GetComponent<Button>().interactable = false;
                    }
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
        }

        public void OnScrollView(Vector2 point)
        {
            if (Mathf.Approximately(Vector2.Distance(this._previousPoint, point), 0f))
            {
                this._previousPoint = point;
            }
            else
            {
                if (this._createdStorageItemNum < this._storageItemList.Count)
                {
                    if ((this._gridConstraint == GridLayoutGroup.Constraint.FixedColumnCount) && (point.y < 0.15f))
                    {
                        this.LoadStorageItemFromSpecifiedIndex(this._createdStorageItemNum);
                    }
                    if ((this._gridConstraint == GridLayoutGroup.Constraint.FixedRowCount) && (point.x > 0.85f))
                    {
                        this.LoadStorageItemFromSpecifiedIndex(this._createdStorageItemNum);
                    }
                }
                Vector2 position = this._maskRectTrans.TransformPoint((Vector3) this._maskRectTrans.rect.position);
                Vector2 size = this._maskRectTrans.TransformVector((Vector3) this._maskRectTrans.rect.size);
                Rect rect = new Rect(position, size);
                for (int i = 0; i < this._contentTrans.childCount; i++)
                {
                    Transform child = this._contentTrans.GetChild(i);
                    RectTransform transform2 = child as RectTransform;
                    Vector2 vector3 = transform2.TransformPoint((Vector3) transform2.rect.position);
                    Vector2 vector4 = transform2.TransformVector((Vector3) transform2.rect.size);
                    Rect other = new Rect(vector3, vector4);
                    bool flag = rect.Overlaps(other, true);
                    child.gameObject.SetActive(flag);
                }
            }
        }

        public void SetCreateDelegate(CreateGameObjecDelegate action)
        {
            this.CreateGameObjectAndSetupView = action;
        }

        public void SetStorageItemList(List<IScrollViewItem> storageItemList, GridLayoutGroup.Constraint constraint = 1)
        {
            this.Init();
            IEnumerator enumerator = this._contentTrans.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    UnityEngine.Object.Destroy(current.gameObject);
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
            Vector2 cellSize = this._grid.cellSize;
            RectTransform transform2 = base.transform as RectTransform;
            this._grid.constraint = constraint;
            float width = 0f;
            float num2 = 0f;
            float height = 0f;
            float num4 = 0f;
            this._gridConstraint = constraint;
            if (constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                width = transform2.rect.width;
                height = transform2.rect.height;
                num2 = cellSize.x + this._grid.spacing.x;
                num4 = cellSize.y + this._grid.spacing.y;
            }
            else if (constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                width = transform2.rect.height;
                height = transform2.rect.width;
                num2 = cellSize.y + this._grid.spacing.y;
                num4 = cellSize.x + this._grid.spacing.x;
            }
            this._grid.constraintCount = Mathf.FloorToInt(width / num2);
            this._storageItemList = storageItemList;
            this._createdStorageItemNum = 0;
            this._max_storage_item_num_shown = Mathf.FloorToInt((height / num4) + 2f) * this._grid.constraintCount;
            this.LoadStorageItemFromSpecifiedIndex(0);
        }

        private void Update()
        {
            if ((this._storageItemList != null) && (this._createdStorageItemNum < this._storageItemList.Count))
            {
                this._timer += Time.deltaTime;
                if (this._timer > this._loadPeriod)
                {
                    this.LoadStorageItemFromSpecifiedIndex(this._createdStorageItemNum);
                    this._timer = 0f;
                }
            }
        }
    }
}

