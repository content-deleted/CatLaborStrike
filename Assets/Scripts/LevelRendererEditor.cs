using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelRenderer))]
public class LevelRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelRenderer renderer = (LevelRenderer)target;
        if(GUILayout.Button("Render Level"))
        {
            renderer.RenderLevel(new Level(renderer.editorLevel));
        }
    }
}