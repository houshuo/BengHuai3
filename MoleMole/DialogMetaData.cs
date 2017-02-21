namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DialogMetaData : IHashable
    {
        public readonly string audio;
        public readonly int avatarID;
        public readonly List<PlotChatNode> content;
        public readonly int dialogID;
        public readonly int screenSide;
        public readonly string source;

        public DialogMetaData(int dialogID, int avatarID, int screenSide, string source, List<PlotChatNode> content, string audio)
        {
            this.dialogID = dialogID;
            this.avatarID = avatarID;
            this.screenSide = screenSide;
            this.source = source;
            this.content = content;
            this.audio = audio;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.dialogID, ref lastHash);
            HashUtils.ContentHashOnto(this.avatarID, ref lastHash);
            HashUtils.ContentHashOnto(this.screenSide, ref lastHash);
            HashUtils.ContentHashOnto(this.source, ref lastHash);
            if (this.content != null)
            {
                foreach (PlotChatNode node in this.content)
                {
                    HashUtils.ContentHashOnto(node.chatContent, ref lastHash);
                    HashUtils.ContentHashOnto(node.chatDuration, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.audio, ref lastHash);
        }

        public class PlotChatNode
        {
            public readonly string chatContent;
            public readonly float chatDuration;

            public PlotChatNode(string nodeString)
            {
                if (nodeString.Contains(":"))
                {
                    char[] seperator = new char[] { ':' };
                    List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
                    this.chatContent = stringListFromString[0].Trim();
                    this.chatDuration = float.Parse(stringListFromString[1]);
                }
                else
                {
                    this.chatContent = nodeString;
                    this.chatDuration = MiscData.Config.BasicConfig.DefaultChatDuration;
                }
            }

            public PlotChatNode(string chatContent, float chatDuration)
            {
                this.chatContent = chatContent;
                this.chatDuration = chatDuration;
            }
        }
    }
}

