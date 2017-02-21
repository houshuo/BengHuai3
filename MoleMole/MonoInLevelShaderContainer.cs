namespace MoleMole
{
    using UnityEngine;

    public class MonoInLevelShaderContainer : MonoBehaviour
    {
        [Header("shaders need to be warm up in level, and dontdestroyonload, because they are small in memory")]
        public Shader[] shaders;
    }
}

