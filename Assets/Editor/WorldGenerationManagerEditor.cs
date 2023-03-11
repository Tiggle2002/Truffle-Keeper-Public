using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(WorldGenerationManager), true)]
[CanEditMultipleObjects]

public class WorldGenerationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate"))
        {
           (target as WorldGenerationManager).Generate();
        }

    }
}