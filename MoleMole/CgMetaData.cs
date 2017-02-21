namespace MoleMole
{
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class CgMetaData : IHashable
    {
        public readonly string CgIconSpritePath;
        public readonly int CgID;
        public readonly string CgPath;
        public readonly int levelID;

        public CgMetaData(int CgID, int levelID, string CgPath, string CgIconSpritePath)
        {
            this.CgID = CgID;
            this.levelID = levelID;
            this.CgPath = CgPath;
            this.CgIconSpritePath = CgIconSpritePath;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.CgID, ref lastHash);
            HashUtils.ContentHashOnto(this.levelID, ref lastHash);
            HashUtils.ContentHashOnto(this.CgPath, ref lastHash);
            HashUtils.ContentHashOnto(this.CgIconSpritePath, ref lastHash);
        }
    }
}

