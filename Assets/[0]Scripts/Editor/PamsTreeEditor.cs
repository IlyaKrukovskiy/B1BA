using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(PamsTree))]
public class PamsTreeEditor : Editor
{
    private PamsTree tree;

    public GUIStyle middleStyle = new GUIStyle();
    public GUIStyle rightStyle = new GUIStyle();

    int numberOfLines = 2;

    public List<PamsElement> parents = new List<PamsElement>();

    public PamsElement currentElement;

    public bool IsDescriptionOpened = false;
    public bool IsPropertiesOpened = false;
    public bool IsLinesOpened = false;
    public bool IsChildrenOpened = false;

    public override void OnInspectorGUI()
    {
        tree = (PamsTree)target;

        middleStyle.alignment = TextAnchor.MiddleCenter;
        rightStyle.alignment = TextAnchor.MiddleRight;

        if (tree.root == null)
        {
            tree.root = new PamsElement();
            tree.root.data = new PamsElementData();
        }

        if (parents.Count == 0)
        {
            currentElement = tree.root;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear", GUILayout.Width(50)))
        {
            tree.root = new PamsElement();
        }
        EditorGUILayout.LabelField("ПЗРК Игла", middleStyle);
        EditorGUILayout.EndHorizontal();

        DrawParents();

        EditorGUILayout.BeginVertical();

        DrawNameContainer();
        DrawDescriptionContainer();
        DrawPropertiesContainer();
        DrawLinesConatiner();
        DrawChildrenContainer();

        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(tree);
        }

    }

    public void DrawParents()
    {

        for (int i = 0; i < parents.Count; i++)
        {
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(parents[i].data.name, middleStyle))
            {
                currentElement = parents[i];

                List<PamsElement> newParents = new List<PamsElement>();

                for (int j = 0; j < i; j++)
                {
                    newParents.Add(parents[j]);
                }

                parents = new List<PamsElement>(newParents);

                break;

            }

            EditorGUILayout.LabelField("\\/", middleStyle);

            EditorGUILayout.EndVertical();
        }
    }

    public void DrawNameContainer()
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("Name : ", GUILayout.Width(50));
        currentElement.data.name = EditorGUILayout.TextField(currentElement.data.name);
        EditorGUILayout.EndHorizontal();
    }

    public void DrawDescriptionContainer()
    {
        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("Description");
        IsDescriptionOpened = EditorGUILayout.Toggle(IsDescriptionOpened, GUILayout.Width(15));

        EditorGUILayout.EndHorizontal();

        if (IsDescriptionOpened)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(35));
            EditorGUILayout.LabelField("Lines", GUILayout.Width(35));
            numberOfLines = EditorGUILayout.IntField(numberOfLines, GUILayout.Width(35));
            EditorGUILayout.EndVertical();

            
            currentElement.data.description = EditorGUILayout.TextArea(
                currentElement.data.description, EditorStyles.textArea,
                GUILayout.Height(16 * numberOfLines));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }

    public void DrawPropertiesContainer()
    {
        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("Properties");
        IsPropertiesOpened = EditorGUILayout.Toggle(IsPropertiesOpened, GUILayout.Width(15));

        EditorGUILayout.EndHorizontal();

        if (IsPropertiesOpened)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("GameObject", GUILayout.Width(75));
            currentElement.data.GO = (GameObject)EditorGUILayout.ObjectField(
                currentElement.data.GO, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Position", GUILayout.Width(75));
            currentElement.data.position = EditorGUILayout
                .Vector3Field("", currentElement.data.position);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rotation", GUILayout.Width(75));
            currentElement.data.rotation = EditorGUILayout
                .Vector3Field("", currentElement.data.rotation);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preview Rotation", GUILayout.Width(75));
            currentElement.data.previewRotation = EditorGUILayout
                .Vector2Field("", currentElement.data.previewRotation);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Camera Distance", GUILayout.Width(75));
            currentElement.data.cameraDistance = EditorGUILayout.FloatField(
                "", currentElement.data.cameraDistance);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }

    public void DrawLinesConatiner()
    {
        if (currentElement.data.lines == null)
        {
            currentElement.data.lines = new List<LineData>();
        }

        EditorGUILayout.BeginHorizontal("box");

        if (GUILayout.Button("Clear", GUILayout.Width(50)))
        {
            currentElement.data.lines.Clear();
        }
        EditorGUILayout.LabelField("Lines", middleStyle, GUILayout.MinWidth(40));
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            currentElement.data.lines.Add(new LineData());
        }
        IsLinesOpened = EditorGUILayout.Toggle(IsLinesOpened, GUILayout.Width(15));

        EditorGUILayout.EndHorizontal();

        if (IsLinesOpened)
        {
            for (int i = 0; i < currentElement.data.lines.Count; i++)
            {
                EditorGUILayout.LabelField("|", middleStyle, GUILayout.Height(5));
                DrawLineContainer(currentElement.data.lines[i]);
            }
            EditorGUILayout.LabelField("|", middleStyle, GUILayout.Height(5));
        }        
    }

    public void DrawLineContainer(LineData lineData)
    {
        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("Line name", GUILayout.Width(70));
        lineData.name = EditorGUILayout.TextField(lineData.name);
        if (GUILayout.Button("-", GUILayout.Width(20)))
        {
            currentElement.data.lines.Remove(lineData);
            return;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("Line properties");

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Position", GUILayout.Width(75));
        lineData.localPosition = EditorGUILayout.Vector3Field("", lineData.localPosition);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rotation", GUILayout.Width(75));
        lineData.rotation = EditorGUILayout.FloatField("", lineData.rotation);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is x reverse?", GUILayout.Width(75));
        lineData.xReverse = EditorGUILayout.Toggle("", lineData.xReverse);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is y reverse?", GUILayout.Width(75));
        lineData.yReverse = EditorGUILayout.Toggle("", lineData.yReverse);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is clickable?", GUILayout.Width(75));
        lineData.isClickable = EditorGUILayout.Toggle("", lineData.isClickable);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is for section?", GUILayout.Width(75));
        lineData.isForSection = EditorGUILayout.Toggle("", lineData.isForSection);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();

    }


    string elementName = "";
    public void DrawChildrenContainer()
    {
        if (currentElement.children == null)
        {
            currentElement.children = new List<PamsElement>();
        }

        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("Children");
        IsChildrenOpened = EditorGUILayout.Toggle(IsChildrenOpened, GUILayout.Width(15));

        EditorGUILayout.EndHorizontal();

        if (IsChildrenOpened)
        {
          
            EditorGUILayout.BeginHorizontal("box");

            EditorGUILayout.LabelField("Name", GUILayout.Width(40));
            elementName = EditorGUILayout.TextField(elementName);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            { 

                PamsElement newElement = new PamsElement();
                newElement.data = new PamsElementData();
                newElement.data.name = elementName;
                newElement.children = new List<PamsElement>();
                newElement.data.lines = new List<LineData>();

                currentElement.children.Add(newElement);
                LineData newLineData = new LineData();
                newLineData.name = elementName;
                currentElement.data.lines.Add(newLineData);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");

            for (int i = 0; i < currentElement.children.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(currentElement.children[i].data.name))
                {
                    parents.Add(currentElement);
                    currentElement = currentElement.children[i];
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    currentElement.children.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        
    }


    public static void SetObjectDirty(ScriptableObject obj)
    {
        EditorUtility.SetDirty(obj);
    }
}
