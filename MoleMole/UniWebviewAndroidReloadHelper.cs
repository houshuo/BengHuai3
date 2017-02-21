namespace MoleMole
{
    using System;
    using UnityEngine;

    public class UniWebviewAndroidReloadHelper : MonoBehaviour
    {
        public void OnApplicationPause(bool pause)
        {
            UniWebView component = base.GetComponent<UniWebView>();
            if (component != null)
            {
                if (pause)
                {
                    component.Hide(false, UniWebViewTransitionEdge.None, 0.4f, null);
                }
                else
                {
                    component.Show(false, UniWebViewTransitionEdge.None, 0.4f, null);
                }
            }
        }
    }
}

