namespace MoleMole
{
    using System;

    public class MonoDummyDynamicObject : BaseMonoDynamicObject
    {
        private bool _isToBeRemoved;

        public override bool IsActive()
        {
            return !this._isToBeRemoved;
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemoved;
        }

        public override void SetDied()
        {
            base.SetDied();
            this._isToBeRemoved = true;
            Singleton<EffectManager>.Instance.ClearEffectsByOwner(base._runtimeID);
        }
    }
}

