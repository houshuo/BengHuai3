namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class MainMenuLight : MonoBehaviour
    {
        private float _avatarAngle;
        private Transform _avatarTrsf;
        private float _incidentAngle;
        private float _incidentAngleTranistionSpeed;
        private float _incidentAngleTranistionTarget;
        private float _lastIncidentAngle;
        private int _sectionId;
        private float _targetIncidentAngle;
        [Header("Limit Mode")]
        public Vector2 avatarIncidentAngleRange = new Vector2(-80f, 80f);
        [Header("Fixed Mode")]
        public float fixedAngle;
        public bool isInterpolate = true;
        public bool isLightReferToAvatar;
        public Mode mode = Mode.Section;
        [Header("Section Mode")]
        public AngleSection[] sections;
        public AngleSection[] sectionsInterpolate;
        public float transitionDuration;
        public float transitionSmooth = 0.3f;

        private void AdjustAvatarIncidentAngle()
        {
            Transform transform = this._avatarTrsf;
            this.GetAvatar();
            Camera main = Camera.main;
            if ((this._avatarTrsf != null) && (main != null))
            {
                if (this.isLightReferToAvatar)
                {
                    this._incidentAngle = this.GetIncidentAngle(this._avatarTrsf, base.transform.forward);
                }
                else
                {
                    this._incidentAngle = this.GetIncidentAngle(main.transform, base.transform.forward);
                }
                if (this._avatarTrsf != transform)
                {
                    this._lastIncidentAngle = this._incidentAngle;
                }
                this.RotateInXZ(-this._incidentAngle);
                if (this.mode == Mode.Limit)
                {
                    this._incidentAngleTranistionTarget = Mathf.Clamp(this._incidentAngle, this.avatarIncidentAngleRange.x, this.avatarIncidentAngleRange.y);
                    this._targetIncidentAngle = this.TransitIncidentAngle();
                }
                else if (this.mode == Mode.Fixed)
                {
                    this._incidentAngleTranistionTarget = this.fixedAngle;
                    this._targetIncidentAngle = this.TransitIncidentAngle();
                }
                else
                {
                    this._targetIncidentAngle = this.IncidentAngleBySection();
                }
                this.RotateInXZ(this._targetIncidentAngle);
                this._lastIncidentAngle = this._targetIncidentAngle;
            }
        }

        private void CheckSections(ref AngleSection[] sections)
        {
            if ((sections == null) || (sections.Length == 0))
            {
                sections = new AngleSection[] { new AngleSection(0f, 360f, 0f) };
            }
            foreach (AngleSection section in sections)
            {
                section.start = RegularAngle(section.start);
                section.end = RegularAngle(section.end);
                if (section.start > section.end)
                {
                    section.start -= 360f;
                }
            }
            SortSections(sections);
            AngleSection section2 = null;
            List<AngleSection> list = new List<AngleSection>();
            foreach (AngleSection section3 in sections)
            {
                if ((section2 != null) && (section3.start > (section2.end + float.Epsilon)))
                {
                    list.Add(new AngleSection(section2.end, section3.start, section3.incidentAngle));
                }
                list.Add(section3);
                section2 = section3;
            }
            float a = RegularAngle(list[0].start);
            if (Mathf.Approximately(a, 0f))
            {
                a = 360f;
            }
            if (a > (section2.end + 0.001f))
            {
                list.Add(new AngleSection(section2.end, a, section2.incidentAngle));
            }
            sections = list.ToArray();
        }

        private void GetAvatar()
        {
            if (this._avatarTrsf == null)
            {
                BaseMonoUIAvatar avatar = UnityEngine.Object.FindObjectOfType<BaseMonoUIAvatar>();
                if (avatar != null)
                {
                    this._avatarTrsf = avatar.transform;
                }
            }
        }

        private float GetIncidentAngle(Transform trsf, Vector3 dir)
        {
            dir = trsf.InverseTransformDirection(dir);
            return (Mathf.Atan2(-dir.x, -dir.z) * 57.29578f);
        }

        private int GetSection(AngleSection[] sections, float angle)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                AngleSection section = sections[i];
                if (section.Contain(angle))
                {
                    return i;
                }
            }
            if (!sections[0].Contain(angle - 360f))
            {
                SuperDebug.VeryImportantAssert(false, "failed to get section");
            }
            return 0;
        }

        private float IncidentAngleBySection()
        {
            this._avatarAngle = this.GetIncidentAngle(this._avatarTrsf, Camera.main.transform.forward);
            if (this._avatarAngle < 0f)
            {
                this._avatarAngle += 360f;
            }
            if (this.isInterpolate)
            {
                this._sectionId = this.GetSection(this.sectionsInterpolate, this._avatarAngle);
                AngleSection section = this.sectionsInterpolate[this._sectionId];
                int index = (this._sectionId + 1) % this.sectionsInterpolate.Length;
                float num3 = this._avatarAngle - section.start;
                if (num3 > 360f)
                {
                    num3 -= 360f;
                }
                return Mathf.LerpAngle(section.incidentAngle, this.sectionsInterpolate[index].incidentAngle, num3 / (section.end - section.start));
            }
            this._sectionId = this.GetSection(this.sections, this._avatarAngle);
            this._incidentAngleTranistionTarget = this.sections[this._sectionId].incidentAngle;
            return this.TransitIncidentAngle();
        }

        private void OnEnable()
        {
            this.CheckSections(ref this.sections);
            this.CheckSections(ref this.sectionsInterpolate);
        }

        private static float RegularAngle(float angle)
        {
            if (!Mathf.Approximately(angle, 360f))
            {
                angle = Mathf.DeltaAngle(0f, angle);
                if (angle < 0f)
                {
                    angle += 360f;
                }
            }
            return angle;
        }

        private void RotateInXZ(float angle)
        {
            base.transform.Rotate(0f, angle, 0f, Space.World);
        }

        public void SetAvatar(Transform avatarTrsf)
        {
            this._avatarTrsf = avatarTrsf;
        }

        private static void SortSections(AngleSection[] sections)
        {
            for (int i = 0; i < (sections.Length - 1); i++)
            {
                for (int j = i + 1; j < sections.Length; j++)
                {
                    if (sections[i].start > sections[j].start)
                    {
                        AngleSection section = sections[i];
                        sections[i] = sections[j];
                        sections[j] = section;
                    }
                }
            }
        }

        private float TransitIncidentAngle()
        {
            return Mathf.SmoothDampAngle(this._lastIncidentAngle, this._incidentAngleTranistionTarget, ref this._incidentAngleTranistionSpeed, this.transitionSmooth);
        }

        private void Update()
        {
            this.AdjustAvatarIncidentAngle();
        }

        [Serializable]
        public class AngleSection
        {
            public float end;
            public float incidentAngle;
            private static readonly float SECTION_ANGLE_TOLERANCE = 0.1f;
            public float start;

            public AngleSection()
            {
            }

            public AngleSection(float start, float end, float incidentAngle)
            {
                this.start = start;
                this.end = end;
                this.incidentAngle = incidentAngle;
            }

            public bool Contain(float angle)
            {
                return ((angle > (this.start - SECTION_ANGLE_TOLERANCE)) && (angle <= (this.end + SECTION_ANGLE_TOLERANCE)));
            }
        }

        public enum Mode
        {
            Limit,
            Fixed,
            Section
        }
    }
}

