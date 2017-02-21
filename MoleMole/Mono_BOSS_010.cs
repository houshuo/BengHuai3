namespace MoleMole
{
    using System;
    using UnityEngine;

    public class Mono_BOSS_010 : BaseMonoBoss
    {
        private ResizeColliderEntry _curEntry;
        private Vector3 _origCenter;
        private Vector3 _origSize;
        private ResizeState _state;
        [Header("Target BoxCollider to scale")]
        public BoxCollider boxCollider;
        [Header("Scale By Skill ID Entries")]
        public ResizeColliderEntry[] scaleBySkillIDs;

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (this._state == ResizeState.InStateWaiting)
            {
                if (this.GetCurrentNormalizedTime() > this._curEntry.normalizedTimeStart)
                {
                    this.boxCollider.size = this._curEntry.size;
                    this.boxCollider.center = this._curEntry.center;
                    this._state = ResizeState.InStateResized;
                }
            }
            else if ((this._state == ResizeState.InStateResized) && (this.GetCurrentNormalizedTime() > this._curEntry.normalizedTimeStop))
            {
                this.boxCollider.size = this._origSize;
                this.boxCollider.center = this._origCenter;
                this._state = ResizeState.Idle;
            }
        }

        protected override void PostInit()
        {
            base.PostInit();
            base.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.onCurrentSkillIDChanged, new Action<string, string>(this.ScaleColliderBySkillID));
            this._state = ResizeState.Idle;
            this._origCenter = this.boxCollider.center;
            this._origSize = this.boxCollider.size;
            this._curEntry = null;
        }

        private ResizeColliderEntry ScaleBySkillIDsContains(string skillID)
        {
            if (skillID != null)
            {
                for (int i = 0; i < this.scaleBySkillIDs.Length; i++)
                {
                    if (this.scaleBySkillIDs[i].skillID == skillID)
                    {
                        return this.scaleBySkillIDs[i];
                    }
                }
            }
            return null;
        }

        private void ScaleColliderBySkillID(string from, string to)
        {
            ResizeColliderEntry entry = this.ScaleBySkillIDsContains(to);
            if (entry != null)
            {
                this._curEntry = entry;
                this._state = ResizeState.InStateWaiting;
            }
            else if (this._state == ResizeState.InStateResized)
            {
                this.boxCollider.size = this._origSize;
                this.boxCollider.center = this._origCenter;
                this._curEntry = null;
                this._state = ResizeState.Idle;
            }
            else if (this._state == ResizeState.InStateWaiting)
            {
                this._curEntry = null;
                this._state = ResizeState.Idle;
            }
        }

        [Serializable]
        public class ResizeColliderEntry
        {
            public Vector3 center;
            public float normalizedTimeStart;
            public float normalizedTimeStop;
            public Vector3 size;
            public string skillID;
        }

        private enum ResizeState
        {
            Idle,
            InStateWaiting,
            InStateResized
        }
    }
}

