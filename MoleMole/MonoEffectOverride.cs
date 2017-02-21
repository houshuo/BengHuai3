namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [fiInspectorOnly]
    public class MonoEffectOverride : MonoBehaviour
    {
        public Dictionary<string, Color> colorOverrides;
        public Dictionary<string, string> effectOverlays;
        public Dictionary<string, string> effectOverrides;
        public List<string> effectPredicates;
        public Dictionary<string, float> floatOverrides;
        public Dictionary<string, Material> materialOverrides;

        private void Awake()
        {
            this.materialOverrides = new Dictionary<string, Material>();
            this.colorOverrides = new Dictionary<string, Color>();
            this.effectOverlays = new Dictionary<string, string>();
            this.floatOverrides = new Dictionary<string, float>();
            this.effectOverrides = new Dictionary<string, string>();
            this.effectPredicates = new List<string>();
        }
    }
}

