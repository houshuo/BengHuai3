namespace MoleMole
{
    using System;

    public class ActDataItem
    {
        private ActMetaData _metaData;
        public int actId;
        public int chapterId;
        public string levelPanelPath;

        public ActDataItem(int actId)
        {
            this._metaData = ActMetaDataReader.GetActMetaDataByKey(actId);
            this.actId = this._metaData.actId;
            this.chapterId = this._metaData.chapterId;
            this.levelPanelPath = this._metaData.levelPannelPath;
        }

        public int actIndex
        {
            get
            {
                return (this._metaData.numInChapter - 1);
            }
        }

        public string actName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.actName, new object[0]);
            }
        }

        public string actTitle
        {
            get
            {
                return ((this._metaData.actType != 2) ? ("Act." + this._metaData.numInChapter) : "Act.Extra");
            }
        }

        public ActType actType
        {
            get
            {
                return (ActType) this._metaData.actType;
            }
        }

        public string BGImgPath
        {
            get
            {
                return this._metaData.bgImgPath;
            }
        }

        public string smallImgPath
        {
            get
            {
                return this._metaData.smallImgPath;
            }
        }

        public enum ActType
        {
            Extra = 2,
            Normal = 1
        }
    }
}

