namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoStageEnv : MonoBehaviour
    {
        public Transform lightForwardTransform;

        public void Awake()
        {
            this.spawnPoints = base.transform.GetComponentsInChildren<MonoSpawnPoint>();
        }

        public int GetNamedSpawnPointIx(string name)
        {
            for (int i = 0; i < this.spawnPoints.Length; i++)
            {
                if (this.spawnPoints[i].name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public MonoSpawnPoint[] spawnPoints { get; private set; }
    }
}

