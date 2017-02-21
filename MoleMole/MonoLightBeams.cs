namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [ExecuteInEditMode]
    public class MonoLightBeams : MonoBehaviour
    {
        [Header("Swing amplitude range (degree)")]
        public Vector2 amplitudeRange = new Vector2(30f, 60f);
        private Beams[] beams;
        [Header("Swing cycle time range")]
        public Vector2 cycleTimeRange = new Vector2(5f, 20f);

        private void Init()
        {
            List<Beams> list = new List<Beams>();
            int seed = UnityEngine.Random.seed;
            UnityEngine.Random.seed = 0;
            foreach (Transform transform in base.GetComponentsInChildren<Transform>())
            {
                if (transform != base.transform)
                {
                    Beams beams;
                    beams = new Beams {
                        amplitude = UnityEngine.Random.Range(this.amplitudeRange.x, this.amplitudeRange.y),
                        cycleTime = Mathf.Max(UnityEngine.Random.Range(this.cycleTimeRange.x, this.cycleTimeRange.y), 0.01f),
                        phaseTime = UnityEngine.Random.Range(0f, beams.cycleTime),
                        transform = transform
                    };
                    list.Add(beams);
                }
            }
            UnityEngine.Random.seed = seed;
            this.beams = list.ToArray();
        }

        private void OnDestroy()
        {
        }

        private void OnDisable()
        {
        }

        private void OnEnable()
        {
            this.Init();
        }

        private void Update()
        {
            float deltaTime;
            if (Application.isPlaying)
            {
                deltaTime = Time.deltaTime;
            }
            else
            {
                deltaTime = 0.03333334f;
            }
            for (int i = 0; i < this.beams.Length; i++)
            {
                this.beams[i].phaseTime += deltaTime;
                float f = ((this.beams[i].phaseTime * 2f) * 3.141593f) / this.beams[i].cycleTime;
                float num4 = Mathf.Sin(f);
                this.beams[i].transform.SetLocalEulerAnglesZ(num4 * this.beams[0].amplitude);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Beams
        {
            public float amplitude;
            public float cycleTime;
            public float phaseTime;
            public Transform transform;
        }
    }
}

