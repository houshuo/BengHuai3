namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class CabinTechTreeNode
    {
        private OnTechTreeNodeActive _activeHandler;
        private TechTreeNodeStatus _internal_status;
        private List<TechTreeNodeLockInfo> _lockInfoList = new List<TechTreeNodeLockInfo>();
        public CabinTechTreeMetaData _metaData;
        public TechTreeNodeStatus _status_before_reset;
        private CabinTechTree _tree;

        public CabinTechTreeNode(CabinTechTree tree)
        {
            this._tree = tree;
            this._status = TechTreeNodeStatus.Lock;
        }

        public string GetComment()
        {
            if (this._status == TechTreeNodeStatus.Unlock_Ready_Active)
            {
                int leftPowerCost = Singleton<IslandModule>.Instance.GetLeftPowerCost();
                if (leftPowerCost < this._metaData.PowerCost)
                {
                    return string.Format("lack power, left: {0}, meta: {1}", leftPowerCost, this._metaData.PowerCost);
                }
            }
            return string.Empty;
        }

        public List<TechTreeNodeLockInfo> GetLockInfo()
        {
            this._lockInfoList.Clear();
            if ((this._metaData.UnlockLevel > 0) && (Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType) this._metaData.Cabin).level < this._metaData.UnlockLevel))
            {
                TechTreeNodeLockInfo info = new TechTreeNodeLockInfo {
                    _lockType = TechTreeNodeLock.CabinLevel,
                    _needLevel = this._metaData.UnlockLevel
                };
                this._lockInfoList.Add(info);
            }
            if (this._metaData.UnlockAvatarID > 0)
            {
                AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this._metaData.UnlockAvatarID);
                if (!avatarByID.UnLocked)
                {
                    TechTreeNodeLockInfo item = new TechTreeNodeLockInfo {
                        _lockType = TechTreeNodeLock.AvatarUnlock,
                        _needLevel = this._metaData.UnlockAvatarLevel
                    };
                    this._lockInfoList.Add(item);
                }
                else if (avatarByID.level < this._metaData.UnlockAvatarLevel)
                {
                    TechTreeNodeLockInfo info3 = new TechTreeNodeLockInfo {
                        _lockType = TechTreeNodeLock.AvatarLevel,
                        _needLevel = this._metaData.UnlockAvatarLevel
                    };
                    this._lockInfoList.Add(info3);
                }
            }
            return this._lockInfoList;
        }

        public List<CabinTechTreeNode> GetNeibours()
        {
            return this._tree.GetNeibours(this);
        }

        public TechTreeNodeStatus GetStatus()
        {
            if (this._status == TechTreeNodeStatus.Active)
            {
                return this._status;
            }
            if ((this._metaData.UnlockLevel > 0) && (Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType) this._metaData.Cabin).level < this._metaData.UnlockLevel))
            {
                return TechTreeNodeStatus.Lock;
            }
            if (this._metaData.UnlockAvatarID > 0)
            {
                AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this._metaData.UnlockAvatarID);
                if (!avatarByID.UnLocked || (avatarByID.level < this._metaData.UnlockAvatarLevel))
                {
                    return TechTreeNodeStatus.Lock;
                }
            }
            if ((this._metaData.X == 0) && (this._metaData.Y == 0))
            {
                return TechTreeNodeStatus.Unlock_Ready_Active;
            }
            bool flag = false;
            foreach (CabinTechTreeNode node in this._tree.GetNeibours(this))
            {
                if (node._status == TechTreeNodeStatus.Active)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                return TechTreeNodeStatus.Unlock_Ready_Active;
            }
            return TechTreeNodeStatus.Unlock_Ban_Active;
        }

        public void RegisterCallback(OnTechTreeNodeActive activeHandler)
        {
            this._activeHandler = activeHandler;
        }

        public void UnRegisterCallback()
        {
            this._activeHandler = null;
        }

        public TechTreeNodeStatus _status
        {
            get
            {
                return this._internal_status;
            }
            set
            {
                if (((this._status_before_reset == TechTreeNodeStatus.Unlock_Ready_Active) && (this._internal_status != TechTreeNodeStatus.Active)) && ((value == TechTreeNodeStatus.Active) && (this._activeHandler != null)))
                {
                    this._activeHandler();
                }
                this._internal_status = value;
            }
        }

        public string IconPath
        {
            get
            {
                return this._metaData.Icon;
            }
        }
    }
}

