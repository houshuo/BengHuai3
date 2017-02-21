namespace MoleMole
{
    using System;

    public enum EventRemoteState
    {
        Idle,
        NeedCheckForRemote,
        IsRedirected,
        NeedToReplicateToRemote,
        IsAutorityReceiveRedirected,
        IsRemoteReceiveHandledReplcated
    }
}

