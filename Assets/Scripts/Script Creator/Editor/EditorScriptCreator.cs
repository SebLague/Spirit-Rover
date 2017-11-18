using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class EditorScriptCreator {

	[MenuItem("Assets/Create Editor Script %#a", false, 20)] // guesswork required for priorities (or reflector to look at source)
	static void AttachSelectedScriptsToNewEmptyObject()
	{
		Type[] selectedMonoBehaviourTypes = GetSelectedMonoBehaviourTypes();

        if (selectedMonoBehaviourTypes.Length > 0)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
            {
                AssetDatabase.CreateFolder("Assets", "Editor");
            }

            foreach (Type scriptType in selectedMonoBehaviourTypes)
            {

                string targetScriptName = scriptType.ToString();
                string targetVariableName = Char.ToLower(targetScriptName[0]) + targetScriptName.Substring(1);
                string editorScriptName = targetScriptName + "Editor";
                string path = "Assets/Editor/" + editorScriptName + ".cs";

                if (!File.Exists(path))
                {
					string templateGUID = AssetDatabase.FindAssets("EditorTemplate t:textasset")[0];
					TextAsset templateFile = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(templateGUID));
					string templateText = templateFile.text;

                    string editorScriptContents = templateText;
                    editorScriptContents = editorScriptContents.Replace("@TargetScriptName", targetScriptName);
                    editorScriptContents = editorScriptContents.Replace("@targetVariableName", targetVariableName);
                    editorScriptContents = editorScriptContents.Replace("@EditorScriptName", editorScriptName);

                    StreamWriter writer = new StreamWriter(path);
                    writer.Write(editorScriptContents);
                    writer.Close();
                    AssetDatabase.Refresh();
                }
            }
        }
	}

	[MenuItem("Assets/Create Editor Script %#a", true, 20)]
	public static bool AttachSelectedScriptsToNewEmptyObject_Validation()
	{
		return GetSelectedMonoBehaviourTypes().Length > 0;
	}

	static Type[] GetSelectedMonoBehaviourTypes()
	{
		List<Type> selectedScriptTypes = new List<Type>();

		foreach (string guid in Selection.assetGUIDs)
		{
            
			string path = AssetDatabase.GUIDToAssetPath(guid);
			if (!path.Contains("/Editor/")) // Don't include scripts inside an editor folder
			{
				MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

				if (script != null && script.GetClass() != null && script.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
				{
					selectedScriptTypes.Add(script.GetClass());

				}
			}
		}

		return selectedScriptTypes.ToArray();
	}

}
