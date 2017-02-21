namespace MoleMole.MainMenu
{
    using System;

    [Serializable]
    public class CloudScene
    {
        public CloudType[] CloudTypes;
        public CompoundCloudType[] CompoundCloudTypes;
        public LayerCloudType[] LayerCloudTypes;
        public LightningType[] LightningTypes;
        public string Name;

        public void Init(CloudEmitter cloudEmitter)
        {
            foreach (LightningType type in this.LightningTypes)
            {
                type.Init(cloudEmitter);
            }
        }
    }
}

