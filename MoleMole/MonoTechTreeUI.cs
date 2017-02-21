namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoTechTreeUI : MonoBehaviour
    {
        private int _column;
        [SerializeField]
        private RectTransform _content;
        private List<CabinTechTreeNode> _nodeDataList = new List<CabinTechTreeNode>();
        [SerializeField]
        private GameObject _nodePrefab;
        private Vector3 _nodeScale = new Vector3(1f, 1f, 1f);
        private List<MonoTechNodeUI> _nodeUIObjList = new List<MonoTechNodeUI>();
        private Vector2 _originNormalizedPositin;
        private int _row;
        private CabinTechTree _tree;
        private int _x_meta_to_ui;
        private int _y_meta_to_ui;

        public void ClearNodes()
        {
            foreach (MonoTechNodeUI eui in this._nodeUIObjList)
            {
                UnityEngine.Object.DestroyImmediate(eui.gameObject);
            }
            this._nodeUIObjList.Clear();
        }

        private CabinTechTreeNode GetDataByUI(int x, int y)
        {
            int num = 0;
            int num2 = 0;
            this.UI2Meta(x, y, ref num, ref num2);
            return this._tree.GetNode(num, num2);
        }

        private void GetTreeArea()
        {
            int y = 0;
            int num2 = 0;
            int x = 0;
            int num4 = 0;
            for (int i = 0; i < this._nodeDataList.Count; i++)
            {
                CabinTechTreeNode node = this._nodeDataList[i];
                if (i == 0)
                {
                    x = node._metaData.X;
                    num4 = node._metaData.X;
                    y = node._metaData.Y;
                    num2 = node._metaData.Y;
                }
                num4 = (node._metaData.X <= num4) ? num4 : node._metaData.X;
                x = (node._metaData.X >= x) ? x : node._metaData.X;
                num2 = (node._metaData.Y <= num2) ? num2 : node._metaData.Y;
                y = (node._metaData.Y >= y) ? y : node._metaData.Y;
            }
            this._row = (num2 - y) + 1;
            this._column = (num4 - x) + 1;
            this._x_meta_to_ui = (x >= 0) ? x : -x;
            this._y_meta_to_ui = (y >= 0) ? y : -y;
        }

        public bool HasChildren()
        {
            return (this._nodeUIObjList.Count > 0);
        }

        public void InitNodes(CabinTechTree tree)
        {
            this._tree = tree;
            this._nodeDataList = this._tree.GetNodeList();
            this.GetTreeArea();
            this._content.GetComponent<GridLayoutGroup>().constraintCount = this._row;
            this._originNormalizedPositin = new Vector2(0f, 0f);
            for (int i = 0; i < this._row; i++)
            {
                for (int j = 0; j < this._column; j++)
                {
                    GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(this._nodePrefab);
                    obj2.transform.SetParent(this._content.transform);
                    obj2.GetComponent<RectTransform>().localScale = this._nodeScale;
                    MonoTechNodeUI component = obj2.GetComponent<MonoTechNodeUI>();
                    CabinTechTreeNode dataByUI = this.GetDataByUI(j, i);
                    if (dataByUI != null)
                    {
                        if ((dataByUI._metaData.X == 0) && (dataByUI._metaData.Y == 0))
                        {
                            this._originNormalizedPositin.x = ((float) j) / ((float) this._row);
                            this._originNormalizedPositin.y = 1f - (((float) i) / ((float) this._column));
                        }
                        obj2.name = string.Format("TechTreeNode_{0}", dataByUI._metaData.ID);
                    }
                    component.Init(dataByUI, j, i);
                    component.RefreshStatus();
                    this._nodeUIObjList.Add(component);
                }
            }
        }

        private void Meta2UI(int x, int y, ref int x1, ref int y1)
        {
            x1 = x + this._x_meta_to_ui;
            y1 = y + this._y_meta_to_ui;
        }

        public void RefreshUI()
        {
            foreach (MonoTechNodeUI eui in this._nodeUIObjList)
            {
                eui.RefreshStatus();
            }
        }

        public void SetOriginPosition()
        {
            base.transform.GetComponent<ScrollRect>().normalizedPosition = this._originNormalizedPositin;
        }

        private void UI2Meta(int x, int y, ref int x1, ref int y1)
        {
            x1 = x - this._x_meta_to_ui;
            y1 = y - this._y_meta_to_ui;
        }
    }
}

