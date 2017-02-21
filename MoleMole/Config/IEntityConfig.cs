namespace MoleMole.Config
{
    using System;

    public interface IEntityConfig
    {
        ConfigEntityAnimEvent TryGetAnimEvent(string animEventID);
    }
}

