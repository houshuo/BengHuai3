namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoCabinOverViewAnimation : MonoBehaviour
    {
        public Animation _animation;

        public void AnimationStart()
        {
            this._animation.Play();
        }

        public void AnimationStop()
        {
            this._animation.Stop();
        }
    }
}

