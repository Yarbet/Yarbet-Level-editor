using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleManager : MonoBehaviour
{
    public ButtonStyle[] buttonstyles; 
}
[System.Serializable]
public struct ButtonStyle
{
    public Texture2D Icon;
    public string ButtonTex;
    public GameObject PreFabObject;
    [HideInInspector]
    public GUIStyle NodeStyle;
    
}
