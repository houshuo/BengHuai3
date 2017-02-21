namespace MoleMole
{
    using System;

    public class LDEvtSectionTransitionEnter : BaseLDEvent
    {
        public LDEvtSectionTransitionEnter()
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(0.18f, false, null, null);
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EnterTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
            Singleton<LevelDesignManager>.Instance.MuteInput();
        }

        public override void Core()
        {
            if (!Singleton<MainUIManager>.Instance.GetInLevelUICanvas().IsStageTransitPanelFading() && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelTransiting))
            {
                base.Done();
            }
        }
    }
}

