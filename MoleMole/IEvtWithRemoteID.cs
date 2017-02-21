namespace MoleMole
{
    using System;

    public interface IEvtWithRemoteID
    {
        uint GetChannelID();
        uint GetRemoteID();
        uint GetSenderID();
    }
}

