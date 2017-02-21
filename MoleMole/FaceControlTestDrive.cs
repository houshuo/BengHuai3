namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class FaceControlTestDrive : MonoBehaviour
    {
        private int _currentFaceIndex;
        public Transform cameraTransform;
        public MonoAvatarFaceControl faceControl;
        public Transform lookAtEdit;
        public Transform lookAtNormal;
        public Transform positionEdit;
        public Transform positionNormal;

        private void OnGUI()
        {
        }
    }
}

