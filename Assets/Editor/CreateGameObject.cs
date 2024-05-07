#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CreateGameObject : EditorWindow
{
    string xSum = "";
    string ySum = "";
    string zSum = "";
    string xInterval = "2";
    string yInterval = "2";
    string zInterval = "2";
    static PrimitiveType primitiveType;
    static bool isCustomPrefab;

    [MenuItem("GameObject/Create GameObject/Create Cube")]
    static void CreateCube()
    {
        OpenWindow(PrimitiveType.Cube);
    }
    [MenuItem("GameObject/Create GameObject/Create Sphere")]
    static void CreateSphere()
    {
        OpenWindow(PrimitiveType.Sphere);
    }
    [MenuItem("GameObject/Create GameObject/Create Capsule")]
    static void CreateCapsule()
    {
        OpenWindow(PrimitiveType.Capsule);
    }
    [MenuItem("GameObject/Create GameObject/Create Cylinder")]
    static void CreateCylinder()
    {
        OpenWindow(PrimitiveType.Cylinder);
    }
    [MenuItem("GameObject/Create GameObject/Create Custom Prefeb")]
    static void CreateCustomPrefeb()
    {
        isCustomPrefab = true;
        GetWindow(typeof(CreateGameObject)).titleContent = new GUIContent($"Create Custom Prefab");
    }
    static void OpenWindow(PrimitiveType type)
    {
        isCustomPrefab = false;
        primitiveType = type;
        GetWindow(typeof(CreateGameObject)).titleContent = new GUIContent($"Create {primitiveType}");
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("X轴个数：");
        var xSumCache = xSum;
        xSum = GUILayout.TextField(xSum);
        xSum = Regex.IsMatch(xSum, "^\\+?[1-9][0-9]*$") ? xSum : xSumCache;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Y轴个数：");
        var ySumCache = ySum;
        ySum = GUILayout.TextField(ySum);
        ySum = Regex.IsMatch(ySum, "^\\+?[1-9][0-9]*$") ? ySum : ySumCache;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Z轴个数：");
        var zSumCache = zSum;
        zSum = GUILayout.TextField(zSum);
        zSum = Regex.IsMatch(zSum, "^\\+?[1-9][0-9]*$") ? zSum : zSumCache;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("X轴间距：");
        var xIntervalCache = xInterval;
        xInterval = GUILayout.TextField(xInterval);
        xInterval = Regex.IsMatch(xInterval, "(^[0-9]*[1-9][0-9]*$)|(^([0-9]{1,}[.][0-9]*)*$)") ? xInterval : xIntervalCache;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Y轴间距：");
        var yIntervalCache = yInterval;
        yInterval = GUILayout.TextField(yInterval);
        yInterval = Regex.IsMatch(yInterval, "(^[0-9]*[1-9][0-9]*$)|(^([0-9]{1,}[.][0-9]*)$)") ? yInterval : yIntervalCache;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Z轴间距：");
        var zIntervalCache = zInterval;
        zInterval = GUILayout.TextField(zInterval);
        zInterval = Regex.IsMatch(zInterval, "(^[0-9]*[1-9][0-9]*$)|(^([0-9]{1,}[.][0-9]*)$)") ? zInterval : zIntervalCache;
        GUILayout.EndHorizontal();

        if (isCustomPrefab)
        {
            GUILayout.Label("请将预制体命名为：CustomPrefab并放置于Resources文件夹下。");
            if (!File.Exists(Application.dataPath + "/Resources/CustomPrefab.prefab"))
            {
                GUILayout.Label("在Resources文件夹下未找到名称为CustomPrefab的预制体");
            }
            else
            {
                GUILayout.Label("已找到预制体CustomPrefab");
            }
        }

        var isClick = GUILayout.Button("创建");
        if (isClick)
        {
            var gameObjectName = "";
            GameObject obj = null;
            if (isCustomPrefab)
            {
                gameObjectName = "CustomPrefab";
                obj = Resources.Load("CustomPrefab") as GameObject;
            }
            else
            {
                gameObjectName = primitiveType.ToString();
            }
            int.TryParse(xSum, out int x);
            int.TryParse(ySum, out int y);
            int.TryParse(zSum, out int z);
            float.TryParse(xInterval, out float xInter);
            float.TryParse(yInterval, out float yInter);
            float.TryParse(zInterval, out float zInter);
            var activeTransform = new GameObject($"{gameObjectName}*{x * y * z}").transform;
            activeTransform.SetParent(Selection.activeTransform);
            var createdPointX = -x * xInter / 2;
            for (int i = 0; i < x; i++)
            {
                var createdPointY = -y * yInter / 2;
                for (int j = 0; j < y; j++)
                {
                    var createdPointZ = -z * zInter / 2;
                    for (int k = 0; k < z; k++)
                    {
                        GameObject gameObject;

                        if (obj == null)
                            gameObject = GameObject.CreatePrimitive(primitiveType);
                        else
                        {
                            gameObject = Instantiate(obj);
                            gameObject.name = gameObjectName;
                        }

                        gameObject.transform.SetParent(activeTransform);
                        gameObject.transform.localPosition = new Vector3(createdPointX, createdPointY, createdPointZ);
                        createdPointZ += zInter;
                    }
                    createdPointY += yInter;
                }
                createdPointX += xInter;
            }

            GetWindow(typeof(CreateGameObject)).Close();
        }
    }
}

#endif