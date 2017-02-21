namespace MoleMole
{
    using System;

    public class LDWaitComboBreak : BaseLDEvent
    {
        public LDWaitComboBreak(double runtimeID)
        {
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Combine(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.UpdateAttackSpeedByCombo));
        }

        private void UpdateAttackSpeedByCombo(int from, int to)
        {
            if (to < from)
            {
                Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Remove(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.UpdateAttackSpeedByCombo));
                base.Done();
            }
        }
    }
}

