namespace MoleMole
{
    using System;

    public class MonoLoadingCanvas : BaseMonoCanvas
    {
        public void Awake()
        {
            Singleton<MainUIManager>.Instance.SetMainCanvas(this);
        }

        public void OnInLevelVideoBeginCallback(CgDataItem cgDataItem)
        {
        }

        public void OnInLevelVideoEndCallback(CgDataItem cgDataItem)
        {
        }

        public override void PlayVideo(CgDataItem cgDataItem)
        {
        }

        public override void Start()
        {
            LoadingPageContext context = new LoadingPageContext(Singleton<MainUIManager>.Instance.bDestroyUntilNotify);
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            base.Start();
        }
    }
}

