namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoGameEntryElevator : MonoBehaviour
    {
        private int _playBackgroundAnimCount;
        private const int _playBackgroundAnimLoopCount = 500;

        private void Awake()
        {
            this.EnablePlayBackgroundAnim = false;
        }

        private void FixedUpdate()
        {
            if (this.EnablePlayBackgroundAnim)
            {
                if ((this._playBackgroundAnimCount == 0) || ((this._playBackgroundAnimCount % 500) == 0))
                {
                    this.PlayBackgroundAnimation();
                    this._playBackgroundAnimCount = 0;
                }
                this._playBackgroundAnimCount++;
            }
        }

        [AnimationCallback]
        public void OnDoorAnimOver()
        {
            (Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry).OnElevatorDoorAnimOver();
        }

        [AnimationCallback]
        public void OnFloorAnimEvent(int phase)
        {
            (Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry).OnElevatorFloorAnimEvent(phase);
        }

        private void PlayBackgroundAnimation()
        {
            base.transform.GetComponent<Animation>().Blend("Background");
        }

        private void Start()
        {
            PostFX component = Camera.main.GetComponent<PostFX>();
            if (component != null)
            {
                component.WriteDepthTexture = true;
            }
        }

        public bool EnablePlayBackgroundAnim { get; set; }
    }
}

