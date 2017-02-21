namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [AddComponentMenu("UI/Effects/Letter Spacing", 14), RequireComponent(typeof(Text))]
    public class LetterSpacing : BaseMeshEffect, ILayoutElement
    {
        public bool autoFixLine;
        [SerializeField]
        private float m_spacing;

        protected LetterSpacing()
        {
        }

        public void AccommodateText()
        {
            if (this.autoFixLine && !string.IsNullOrEmpty(this.text.text))
            {
                float width = base.transform.GetComponent<RectTransform>().rect.width;
                if (((width <= 0f) && base.gameObject.activeSelf) && (this.text.text.Length > 0))
                {
                    base.StartCoroutine(this.ReAccommodateText());
                }
                else
                {
                    float num2 = (this.spacing * this.text.fontSize) / 100f;
                    float fontSize = this.text.fontSize;
                    string str = this.text.text.Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
                    string str2 = str;
                    int num4 = 1;
                    bool flag = false;
                    float num5 = 0f;
                    for (int i = 0; i < str.Length; i++)
                    {
                        int num7;
                        int num8;
                        char ch2 = str[i];
                        switch (ch2)
                        {
                            case '<':
                            {
                                flag = true;
                                continue;
                            }
                            case '>':
                            {
                                flag = false;
                                continue;
                            }
                            default:
                            {
                                if ((ch2 == '{') && (((i < (str.Length - 2)) && (str[i + 1] == '{')) && (str[i + 2] == '{')))
                                {
                                    str2 = str2.Insert(i, Environment.NewLine);
                                    num4++;
                                    i += 2;
                                    num5 = 0f;
                                    continue;
                                }
                                if (flag)
                                {
                                    continue;
                                }
                                num5 += fontSize + num2;
                                if ((num5 - num2) <= width)
                                {
                                    continue;
                                }
                                char ch = str[i];
                                if (((ch != '.') && !int.TryParse(ch.ToString(), out num7)) || ((i + 1) >= str.Length))
                                {
                                    goto Label_0265;
                                }
                                if (str[i + 1] != '%')
                                {
                                    ch2 = str[i + 1];
                                    if (!int.TryParse(ch2.ToString(), out num7))
                                    {
                                        goto Label_0265;
                                    }
                                }
                                num8 = i - 1;
                                goto Label_0216;
                            }
                        }
                    Label_0210:
                        num8--;
                    Label_0216:
                        if (num8 > 0)
                        {
                            char ch3 = str[num8];
                            if (int.TryParse(ch3.ToString(), out num7))
                            {
                                goto Label_0210;
                            }
                        }
                        str2 = str2.Insert(num8 + num4, Environment.NewLine);
                        num4++;
                        num5 = (i - num8) * (num2 + fontSize);
                        continue;
                    Label_0265:
                        str2 = str2.Insert(i + num4, Environment.NewLine);
                        num4++;
                        num5 = 0f;
                    }
                    str2 = str2.Replace("{{{", string.Empty);
                    this.text.text = str2;
                }
            }
        }

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        public int GetLineCount(string targetText)
        {
            float width = base.transform.GetComponent<RectTransform>().rect.width;
            if (width <= 0f)
            {
                return 1;
            }
            float num2 = (this.spacing * this.text.fontSize) / 100f;
            float fontSize = this.text.fontSize;
            string str = targetText.Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
            string str2 = str;
            int num4 = 1;
            bool flag = false;
            float num5 = 0f;
            for (int i = 0; i < str.Length; i++)
            {
                int num7;
                int num8;
                char ch2 = str[i];
                switch (ch2)
                {
                    case '<':
                    {
                        flag = true;
                        continue;
                    }
                    case '>':
                    {
                        flag = false;
                        continue;
                    }
                    default:
                    {
                        if ((ch2 == '{') && (((i < (str.Length - 2)) && (str[i + 1] == '{')) && (str[i + 2] == '{')))
                        {
                            str2 = str2.Insert(i, Environment.NewLine);
                            num4++;
                            i += 2;
                            num5 = 0f;
                            continue;
                        }
                        if (flag)
                        {
                            continue;
                        }
                        num5 += fontSize + num2;
                        if ((num5 - num2) <= width)
                        {
                            continue;
                        }
                        char ch = str[i];
                        if (((ch != '.') && !int.TryParse(ch.ToString(), out num7)) || ((i + 1) >= str.Length))
                        {
                            goto Label_0207;
                        }
                        if (str[i + 1] != '%')
                        {
                            ch2 = str[i + 1];
                            if (!int.TryParse(ch2.ToString(), out num7))
                            {
                                goto Label_0207;
                            }
                        }
                        num8 = i - 1;
                        goto Label_01B8;
                    }
                }
            Label_01B2:
                num8--;
            Label_01B8:
                if (num8 > 0)
                {
                    char ch3 = str[num8];
                    if (int.TryParse(ch3.ToString(), out num7))
                    {
                        goto Label_01B2;
                    }
                }
                str2 = str2.Insert(num8 + num4, Environment.NewLine);
                num4++;
                num5 = (i - num8) * (num2 + fontSize);
                continue;
            Label_0207:
                str2 = str2.Insert(i + num4, Environment.NewLine);
                num4++;
                num5 = 0f;
            }
            str2 = str2.Replace("{{{", string.Empty);
            return ((num4 <= 0) ? 1 : num4);
        }

        private string[] GetLines()
        {
            char[] separator = new char[] { '\n' };
            return this.text.text.Split(separator);
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (this.IsActive())
            {
                List<UIVertex> verts = new List<UIVertex>();
                vh.GetUIVertexStream(verts);
                this.ModifyVertices(verts);
                vh.Clear();
                vh.AddUIVertexTriangleStream(verts);
            }
        }

        public void ModifyVertices(List<UIVertex> verts)
        {
            if (this.IsActive())
            {
                string[] lines = this.GetLines();
                float num = (this.spacing * this.text.fontSize) / 100f;
                float num2 = 0f;
                int num3 = 0;
                bool flag = false;
                switch (this.text.alignment)
                {
                    case TextAnchor.UpperLeft:
                    case TextAnchor.MiddleLeft:
                    case TextAnchor.LowerLeft:
                        num2 = 0f;
                        break;

                    case TextAnchor.UpperCenter:
                    case TextAnchor.MiddleCenter:
                    case TextAnchor.LowerCenter:
                        num2 = 0.5f;
                        break;

                    case TextAnchor.UpperRight:
                    case TextAnchor.MiddleRight:
                    case TextAnchor.LowerRight:
                        num2 = 1f;
                        break;
                }
                for (int i = 0; i < lines.Length; i++)
                {
                    string str = lines[i];
                    float num5 = ((str.Length - 1) * num) * num2;
                    int num6 = 0;
                    for (int j = 0; j < str.Length; j++)
                    {
                        int num8 = num3 * 6;
                        int num9 = (num3 * 6) + 1;
                        int num10 = (num3 * 6) + 2;
                        int num11 = (num3 * 6) + 3;
                        int num12 = (num3 * 6) + 4;
                        int num13 = (num3 * 6) + 5;
                        bool flag2 = false;
                        switch (this.text.text[num3])
                        {
                            case '<':
                                flag = true;
                                flag2 = true;
                                break;

                            case '>':
                                flag = false;
                                flag2 = true;
                                break;
                        }
                        if (flag2 || flag)
                        {
                            num6++;
                            num3++;
                        }
                        else
                        {
                            if (num13 > (verts.Count - 1))
                            {
                                return;
                            }
                            UIVertex vertex = verts[num8];
                            UIVertex vertex2 = verts[num9];
                            UIVertex vertex3 = verts[num10];
                            UIVertex vertex4 = verts[num11];
                            UIVertex vertex5 = verts[num12];
                            UIVertex vertex6 = verts[num13];
                            Vector3 vector = (Vector3) (Vector3.right * ((num * (j - num6)) - num5));
                            vertex.position += vector;
                            vertex2.position += vector;
                            vertex3.position += vector;
                            vertex4.position += vector;
                            vertex5.position += vector;
                            vertex6.position += vector;
                            verts[num8] = vertex;
                            verts[num9] = vertex2;
                            verts[num10] = vertex3;
                            verts[num11] = vertex4;
                            verts[num12] = vertex5;
                            verts[num13] = vertex6;
                            num3++;
                        }
                    }
                    num3++;
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator ReAccommodateText()
        {
            return new <ReAccommodateText>c__Iterator7A { <>f__this = this };
        }

        protected override void Start()
        {
            base.Start();
            this.AccommodateText();
        }

        public float flexibleHeight
        {
            get
            {
                return this.text.flexibleHeight;
            }
        }

        public float flexibleWidth
        {
            get
            {
                return this.text.flexibleWidth;
            }
        }

        public int layoutPriority
        {
            get
            {
                return this.text.layoutPriority;
            }
        }

        public float minHeight
        {
            get
            {
                return this.text.minHeight;
            }
        }

        public float minWidth
        {
            get
            {
                return this.text.minWidth;
            }
        }

        public float preferredHeight
        {
            get
            {
                return this.text.preferredHeight;
            }
        }

        public float preferredWidth
        {
            get
            {
                string text = this.text.text;
                bool flag = false;
                int num = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    switch (text[i])
                    {
                        case '<':
                        {
                            flag = true;
                            continue;
                        }
                        case '>':
                        {
                            flag = false;
                            continue;
                        }
                    }
                    if (!flag)
                    {
                        num++;
                    }
                }
                return (this.text.preferredWidth + (((this.spacing * this.text.fontSize) / 100f) * num));
            }
        }

        public float spacing
        {
            get
            {
                return this.m_spacing;
            }
            set
            {
                if (this.m_spacing != value)
                {
                    this.m_spacing = value;
                    if (base.get_graphic() != null)
                    {
                        base.get_graphic().SetVerticesDirty();
                    }
                    LayoutRebuilder.MarkLayoutForRebuild((RectTransform) base.transform);
                }
            }
        }

        private Text text
        {
            get
            {
                return base.gameObject.GetComponent<Text>();
            }
        }

        [CompilerGenerated]
        private sealed class <ReAccommodateText>c__Iterator7A : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal LetterSpacing <>f__this;

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
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.AccommodateText();
                        this.$PC = -1;
                        break;
                }
                return false;
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

