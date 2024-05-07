#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectGameObject : EditorWindow
{
    string prefix = "";
    [MenuItem("GameObject/Select GameObject")]
    static void CreateCube()
    {
        GetWindow(typeof(SelectGameObject)).titleContent = new GUIContent($"����ǰ׺ѡ������");
    }
    void OnGUI()
    {
        var objects = new List<GameObject>();

        var activeTransform = Selection.activeTransform;

        GUILayout.BeginHorizontal();
        GUILayout.Label("���ƣ�");
        prefix = GUILayout.TextField(prefix);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("ѡ��"))
        {
            for (int i = 0; i < activeTransform.childCount; i++)
                if (activeTransform.GetChild(i).name.StartsWith(prefix))
                    objects.Add(activeTransform.GetChild(i).gameObject);
            if (objects.Count > 0)
                Selection.objects = objects.ToArray();
        }
    }
}
#endif