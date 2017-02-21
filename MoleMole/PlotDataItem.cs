namespace MoleMole
{
    using System;

    public class PlotDataItem
    {
        public int endDialogID;
        public int levelID;
        public int plotID;
        public int startDialogID;

        public PlotDataItem(PlotMetaData plotMetaData)
        {
            this.plotID = plotMetaData.plotID;
            this.levelID = plotMetaData.levelID;
            this.startDialogID = plotMetaData.startDialogID;
            this.endDialogID = plotMetaData.endDialogID;
        }

        public PlotDataItem(int plotID, int levelID, int startDialogID, int endDialogID)
        {
            this.plotID = plotID;
            this.levelID = levelID;
            this.startDialogID = startDialogID;
            this.endDialogID = endDialogID;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.plotID, this.levelID, this.startDialogID, this.endDialogID };
            return string.Format("<PlotDataItem>\nplotID: {0}\nlevelID: {1}\nstartDialogID: {2}\nendDialogID: {3}", args);
        }
    }
}

