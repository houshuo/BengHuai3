namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class DialogDataItem
    {
        public string audio;
        public int avatarID;
        public int dialogID;
        public List<DialogMetaData.PlotChatNode> plotChatNodeList;
        public MonoStoryScreen.SelectScreenSide screenSide;
        public string source;

        public DialogDataItem(DialogMetaData dialogMetaData)
        {
            this.dialogID = dialogMetaData.dialogID;
            this.avatarID = dialogMetaData.avatarID;
            if (dialogMetaData.screenSide == 0)
            {
                this.screenSide = MonoStoryScreen.SelectScreenSide.Left;
            }
            else if (dialogMetaData.screenSide == 1)
            {
                this.screenSide = MonoStoryScreen.SelectScreenSide.Right;
            }
            else
            {
                this.screenSide = MonoStoryScreen.SelectScreenSide.None;
            }
            this.source = dialogMetaData.source;
            this.plotChatNodeList = dialogMetaData.content;
            this.audio = dialogMetaData.audio;
        }

        public DialogDataItem(int dialogID, int avatarID, int side, string source, List<DialogMetaData.PlotChatNode> plotChatNodeList, string audio)
        {
            this.dialogID = dialogID;
            this.avatarID = avatarID;
            if (side == 0)
            {
                this.screenSide = MonoStoryScreen.SelectScreenSide.Left;
            }
            else if (side == 1)
            {
                this.screenSide = MonoStoryScreen.SelectScreenSide.Right;
            }
            else
            {
                this.screenSide = MonoStoryScreen.SelectScreenSide.None;
            }
            this.source = source;
            this.plotChatNodeList = plotChatNodeList;
            this.audio = audio;
        }

        public override string ToString()
        {
            return string.Format("<DialogDataItem>\ndialogID: {0}\navatarID: {1}\nsource: {2}\n", this.dialogID, this.avatarID, this.source);
        }
    }
}

