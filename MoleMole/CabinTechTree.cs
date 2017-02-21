namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CabinTechTree
    {
        private List<CabinTech> _activeNodeList = new List<CabinTech>();
        private bool _bInited;
        private List<CabinTechTreeNode> _itemList = new List<CabinTechTreeNode>();
        private CabinType _ownerType;
        private List<CabinTechTreeNode> _return_active_node_list = new List<CabinTechTreeNode>();

        public CabinTechTree(CabinType type)
        {
            this._ownerType = type;
            this._bInited = false;
        }

        public bool AbilityUnLock(CabinTechEffectType techType)
        {
            foreach (CabinTechTreeNode node in this._itemList)
            {
                if ((node._status == TechTreeNodeStatus.Active) && (node._metaData.AbilityType == techType))
                {
                    return true;
                }
            }
            return false;
        }

        public int GetAbilitySum(CabinTechEffectType techType, int index)
        {
            int num = 0;
            foreach (CabinTechTreeNode node in this._itemList)
            {
                if ((node._status == TechTreeNodeStatus.Active) && (node._metaData.AbilityType == techType))
                {
                    int num2 = 0;
                    if (index == 1)
                    {
                        num2 = Mathf.FloorToInt((float) node._metaData.Argument1);
                    }
                    else if (index == 2)
                    {
                        num2 = Mathf.FloorToInt((float) node._metaData.Argument2);
                    }
                    num += num2;
                }
            }
            return num;
        }

        public List<CabinTechTreeNode> GetActiveNodeList()
        {
            this._return_active_node_list.Clear();
            foreach (CabinTechTreeNode node in this._itemList)
            {
                if (node._status == TechTreeNodeStatus.Active)
                {
                    this._return_active_node_list.Add(node);
                }
            }
            return this._return_active_node_list;
        }

        public int GetAvailableNodesDiff(int level)
        {
            int num = 0;
            foreach (CabinTechTreeNode node in this._itemList)
            {
                if (node._metaData.UnlockLevel == level)
                {
                    num++;
                }
            }
            return num;
        }

        public List<CabinTechTreeNode> GetNeibours(CabinTechTreeNode node)
        {
            List<CabinTechTreeNode> list = new List<CabinTechTreeNode>();
            foreach (CabinTechTreeNode node2 in this._itemList)
            {
                if (node2._metaData.X == node._metaData.X)
                {
                    switch ((node2._metaData.Y - node._metaData.Y))
                    {
                        case 1:
                        case -1:
                            list.Add(node2);
                            break;
                    }
                }
                else if (node2._metaData.Y == node._metaData.Y)
                {
                    switch ((node2._metaData.X - node._metaData.X))
                    {
                        case 1:
                        case -1:
                            list.Add(node2);
                            break;
                    }
                }
            }
            return list;
        }

        public CabinTechTreeNode GetNode(int x, int y)
        {
            foreach (CabinTechTreeNode node in this._itemList)
            {
                if ((node._metaData.X == x) && (node._metaData.Y == y))
                {
                    return node;
                }
            }
            return null;
        }

        public List<CabinTechTreeNode> GetNodeList()
        {
            return this._itemList;
        }

        public int GetPowerUsed()
        {
            int num = 0;
            foreach (CabinTechTreeNode node in this._itemList)
            {
                if (node._status == TechTreeNodeStatus.Active)
                {
                    num += node._metaData.PowerCost;
                }
            }
            return num;
        }

        public int GetResetScoin()
        {
            int num = 0;
            foreach (CabinTechTreeNode node in this._itemList)
            {
                if (node._status == TechTreeNodeStatus.Active)
                {
                    num += node._metaData.ResetSCoin;
                }
            }
            return num;
        }

        public void InitMetaData()
        {
            if (!this._bInited)
            {
                this._bInited = true;
                this._itemList.Clear();
                foreach (CabinTechTreeMetaData data in CabinTechTreeMetaDataReader.GetItemList())
                {
                    if (data.Cabin == this._ownerType)
                    {
                        CabinTechTreeNode item = new CabinTechTreeNode(this) {
                            _metaData = data
                        };
                        this._itemList.Add(item);
                    }
                }
                if (this._activeNodeList.Count > 0)
                {
                    foreach (CabinTech tech in this._activeNodeList)
                    {
                        this.SetNodeActive(tech.get_pos_x(), tech.get_pos_y());
                    }
                    this._activeNodeList.Clear();
                }
                this.RefreshNodes();
            }
        }

        public void Log()
        {
            foreach (CabinTechTreeNode node in this._itemList)
            {
            }
        }

        public void OnReceiveActiveNodes(List<CabinTech> list)
        {
            if (!this._bInited)
            {
                foreach (CabinTech tech in list)
                {
                    CabinTech item = new CabinTech();
                    item.set_pos_x(tech.get_pos_x());
                    item.set_pos_y(tech.get_pos_y());
                    this._activeNodeList.Add(item);
                }
            }
            else
            {
                this.ResetNodes();
                foreach (CabinTech tech3 in list)
                {
                    this.SetNodeActive(tech3.get_pos_x(), tech3.get_pos_y());
                }
                this.RefreshNodes();
            }
        }

        public void RefreshNodes()
        {
            foreach (CabinTechTreeNode node in this._itemList)
            {
                node._status = node.GetStatus();
            }
        }

        public void ResetNodes()
        {
            foreach (CabinTechTreeNode node in this._itemList)
            {
                node._status_before_reset = node._status;
                node._status = TechTreeNodeStatus.Lock;
            }
        }

        public void SetNodeActive(int x, int y)
        {
            foreach (CabinTechTreeNode node in this._itemList)
            {
                if ((node._metaData.X == x) && (node._metaData.Y == y))
                {
                    node._status = TechTreeNodeStatus.Active;
                    break;
                }
            }
        }
    }
}

