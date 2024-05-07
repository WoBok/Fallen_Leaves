using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class LeavesShaderEditor : BaseShaderGUI
{
    readonly MaterialGradientDrawer _gradientDrawer = new MaterialGradientDrawer();
    readonly MaterialVector2Drawer _vectorDrawer = new MaterialVector2Drawer();
    bool m_UseGradient = true;
    bool m_NeedUpdateGradient = false;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        m_UseGradient = EditorGUILayout.Toggle("Use Gradient", m_UseGradient);

        foreach (var property in properties)
        {
            bool hideInInspector = (property.flags & MaterialProperty.PropFlags.HideInInspector) != 0;
            if (hideInInspector)
            {
                continue;
            }

            if (property.type == MaterialProperty.PropType.Texture && property.name.Contains("GradientTexture") && m_UseGradient)
            {
                EditorGUILayout.Space(18);
                _gradientDrawer.OnGUI(Rect.zero, property, property.displayName, materialEditor);
            }
            else if (property.type == MaterialProperty.PropType.Vector &&
                       property.displayName.Contains("[Vector2]"))
            {
                EditorGUILayout.Space(18);
                _vectorDrawer.OnGUI(Rect.zero, property, property.displayName, materialEditor);
            }
            else
            {
                var guiContent = new GUIContent(property.displayName);
                materialEditor.ShaderProperty(property, guiContent);
            }

            if (!m_UseGradient)
            {
                m_NeedUpdateGradient = true;
            }
            if (m_UseGradient && m_NeedUpdateGradient)
            {
                _gradientDrawer.UpdateCurrentGradient();
                m_NeedUpdateGradient = false;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (SupportedRenderingFeatures.active.editableMaterialRenderQueue) materialEditor.RenderQueueField();
        materialEditor.EnableInstancingField();
        materialEditor.DoubleSidedGIField();
    }
}