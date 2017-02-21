namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    public class GalTouchTestAvatar : MonoBehaviour, IBodyPartTouchable
    {
        public int avatarId = 0x65;
        private GalTouchSystem galTouchSystem;
        public Transform headRoot;
        public int heartLevel = 1;
        public TestMatInfoProvider leftEyeProvider;
        public Renderer leftEyeRenderer;
        public TestMatInfoProvider mouthProvider;
        public Renderer mouthRenderer;
        public TestMatInfoProvider rightEyeProvider;
        public Renderer rightEyeRenderer;
        public GameObject[] switchObjects;

        public void BodyPartTouched(BodyPartType type, Vector3 point)
        {
            this.galTouchSystem.BodyPartTouched(type);
        }

        private void OnGalTouchSystemIdleChanged(bool idle)
        {
            if (idle)
            {
                int index = 0;
                int length = this.switchObjects.Length;
                while (index < length)
                {
                    this.switchObjects[index].SetActive(false);
                    index++;
                }
            }
        }

        public void ResetGalTouchSystem()
        {
            this.galTouchSystem.Init(base.GetComponent<Animator>(), this.avatarId, this.heartLevel, this.leftEyeRenderer, this.rightEyeRenderer, this.mouthRenderer, this.leftEyeProvider, this.rightEyeProvider, this.mouthProvider, this.headRoot);
            this.galTouchSystem.enable = true;
        }

        private void Start()
        {
            this.galTouchSystem = new GalTouchSystem();
            this.ResetGalTouchSystem();
            MonoBodyPart[] componentsInChildren = base.GetComponentsInChildren<MonoBodyPart>();
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                componentsInChildren[index].SetBodyPartTouchable(this);
                index++;
            }
            this.galTouchSystem.enable = true;
            this.galTouchSystem.IdleChanged += new UnityAction<bool>(this.OnGalTouchSystemIdleChanged);
        }

        public void SwitchOff(string name)
        {
            int index = 0;
            int length = this.switchObjects.Length;
            while (index < length)
            {
                if (this.switchObjects[index].name == name)
                {
                    this.switchObjects[index].SetActive(false);
                    return;
                }
                index++;
            }
        }

        public void SwitchOn(string name)
        {
            int index = 0;
            int length = this.switchObjects.Length;
            while (index < length)
            {
                if (this.switchObjects[index].name == name)
                {
                    this.switchObjects[index].SetActive(true);
                    return;
                }
                index++;
            }
        }

        public void TriggerAudioPattern(string name)
        {
            Singleton<WwiseAudioManager>.Instance.Post(name, null, null, null);
        }

        private void Update()
        {
            if (this.galTouchSystem != null)
            {
                this.galTouchSystem.Process(Time.deltaTime);
            }
        }
    }
}

