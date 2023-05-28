using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Rect rect;
    public GUIStyle style;
    public Node(Vector2 position, float width, float hight, GUIStyle defaultstyle)
    {
        rect = new Rect(position.x, position.y, width, hight);
        style = defaultstyle;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        GUI.Box(rect, "", style);
    }

    public void Setstyle(GUIStyle NodeStyle)
    {
        style = NodeStyle;
    }
}
