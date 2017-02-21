using System;
using UnityEngine;

public class MonoBodyPart : MonoBehaviour
{
    private IBodyPartTouchable _bodyPartTouchable;
    private Collider _collider;
    private int _preTouchCount;
    public BodyPartType type;

    public void SetBodyPartTouchable(IBodyPartTouchable touchable)
    {
        this._bodyPartTouchable = touchable;
    }

    private void Start()
    {
        this._collider = base.GetComponent<Collider>();
    }

    private void Update()
    {
        if (this._collider != null)
        {
            bool flag = (Input.touches.Length > 0) && (this._preTouchCount == 0);
            Vector3 position = !flag ? Vector3.zero : new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0f);
            if (Input.GetMouseButtonDown(0))
            {
                flag = true;
                position = Input.mousePosition;
            }
            if (flag)
            {
                Ray ray = Camera.main.ScreenPointToRay(position);
                RaycastHit hitInfo = new RaycastHit();
                if (this._collider.Raycast(ray, out hitInfo, 9999.9f) && (this._bodyPartTouchable != null))
                {
                    this._bodyPartTouchable.BodyPartTouched(this.type, hitInfo.point);
                }
            }
            this._preTouchCount = Input.touches.Length;
        }
    }
}

