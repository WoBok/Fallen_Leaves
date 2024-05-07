using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class DoCreateShaderScritpAsset : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
        if (o != null)
            ProjectWindowUtil.ShowCreatedAsset(o);
    }
    internal static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
    {
        if (File.Exists(resourceFile))
        {
            string fileName = Path.GetFileNameWithoutExtension(pathName);
            fileName = fileName.Replace('_', ' ');
            string templateText = File.ReadAllText(resourceFile);
            templateText = templateText.Replace("#PATH#", $"URP Shader/{fileName}");
            string templateContent = SetLineEndings(templateText, EditorSettings.lineEndingsForNewScripts);
            string fullPath = Path.GetFullPath(pathName);
            File.WriteAllText(fullPath, templateContent);
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }
        return null;
    }
    internal static string SetLineEndings(string content, LineEndingsMode lineEndingsMode)
    {
        content = Regex.Replace(content, "\\r\\n?|\\n", lineEndingsMode switch
        {
            LineEndingsMode.OSNative => (Application.platform != RuntimePlatform.WindowsEditor) ? "\n" : "\r\n",
            LineEndingsMode.Unix => "\n",
            LineEndingsMode.Windows => "\r\n",
            _ => "\n",
        });
        return content;
    }
}
