using System;
using UnityEngine;

public interface IBodyPartTouchable
{
    void BodyPartTouched(BodyPartType type, Vector3 point);
}

