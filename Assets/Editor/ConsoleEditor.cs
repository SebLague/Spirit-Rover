using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Console))]
public class ConsoleEditor : Editor {

    Console console;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate text fields"))
        {
            console.GenerateTextFields();
        }

		if (GUILayout.Button("Clear text array"))
		{
            console.DeleteTextFields();
		}

        EditorGUI.BeginChangeCheck();
        console.testString = GUILayout.TextField(console.testString);
        if (EditorGUI.EndChangeCheck())
        {
            console.GenerateTextFields();
        }
    }

    void OnEnable() {
        console = (Console)target;
    }
  
}