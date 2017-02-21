namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class ConfigIntToString : MonoBehaviour
    {
        public FaceAnimationConvertItem[] items;

        private void ConvertBlocks(FaceAnimationFrameBlock[] blocks, string[] names)
        {
        }

        private void ConvertConfig(FaceAnimationConvertItem item)
        {
            int index = 0;
            int length = item.config.items.Length;
            while (index < length)
            {
                FaceAnimationItem item2 = item.config.items[index];
                this.ConvertBlocks(item2.leftEyeBlocks, item.leftEyeProvider.GetMatInfoNames());
                this.ConvertBlocks(item2.rightEyeBlocks, item.rightEyeProvider.GetMatInfoNames());
                this.ConvertBlocks(item2.mouthBlocks, item.mouthProvider.GetMatInfoNames());
                index++;
            }
        }

        private void Execute()
        {
            int index = 0;
            int length = this.items.Length;
            while (index < length)
            {
                this.ConvertConfig(this.items[index]);
                index++;
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Execute", new GUILayoutOption[0]))
            {
                this.Execute();
            }
        }
    }
}

