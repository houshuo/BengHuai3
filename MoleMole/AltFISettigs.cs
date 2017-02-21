namespace MoleMole
{
    using FullInspector;
    using System;

    public class AltFISettigs : fiSettingsProcessor
    {
        public void Process()
        {
            fiSettings.GetTypeNameSpaceFormat = "MoleMole.Config.{0},Assembly-CSharp";
            fiSettings.EmitWarnings = true;
            fiSettings.EnableAnimation = false;
            fiSettings.AutomaticReferenceInstantation = false;
            FullSerializerSerializer.AddProcessor<ConfigIOnLoadedConverter>();
        }
    }
}

