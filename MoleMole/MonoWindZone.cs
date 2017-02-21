namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoWindZone : MonoBehaviour
    {
        private float _time;
        private Transform _trsf;
        public float BushesFrequency = 0.05f;
        public float BushesMagnitude = 0.1f;
        public float LeavesFrequency = 0.05f;
        public float LeavesMagnitude = 0.05f;
        public GameObject LeavesRootObject;
        public float RotateFrequency = 0.07f;
        public float RotateMagnitude = 0.05f;
        [Header("Objects to effect")]
        public GameObject TrunksObject;

        public void Awake()
        {
            this._trsf = base.transform;
            this._time = 0f;
        }

        public void Init()
        {
            new TreesPreprocessor(this.TrunksObject, this.LeavesRootObject).Process();
        }

        private void Update()
        {
            this._time += Time.deltaTime * this._TimeScale;
            Vector3 forward = this._trsf.forward;
            Shader.SetGlobalVector("_miHoYo_Wind", new Vector4(forward.x, forward.y, forward.z, this._time));
            Shader.SetGlobalVector("_miHoYo_WindParams1", new Vector4(this.LeavesMagnitude, this.LeavesFrequency, this.RotateMagnitude, this.RotateFrequency));
            Shader.SetGlobalVector("_miHoYo_WindParams2", new Vector4(this.BushesMagnitude, this.BushesFrequency));
        }

        private float _TimeScale
        {
            get
            {
                LevelManager instance = Singleton<LevelManager>.Instance;
                if (instance == null)
                {
                    return 1f;
                }
                return instance.levelEntity.TimeScale;
            }
        }
    }
}

