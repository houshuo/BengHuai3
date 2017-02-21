namespace MoleMole
{
    using proto;
    using System;

    public class CabinAvatarEnhanceDataItem : CabinDataItemBase
    {
        public AvatarClassType _classType;

        public float GetAvatarAttrEnhance(AvatarAttrType attrType)
        {
            float num = 0f;
            foreach (CabinTechTreeNode node in base._techTree.GetActiveNodeList())
            {
                CabinTechTreeMetaData data = node._metaData;
                if (data.Argument1 == attrType)
                {
                    num += ((float) data.Argument2) / 100f;
                }
            }
            return num;
        }

        public int GetTotalEnhancePoint()
        {
            return base._techTree.GetActiveNodeList().Count;
        }

        public void SetAvatarClassType(CabinType cabinType)
        {
            if (cabinType == 2)
            {
                this._classType = 1;
            }
            else if (cabinType == 6)
            {
                this._classType = 2;
            }
            else if (cabinType == 7)
            {
                this._classType = 3;
            }
        }
    }
}

