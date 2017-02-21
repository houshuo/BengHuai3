namespace MoleMole
{
    using FullInspector;
    using System.Collections.Generic;
    using UnityEngine;

    [fiInspectorOnly]
    public class MonoEditorUIContextInspector : MonoBehaviour
    {
        [InspectorCollapsedFoldout]
        public ViewCache diaglogCache;
        [InspectorCollapsedFoldout]
        public ViewCache pageCache;
        [InspectorCollapsedFoldout]
        public List<BasePageContext> pageContextStack = new List<BasePageContext>();
        [InspectorCollapsedFoldout]
        public ViewCache widgetCache;
    }
}

