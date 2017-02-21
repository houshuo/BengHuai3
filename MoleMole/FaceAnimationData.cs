namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class FaceAnimationData
    {
        private static Dictionary<string, ConfigFaceAnimation> _dictFaceAnimation = new Dictionary<string, ConfigFaceAnimation>();

        public static ConfigFaceAnimation GetFaceAnimation(string name)
        {
            if (_dictFaceAnimation.ContainsKey(name))
            {
                return _dictFaceAnimation[name];
            }
            return null;
        }

        public static void ReloadFromFile()
        {
            _dictFaceAnimation.Clear();
            _dictFaceAnimation["Kiana"] = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Kiana");
            _dictFaceAnimation["Mei"] = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Mei");
            _dictFaceAnimation["Bronya"] = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Bronya");
            _dictFaceAnimation["Himeko"] = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Bronya");
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator6C { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator6C : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncReqeust>__1;
            internal float <step>__0;
            internal Action<float> moveOneStepCallback;
            internal float progressSpan;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<step>__0 = this.progressSpan / 4f;
                        FaceAnimationData._dictFaceAnimation.Clear();
                        this.<asyncReqeust>__1 = ConfigUtil.LoadConfigAsync("FaceAnimation/Kiana", BundleType.RESOURCE_FILE);
                        this.$current = this.<asyncReqeust>__1.operation;
                        this.$PC = 1;
                        goto Label_01F6;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__0);
                        }
                        FaceAnimationData._dictFaceAnimation["Kiana"] = (ConfigFaceAnimation) this.<asyncReqeust>__1.asset;
                        this.<asyncReqeust>__1 = ConfigUtil.LoadConfigAsync("FaceAnimation/Mei", BundleType.RESOURCE_FILE);
                        this.$current = this.<asyncReqeust>__1.operation;
                        this.$PC = 2;
                        goto Label_01F6;

                    case 2:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__0);
                        }
                        FaceAnimationData._dictFaceAnimation["Mei"] = (ConfigFaceAnimation) this.<asyncReqeust>__1.asset;
                        this.<asyncReqeust>__1 = ConfigUtil.LoadConfigAsync("FaceAnimation/Bronya", BundleType.RESOURCE_FILE);
                        this.$current = this.<asyncReqeust>__1.operation;
                        this.$PC = 3;
                        goto Label_01F6;

                    case 3:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__0);
                        }
                        FaceAnimationData._dictFaceAnimation["Bronya"] = (ConfigFaceAnimation) this.<asyncReqeust>__1.asset;
                        this.<asyncReqeust>__1 = ConfigUtil.LoadConfigAsync("FaceAnimation/Bronya", BundleType.RESOURCE_FILE);
                        this.$current = this.<asyncReqeust>__1.operation;
                        this.$PC = 4;
                        goto Label_01F6;

                    case 4:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__0);
                        }
                        FaceAnimationData._dictFaceAnimation["Himeko"] = (ConfigFaceAnimation) this.<asyncReqeust>__1.asset;
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_01F6:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

