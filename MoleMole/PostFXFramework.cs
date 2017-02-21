namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public abstract class PostFXFramework : MonoBehaviour
    {
        protected bool _isSupported = true;

        protected PostFXFramework()
        {
        }

        protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
        {
            if (s == null)
            {
                Debug.LogError("Missing shader in " + this);
                base.enabled = false;
                return null;
            }
            if ((s.isSupported && (m2Create != null)) && (m2Create.shader == s))
            {
                return m2Create;
            }
            if (!s.isSupported)
            {
                this.NotSupported();
                Debug.LogError(string.Concat(new object[] { "The shader ", s, " on effect ", this, " is not supported on this platform!" }));
                return null;
            }
            m2Create = new Material(s);
            m2Create.hideFlags = HideFlags.DontSave;
            if (m2Create != null)
            {
                return m2Create;
            }
            return null;
        }

        protected void CreateCamera(Camera srcCam, ref Camera destCam, string name = null, HideFlags hideFlags = 0x3d)
        {
            if ((srcCam != null) && (destCam == null))
            {
                string str = !string.IsNullOrEmpty(name) ? name : ("__RefCamera for " + srcCam.GetInstanceID());
                System.Type[] components = new System.Type[] { typeof(Camera) };
                GameObject obj2 = new GameObject(str, components);
                destCam = obj2.GetComponent<Camera>();
                destCam.enabled = false;
                obj2.hideFlags = hideFlags;
            }
        }

        protected void NotSupported()
        {
            base.enabled = false;
            this._isSupported = false;
        }

        protected virtual void OnEnable()
        {
        }
    }
}

