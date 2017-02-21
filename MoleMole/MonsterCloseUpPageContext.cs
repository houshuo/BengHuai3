namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class MonsterCloseUpPageContext : BasePageContext
    {
        private const string IN_LEVEL_CLOSE_UP_PREFAB_PREFIX = "UI/InLevelCloseUp/CloseUp_";
        public readonly string monsterName;

        public MonsterCloseUpPageContext(string monsterName)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "CloseUpPageContext",
                viewPrefabPath = "UI/Menus/Page/InLevel/MonsterCloseUpPage"
            };
            base.config = pattern;
            this.monsterName = monsterName;
        }

        protected override void BindViewCallbacks()
        {
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.MonsterCloseUpEnd)
            {
                Singleton<MainUIManager>.Instance.BackPage();
            }
            return false;
        }

        protected override bool SetupView()
        {
            string path = "UI/InLevelCloseUp/CloseUp_" + this.monsterName;
            base.view.transform.DestroyChildren();
            GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(path, BundleType.RESOURCE_FILE));
            obj2.transform.SetParent(base.view.transform, false);
            obj2.GetComponent<Animation>().Play();
            return false;
        }
    }
}

