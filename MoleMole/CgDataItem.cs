namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class CgDataItem
    {
        public string cgIconPath;
        public int cgID;
        public string cgPath;
        public int levelID;

        public CgDataItem(CgMetaData cgMetaData)
        {
            this.cgID = cgMetaData.CgID;
            this.levelID = cgMetaData.levelID;
            this.cgPath = cgMetaData.CgPath;
            this.cgIconPath = cgMetaData.CgIconSpritePath;
        }

        public CgDataItem(int cgID, int levelID, string cgPath, string iconPath = "")
        {
            this.cgID = cgID;
            this.levelID = levelID;
            this.cgPath = cgPath;
            this.cgIconPath = iconPath;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.cgID, this.levelID, this.cgPath, this.cgIconPath };
            return string.Format("<CgDataItem>\ncgID: {0}\nlevelID: {1}\nCgPaht: {2}\nCgIconSpritePath : {3}", args);
        }
    }
}

