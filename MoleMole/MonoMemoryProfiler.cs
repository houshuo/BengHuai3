namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoMemoryProfiler : MonoBehaviour
    {
        private E_AssetDetail _assetDetail;
        private AssetList _assetList;
        private static float _autoSampleInterval = 2f;
        private int _heigth = 800;
        private int _leftOffset;
        private uint _memory_animationClip;
        private uint _memory_gameobject;
        private uint _memory_material;
        private uint _memory_mesh;
        private uint _memory_monoHeap;
        private uint _memory_monoUsed;
        private uint _memory_renderTexture;
        private uint _memory_texture;
        private int _moveStep = 10;
        private int _num_gameobject;
        private Vector2 _pivot;
        private static MonoMemoryProfiler _profiler;
        private Vector2 _scale = new Vector2(2f, 2f);
        private float _timeEclapse;
        private int _top = 80;
        private int _width = 600;

        public static void CreateMemoryProfiler()
        {
            if (_profiler == null)
            {
                GameObject target = new GameObject("MemoryProfilerGUI");
                target.AddComponent<MonoMemoryProfiler>();
                UnityEngine.Object.DontDestroyOnLoad(target);
                _profiler = target.GetComponent<MonoMemoryProfiler>();
            }
        }

        private void DoDestroy()
        {
            if (_profiler != null)
            {
                UnityEngine.Object.Destroy(_profiler.gameObject);
                _profiler = null;
            }
        }

        private void DoProfiler()
        {
            this._assetList.Clear();
            foreach (Texture texture in Resources.FindObjectsOfTypeAll(typeof(Texture)))
            {
                uint runtimeMemorySize = (uint) Profiler.GetRuntimeMemorySize(texture);
                if (texture is RenderTexture)
                {
                    this._memory_renderTexture += runtimeMemorySize;
                    if (this._assetDetail == E_AssetDetail.RenderTextures)
                    {
                        this._assetList.TryAdd(runtimeMemorySize, texture.name);
                    }
                }
                else
                {
                    this._memory_texture += runtimeMemorySize;
                    if (this._assetDetail == E_AssetDetail.Textures)
                    {
                        this._assetList.TryAdd(runtimeMemorySize, texture.name);
                    }
                }
            }
            foreach (Mesh mesh in Resources.FindObjectsOfTypeAll(typeof(Mesh)))
            {
                uint size = (uint) Profiler.GetRuntimeMemorySize(mesh);
                this._memory_mesh += size;
                if (this._assetDetail == E_AssetDetail.Meshes)
                {
                    this._assetList.TryAdd(size, mesh.name);
                }
            }
            foreach (AnimationClip clip in Resources.FindObjectsOfTypeAll(typeof(AnimationClip)))
            {
                uint num6 = (uint) Profiler.GetRuntimeMemorySize(clip);
                this._memory_animationClip += num6;
                if (this._assetDetail == E_AssetDetail.AnimationClips)
                {
                    this._assetList.TryAdd(num6, clip.name);
                }
            }
            foreach (Material material in Resources.FindObjectsOfTypeAll(typeof(Material)))
            {
                uint num8 = (uint) Profiler.GetRuntimeMemorySize(material);
                this._memory_material += num8;
                if (this._assetDetail == E_AssetDetail.Meterials)
                {
                    this._assetList.TryAdd(num8, material.name);
                }
            }
            this._assetList.Sort();
            this._num_gameobject = 0;
            foreach (GameObject obj2 in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                this._num_gameobject++;
                uint num10 = (uint) Profiler.GetRuntimeMemorySize(obj2);
                this._memory_gameobject += num10;
            }
            this._memory_texture = this.ToMegaBytes(this._memory_texture);
            this._memory_renderTexture = this.ToMegaBytes(this._memory_renderTexture);
            this._memory_mesh = this.ToMegaBytes(this._memory_mesh);
            this._memory_animationClip = this.ToMegaBytes(this._memory_animationClip);
            this._memory_material = this.ToMegaBytes(this._memory_material);
            this._memory_monoHeap = this.ToMegaBytes(Profiler.GetMonoHeapSize());
            this._memory_monoUsed = this.ToMegaBytes(Profiler.GetMonoUsedSize());
            this._memory_gameobject = this.ToMegaBytes(this._memory_gameobject);
        }

        private void OnGUI()
        {
            this._pivot.x = Screen.width / 2;
            this._pivot.y = 0f;
            GUIUtility.ScaleAroundPivot(this._scale, this._pivot);
            GUI.backgroundColor = Color.black;
            float num = ((Screen.width - this._width) / 2) + this._leftOffset;
            GUILayout.BeginArea(new Rect(Mathf.Clamp(num, 0f, (float) (Screen.width - this._width)), (float) this._top, (float) this._width, (float) this._heigth));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (GUILayout.Button("Memory Profiler", new GUILayoutOption[0]))
            {
                this.DoDestroy();
            }
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(string.Format("Normal Texture: {0} MB", this._memory_texture), new GUILayoutOption[0]))
            {
                this._assetDetail = E_AssetDetail.Textures;
            }
            if (GUILayout.Button(string.Format("Render Texture: {0} MB", this._memory_renderTexture), new GUILayoutOption[0]))
            {
                this._assetDetail = E_AssetDetail.RenderTextures;
            }
            if (GUILayout.Button(string.Format("Mesh:           {0} MB", this._memory_mesh), new GUILayoutOption[0]))
            {
                this._assetDetail = E_AssetDetail.Meshes;
            }
            if (GUILayout.Button(string.Format("AnimationClip:  {0} MB", this._memory_animationClip), new GUILayoutOption[0]))
            {
                this._assetDetail = E_AssetDetail.AnimationClips;
            }
            if (GUILayout.Button(string.Format("Material:       {0} MB", this._memory_material), new GUILayoutOption[0]))
            {
                this._assetDetail = E_AssetDetail.Meterials;
            }
            GUI.backgroundColor = Color.yellow;
            GUILayout.Button(string.Format("Mono Heap:      {0} MB", this._memory_monoHeap), new GUILayoutOption[0]);
            GUILayout.Button(string.Format("Mono Used:      {0} MB", this._memory_monoUsed), new GUILayoutOption[0]);
            GUILayout.Button(string.Format("{0} GameObjects: {1} MB", this._num_gameobject, this._memory_gameobject), new GUILayoutOption[0]);
            GUI.backgroundColor = Color.black;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("Left", new GUILayoutOption[0]))
            {
                this._leftOffset -= this._moveStep;
            }
            if (GUILayout.Button("Right", new GUILayoutOption[0]))
            {
                this._leftOffset += this._moveStep;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUI.backgroundColor = Color.gray;
            if (this._assetDetail != E_AssetDetail.None)
            {
                foreach (AssetList.AssetItem item in this._assetList.GetList())
                {
                    GUILayout.Button(string.Format("{0} KB  {1}", this.ToKiloBytes(item._size), item._name), new GUILayoutOption[0]);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public static bool ParseCommand(string cmd)
        {
            float num;
            if (!cmd.StartsWith("mem", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            _autoSampleInterval = 2f;
            if ((cmd.Length > 3) && float.TryParse(cmd.Substring(3).Trim(), out num))
            {
                _autoSampleInterval = num;
            }
            return true;
        }

        private void Start()
        {
            this._assetList = new AssetList(10);
            this._assetDetail = E_AssetDetail.Textures;
            this.DoProfiler();
        }

        private uint ToKiloBytes(uint _bytes)
        {
            return (_bytes / 0x400);
        }

        private uint ToMegaBytes(uint _bytes)
        {
            return ((_bytes / 0x400) / 0x400);
        }

        private void Update()
        {
            this._timeEclapse += Time.deltaTime;
            if (this._timeEclapse > _autoSampleInterval)
            {
                this._timeEclapse = 0f;
                this.DoProfiler();
            }
        }

        public enum E_AssetDetail
        {
            None,
            Textures,
            RenderTextures,
            Meshes,
            AnimationClips,
            Meterials
        }
    }
}

