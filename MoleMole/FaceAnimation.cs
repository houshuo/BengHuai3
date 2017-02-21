namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class FaceAnimation
    {
        private FaceAnimationFrameInfo _curFrameInfo;
        private Dictionary<string, FaceAnimationFrameInfo> _frameInfoDict = new Dictionary<string, FaceAnimationFrameInfo>();
        private FacePartControl _leftEye;
        private FacePartControl _mouth;
        private bool _playing;
        private FaceAnimationPlayMode _playMode;
        private FacePartControl _rightEye;
        private float _timer;

        private void BuildAnimationFrameInfo(ConfigFaceAnimation config)
        {
            this._frameInfoDict.Clear();
            int index = 0;
            int length = config.items.Length;
            while (index < length)
            {
                FaceAnimationFrameInfo info = new FaceAnimationFrameInfo {
                    name = config.items[index].name,
                    length = config.items[index].length,
                    timePerFrame = config.items[index].timePerFrame,
                    leftEyeFrames = this.GetFrameInfoFromBlocks(config.items[index].leftEyeBlocks, config.items[index].length, this._leftEye.GetFrameNames()),
                    rightEyeFrames = this.GetFrameInfoFromBlocks(config.items[index].rightEyeBlocks, config.items[index].length, this._rightEye.GetFrameNames()),
                    mouthFrames = this.GetFrameInfoFromBlocks(config.items[index].mouthBlocks, config.items[index].length, this._mouth.GetFrameNames())
                };
                this._frameInfoDict[info.name] = info;
                index++;
            }
            if ((this._curFrameInfo != null) && this._frameInfoDict.ContainsKey(this._curFrameInfo.name))
            {
                this._curFrameInfo = this._frameInfoDict[this._curFrameInfo.name];
            }
        }

        private int[] GetFrameInfoFromBlocks(FaceAnimationFrameBlock[] blocks, int length, string[] names)
        {
            int[] numArray = new int[length];
            int num = 0;
            if (blocks != null)
            {
                int index = 0;
                int num3 = blocks.Length;
                while (index < num3)
                {
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = names.Length;
                    while (num5 < num6)
                    {
                        if (blocks[index].frameKey == names[num5])
                        {
                            num4 = num5;
                            break;
                        }
                        num5++;
                    }
                    int num7 = 0;
                    int frameLength = blocks[index].frameLength;
                    while (num7 < frameLength)
                    {
                        numArray[num++] = num4;
                        if (num >= length)
                        {
                            break;
                        }
                        num7++;
                    }
                    if (num >= length)
                    {
                        return numArray;
                    }
                    index++;
                }
            }
            return numArray;
        }

        private void NormalizeTimer()
        {
            if (this._curFrameInfo != null)
            {
                float num = this._curFrameInfo.timePerFrame * this._curFrameInfo.length;
                while (this._timer >= num)
                {
                    this._timer -= num;
                }
            }
        }

        public void PlayFaceAnimation(string name, FaceAnimationPlayMode mode = 0)
        {
            if (!string.IsNullOrEmpty(name) && this._frameInfoDict.ContainsKey(name))
            {
                this._curFrameInfo = this._frameInfoDict[name];
                this._timer = 0f;
                this._playing = true;
                this._playMode = mode;
            }
        }

        public void PrepareFaceAnmation(string name)
        {
            if (!string.IsNullOrEmpty(name) && this._frameInfoDict.ContainsKey(name))
            {
                this._curFrameInfo = this._frameInfoDict[name];
                this._timer = 0f;
                this._playing = false;
            }
        }

        public void Process(float dt)
        {
            if (this._playing && (this._curFrameInfo != null))
            {
                this._timer += dt;
                this.SetupFace();
            }
        }

        private void SetLastFrameFace()
        {
            if (this._curFrameInfo != null)
            {
                this._leftEye.SetFacePartIndex(this._curFrameInfo.leftEyeFrames[this._curFrameInfo.length - 1]);
                this._rightEye.SetFacePartIndex(this._curFrameInfo.rightEyeFrames[this._curFrameInfo.length - 1]);
                this._mouth.SetFacePartIndex(this._curFrameInfo.mouthFrames[this._curFrameInfo.length - 1]);
            }
        }

        public void SetTime(float time)
        {
            if (!this._playing && (this._curFrameInfo != null))
            {
                this._timer = time;
                this.SetupFace();
            }
        }

        public void SetTimePerFrame(float time)
        {
            if (this._curFrameInfo != null)
            {
                this._curFrameInfo.timePerFrame = time;
            }
        }

        public void Setup(ConfigFaceAnimation config, FacePartControl leftEye, FacePartControl rightEye, FacePartControl mouth)
        {
            this._leftEye = leftEye;
            this._rightEye = rightEye;
            this._mouth = mouth;
            this.BuildAnimationFrameInfo(config);
        }

        private void SetupFace()
        {
            int index = (int) (this._timer / this._curFrameInfo.timePerFrame);
            if ((index >= 0) && (index < this._curFrameInfo.length))
            {
                this._leftEye.SetFacePartIndex(this._curFrameInfo.leftEyeFrames[index]);
                this._rightEye.SetFacePartIndex(this._curFrameInfo.rightEyeFrames[index]);
                this._mouth.SetFacePartIndex(this._curFrameInfo.mouthFrames[index]);
            }
            else if (this._playMode == FaceAnimationPlayMode.Normal)
            {
                this.Stop();
            }
            else if (this._playMode == FaceAnimationPlayMode.Clamp)
            {
                this.SetLastFrameFace();
            }
            else if (this._playMode == FaceAnimationPlayMode.Loop)
            {
                this.NormalizeTimer();
            }
        }

        public void Stop()
        {
            if (this._playing)
            {
                this._leftEye.Reset();
                this._rightEye.Reset();
                this._mouth.Reset();
                this._playing = false;
            }
        }

        public bool isPlaying
        {
            get
            {
                return this._playing;
            }
        }
    }
}

