namespace MoleMole
{
    using System;

    public class LDEvtSectionTransitionExit : BaseLDEvent
    {
        public LDEvtSectionTransitionExit(string sectionLevelAnim)
        {
            if (!string.IsNullOrEmpty(sectionLevelAnim))
            {
                Singleton<LevelDesignManager>.Instance.PlayCameraAnimationOnEnv(sectionLevelAnim, false, false, true, CameraAnimationCullingType.CullAvatars);
            }
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(0.18f, false, null, null);
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
        }

        public override void Core()
        {
            if (!Singleton<MainUIManager>.Instance.GetInLevelUICanvas().IsStageTransitPanelFading() && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning))
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
                Singleton<LevelDesignManager>.Instance.RecoveryInput();
                base.Done();
            }
        }
    }
}

