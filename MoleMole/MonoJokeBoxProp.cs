namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class MonoJokeBoxProp : MonoBarrelProp
    {
        private bool _isAlive;
        private KillEffect _killEffect;
        private const string TRIGGER_BORN = "BornTrigger";

        [AnimationCallback]
        public void BornEndTriggerDied()
        {
            base.SetDied(this._killEffect);
        }

        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
            base.DestroyDelay = 2.3f;
            this._isAlive = true;
        }

        public override bool IsActive()
        {
            return this._isAlive;
        }

        public override void SetDied(KillEffect killEffect)
        {
            base.SetCountedDenySelect(true, true);
            this._isAlive = false;
            this._killEffect = killEffect;
            this.SetTrigger("BornTrigger");
        }
    }
}

