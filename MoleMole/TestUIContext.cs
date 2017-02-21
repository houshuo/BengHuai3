namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using UnityEngine;

    public class TestUIContext : BaseWidgetContext
    {
        public TestUIContext(GameObject view)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "TestUIContext"
            };
            base.config = pattern;
            base.view = view;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 7) && this.OnPlayerLoginRsp(pkt.getData<PlayerLoginRsp>()));
        }

        private bool OnPlayerLoginRsp(PlayerLoginRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                MonoTestUI sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoTestUI;
                if (sceneCanvas.avatar3dModelContext != null)
                {
                    return false;
                }
                sceneCanvas.MainCamera.SetActive(true);
                sceneCanvas.MainMenu_SpaceShip.SetActive(true);
                sceneCanvas.avatar3dModelContext = new Avatar3dModelContext(null);
                Singleton<MainUIManager>.Instance.ShowWidget(sceneCanvas.avatar3dModelContext, UIType.Root);
                GameObject view = GameObject.Find("MainMenu_SpaceShip");
                GameObject uiMainCamera = GameObject.Find("MainCamera");
                SpaceShipModelContext widget = new SpaceShipModelContext(view, uiMainCamera);
                Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
                GraphicsSettingData.ApplySettingConfig();
                AudioSettingData.ApplySettingConfig();
                this.TestCode(sceneCanvas);
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]),
                    notDestroyAfterTouchBG = true
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private void TestCode(MonoTestUI testUI)
        {
            PlayerStatusWidgetContext widget = new PlayerStatusWidgetContext();
            Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            Singleton<MainUIManager>.Instance.ShowPage(new MainPageContext(), UIType.Page);
        }
    }
}

