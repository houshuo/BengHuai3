namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [ExecuteInEditMode]
    public class WeaponIconMaker : MonoBehaviour
    {
        private List<GameObject> _objList;
        private PostFX _postFX;
        private GameObject _weaponObj;
        private static readonly string BOUNDING_NAME = "Bounding";
        public Camera cannonCamera;
        public Transform cannonHolder;
        private static readonly string CLONE_SUFFIX = "(Clone)";
        [Range(1f, 179f)]
        public float fov = 20f;
        public int imageSize = 0x200;
        public Camera katanaCamera;
        private static readonly string LEFT_PISTOL_NAME = "LeftPistol";
        public Transform leftPistolHolder;
        private static readonly string LONG_SWORD_NAME = "LongSword";
        public Transform longSwordHolder;
        public Camera mainCamera;
        public string outputPath;
        public Camera pistolCamera;
        private static readonly string RIGHT_PISTOL_NAME = "RightPistol";
        public Transform rightPistolHolder;
        private static readonly string SHORT_SWORD_NAME = "ShortSword";
        public Transform shortSwordHolder;
        private static readonly string[] TYPE_PREFIX = new string[] { "Weapon_Cannon", "Weapon_Katana", "Weapon_Pistol" };
        public WeaponType weaponType;
        public bool wholeForKatana;

        private string _generate()
        {
            if (this._weaponObj == null)
            {
                return "Please load prefab first";
            }
            if (this.weaponType == WeaponType.Cannon)
            {
                this.SaveImage(this._weaponName);
            }
            else if (this.weaponType == WeaponType.Katana)
            {
                GameObject gameObject = this._weaponObj.transform.FindChild(LONG_SWORD_NAME).gameObject;
                GameObject obj3 = this._weaponObj.transform.FindChild(SHORT_SWORD_NAME).gameObject;
                if ((gameObject == null) || (obj3 == null))
                {
                    return "Sword missing";
                }
                gameObject.SetActive(true);
                obj3.SetActive(false);
                this.SaveImage(this._weaponName + "_" + LONG_SWORD_NAME);
                gameObject.SetActive(false);
                obj3.SetActive(true);
                this.SaveImage(this._weaponName + "_" + SHORT_SWORD_NAME);
                gameObject.SetActive(true);
                obj3.SetActive(true);
            }
            else
            {
                GameObject obj4 = this._weaponObj.transform.FindChild(LEFT_PISTOL_NAME).gameObject;
                GameObject obj5 = this._weaponObj.transform.FindChild(RIGHT_PISTOL_NAME).gameObject;
                if ((obj4 == null) || (obj5 == null))
                {
                    return "Missing pistol";
                }
                obj4.SetActive(true);
                obj5.SetActive(false);
                this.SaveImage(this._weaponName + "_" + LEFT_PISTOL_NAME);
                obj4.SetActive(false);
                obj5.SetActive(true);
                this.SaveImage(this._weaponName + "_" + RIGHT_PISTOL_NAME);
                obj4.SetActive(true);
                obj5.SetActive(true);
                this.SaveImage(this._weaponName);
            }
            return null;
        }

        private string _load(string srcPath)
        {
            Screen.SetResolution(this.imageSize, this.imageSize, false);
            GameObject original = Miscs.LoadResource<GameObject>(srcPath, BundleType.RESOURCE_FILE);
            if (original == null)
            {
                return ("Fail to load prefab at " + srcPath);
            }
            foreach (GameObject obj3 in this._objList)
            {
                if (obj3 != null)
                {
                    UnityEngine.Object.DestroyImmediate(obj3);
                }
            }
            this._objList.Clear();
            this._weaponObj = UnityEngine.Object.Instantiate<GameObject>(original);
            string str = this.CheckType();
            if (!string.IsNullOrEmpty(str))
            {
                return str;
            }
            ClearTransform(this._weaponObj);
            this._objList.Add(this._weaponObj);
            if (this.weaponType == WeaponType.Cannon)
            {
                this.mainCamera.CopyFrom(this.cannonCamera);
                this._weaponObj.transform.SetParent(this.cannonHolder, false);
                ClearTransform(this._weaponObj);
                this.AdjustTransform(this._weaponObj, this.cannonHolder.FindChild(BOUNDING_NAME).gameObject);
                this._weaponObj.transform.SetParent(null);
                this.SetCannonMaterial(this._weaponObj);
            }
            else if (this.weaponType == WeaponType.Katana)
            {
                this.mainCamera.CopyFrom(this.katanaCamera);
                GameObject gameObject = this._weaponObj.transform.FindChild(LONG_SWORD_NAME).gameObject;
                GameObject obj5 = this._weaponObj.transform.FindChild(SHORT_SWORD_NAME).gameObject;
                if ((gameObject == null) || (obj5 == null))
                {
                    return "Sword missing";
                }
                gameObject.transform.SetParent(this.longSwordHolder, false);
                ClearTransform(gameObject);
                if (this.wholeForKatana)
                {
                    this.AdjustTransform(gameObject, this.longSwordHolder.FindChild(BOUNDING_NAME).gameObject);
                }
                gameObject.transform.SetParent(this._weaponObj.transform);
                obj5.transform.SetParent(this.shortSwordHolder, false);
                ClearTransform(obj5);
                if (this.wholeForKatana)
                {
                    this.AdjustTransform(obj5, this.longSwordHolder.FindChild(BOUNDING_NAME).gameObject);
                }
                obj5.transform.SetParent(this._weaponObj.transform);
                this.SetKatanaMaterial(this._weaponObj);
            }
            else
            {
                this.mainCamera.CopyFrom(this.pistolCamera);
                GameObject obj6 = this._weaponObj.transform.FindChild(LEFT_PISTOL_NAME).gameObject;
                GameObject obj7 = this._weaponObj.transform.FindChild(RIGHT_PISTOL_NAME).gameObject;
                if ((obj6 == null) || (obj7 == null))
                {
                    return "Missing pistol";
                }
                obj6.transform.SetParent(this.leftPistolHolder, false);
                ClearTransform(obj6);
                this.AdjustTransform(obj6, this.leftPistolHolder.FindChild(BOUNDING_NAME).gameObject);
                obj6.transform.SetParent(this._weaponObj.transform);
                obj7.transform.SetParent(this.rightPistolHolder, false);
                ClearTransform(obj7);
                this.AdjustTransform(obj7, this.rightPistolHolder.FindChild(BOUNDING_NAME).gameObject);
                obj7.transform.SetParent(this._weaponObj.transform);
                this.SetPistolMaterial(this._weaponObj);
            }
            this.fov = this.mainCamera.fieldOfView;
            return null;
        }

        [DebuggerHidden]
        private IEnumerator _waitForEndOfFrame()
        {
            return new <_waitForEndOfFrame>c__Iterator50();
        }

        private void AdjustTransform(GameObject obj, GameObject boundObj)
        {
            float num2;
            float xMinZ = 0f;
            Rect boundsInScreen = this.GetBoundsInScreen(boundObj, ref xMinZ);
            Rect rect2 = this.GetBoundsInScreen(obj, ref xMinZ);
            Vector3 position = new Vector3(rect2.xMin, rect2.center.y, xMinZ);
            Vector3 vector2 = new Vector3(boundsInScreen.xMin, boundsInScreen.center.y, xMinZ);
            Transform transform = obj.transform;
            transform.position += this.mainCamera.ScreenToWorldPoint(vector2) - this.mainCamera.ScreenToWorldPoint(position);
            rect2 = this.GetBoundsInScreen(obj, ref xMinZ);
            if ((rect2.width / rect2.height) < (boundsInScreen.width / boundsInScreen.height))
            {
                num2 = boundsInScreen.height / rect2.height;
            }
            else
            {
                num2 = boundsInScreen.width / rect2.width;
            }
            obj.transform.SetLocalScaleX(num2);
            obj.transform.SetLocalScaleY(num2);
            obj.transform.SetLocalScaleZ(num2);
            rect2 = this.GetBoundsInScreen(obj, ref xMinZ);
            position = new Vector3(rect2.xMin, rect2.center.y, xMinZ);
            Transform transform2 = obj.transform;
            transform2.position += this.mainCamera.ScreenToWorldPoint(vector2) - this.mainCamera.ScreenToWorldPoint(position);
        }

        private void Awake()
        {
            this._objList = new List<GameObject>();
            this._postFX = this.mainCamera.GetComponent<PostFX>();
        }

        private string CheckType()
        {
            for (int i = 0; i < TYPE_PREFIX.Length; i++)
            {
                if (this._weaponName.StartsWith(TYPE_PREFIX[i]))
                {
                    this.weaponType = (WeaponType) i;
                    return null;
                }
            }
            return "Invalid weapon type";
        }

        private static void ClearTransform(GameObject obj)
        {
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
        }

        public string Generate()
        {
            return this._generate();
        }

        private Rect GetBoundsInScreen(GameObject obj, ref float xMinZ)
        {
            Rect rect = new Rect();
            bool flag = true;
            foreach (MeshFilter filter in obj.GetComponentsInChildren<MeshFilter>(true))
            {
                Mesh sharedMesh = filter.sharedMesh;
                for (int i = 0; i < sharedMesh.vertexCount; i++)
                {
                    Vector3 position = sharedMesh.vertices[i];
                    Vector3 vector2 = this.mainCamera.WorldToScreenPoint(filter.transform.TransformPoint(position));
                    if (flag)
                    {
                        rect.min = vector2;
                        rect.max = vector2;
                        xMinZ = vector2.z;
                        flag = false;
                    }
                    else
                    {
                        if (rect.xMin > vector2.x)
                        {
                            rect.xMin = vector2.x;
                            xMinZ = vector2.z;
                        }
                        rect.xMax = Mathf.Max(rect.xMax, vector2.x);
                        rect.yMin = Mathf.Min(rect.yMin, vector2.y);
                        rect.yMax = Mathf.Max(rect.yMax, vector2.y);
                    }
                }
            }
            return rect;
        }

        public string Load(string srcPath)
        {
            return this._load(srcPath);
        }

        private void MakeSingle(GameObject obj, Transform holder, bool needAdjust = false)
        {
            this._objList.Add(obj);
            obj.transform.SetParent(holder, false);
            ClearTransform(obj);
            if (needAdjust)
            {
                this.AdjustTransform(obj, holder.FindChild(BOUNDING_NAME).gameObject);
            }
        }

        private void SaveImage(string fileName)
        {
            RenderTextureWrapper wrapper = GraphicsUtils.GetRenderTexture(this.imageSize, this.imageSize, 0, RenderTextureFormat.ARGB32);
            this.mainCamera.targetTexture = (RenderTexture) wrapper;
            this._postFX.WriteAlpha = true;
            this.mainCamera.Render();
            this._postFX.WriteAlpha = false;
            this.mainCamera.targetTexture = null;
            RenderTexture.active = (RenderTexture) wrapper;
            Texture2D textured = new Texture2D(wrapper.width, wrapper.height, TextureFormat.ARGB32, false);
            textured.ReadPixels(new Rect(0f, 0f, (float) wrapper.width, (float) wrapper.height), 0, 0);
            textured.Apply();
            byte[] bytes = textured.EncodeToPNG();
            File.WriteAllBytes(string.Format("{0}/{1}.png", this.outputPath, fileName), bytes);
            UnityEngine.Object.Destroy(textured);
            GraphicsUtils.ReleaseRenderTexture(wrapper);
        }

        private void SetCannonMaterial(GameObject obj)
        {
            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>(true))
            {
                foreach (Material material in renderer.materials)
                {
                    if (material.HasProperty("_ColorTransFactor"))
                    {
                        material.SetFloat("_ColorTransFactor", 1f);
                    }
                    if (material.HasProperty("_Opaqueness"))
                    {
                        material.SetFloat("_Opaqueness", 1f);
                    }
                }
            }
        }

        public void SetFOV(float targetFOV)
        {
            Vector3 position = this._weaponObj.transform.position;
            position = this.mainCamera.worldToCameraMatrix.MultiplyPoint(position);
            float num = Mathf.Tan((this.mainCamera.fieldOfView * 0.01745329f) * 0.5f);
            float num2 = Mathf.Tan((targetFOV * 0.01745329f) * 0.5f);
            position.z *= num / num2;
            position = this.mainCamera.cameraToWorldMatrix.MultiplyPoint(position);
            this._weaponObj.transform.position = position;
            this.mainCamera.fieldOfView = targetFOV;
        }

        private void SetKatanaMaterial(GameObject obj)
        {
            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i] = new Material(renderer.materials[i]);
                    Material material1 = renderer.materials[i];
                    material1.name = material1.name + "(Instance)";
                }
            }
        }

        private void SetPistolMaterial(GameObject obj)
        {
            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i] = new Material(renderer.materials[i]);
                    Material material1 = renderer.materials[i];
                    material1.name = material1.name + "(Instance)";
                }
            }
        }

        private static string TrimSuffix(string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            return s;
        }

        private string _weaponName
        {
            get
            {
                if (this._weaponObj != null)
                {
                    return TrimSuffix(this._weaponObj.name, CLONE_SUFFIX);
                }
                return null;
            }
        }

        [CompilerGenerated]
        private sealed class <_waitForEndOfFrame>c__Iterator50 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public enum WeaponType
        {
            Cannon,
            Katana,
            Pistol
        }
    }
}

