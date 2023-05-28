using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;


public class GridMapCreator : EditorWindow
{
    Vector2 offset;
    Vector2 drag;
    List<List<Node>> nodes;
    List<List<PartScripts>> Parts;
    GUIStyle empty;
    Vector2 nodePos;
    StyleManager styleman;
    bool isErasing;
    Rect MenuBar;
    private GUIStyle currenStyle;
    GameObject TheMap;
    [MenuItem("Yarbet/Grid map creator")]
   private static void OpenWindow()
    {
        GridMapCreator window = GetWindow<GridMapCreator>();
        window.titleContent = new GUIContent("Grip Map Creator");
        
    }

    private void OnEnable()
    { 
        SetupStyles();
        SetUpNodesAndParts();
        SetUpMap();     
    }

    private void SetUpMap()
    {
        try 
        { 
            TheMap = GameObject.FindGameObjectWithTag("Map");
            RestoretheMap(TheMap);
        } 
        catch (Exception e) { }
        if (TheMap == null) 
        {
            TheMap = new GameObject("Map");
            TheMap.tag = "Map";
        }
    }

    private void RestoretheMap(GameObject theMap)
    {
        if (TheMap.transform.childCount > 0)
        {
            for(int i = 0; i < theMap.transform.childCount; i++)
            {
                int ii = theMap.transform.GetChild(i).GetComponent<PartScripts>().Row;
                int jj = theMap.transform.GetChild(i).GetComponent<PartScripts>().Column;
                GUIStyle TheStyle = theMap.transform.GetChild(i).GetComponent<PartScripts>().style;
                nodes[ii][jj].Setstyle(TheStyle);
                Parts[ii][jj] = theMap.transform.GetChild(i).GetComponent<PartScripts>();
                Parts[ii][jj].Part = theMap.transform.GetChild(i).gameObject;
                Parts[ii][jj].name = theMap.transform.GetChild(i).name;
                Parts[ii][jj].Row = ii;
                Parts[ii][jj].Column = jj;
            }
        }
    }

    private void SetupStyles()
    {
        try  
        { 
            styleman = GameObject.FindGameObjectWithTag("StyleManager").GetComponent<StyleManager>();
            for (int i = 0; i<styleman.buttonstyles.Length; i++)
            {
                styleman.buttonstyles[i].NodeStyle = new GUIStyle();
                styleman.buttonstyles[i].NodeStyle.normal.background = styleman.buttonstyles[i].Icon;
            }
        }   
        catch (Exception e) { }
        empty = styleman.buttonstyles[0].NodeStyle;
        currenStyle = styleman.buttonstyles[1].NodeStyle;
    }

    private void SetUpNodesAndParts()
    {
        nodes = new List<List<Node>>();
        Parts = new List<List<PartScripts>>();
        for (int i = 0; i<20; i++)
        {
            nodes.Add(new List<Node>());
            Parts.Add(new List<PartScripts>());
            for (int j = 0; j < 10; j++)
            {
                nodePos.Set(i*30, j*30);
                nodes[i].Add(new Node(nodePos, 30,30,empty));
                Parts[i].Add(null);
            }
        }
    }

    private void OnGUI()
    {
        DrawGrid();
        DrawNodes();
        DrawMenuBar();
        processNodes(Event.current);
        ProcessGrid(Event.current);
        if (GUI.changed)
        {
            Repaint();
        }
    }

    //agregando un menú en la barra de arriba de la herramienta y le agregamos botones 
    private void DrawMenuBar()
    {
        MenuBar = new Rect(0, 0, position.width, 20);
        GUILayout.BeginArea(MenuBar, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();
        for(int i =0; i<styleman.buttonstyles.Length; i++)
        {
            if (GUILayout.Toggle((currenStyle == styleman.buttonstyles[i].NodeStyle), new GUIContent(styleman.buttonstyles[i].ButtonTex), EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                currenStyle = styleman.buttonstyles[i].NodeStyle;
            }
        }

        
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

    }

    private void processNodes(Event e)
    {
        int Row = (int)((e.mousePosition.x - offset.x) / 30);
        int Col = (int)((e.mousePosition.y - offset.y) / 30);
        if ((e.mousePosition.x - offset.x) < 0 || (e.mousePosition.x - offset.x) > 600 || (e.mousePosition.y - offset.y) < 0 || (e.mousePosition.y - offset.y) > 300)
        {}
        else
        {   
            if (e.type==EventType.MouseDown)
            {
                if (nodes[Row][Col].style.normal.background.name=="Empty")
                {
                    isErasing = false;        
                }
                else
                {
                    isErasing = true;
                }
                PaintNodes(Row, Col);
            }
            if(e.type == EventType.MouseDrag)
            {
                PaintNodes(Row, Col);
                e.Use();
            }
        }
    }

    private void PaintNodes(int Row, int Col)
    {
        if(isErasing)
        {
            if (Parts[Row][Col] != null)
            { 
                nodes[Row][Col].Setstyle(empty);
                DestroyImmediate(Parts[Row][Col].gameObject);
                GUI.changed = true;
            }
            Parts[Row][Col] = null;
        }
        else 
        {
            if (Parts[Row][Col] == null)
            {
                nodes[Row][Col].Setstyle(currenStyle);
                GameObject G = Instantiate(Resources.Load("MapParts/"+currenStyle.normal.background.name)) as GameObject;
                G.name = currenStyle.normal.background.name;
                G.transform.position = new Vector3(Col * 10, 0, Row * 10 ) + Vector3.forward * 5 + Vector3.right * 5;
                G.transform.parent = TheMap.transform;
                Parts[Row][Col] = G.GetComponent<PartScripts>();
                Parts[Row][Col].Part = G;
                Parts[Row][Col].name = G.name;
                Parts[Row][Col].Row = Row;
                Parts[Row][Col].Column = Col;
                Parts[Row][Col].style = currenStyle;
                GUI.changed = true;
            }     
        }
    }


    //Para detallar el alto y ancho del gridmap
    private void DrawNodes()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                nodes[i][j].Draw();
            }
        }
    }

    private void ProcessGrid(Event e)
    {
        drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDrag:
                if(e.button == 0)
                {
                    OnMouseDrag(e.delta);
                }
                break;
        }
    }

    //para arrastrar el gridmap con el mouse
    private void OnMouseDrag(Vector2 delta)
    {
        drag = delta;

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                nodes[i][j].Drag(delta);
            }
        }
        GUI.changed = true;
    }

    //dibujar la grilla 
    private void DrawGrid()
    {
        int widthDivider = Mathf.CeilToInt(position.width / 20);
        int HightDivider = Mathf.CeilToInt(position.height / 20);
        Handles.BeginGUI();
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        offset += drag;
        Vector3 newOffset = new Vector3(offset.x%20, offset.y%20,0);
        for (int i = 0; i<widthDivider; i++)
        {
            Handles.DrawLine(new Vector3(20*i,-20,0)+newOffset, new Vector3(20*i,position.height,0) + newOffset);
        }

        for (int i = 0; i < HightDivider; i++)
        {
            Handles.DrawLine(new Vector3(-20,20*i,0)+newOffset, new Vector3(position.width, 20*i,0) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
}
