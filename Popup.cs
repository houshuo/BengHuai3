using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Popup
{
    private static Popup current;
    private bool isClicked;
    private bool isVisible;
    private int selectedItemIndex;

    public int GetSelectedItemIndex()
    {
        return this.selectedItemIndex;
    }

    public int List(Rect box, GUIContent[] items, GUIStyle boxStyle, GUIStyle listStyle, int xCount = 1)
    {
        if (this.isVisible)
        {
            Rect position = new Rect(box.x, box.y + box.height, box.width, (box.height * items.Length) / ((float) xCount));
            GUI.Box(position, string.Empty, boxStyle);
            GUI.depth--;
            this.selectedItemIndex = GUI.SelectionGrid(position, this.selectedItemIndex, items, xCount, listStyle);
            GUI.depth++;
            if (GUI.changed)
            {
                current = null;
            }
        }
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        if (Event.current.GetTypeForControl(controlID) == EventType.MouseUp)
        {
            current = null;
        }
        if (GUI.Button(new Rect(box.x, box.y, box.width, box.height), items[this.selectedItemIndex]))
        {
            if (!this.isClicked)
            {
                current = this;
                this.isClicked = true;
            }
            else
            {
                this.isClicked = false;
            }
        }
        if (current == this)
        {
            this.isVisible = true;
        }
        else
        {
            this.isVisible = false;
            this.isClicked = false;
        }
        return this.selectedItemIndex;
    }
}

